using System;
using Engine.Networking.Packets;

namespace Engine.Networking
{
    public class PacketArgs : EventArgs
    {
        public Packet Packet;

        public PacketArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}