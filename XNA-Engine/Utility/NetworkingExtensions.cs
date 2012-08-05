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
        /// Reads a message from a stream where the first 4 bytes are the size of the rest of the message
        /// </summary>
        public static byte[] ReadWithHeader(this Stream stream)
        {
            byte[] header = new byte[4];
            stream.ReadExact(header, 0, 4);
            int length = BitConverter.ToInt32(header, 0);
            
            byte[] buffer = new byte[length];
            stream.ReadExact(buffer, 0, length);
            return buffer;
        }

        /// <summary>
        /// Reads a message from a stream where the first 4 bytes are the size of the rest of the message
        /// </summary>
        public static string ReadStringWithHeader(this Stream stream)
        {
            byte[] buffer = stream.ReadWithHeader();
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Writes a message to the stream, prepending a fixed size header (4 bytes) which describes the size of the rest of the message
        /// </summary>
        public static void WriteWithHeader(this Stream stream, byte[] buffer, int offset, int length)
        {
            byte[] header = BitConverter.GetBytes(length - offset);
            stream.Write(header, 0, 4);
            stream.Write(buffer, offset, length);
        }

        /// <summary>
        /// Writes a message to the stream, prepending a fixed size header (4 bytes) which describes the size of the rest of the message
        /// </summary>
        public static void WriteWithHeader(this Stream stream, string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            stream.WriteWithHeader(buffer, 0, buffer.Length);
        }
    }
}
