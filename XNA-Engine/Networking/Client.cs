using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Engine.Networking.Packets;
using Engine.Utility;

namespace Engine.Networking
{
    /// <summary>
    ///   Attempts to abstract the network layer of the TcpClient,
    ///   so that this is simply a read/write container.
    ///   Manages concurrent read/writes using ConcurrentQueues
    /// </summary>
    public class Client
    {
        private readonly ConcurrentQueue<byte[]> _readQueue;
        private readonly Thread _readThread;
        private readonly ConcurrentQueue<byte[]> _writeQueue;
        private readonly Thread _writeThread;
        private readonly Func<byte[], Packet> _buildPacket;
 
        /// <summary>
        ///   Construct a client that manages concurrent reads/writes to a TcpClient
        ///   Default behavior starts the client immediately
        /// </summary>
        /// <param name="baseClient"> </param>
        /// <param name="start"> </param>
        public Client(TcpClient baseClient, Func<byte[], Packet> packetBuildFunc, bool start = true)
        {
            TcpClient = baseClient;
            IsAlive = false;

            _readQueue = new ConcurrentQueue<byte[]>();
            _writeQueue = new ConcurrentQueue<byte[]>();
            _readThread = new Thread(ReadLoop);
            _writeThread = new Thread(WriteLoop);
            _buildPacket = packetBuildFunc;
            if (start) Start();
        }

        /// <summary>
        ///   Underlying TcpClient
        /// </summary>
        public TcpClient TcpClient { get; protected set; }

        /// <summary>
        ///   Whether there are pending messages to be read.
        /// </summary>
        public bool HasQueuedReadMessages
        {
            get { return _readQueue.Count > 0; }
        }

        /// <summary>
        ///   Whether the underlying TcpClient is still alive,
        ///   and handling reads/writes
        /// </summary>
        public bool IsAlive { get; protected set; }

        /// <summary>
        ///   Returns the underlying TcpClient's IPAddress as a string
        /// </summary>
        public string IpString
        {
            get { return TcpClient.GetIP(); }
        }

        /// <summary>
        ///   Writes a message to the client.
        ///   Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="buffer"> </param>
        public void Write(byte[] buffer)
        {
            _writeQueue.Enqueue(buffer.Copy());
        }

        /// <summary>
        ///   Writes a packet to the client.
        ///   Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="packet"> </param>
        public void WritePacket(Packet packet)
        {
            Write(packet.ByteStream);
        }

        /// <summary>
        ///   Tries to read a message from the client.
        ///   Returns null if there is no pending message from the client.
        ///   Messages to be read are enqueued by a worker thread and added in the order they are received
        /// </summary>
        public byte[] Read()
        {
            byte[] buffer;
            return _readQueue.TryDequeue(out buffer) ? buffer : null;
        }

        /// <summary>
        ///   Tries to read a message from the client.
        ///   Returns an empty packet if there is no pending message from the client.
        ///   Messages to be read are enqueued by a worker thread and added in the order they are received
        /// </summary>
        /// <returns> </returns>
        public Packet ReadPacket()
        {
            var buffer = Read();
            return buffer != null ? _buildPacket(buffer) : Packet.EmptyPacket;
        }

        /// <summary>
        ///   Start reading/writing
        /// </summary>
        public void Start()
        {
            IsAlive = true;
            _readThread.Start();
            _writeThread.Start();
        }

        /// <summary>
        ///   Close the TcpClient and its underlying NetworkStream
        /// </summary>
        public void Close()
        {
            TcpClient.GetStream().Close();
            TcpClient.Close();
        }

        private void ReadLoop()
        {
            var stream = TcpClient.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    var buffer = stream.ReadWithHeader();
                    if (buffer == null) continue;
                    _readQueue.Enqueue(buffer);
                }
                catch
                {
                    break;
                }
            }
            IsAlive = false;
        }

        private void WriteLoop()
        {
            var stream = TcpClient.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    byte[] buffer;
                    var needsWrite = _writeQueue.TryDequeue(out buffer);
                    if (needsWrite) stream.WriteWithHeader(buffer, 0, buffer.Length);
                }
                catch
                {
                    break;
                }
            }
            IsAlive = false;
        }
    }
}