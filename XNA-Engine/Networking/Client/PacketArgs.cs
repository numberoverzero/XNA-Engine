using System;
using Engine.Networking.Packets;

namespace Engine.Networking
{
    public class PacketArgs : EventArgs
    {
        public Packet Packet;
        public Client Client;

        public PacketArgs(Packet packet, Client client)
        {
            Packet = packet;
            Client = client;
        }
    }
}