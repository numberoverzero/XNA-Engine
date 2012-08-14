using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Engine.Utility;
using Engine.Networking.Packets;

namespace Engine.Networking
{
    /// <summary>
    /// Attempts to abstract the network layer of the TcpClient,
    /// so that this is simply a read/write container.
    /// Manages concurrent read/writes using ConcurrentQueues
    /// </summary>
    public class Client
    {
        Thread readThread, writeThread;
        ConcurrentQueue<byte[]> readQueue, writeQueue;
        /// <summary>
        /// Underlying TcpClient
        /// </summary>
        public TcpClient TcpClient { get; protected set; }

        /// <summary>
        /// Construct a client that manages concurrent reads/writes to a TcpClient
        /// Default behavior starts the client immediately
        /// </summary>
        /// <param name="baseClient"></param>
        /// <param name="start"></param>
        public Client(TcpClient baseClient, bool start = true)
        {
            TcpClient = baseClient;
            IsAlive = false;

            readQueue = new ConcurrentQueue<byte[]>();
            writeQueue = new ConcurrentQueue<byte[]>();
            readThread = new Thread(new ThreadStart(ReadLoop));
            writeThread = new Thread(new ThreadStart(WriteLoop));
            if (start) Start();
        }

        /// <summary>
        /// Writes a message to the client.
        /// Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="buffer"></param>
        public void Write(byte[] buffer)
        {
            writeQueue.Enqueue(buffer.Copy());
        }

        /// <summary>
        /// Writes a packet to the client.
        /// Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="packet"></param>
        public void WritePacket(Packet packet)
        {
            Write(packet.ByteStream);
        }

        /// <summary>
        /// Tries to read a message from the client.
        /// Returns null if there is no pending message from the client.
        /// Messages to be read are enqueued by a worker thread and added in the order they are received
        /// </summary>
        public byte[] Read()
        {
            byte[] buffer;
            bool hasMsg = readQueue.TryDequeue(out buffer);
            return hasMsg ? buffer : null;
        }

        /// <summary>
        /// Tries to read a message from the client.
        /// Returns an empty packet if there is no pending message from the client.
        /// Messages to be read are enqueued by a worker thread and added in the order they are received
        /// </summary>
        /// <returns></returns>
        public Packet ReadPacket()
        {
            byte[] buffer = Read();
            if (buffer != null)
                return Packet.Parse(buffer);
            return Packet.EmptyPacket;
        }

        /// <summary>
        /// Start reading/writing
        /// </summary>
        public void Start()
        {
            IsAlive = true;
            readThread.Start();
            writeThread.Start();
        }

        /// <summary>
        /// Close the TcpClient and its underlying NetworkStream
        /// </summary>
        public void Close()
        {
            TcpClient.GetStream().Close();
            TcpClient.Close();
        }

        /// <summary>
        /// Whether there are pending messages to be read.
        /// </summary>
        public bool HasQueuedReadMessages
        {
            get { return readQueue.Count > 0; }
        }

        /// <summary>
        /// Whether the underlying TcpClient is still alive,
        /// and handling reads/writes
        /// </summary>
        public bool IsAlive { get; protected set; }

        /// <summary>
        /// Returns the underlying TcpClient's IPAddress as a string
        /// </summary>
        public string IPString { get { return TcpClient.GetIP(); } }

        void ReadLoop()
        {
            byte[] buffer;
            var stream = TcpClient.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    buffer = stream.ReadWithHeader();
                    if (buffer == null) continue;
                    readQueue.Enqueue(buffer);
                }
                catch { break; }
            }
            IsAlive = false;
        }
        void WriteLoop()
        {
            bool needsWrite;
            byte[] buffer;
            var stream = TcpClient.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    needsWrite = writeQueue.TryDequeue(out buffer);
                    if (needsWrite) stream.WriteWithHeader(buffer, 0, buffer.Length);
                }
                catch { break; }
            }
            IsAlive = false;
        }

    }
}
