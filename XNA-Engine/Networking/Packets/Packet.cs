using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.DataStructures;
using Engine.Serialization;
using Engine.Utility;

namespace Engine.Networking.Packets
{
    /// <summary>
    /// Wrapper around the byte array that is sent to/read from a network stream
    /// </summary>
    public abstract class Packet : IByteSerializeable
    {
        public static Func<string, int> GetTypeFunction;
        public static Func<int, string> GetNameFunction;
 
        public int Type
        {
            get { return GetTypeFunction(GetType().Name); }
        }

        /// <summary>
        /// Determines whether the specified object is equal to this one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return AsByteArray().GetHashCode();
        }

        private class NullPacket : Packet
        {
            /// <summary>
            /// An empty packet does not load any data
            /// </summary>
            /// <param name="buffer"></param>
            public void LoadFromBuffer(byte[] buffer) { }

            public override byte[] AsByteArray()
            {
                return Type.AsByteArray();
            }

            /// <summary>
            /// <para>
            /// Returns the position of the last character of the object in the byte array.
            /// </para>
            /// <para>
            /// Returns a number less than startIndex if the object does not start at the given index.
            /// </para>
            /// </summary>
            /// <param name="bytes"/><param name="startIndex"/>
            /// <returns/>
            public override int FromByteArray(byte[] bytes, int startIndex)
            {
                var b = new ByteArrayReader(bytes, startIndex);
                var type = b.ReadInt32();
                var typeName = GetNameFunction(type);
                if (typeName != GetType().Name) return -1;
                return startIndex + 4;
            }
        }

        static Packet _nullPacket;
        /// <summary>
        /// A packet with no body, and type code 0
        /// </summary>
        public static Packet EmptyPacket
        {
            get { return _nullPacket ?? (_nullPacket = new NullPacket()); }
        }

        public abstract byte[] AsByteArray();

        public abstract int FromByteArray(byte[] bytes, int startIndex);
    }

}
