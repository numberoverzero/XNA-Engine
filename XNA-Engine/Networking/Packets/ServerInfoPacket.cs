using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Utility;

namespace Engine.Networking.Packets
{
    /// <summary>
    /// Contains info about the server, such as IP address and server name
    /// </summary>
    public class ServerInfoPacket : Packet
    {
        /// <summary>
        /// Server IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Name of the server
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the server's capabilities, rules, etc
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Port the server sends/receives on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Current players connected (or taking up player slots, whatever that means for a server
        /// </summary>
        public int CurrentPlayers { get; set; }

        /// <summary>
        /// Maximum player slots or connections
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Whether the server requires a password
        /// </summary>
        public bool HasPassword { get; set; }

        StringBuilder builder;

        /// <summary>
        /// See <see cref="Packet.ByteStream"/>
        /// </summary>
        public override byte[] ByteStream
        {
            get {
                builder = new StringBuilder(3);
                builder.Append(string_fmt.format(IP));
                builder.Append(string_fmt.format(Name));
                builder.Append(string_fmt.format(Description));
                string msg = builder.ToString();
                int msgSize = msg.ByteCount(Encoding.UTF8);

                byte[] buffer = new byte[
                    4 // Packet Type
                    + 1 // bool fields
                    + 12 // int fields
                    + msgSize // strings
                    ];

                // Packet Type
                Array.Copy(fromInt(2), buffer, 4);
                // bool fields
                Array.Copy(BitConverter.GetBytes(HasPassword), 0, buffer, 4, 1);

                // int fields
                Array.Copy(BitConverter.GetBytes(Port), 0, buffer, 5, 4);
                Array.Copy(BitConverter.GetBytes(CurrentPlayers), 0, buffer, 9, 4);
                Array.Copy(BitConverter.GetBytes(MaxPlayers), 0, buffer, 13, 4);

                // string fields
                Array.Copy(msg.GetBytes(Encoding.UTF8), 0, buffer, 17, msgSize);

                return buffer;
            }
        }

        /// <summary>
        /// See <see cref="Packet.LoadFromBuffer"/>
        /// </summary>
        /// <param name="buffer"></param>
        protected override void LoadFromBuffer(byte[] buffer)
        {
            if (buffer.Length < 17) return;

            HasPassword = BitConverter.ToBoolean(buffer, 4);
            Port = BitConverter.ToInt32(buffer, 5);
            CurrentPlayers = BitConverter.ToInt32(buffer, 9);
            MaxPlayers = BitConverter.ToInt32(buffer, 13);
            
            string rawText = Encoding.UTF8.GetString(buffer, 17, buffer.Length - 17);
            string[] pieces = rawText.Split('\0');
            if (pieces.Length != 4) return;
            
            IP = pieces[0];
            Name = pieces[1];
            Description = pieces[2];
        }
    }
}
