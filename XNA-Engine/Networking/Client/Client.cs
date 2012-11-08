using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Engine.Networking.Packets;
using Engine.Utility;

namespace Engine.Networking
{
    /// <summary>
    ///   <para> Attempts to abstract the network layer of the TcpClient, so that this is simply a read/write container. </para>
    ///   <para> Manages concurrent read/writes using ConcurrentQueues </para>
    /// </summary>
    public class Client
    {
        private readonly ConcurrentQueue<byte[]> _readQueue;
        private readonly Thread _readThread;
        private readonly ConcurrentQueue<byte[]> _writeQueue;
        private readonly Thread _writeThread;

        /// <summary>
        ///   <para> Construct a client that manages concurrent reads/writes to a TcpClient </para>
        ///   <para> Default behavior starts the client immediately </para>
        /// </summary>
        public Client(TcpClient baseClient, bool start = true)
        {
            TcpClient = baseClient;
            IsAlive = false;

            _readQueue = new ConcurrentQueue<byte[]>();
            _writeQueue = new ConcurrentQueue<byte[]>();
            _readThread = new Thread(ReadLoop);
            _writeThread = new Thread(WriteLoop);
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
        ///   Whether the underlying TcpClient is still alive and handling reads/writes
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
        ///   Allows a push model so that others don't have to constantly ask the Client if it has packets to be read.
        /// </summary>
        public event EventHandler<PacketArgs> OnReadPacket;

        /// <summary>
        /// Not everyone checks IsAlive periodically - this allows the client to push a message saying it just died
        /// </summary>
        public event EventHandler OnConnectionLost;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Client) obj);
        }

        protected bool Equals(Client other)
        {
            return Equals(TcpClient, other.TcpClient);
        }

        public override int GetHashCode()
        {
            return (TcpClient != null ? TcpClient.GetHashCode() : 0);
        }

        /// <summary>
        ///   Writes a message to the client.
        ///   Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        public void Write(byte[] bytes, int sourceIndex, int length)
        {
            var buffer = new byte[length];
            Array.Copy(bytes, sourceIndex, buffer, 0, length);
            _writeQueue.Enqueue(buffer);
        }

        /// <summary>
        ///   <para> Writes a message to the client. </para>
        ///   para>
        ///   <para> Messages are queued and a thread proccesses them in the order they were enqueued </para>
        /// </summary>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   <para> Writes a packet to the client. </para>
        ///   para>
        ///   <para> Messages are queued and a thread proccesses them in the order they were enqueued </para>
        /// </summary>
        public void WritePacket(Packet packet)
        {
            Write(packet.AsByteArray());
        }

        /// <summary>
        ///   <para> Tries to read a message from the client. </para>
        ///   <para> Returns null if there is no pending message from the client. </para>
        ///   <para> Messages to be read are enqueued by a worker thread and added in the order they are received </para>
        /// </summary>
        public byte[] Read()
        {
            byte[] buffer;
            return _readQueue.TryDequeue(out buffer) ? buffer : null;
        }

        /// <summary>
        ///   <para> Tries to read a message from the client as a Packet. </para>
        ///   <para> Returns null if there is no pending message from the client. </para>
        ///   <para> Messages to be read are enqueued by a worker thread and added in the order they are received </para>
        /// </summary>
        public Packet ReadPacket()
        {
            var bytes = Read();
            if (bytes == null) return null; // We read an empty byte steam

            var packet = Packet.Builder.BuildFrom(bytes);
            if (packet == null || packet.Equals(Packet.EmptyPacket))
                return null; // Unknown or poorly formed packet
            return packet;
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
            if (TcpClient.Connected)
            {
                TcpClient.GetStream().Close();
                TcpClient.Close();
            }
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
                    ReadEnqueue(bytes);
                }
                catch
                {
                    break;
                }
            }
            KillClient();
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
            KillClient();
        }

        private void KillClient()
        {
            if(OnConnectionLost != null) OnConnectionLost(this, null);
            IsAlive = false;
        }

        private void ReadEnqueue(byte[] bytes)
        {
            if(OnReadPacket == null)
            {
                _readQueue.Enqueue(bytes);
                return;
            }

            var packet = Packet.Builder.BuildFrom(bytes);
            OnReadPacket(this, new PacketArgs(packet, this));
        }
    }
}