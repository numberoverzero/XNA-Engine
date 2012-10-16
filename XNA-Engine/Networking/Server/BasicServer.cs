﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Engine.DataStructures;
using Engine.Logging;
using Engine.Networking.Packets;
using Engine.Utility;

namespace Engine.Networking
{
    /// <summary>
    ///   A basic implementation of the <see cref="IServer" /> interface
    /// </summary>
    public class BasicServer : IServer
    {
        private readonly IPAddress _localaddr;
        private readonly int _port;

        /// <summary>
        ///   True if a client has authenticated with the server
        /// </summary>
        protected DefaultDict<Client, bool> AuthTable;

        /// <summary>
        ///   Two-way mapping between a client's TcpClient and their GUID
        /// </summary>
        protected BidirectionalDict<string, Client> ClientTable;

        /// <summary>
        ///   The server log
        /// </summary>
        protected Log Log;

        private Thread _clientPollThread;
        private TcpListener _listener;

        /// <summary>
        ///   Construct a basic server such that it is ready to be started, and possibly using the default connect
        ///   behavior.
        /// </summary>
        public BasicServer(IPAddress localaddr, int port, string logFileName = null)
        {
            IsRunning = false;
            _localaddr = localaddr;
            _port = port;

            ClientTable = new BidirectionalDict<string, Client>();
            AuthTable = new DefaultDict<Client, bool>();

            Log = new Log(logFileName, Frequency.Burst);
            Log.Info("Server initialized: <{0}>::{1}".format(localaddr, port));
        }

        #region IServer Members

        /// <summary>
        ///   See <see cref="IServer.IsRunning" />
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///   See <see cref="IServer.Start" />
        /// </summary>
        public void Start()
        {
            if (IsRunning) return;
            if (_listener != null) _listener.Stop();
            _listener = new TcpListener(_localaddr, _port);
            _listener.Start();

            if (_clientPollThread != null && _clientPollThread.IsAlive) _clientPollThread.Kill();
            _clientPollThread = new Thread(PollForClients);
            _clientPollThread.Start();

            IsRunning = true;
            Log.Info("Server started: <{0}>::<{1}>".format(_localaddr, _port));
            if (OnStart != null) OnStart(this, null);
        }

        /// <summary>
        ///   See <see cref="IServer.Stop" />
        /// </summary>
        public void Stop()
        {
            _clientPollThread.Kill();
            _clientPollThread = null;

            _listener.Stop();
            _listener = null;

            IsRunning = false;
            Log.Info("Server stopped.");
            Log.Flush();
            if (OnStop != null) OnStop(this, null);
        }

        /// <summary>
        ///   See <see cref="IServer.Connect" />
        /// </summary>
        public virtual void Connect(Client client, ServerEventArgs e = null)
        {
            var parameters = new Dictionary<string, string>();
            parameters["Server:Connect:Data:IP"] = client.GetIP;
            if (!IsRunning) return;
            if (e == null)
            {
                e = new ServerEventArgs(false, client, parameters);
            }
            else
            {
                e.Parameters.Merge(parameters);

                // Connect has the final say on these two,
                // since it was the most recent frame from which the Event was fired
                e.Success = true;
                e.Client = client;
            }
            Log.Info("Server:Connect:Data:IP:<{0}>".format(client.GetIP));
            if (OnConnect != null)
                OnConnect(this, e);
        }

        /// <summary>
        ///   See <see cref="IServer.Disconnect" />
        /// </summary>
        public virtual void Disconnect(Client client, ServerEventArgs e = null)
        {
            if (!IsRunning)
            {
                Log.Debug("Server:InvalidFunctionCall:Disconnect:Data:IP:<{0}>".format(client.GetIP));
                return;
            }
            var success = true;
            var parameters = new Dictionary<string, string>();
            parameters["Server:RemoveClientFromTable:Value"] = "false";
            try
            {
                ClientTable.Remove(client);
                parameters["Server:RemoveClientFromTable:Value"] = "true";
                AuthTable.Remove(client);
                parameters["Server:RemoveClientFromAuthTable:Value"] = "true";
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
            Log.Info("Server:Disconnect:Data:IP:<{0}>".format(client.GetIP));
            if (OnDisconnect != null)
                OnDisconnect(this, e);
        }

        /// <summary>
        ///   See <see cref="IServer.Authenticate" />
        /// </summary>
        public virtual void Authenticate(Client client, ServerEventArgs e = null)
        {
            if (!IsRunning)
            {
                Log.Debug("Server:InvalidFunctionCall:Authenticate:Data:IP:<{0}>".format(client.GetIP));
                return;
            }

            if (e == null)
            {
                e = new ServerEventArgs(false, client);
            }
            else
            {
                e.Client = client;
            }
            AuthTable[client] = e.Success;
            Log.Info(e.Success
                         ? "Server:AuthSucceed:Data:IP:<{0}>".format(client.GetIP)
                         : "Server:AuthFail:Data:IP:<{0}>".format(client.GetIP));
            if (OnAuthenticate != null)
                OnAuthenticate(this, e);
        }

        /// <summary>
        ///   See <see cref="IServer.ReceivePacket" />
        /// </summary>
        public virtual void ReceivePacket(Packet packet, Client client)
        {
            if (!IsRunning)
                Log.Debug("Server:InvalidFunctionCall:ReceivePacket:Data:Packet:<{0}>".format(packet));
        }

        /// <summary>
        ///   See <see cref="IServer.SendPacket" />
        /// </summary>
        public virtual void SendPacket(Packet packet, params Client[] clients)
        {
            if (!IsRunning)
            {
                Log.Debug("Server:InvalidFunctionCall:SendPacket:Data:Packet:<{0}>".format(packet));
                return;
            }
            if (clients.Length == 0)
                clients = ClientTable.GetValuesType2().ToArray();
            Log.Debug("Server:SendPacket:Data:Packet:<{0}>".format(packet));
            foreach (var client in clients)
                try
                {
                    if (IsAuthenticated(client)) client.WritePacket(packet);
                }
                catch
                {
                    OnSendPacketException(packet, "Unknown", client);
                }
        }

        /// <summary>
        ///   See <see cref="IServer.IsAuthenticated" />
        /// </summary>
        public bool IsAuthenticated(Client client)
        {
            return AuthTable[client];
        }

        /// <summary>
        ///   See <see cref="IServer.GetClientString" />
        /// </summary>
        public string GetClientString(Client client)
        {
            return !IsRunning ? null : ClientTable[client];
        }

        /// <summary>
        ///   See <see cref="IServer.GetClient" />
        /// </summary>
        public Client GetClient(string client)
        {
            return !IsRunning ? null : ClientTable[client];
        }

        /// <summary>
        ///   See <see cref="IServer.GetClientStrings" />
        /// </summary>
        public IEnumerable<string> GetClientStrings(params Client[] clients)
        {
            return from client in clients select ClientTable[client];
        }

        /// <summary>
        ///   See <see cref="IServer.GetClients" />
        /// </summary>
        public IEnumerable<Client> GetClients(params string[] clients)
        {
            return from client in clients select ClientTable[client];
        }

        /// <summary>
        ///   See <see cref="IServer.OnStart" />
        /// </summary>
        public event EventHandler OnStart;

        /// <summary>
        ///   See <see cref="IServer.OnStop" />
        /// </summary>
        public event EventHandler OnStop;

        /// <summary>
        ///   See <see cref="IServer.OnShutdown" />
        /// </summary>
        public event EventHandler OnShutdown;

        /// <summary>
        ///   See <see cref="IServer.OnConnect" />
        /// </summary>
        public event EventHandler<ServerEventArgs> OnConnect;

        /// <summary>
        ///   See <see cref="IServer.OnDisconnect" />
        /// </summary>
        public event EventHandler<ServerEventArgs> OnDisconnect;

        /// <summary>
        ///   See <see cref="IServer.OnAuthenticate" />
        /// </summary>
        public event EventHandler<ServerEventArgs> OnAuthenticate;

        #endregion

        /// <summary>
        ///   Checks listener forever and adds new clients as they try to connect
        /// </summary>
        protected void PollForClients()
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                new Thread(() => Connect(new Client(client))).Start();
            }
        }

        /// <summary>
        ///   Default handler for the OnConnect event.
        ///   Registers the client in the clientTable, registers a new thread in the clientThread table,
        ///   and starts that thread.  The thread calls DefaultClientThreadFunction
        /// </summary>
        protected virtual void DefaultHandle_OnConnect(object sender, ServerEventArgs args)
        {
            var client = args.Client;
            ClientTable[client] = new Guid().ToString();
            client.OnReadPacket += OnClientRead;
        }

        /// <summary>
        ///   Called when a client receives a new packet
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="args"> </param>
        protected void OnClientRead(object sender, EventArgs args)
        {
            var client = sender as Client;
            if (client == null) return;

            if (!ClientTable.Contains(client))
                // Don't waste our time reading from clients that aren't tracked in the client table
                client.OnReadPacket -= OnClientRead;
            else
                ReceivePacket(client.ReadPacket(), client);
        }

        /// <summary>
        ///   Called when we try to read from a client stream and fail.
        /// </summary>
        protected virtual void OnClientReadException(string reason, Client client)
        {
            // Nothing to do if we didn't track the client
            if (!ClientTable.Contains(client))
            {
                Log.Debug(
                    "Server:InvalidFunctionCall:OnClientReadException:UnknownClient:Data:IP:<{0}>".format(
                        client.GetIP));
                return;
            }

            const bool success = false;
            var parameters = new Dictionary<string, string>
                                 {
                                     {"Exception:ServerException", "ClientReadException"},
                                     {"ClientReadException:Data:Reason", reason}
                                 };
            var e = new ServerEventArgs(success, client, parameters);
            Disconnect(client, e);
        }

        /// <summary>
        ///   Called when we try to send a message to a client but that send fails.
        /// </summary>
        protected virtual void OnSendPacketException(Packet packet, string reason, Client client)
        {
            // Nothing to do if we didn't track the client
            if (!ClientTable.Contains(client))
            {
                Log.Debug(
                    "Server:InvalidFunctionCall:OnSendPacketException:UnknownClient:Data:IP:<{0}>".format(
                        client.GetIP));
                return;
            }
            const bool success = false;
            var parameters = new Dictionary<string, string>
                                 {
                                     {"Exception:ServerException", "SendPacketFailedException"},
                                     {"SendPacketFailedException:Data:Value", packet.ToString()},
                                     {"SendPacketFailedException:Data:Reason", reason}
                                 };
            var e = new ServerEventArgs(success, client, parameters);
            Console.WriteLine("OnSendPacketException");
            Disconnect(client, e);
        }
    }
}