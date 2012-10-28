using System;
using System.Collections.Generic;
using System.Linq;
using Engine.DataStructures;
using Engine.Utility;

namespace Engine.Networking.Packets
{
    public class PacketBuilder
    {
        private readonly BidirectionalDict<string, int> _mapping = new BidirectionalDict<string, int>();
        private readonly Dictionary<string, Packet> _packetCache = new Dictionary<string, Packet>();
        private int _nextType;

        public PacketBuilder()
        {
            RegisterPacket(Packet.EmptyPacket);
        }

        public void RegisterPacket(Packet packet)
        {
            var name = packet.GetType().Name;
            _packetCache[name] = packet.Copy();
            _mapping[name] = _nextType++;
        }

        public void RegisterPackets(params Packet[] packets)
        {
            packets.Each(RegisterPacket);
        }

        public int TypeFromName(string packetName)
        {
            return _mapping.Contains(packetName) ? _mapping[packetName] : 0;
        }

        public string NameFromType(int type)
        {
            return _mapping.Contains(type) ? _mapping[type] : _mapping[0];
        }

        public Packet BuildFrom(byte[] bytes)
        {
            var reader = new ByteArrayReader(bytes, 0);
            var typeInt = reader.ReadInt32();
            var name = NameFromType(typeInt);
            var packet = _packetCache[name].Copy();
            packet.FromByteArray(bytes, 0);
            return packet;
        }
    }
}