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
        private readonly Func<byte[], Packet> _buildPacket;
        private readonly ConcurrentQueue<byte[]> _readQueue;
        private readonly Thread _readThread;
        private readonly ConcurrentQueue<byte[]> _writeQueue;
        private readonly Thread _writeThread;

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
        public string GetIP
        {
            get { return TcpClient.GetIP(); }
        }

        /// <summary>
        ///   Writes a message to the client.
        ///   Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="bytes"> </param>
        /// <param name="sourceIndex"> </param>
        /// <param name="length"> </param>
        public void Write(byte[] bytes, int sourceIndex, int length)
        {
            var buffer = new byte[length];
            Array.Copy(bytes, sourceIndex, buffer, 0, length);
            _writeQueue.Enqueue(buffer);
        }

        /// <summary>
        ///   Writes a message to the client.
        ///   Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="bytes"> </param>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   Writes a packet to the client.
        ///   Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="packet"> </param>
        public void WritePacket(Packet packet)
        {
            Write(packet.AsByteArray());
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
            var bytes = Read();
            return bytes != null ? _buildPacket(bytes) : Packet.EmptyPacket;
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
            if (_writeThread.IsAlive) _writeThread.Kill();
            if (_readThread.IsAlive) _readThread.Kill();
            IsAlive = false;
        }

        private void ReadLoop()
        {
            var stream = TcpClient.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    var bytes = stream.ReadWithHeader();
                    if (bytes == null) continue;
                    _readQueue.Enqueue(bytes);
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
                    byte[] bytes;
                    var needsWrite = _writeQueue.TryDequeue(out bytes);
                    if (needsWrite) stream.WriteWithHeader(bytes);
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