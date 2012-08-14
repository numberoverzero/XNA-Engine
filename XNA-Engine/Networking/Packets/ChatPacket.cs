using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Utility;

namespace Engine.Networking.Packets
{
    /// <summary>
    /// A packet containing fields for a message and who the message is to and from
    /// </summary>
    public class ChatPacket : Packet
    {
        /// <summary>
        /// The message being sent
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Who the message is from
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Who the message is to
        /// </summary>
        public string To { get; set; }

        StringBuilder builder;

        /// <summary>
        /// See <see cref="Packet.ByteStream"/>
        /// </summary>
        public override byte[] ByteStream
        {
            get 
            {
                builder = new StringBuilder(3);
                builder.Append(string_fmt.format(Message));
                builder.Append(string_fmt.format(From));
                builder.Append(string_fmt.format(To));
                string msg = builder.ToString();
                int msgSize = msg.ByteCount(Encoding.UTF8);
                byte[] buffer = new byte[msgSize + 4];
                Array.Copy(fromInt(1), buffer, 4);
                Array.Copy(msg.GetBytes(Encoding.UTF8), 0, buffer, 4, msgSize);
                return buffer;
            }
        }

        /// <summary>
        /// See <see cref="Packet.LoadFromBuffer"/>
        /// </summary>
        /// <param name="buffer"></param>
        protected override void LoadFromBuffer(byte[] buffer)
        {
            string rawText = Encoding.UTF8.GetString(buffer, 4, buffer.Length - 4);
            string[] pieces = rawText.Split('\0');
            if (pieces.Length != 4) return;
            Message = pieces[0];
            From = pieces[1];
            To = pieces[2];
        }
    }
}
