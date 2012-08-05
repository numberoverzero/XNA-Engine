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

        /// <summary>
        /// Attempts to read exactly count bytes into the buffer, starting at offset.
        /// Throws EOS Exception when the requested number of bytes aren't read
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public static void ReadExact(this Stream stream, byte[] buffer, int offset, int count)
        {
            int read;
            while (count > 0 && (read = stream.Read(buffer, offset, count)) > 0)
            {
                offset += read;
                count -= read;
            }
            if (count != 0) throw new EndOfStreamException();
        }

        /// <summary>
        /// Reads a message from a client where the first 4 bytes are the size of the rest of the message
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <remarks>
        /// EXTREMELY susceptible to trickle attacks.
        /// 
        /// </remarks>
        public static string ReadWithHeader(this TcpClient client)
        {
            var stream = client.GetStream();
            
            byte[] header = new byte[4];
            stream.ReadExact(header, 0, 4);
            
            int length = BitConverter.ToInt32(header, 0);
            byte[] messageBuffer = new byte[length];
            stream.ReadExact(messageBuffer, 0, length);
            
            return Encoding.UTF8.GetString(messageBuffer);
        }

        /// <summary>
        /// Writes a message to the client, prepending a fixed size header (4 bytes) which describes the size of the rest of the message
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public static void WriteWithHeader(this TcpClient client, string msg)
        {
            var stream = client.GetStream();
            var messageBuffer = Encoding.UTF8.GetBytes(msg);
            var header = BitConverter.GetBytes(messageBuffer.Length);

            stream.Write(header, 0, 4);
            stream.Write(messageBuffer, 0, messageBuffer.Length);
        }
    }
}
