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
    public class DoCommunicate
    {
        Server server;
        Chat.Sockets.TcpClient client;
        System.Net.Sockets.NetworkStream reader;
        System.Net.Sockets.NetworkStream writer;
        Thread runThread;
        string nickName;

        public DoCommunicate(Server server, System.Net.Sockets.TcpClient client)
        {
            this.server = server;
            this.client = client;
            new Thread(new ThreadStart(startChat)).Start();
        }

        private bool AuthenticateNick(string prompt, string invalidResponse, int attempt, StreamReader reader, StreamWriter writer)
        {
            if (attempt > 0)
                writer.WriteLine(invalidResponse);
            writer.WriteLine(prompt);
            writer.Flush();
            while ((nickName = reader.ReadLine()) == null) ;
            return server.IsNickNameAvailable(nickName);
        }

        private void startChat()
        {
            reader = client.GetStream();
            writer = client.GetStream();

            server.Authenticate("Please enter your nickname:", "That's taken, please try another.", new StreamReader(reader), new StreamWriter(writer), AuthenticateNick);
            server.OnPlayerJoin(nickName, client);


            runThread = new Thread(new ThreadStart(runChat));
            runThread.Start();
        }

        private void runChat()
        {
            string line = "";
            while (true)
            {
                    line = reader.SafeRead();
                    server.ReceiveMsg(line, client);
                
            }
        }
    }
}
