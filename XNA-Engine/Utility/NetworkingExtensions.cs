using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Engine.Utility
{
    /// <summary>
    /// Extensions for networking stuff
    /// </summary>
    public static class NetworkingExtensions
    {
        /// <summary>
        /// Gets the IPAddress of the client as a string
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetIP(this TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }
    }
}
