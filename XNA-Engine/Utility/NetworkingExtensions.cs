using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Engine.Utility
{
    /// <summary>
    /// Extensions for networking stuff
    /// </summary>
    public static class NetworkingExtensions
    {
        /// <summary>
        /// Reads data from the NetworkStream without throwing errors or crashing if the remote shuts down
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string SafeRead(this NetworkStream stream)
        {
            if (stream.CanRead)
            {
                byte[] buffer = new byte[1024];
                StringBuilder sb = new StringBuilder();
                int n = 0;
                try
                {
                    do
                    {
                        n = stream.Read(buffer, 0, buffer.Length);
                        sb.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, n));
                    }
                    while (stream.DataAvailable);
                }
                catch (SocketException e) { return trimmed(sb); }
                catch (IOException e) { return trimmed(sb); }
                return trimmed(sb);
            }
            return null;
        }

        private static string trimmed(StringBuilder sb)
        {
            return sb.ToString().TrimEnd("\r\n");
        }
    }
}
