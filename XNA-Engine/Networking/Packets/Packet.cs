using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Utility;

namespace Engine.Networking.Packets
{
    /// <summary>
    /// Wrapper around the byte array that is sent to/read from a network stream
    /// </summary>
    public abstract class Packet
    {
        /// <summary>
        /// Constructs and returns the byte array that represents the packet
        /// </summary>
        public abstract byte[] ByteStream { get; }

        /// <summary>
        /// Load the data from a buffer according to the packet type's
        /// specification.  Buffer includes the four bytes that specify the type
        /// </summary>
        /// <param name="buffer"></param>
        protected abstract void LoadFromBuffer(byte[] buffer);

        /// <summary>
        /// Parses the packet type based on its first four bytes
        /// and returns an instance of that packet type with its appropriate values loaded.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Packet Parse(byte[] buffer)
        {
            if (buffer.Length < 4) return EmptyPacket;
            int type = BitConverter.ToInt32(buffer, 0);
            Packet packet;
            switch (type)
            {
                default:
                case 0:
                    packet = EmptyPacket;
                    break;
                case 1:
                    packet = new ChatPacket();
                    break;
                case 2:
                    packet = new ServerInfoPacket();
                    break;
            }
            packet.LoadFromBuffer(buffer);
            return packet;
        }

        /// <summary>
        /// Helper method for getting the byte array of an int
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected byte[] fromInt(int i) { return BitConverter.GetBytes(i); }

        /// <summary>
        /// Formats a string to be null-terminated
        /// </summary>
        protected const string string_fmt = "{0}\0";
        
        /// <summary>
        /// Determines whether the specified object is equal to this one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var pObj = obj as Packet;
            if(pObj == null) return false;
            return Equals(pObj);
        }

        private bool Equals(Packet packet)
        {
            var bytes = ByteStream;
            var otherBytes = packet.ByteStream;
            if (bytes.Length != otherBytes.Length) return false;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] != otherBytes[i]) return false;
            return true;
        }

        class NullPacket : Packet
        {
            /// <summary>
            /// Empty except for type code of 0
            /// </summary>
            public override byte[] ByteStream
            {
                get { return fromInt(0); }
            }
            /// <summary>
            /// An empty packet does not load any data
            /// </summary>
            /// <param name="buffer"></param>
            protected override void LoadFromBuffer(byte[] buffer) { }

            
        }

        static Packet _nullPacket;
        /// <summary>
        /// A packet with no body, and type code 0
        /// </summary>
        public static Packet EmptyPacket
        {
            get
            {
                if (_nullPacket == null) _nullPacket = new NullPacket();
                return _nullPacket;
            }
        }
    }    
}
