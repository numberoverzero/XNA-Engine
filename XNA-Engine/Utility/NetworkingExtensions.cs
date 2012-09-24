using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Engine.Utility
{
    /// <summary>
    ///   Extensions for networking stuff
    /// </summary>
    public static class NetworkingExtensions
    {
        /// <summary>
        ///   Gets the IPAddress of the client as a string
        /// </summary>
        /// <param name="client"> </param>
        /// <returns> </returns>
        public static string GetIP(this TcpClient client)
        {
            return ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString();
        }

        /// <summary>
        ///   Attempts to read exactly count bytes into the buffer, starting at offset.
        ///   Throws EOS Exception when the requested number of bytes aren't read
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
        ///   Reads a message from a stream where the first 4 bytes are the size of the rest of the message
        /// </summary>
        public static byte[] ReadWithHeader(this Stream stream)
        {
            var header = new byte[4];
            stream.ReadExact(header, 0, 4);
            var length = BitConverter.ToInt32(header, 0);

            var bytes = new byte[length];
            stream.ReadExact(bytes, 0, length);
            return bytes;
        }

        public static void Write(this Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   Writes a message to the stream, prepending a fixed size header (4 bytes) which describes the size of the rest of the message
        /// </summary>
        public static void WriteWithHeader(this Stream stream, byte[] bytes, int offset, int length)
        {
            var header = (length - offset).AsByteArray();
            stream.Write(header);
            stream.Write(bytes, offset, length);
        }

        /// <summary>
        ///   Writes a message to the stream, prepending a fixed size header (4 bytes) which describes the size of the rest of the message
        /// </summary>
        public static void WriteWithHeader(this Stream stream, byte[] bytes)
        {
            stream.WriteWithHeader(bytes, 0, bytes.Length);
        }
    }
}