using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Engine.DataStructures;
using Engine.Logging;
using Engine.Utility;
using Engine.Networking.Packets;

namespace Engine.Networking
{
    /// <summary>
    /// A basic implementation of the <see cref="IServer"/> interface
    /// </summary>
    public class BasicServer : IServer
    {
        #region Fields

        bool isRunning;
        bool hasShutdown;
        /// <summary>
        /// Two-way mapping between A client's TcpClient and their GUID
        /// </summary>
        protected BidirectionalDict<string, Client> clientTable;
        /// <summary>
        /// Mapping from clients to their read threads
        /// </summary>
        protected ConcurrentDictionary<Client, Thread> clientThreads;

        /// <summary>
        /// True if a client have authenticated with the server
        /// </summary>
        protected DefaultDict<Client, bool> authTable;

        IPAddress localaddr;
        int port;
        TcpListener listener;
        Thread listenerThread;
        /// <summary>
        /// The server log
        /// </summary>
        protected Log log;

        #endregion

        #region Initialization

        /// <summary>
        /// Construct a basic server such that it is ready to be started.
        /// </summary>
        /// <param name="localaddr"></param>
        /// <param name="port"></param>
        public BasicServer(IPAddress localaddr, int port) : this(localaddr, port, null) { }

        /// <summary>
        /// Construct a basic server such that it is ready to be started, and possibly using the default connect
        /// behavior.
        /// </summary>
        /// <param name="localaddr"></param>
        /// <param name="port"></param>
        /// <param name="logFileName"></param>
        public BasicServer(IPAddress localaddr, int port, string logFileName)
        {
            isRunning = hasShutdown = false;
            clientTable = new BidirectionalDict<string, Client>();
            authTable = new DefaultDict<Client, bool>();
            clientThreads = new ConcurrentDictionary<Client, Thread>();
            this.localaddr = localaddr;
            this.port = port;
            log = new Log(logFileName, Frequency.Burst);
            log.Info("Server initialized: <{0}>::{1}".format(localaddr, port));
        }

        #endregion

        #region Server start/stop/shutdown

        /// <summary>
        /// See <see cref="IServer.IsRunning"/>
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }
        /// <summary>
        /// See <see cref="IServer.Start"/>
        /// </summary>
        public void Start()
        {
            if (hasShutdown) return;

            if (listener != null) listener.Stop();
            listener = new TcpListener(localaddr, port);
            listener.Start();

            if (listenerThread != null && listenerThread.IsAlive) listenerThread.Kill();
            listenerThread = new Thread(new ThreadStart(PollForClients));
            listenerThread.Start();

            isRunning = true;
            log.Info("Server started.");
            if(OnStart != null)
                OnStart(this, null);
        }
        /// <summary>
        /// Checks listener forever and adds new clients as they try to connect
        /// </summary>
        protected void PollForClients()
        {
            TcpClient client;
            while (true)
            {
                client = listener.AcceptTcpClient();
                new Thread(() => { Connect(new Client(client)); }).Start();
            }
        }

        /// <summary>
        /// See <see cref="IServer.Stop"/>
        /// </summary>
        public void Stop()
        {
            if (hasShutdown)
            {
                return;
            }
            
            listenerThread.Kill();
            listenerThread = null;

            listener.Stop();
            listener = null;

            isRunning = false;
            log.Info("Server stopped.");
            log.Flush();
            if (OnStop != null)
                OnStop(this, null);
        }
        /// <summary>
        /// See <see cref="IServer.Shutdown"/>
        /// </summary>
        /// <param name="immediate"></param>
        public void Shutdown(bool immediate = false)
        {
            if (hasShutdown) return;
            Stop();
            foreach (var client in clientTable.GetValuesType2().ToArray())
                Disconnect(client);
            hasShutdown = true;
            log.Info("Server shutdown.");
            log.Flush();
            if (OnShutdown != null)
                OnShutdown(this, null);
        }

        #endregion

        #region Connect

        /// <summary>
        /// See <see cref="IServer.Connect"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public virtual void Connect(Client client, ServerEventArgs e = null)
        {
            bool success = true;
            var parameters = new Dictionary<string, string>();
            parameters["Server:Connect:Data:IP"] = client.IPString;
            if (!isRunning || hasShutdown) return;
            if (e == null)
            {
                success = false;
                e = new ServerEventArgs(success, client, parameters);
            }
            else
            {
                e.Parameters.Merge(parameters);

                // Connect has the final say on these two,
                // since it was the most recent frame from which the Event was fired
                e.Success = success;
                e.Client = client;
            }
            log.Info("Server:Connect:Data:IP:<{0}>".format(client.IPString));
            if (OnConnect != null)
                OnConnect(this, e);
        }
        /// <summary>
        /// Default handler for the OnConnect event.
        /// Registers the client in the clientTable, registers a new thread in the clientThread table,
        /// and starts that thread.  The thread calls DefaultClientThreadFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void DefaultHandle_OnConnect(object sender, ServerEventArgs args)
        {
            var client = args.Client;
            clientTable[client] = new Guid().ToString();
            var thread = new Thread(new ParameterizedThreadStart(DefaultClientThreadFunction));
            clientThreads[client] = thread;
            thread.Start(client);
        }
        /// <summary>
        /// Constantly checks a client for incoming messages
        /// </summary>
        /// <param name="oClient"></param>
        protected void DefaultClientThreadFunction(object oClient)
        {
            var client = oClient as Client;
            ChatPacket line;
            while (client.IsAlive)
            {
                Thread.Sleep(1);
                try
                {
                    // We don't read messages from a client until they've authenticated
                    if(IsAuthenticated(client) && client.HasQueuedReadMessages){
                        line = client.ReadPacket() as ChatPacket;
                        ReceivePacket(line, client);
                    }
                }
                catch { break; }
            }
            OnClientReadException("Lost Connection", client);
        }

        /// <summary>
        /// Called when we try to read from a client stream and fail.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="client"></param>
        protected virtual void OnClientReadException(string reason, Client client)
        {
            // Nothing to do if we didn't track the client
            if (!clientTable.Contains(client))
            {
                log.Debug("Server:InvalidFunctionCall:OnClientReadException:UnknownClient:Data:IP:<{0}>".format(client.IPString));
                return;
            }

            bool success = false;
            var parameters = new Dictionary<string, string>();
            parameters.Add("Exception:ServerException", "ClientReadException");
            parameters.Add("ClientReadException:Data:Reason", reason);
            var e = new ServerEventArgs(success, client, parameters);
            Disconnect(client, e);
        }

        #endregion

        /// <summary>
        /// See <see cref="IServer.Disconnect"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public virtual void Disconnect(Client client, ServerEventArgs e = null)
        {
            if (!isRunning || hasShutdown)
            {
                log.Debug("Server:InvalidFunctionCall:Disconnect:Data:IP:<{0}>".format(client.IPString));
                return;
            }
            bool success = true;
            var parameters = new Dictionary<string, string>();
            parameters["Server:RemoveClientFromTable:Value"] = "false";
            parameters["Server:KillClientThread:Value"] = "false";
            parameters["Server:RemoveClientThreadFromTable:Value"] = "false";
            try
            {
                
                clientTable.Remove(client);
                parameters["Server:RemoveClientFromTable:Value"] = "true";
                authTable.Remove(client);
                parameters["Server:RemoveClientFromAuthTable:Value"] = "true";
                clientThreads[client].Kill();
                parameters["Server:KillClientThread:Value"] = "true";
                clientThreads.Remove(client);
                parameters["Server:RemoveClientThreadFromTable:Value"] = "true";
            }
            catch
            {
                success = false;
            }
            if (e == null)
                e = new ServerEventArgs(success, client, parameters);
            else
            {
                e.Parameters.Merge(parameters);
                
                // Disconnect has the final say on these two,
                // since it was the most recent frame from which the Event was fired
                e.Success = success;
                e.Client = client;
            }
            log.Info("Server:Disconnect:Data:IP:<{0}>".format(client.IPString));
            if (OnDisconnect != null)
                OnDisconnect(this, e);
        }
        /// <summary>
        /// See <see cref="IServer.Authenticate"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public virtual void Authenticate(Client client, ServerEventArgs e = null)
        {
            if (!isRunning || hasShutdown)
            {
                log.Debug("Server:InvalidFunctionCall:Authenticate:Data:IP:<{0}>".format(client.IPString));
                return;
            }
            var parameters = new Dictionary<string, string>();
            if (e == null)
            {
                parameters = new Dictionary<string, string>();
                // Always fail default auth attempt
                e = new ServerEventArgs(false, client, parameters);
            }
            else
            {
                // It is assumed that anyone passing ServerEventArgs will have the authority to declare the client authenticated or not
                e.Parameters.Merge(parameters);
                e.Client = client;
            }
            authTable[client] = e.Success;
            if(e.Success)
                log.Info("Server:AuthSucceed:Data:IP:<{0}>".format(client.IPString));
            else
                log.Info("Server:AuthFail:Data:IP:<{0}>".format(client.IPString));
            if (OnAuthenticate != null)
                OnAuthenticate(this, e);
        }

        #region EventHandlers

        /// <summary>
        /// See <see cref="IServer.OnStart"/>
        /// </summary>
        public event EventHandler OnStart;
        /// <summary>
        /// See <see cref="IServer.OnStop"/>
        /// </summary>
        public event EventHandler OnStop;
        /// <summary>
        /// See <see cref="IServer.OnShutdown"/>
        /// </summary>
        public event EventHandler OnShutdown;

        /// <summary>
        /// See <see cref="IServer.OnConnect"/>
        /// </summary>
        public event EventHandler<ServerEventArgs> OnConnect;
        /// <summary>
        /// See <see cref="IServer.OnDisconnect"/>
        /// </summary>
        public event EventHandler<ServerEventArgs> OnDisconnect;
        /// <summary>
        /// See <see cref="IServer.OnAuthenticate"/>
        /// </summary>
        public event EventHandler<ServerEventArgs> OnAuthenticate;

        #endregion

        /// <summary>
        /// See <see cref="IServer.ReceivePacket"/>
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public virtual void ReceivePacket(Packet packet, Client client)
        {
            if (!isRunning || hasShutdown)
            {
                log.Debug("Server:InvalidFunctionCall:ReceivePacket:Data:Packet:<{0}>".format(packet));
                return;
            }
            return;
        }

        #region SendPacket

        /// <summary>
        /// See <see cref="IServer.SendPacket"/>
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="clients"></param>
        public virtual void SendPacket(Packet packet, params Client[] clients)
        {
            if (!isRunning || hasShutdown)
            {
                log.Debug("Server:InvalidFunctionCall:SendPacket:Data:Packet:<{0}>".format(packet));
                return;
            }
            if (clients.Length == 0)
                clients = clientTable.GetValuesType2().ToArray();
            log.Debug("Server:SendPacket:Data:Packet:<{0}>".format(packet));
            foreach (var client in clients)
                try
                {
                    if(IsAuthenticated(client)) WritePacket(packet, client);
                }
                catch
                {
                    OnSendPacketException(packet, "Unknown", client);
                }
        }

        /// <summary>
        /// Tries to write a packet to a client
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        protected virtual void WritePacket(Packet packet, Client client)
        {
            client.WritePacket(packet);
        }

        /// <summary>
        /// Called when we try to send a message to a client but that send fails.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="reason"></param>
        /// <param name="client"></param>
        protected virtual void OnSendPacketException(Packet packet, string reason, Client client)
        {
            // Nothing to do if we didn't track the client
            if (!clientTable.Contains(client))
            {
                log.Debug("Server:InvalidFunctionCall:OnSendPacketException:UnknownClient:Data:IP:<{0}>".format(client.IPString));
                return;
            }
            bool success = false;
            var parameters = new Dictionary<string, string>();
            parameters.Add("Exception:ServerException", "SendPacketFailedException");
            parameters.Add("SendPacketFailedException:Data:Value", packet.ToString());
            parameters.Add("SendPacketFailedException:Data:Reason", reason);
            var e = new ServerEventArgs(success, client, parameters);
            Console.WriteLine("OnSendPacketException");
            Disconnect(client, e);
        }

        #endregion

        #region Client query

        /// <summary>
        /// See <see cref="IServer.IsAuthenticated"/>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsAuthenticated(Client client)
        {
            return authTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClientString"/>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public string GetClientString(Client client)
        {
            if (!isRunning || hasShutdown) return null;
            return clientTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClient"/>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public Client GetClient(string client)
        {
            if (!isRunning || hasShutdown) return null;
            return clientTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClientStrings"/>
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public IEnumerable<string> GetClientStrings(params Client[] clients)
        {
            if (hasShutdown) return new List<string>();
            return from client in clients select clientTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClients"/>
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public IEnumerable<Client> GetClients(params string[] clients)
        {
            if (hasShutdown) return new List<Client>();
            return from client in clients select clientTable[client];
        }

        #endregion
    }
}
