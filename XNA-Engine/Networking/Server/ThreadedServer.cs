using System.Net;

namespace Engine.Networking.Server
{
    public class ThreadedServer : BasicServer
    {
        public ThreadedServer(IPAddress localaddr, int port, string logFileName = null)
            : base(localaddr, port, logFileName)
        {
        }

        protected override void OnClientRead(object sender, PacketArgs args)
        {
            var client = args.Client;
            if (client == null) return;

            if (!ClientTable.Contains(client))
                // Don't waste our time reading from clients that aren't tracked in the client table
                client.OnReadPacket -= OnClientRead;
            else
                ReceivePacket(args.Packet, client);
        }
    }
}