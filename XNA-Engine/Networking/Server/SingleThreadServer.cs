using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using Engine.Utility;

namespace Engine.Networking.Server
{
    /// <summary>
    ///   <para> Has a single thread that queues messages from clients, and a single thread that processes those messages. </para>
    ///   <para> Although this will incur a performance hit, it means that we don't have to worry about the synchronization of operations </para>
    /// </summary>
    public class SingleThreadServer : BasicServer
    {
        private Thread _readThread;
        private readonly ConcurrentQueue<PacketArgs> _readQueue;
  
        public SingleThreadServer(IPAddress localaddr, int port, string logFileName = null, bool tailLog = true)
            : base(localaddr, port, logFileName, tailLog)
        {
            _readQueue = new ConcurrentQueue<PacketArgs>();
        }

        protected override void OnClientRead(object sender, PacketArgs args)
        {
            var client = args.Client;
            if (client == null) return;

            if (!ClientTable.Contains(client))
                // Don't waste our time reading from clients that aren't tracked in the client table
                client.OnReadPacket -= OnClientRead;
            else
                _readQueue.Enqueue(args);
        }

        private void ProcessQueuedReadPackets()
        {
            while(IsRunning)
            {
                Thread.Sleep(1);
                PacketArgs args;
                var hasRead = _readQueue.TryDequeue(out args);
                if(hasRead) ReceivePacket(args.Packet, args.Client);
            }
        }

        public override void Start()
        {
            base.Start();
            if (_readThread != null) _readThread.Kill();
            _readThread = new Thread(ProcessQueuedReadPackets);
            _readThread.Start();
        }
    }
}