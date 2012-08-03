using System;
using System.IO;
using System.Net;
using Chat = System.Net;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Utility;

//*****************************************************************************************
//                           LICENSE INFORMATION
//*****************************************************************************************
//   Server / DoCommunicate
//   Creates a basic basic server/client chat application in C#
//
//   Copyright (C) 2012
//   Joseph Cross
//   Email: joe.mcross@gmail.com
//   Created: 02AUG12
//   
//   Adapted from original by:
//   Copyright (C) 2007  
//   Richard L. McCutchen 
//   Email: richard@psychocoder.net
//   Created: 16SEP07
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
//*****************************************************************************************

namespace Engine.Networking
{
    public class BlahServer
    {
        int maxConnections;
        int serverPort;
        IPAddress serverAddress;
        Thread processingThread;
        Hashtable nickNames;
        Hashtable nickNamesByConnect;

        System.Net.Sockets.TcpListener listener;

        /// <summary>
        /// Set up a server to receive and broadcast messages
        /// Use Server.Start() to begin handling messages
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="serverPort"></param>
        /// <param name="maxConnections"></param>
        public BlahServer(IPAddress serverAddress, int serverPort, int maxConnections)
        {
            this.maxConnections = maxConnections;
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
            nickNames = new Hashtable(maxConnections);
            nickNamesByConnect = new Hashtable(maxConnections);
            listener = new System.Net.Sockets.TcpListener(serverAddress, serverPort);
        }

        /// <summary>
        /// Starts the listener and kills any active listeners
        /// </summary>
        public void Start()
        {
            if (listener == null) return;
            _startProcessing();
        }

        /// <summary>
        /// Stops the listener and kills any active listeners
        /// </summary>
        public void Stop()
        {
            if (processingThread != null && processingThread.IsAlive)
            {
                listener.Stop();
                processingThread.Kill();
            }
        }
        /// <summary>
        /// Starts processing requests.  Can also be considered Restart,
        /// if called when the listener is already started.
        /// </summary>
        protected void _startProcessing()
        {
            Stop();
            processingThread = new Thread(() => 
            {
                while (true)
                    Thread.Sleep(500);
                    if(listener.Pending())
                    {
                        Chat.Sockets.TcpClient client = listener.AcceptTcpClient();
                        DoCommunicate comm = new DoCommunicate(this, client);
                        Console.WriteLine(String.Format("New connection request: {0}", getip(client)));
                    }
            });
            listener.Start();
            processingThread.Start();
        }

        public void SendMsg(string msg)
        {
            StreamWriter writer;
            Chat.Sockets.TcpClient[] clients = new Chat.Sockets.TcpClient[nickNames.Count];
            nickNames.Values.CopyTo(clients, 0);
            for (int i = 0; i < clients.Length; i++)
            {
                try
                {
                    if (msg.Trim() == "" || clients[i] == null)
                        continue;
                    WriteAndFlush(msg, clients[i]);
                }
                catch (Exception e44)
                {
                    e44 = e44;
                    
                    var client = clients[i];
                    var nickName = (string)nickNamesByConnect[client];
                    SendMsgException(msg, nickName, client);
                }
            }
        }

        public void SendMsg(string msg, params string[] nickNames)
        {
            StreamWriter writer;
            Chat.Sockets.TcpClient[] clients = new Chat.Sockets.TcpClient[nickNames.Length];
            for (int i = 0; i < clients.Length; i++)
            {
                var nickName = nickNames[i];
                if (nickName == null || !this.nickNames.Contains(nickName)) continue;
                clients[i] = (Chat.Sockets.TcpClient)this.nickNames[nickNames[i]];
            }
            for (int i = 0; i < nickNames.Length; i++)
            {
                var nickName = nickNames[i];
                if (nickName == null || !this.nickNames.Contains(nickName) || msg.Trim() == "") continue;
                var client = (Chat.Sockets.TcpClient)this.nickNames[nickName];
                try
                {
                    WriteAndFlush(msg, client);
                }
                catch (Exception e44)
                {
                    e44 = e44;
                    SendMsgException(msg, nickName, client);
                }
            }
        }

        private void WriteAndFlush(string msg, Chat.Sockets.TcpClient client)
        {
            var writer = new StreamWriter(client.GetStream());
            writer.WriteLine(msg);
            writer.Flush();
            writer = null;
        }

        public void ReceiveMsg(string msg, Chat.Sockets.TcpClient client)
        {
            Func<string, Chat.Sockets.TcpClient, bool> validator = (m, c) => { return m != null && m.Length > 0; };
            Action<string, Chat.Sockets.TcpClient> validHandler = (m, c) => { SendMsg(nickNamesByConnect[c] + ": " + m); };
            Action<string, Chat.Sockets.TcpClient> invalidHandler = (m, c) => { SendMsg(String.Format("Error: Message <{0}> was invalid, and was not sent.", m), (string)nickNamesByConnect[c]); };
            ProcessMsg(msg, client, validator, validHandler, invalidHandler);
            
        }

        public bool ProcessMsg(string msg, Chat.Sockets.TcpClient client, Func<string, Chat.Sockets.TcpClient, bool> validator,
            Action<string, Chat.Sockets.TcpClient> validHandler, Action<string, Chat.Sockets.TcpClient> invalidHandler)
        {
            bool valid = validator(msg, client);
            var action = valid ? validHandler : invalidHandler;
            action(msg, client);
            return valid;
        }

        public void Authenticate(string prompt, string invalidResponse, StreamReader reader, StreamWriter writer, 
            Func<string, string, int, StreamReader, StreamWriter, bool> authenticateFunc)
        {
            int attempt = 0;
            bool authenticated = authenticateFunc(prompt, invalidResponse, attempt, reader, writer);
            while (!authenticated)
            {
                attempt++; 
                Console.WriteLine(String.Format("Login attempt failed (login attempt {0})", attempt));
                authenticated = authenticateFunc(prompt, invalidResponse, attempt, reader, writer);
            }
        }

        public bool IsNickNameAvailable(string nickName)
        {
            if (nickName == null) return false;
            return !nickNames.Contains(nickName);
        }

        public void OnPlayerJoin(string nickName, Chat.Sockets.TcpClient client)
        {
            nickNames.Add(nickName, client);
            nickNamesByConnect.Add(client, nickName);
            SendMsg(String.Format("Nickname {0} accepted.", nickName), nickName);
            SendMsg(String.Format("Let's all welcome {0} to the chat.", nickName));
            Console.WriteLine(String.Format("JOINED: {0} (client: {1})", nickName, getip(client)));
        }

        public void OnPlayerLeave(string nickName, Chat.Sockets.TcpClient client)
        {
            if (nickName == null) return;
            nickNames.Remove(nickName);
            nickNamesByConnect.Remove(client);
            SendMsg(String.Format("{0} was a whore anyway ({0} left.)", nickName));
            Console.WriteLine(String.Format("LEFT: {0} (client: {1})", nickName, getip(client)));
        }

        protected void SendMsgException(string msg, string nickName, Chat.Sockets.TcpClient client)
        {
            OnPlayerLeave(nickName, client);
        }
        private string getip(Chat.Sockets.TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }
    }
}
