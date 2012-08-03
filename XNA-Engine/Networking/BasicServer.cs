﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Utility;

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
        BidirectionalDict<string, TcpClient> clientTable;
        Dictionary<TcpClient, Thread> clientThreads;
        IPAddress localaddr;
        int port;
        TcpListener listener;
        Thread listenerThread;

        #endregion

        #region Initialization

        /// <summary>
        /// Construct a basic server such that it is ready to be started.
        /// </summary>
        /// <param name="localaddr"></param>
        /// <param name="port"></param>
        public BasicServer(IPAddress localaddr, int port) : this(localaddr, port, false) { }

        /// <summary>
        /// Construct a basic server such that it is ready to be started, and possibly using the default connect
        /// behavior.
        /// </summary>
        /// <param name="localaddr"></param>
        /// <param name="port"></param>
        /// <param name="useDefaultConnectBehavior"></param>
        public BasicServer(IPAddress localaddr, int port, bool useDefaultConnectBehavior)
        {
            isRunning = hasShutdown = false;
            clientTable = new BidirectionalDict<string, TcpClient>();
            clientThreads = new Dictionary<TcpClient, Thread>();
            this.localaddr = localaddr;
            this.port = port;
            if (useDefaultConnectBehavior)
                OnConnect += Handle_OnConnect;
        }

        /// <summary>
        /// Checks listener forever and adds new clients as they try to connect
        /// </summary>
        protected void PollForClients()
        {
            Thread.Sleep(500);
            if (listener.Pending())
            {
                var client = listener.AcceptTcpClient();
                Connect(client);
            }
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

            if (listenerThread != null && listenerThread.IsAlive) listenerThread.Kill();
            listenerThread = new Thread(() => { PollForClients(); });
            listenerThread.Start();

            isRunning = true;
            OnStart(this, null);
        }
        /// <summary>
        /// See <see cref="IServer.Stop"/>
        /// </summary>
        public void Stop()
        {
            if (hasShutdown) return;
            
            listenerThread.Kill();
            listenerThread = null;

            listener.Stop();
            listener = null;

            isRunning = false;
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
            OnShutdown(this, null);
        }

        #endregion

        #region Connect

        /// <summary>
        /// See <see cref="IServer.Connect"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public virtual void Connect(TcpClient client, ServerEventArgs e = null)
        {
            bool success = true;
            var parameters = new Dictionary<string, string>();
            parameters["Server:Connect:Data:IP"] = GetIP(client);
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
            OnConnect(this, e);
        }
        /// <summary>
        /// Default handler for the OnConnect event.
        /// Registers the client in the clientTable, registers a new thread in the clientThread table,
        /// and starts that thread.  The thread calls DefaultClientThreadFunction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void Handle_OnConnect(object sender, ServerEventArgs args)
        {
            clientTable[args.Client] = new Guid().ToString();
            var thread = new Thread(() => { DefaultClientThreadFunction(args); });
            clientThreads[args.Client] = thread;
            thread.Start();
        }
        /// <summary>
        /// Constantly checks a client for incoming messages
        /// </summary>
        /// <param name="args"></param>
        protected void DefaultClientThreadFunction(ServerEventArgs args)
        {
            Action OnBreak = () => { };
            var client = args.Client;
            var stream = client.GetStream();
            string line;
            while (true)
            {
                try
                {
                    if ((line = ClientReadLine(stream)) != null)
                        ReceiveMsg(line, client);
                }
                catch (Exception e)
                {
                    e = e;
                    OnBreak = () => { OnClientReadException("Unknown", client); };
                    break;
                }
            }
            OnBreak();
        }
        /// <summary>
        /// Attempts to read for a client
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected string ClientReadLine(NetworkStream stream)
        {
            if (stream.CanRead)
            {
                byte[] buffer = new byte[1024];
                StringBuilder sb = new StringBuilder();
                int n = 0;
                do
                {
                    n = stream.Read(buffer, 0, buffer.Length);
                    sb.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, n));
                } while (stream.DataAvailable);
                return sb.ToString();
            }
            return null;
        }
        /// <summary>
        /// Called when we try to read from a client stream and fail.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="client"></param>
        protected virtual void OnClientReadException(string reason, TcpClient client)
        {
            // Nothing to do if we didn't track the client
            if (!clientTable.HasItem(client)) return;

            bool success = false;
            var parameters = new Dictionary<string, string>();
            parameters.Add("Exception:ServerException", "ClientReadException");
            parameters.Add("ClientReadException:Data:Reason", reason);
            var e = new ServerEventArgs(success, client, parameters);
            Disconnect(client, e);
        }

        private string GetIP(TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }

        #endregion

        /// <summary>
        /// See <see cref="IServer.Disconnect"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public virtual void Disconnect(TcpClient client, ServerEventArgs e = null)
        {
            if (!isRunning || hasShutdown) return;
            bool success = true;
            var parameters = new Dictionary<string, string>();
            parameters["Server:RemoveClientFromTable:Value"] = "false";
            parameters["Server:KillClientThread:Value"] = "false";
            parameters["Server:RemoveClientThreadFromTable:Value"] = "false";
            try
            {
                
                clientTable.Remove(client);
                parameters["Server:RemoveClientFromTable:Value"] = "true";
                clientThreads[client].Kill();
                parameters["Server:KillClientThread:Value"] = "true";
                clientThreads.Remove(client);
                parameters["Server:RemoveClientThreadFromTable:Value"] = "true";
            }
            catch (Exception e2)
            {
                e2 = e2;
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
            OnDisconnect(this, e);
        }
        /// <summary>
        /// See <see cref="IServer.Authenticate"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public virtual void Authenticate(TcpClient client, ServerEventArgs e = null)
        {
            if (!isRunning || hasShutdown) return;
            bool success = true;
            var parameters = new Dictionary<string, string>();
            if (e == null)
            {
                success = false;
                parameters = new Dictionary<string, string>();
                e = new ServerEventArgs(success, client, parameters);
            }
            else
            {
                e.Parameters.Merge(parameters);

                // Authenticate has the final say on these two,
                // since it was the most recent frame from which the Event was fired
                e.Success = success;
                e.Client = client;
            }
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
        /// See <see cref="IServer.ReceiveMsg"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        public virtual void ReceiveMsg(string msg, TcpClient client)
        {
            if (!isRunning || hasShutdown) return;
        }

        #region SendMsg

        /// <summary>
        /// See <see cref="IServer.SendMsg"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clients"></param>
        public virtual void SendMsg(string msg, params TcpClient[] clients)
        {
            if (!isRunning || hasShutdown) return;
            if (clients.Length == 0)
                clients = clientTable.GetValuesType2().ToArray();
            foreach (var client in clients)
                try
                {
                    WriteMsg(msg, client);
                }
                catch (Exception e)
                {
                    e = e;
                    OnSendMsgException(msg, "Unknown", client);
                }
        }

        /// <summary>
        /// Tries to write a message to a client
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        protected virtual void WriteMsg(string msg, TcpClient client)
        {
            var writer = new StreamWriter(client.GetStream());
            writer.WriteLine(msg);
            writer.Flush();
            writer = null;
        }

        /// <summary>
        /// Called when we try to send a message to a client but that send fails.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="reason"></param>
        /// <param name="client"></param>
        protected virtual void OnSendMsgException(string msg, string reason, TcpClient client)
        {
            // Nothing to do if we didn't track the client
            if (!clientTable.HasItem(client)) return;

            bool success = false;
            var parameters = new Dictionary<string, string>();
            parameters.Add("Exception:ServerException", "SendMsgFailedException");
            parameters.Add("SendMsgFailedException:Data:Value", msg);
            parameters.Add("SendMsgFailedException:Data:Reason", reason);
            var e = new ServerEventArgs(success, client, parameters);
            Disconnect(client, e);
        }

        #endregion

        #region Client lookups

        /// <summary>
        /// See <see cref="IServer.GetClientString"/>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public string GetClientString(TcpClient client)
        {
            if (!isRunning || hasShutdown) return null;
            return clientTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClient"/>
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public TcpClient GetClient(string client)
        {
            if (!isRunning || hasShutdown) return null;
            return clientTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClientStrings"/>
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public IEnumerable<string> GetClientStrings(params TcpClient[] clients)
        {
            if (hasShutdown) return new List<string>();
            return from client in clients select clientTable[client];
        }
        /// <summary>
        /// See <see cref="IServer.GetClients"/>
        /// </summary>
        /// <param name="clients"></param>
        /// <returns></returns>
        public IEnumerable<TcpClient> GetClients(params string[] clients)
        {
            if (hasShutdown) return new List<TcpClient>();
            return from client in clients select clientTable[client];
        }

        #endregion
    }
}
