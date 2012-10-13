using System;
using Engine.DataStructures;
using Engine.Serialization;

namespace Engine.Networking.Packets
{
    /// <summary>
    ///   Wrapper around the byte array that is sent to/read from a network stream
    /// </summary>
    public abstract class Packet : ByteSerializeable
    {
        public static Func<string, int> GetTypeFunction;
        public static Func<int, string> GetNameFunction;
        public static Func<byte[], Packet> BuildPacketFunction;
        private static Packet _nullPacket;

        public int Type
        {
            get { return GetTypeFunction(GetType().Name); }
        }

        /// <summary>
        ///   A packet with no body, and type code 0
        /// </summary>
        public static Packet EmptyPacket
        {
            get { return _nullPacket ?? (_nullPacket = new NullPacket()); }
        }

        /// <summary>
        ///   Determines whether the specified object is equal to this one
        /// </summary>
        /// <param name="obj"> </param>
        /// <returns> </returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return AsByteArray().GetHashCode();
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            builder.Add(Type);
        }


        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            // Type
            reader.ReadInt32();
            return reader.Index;
        }

        #region Nested type: NullPacket

        private class NullPacket : Packet
        {
        }

        #endregion
    }
}