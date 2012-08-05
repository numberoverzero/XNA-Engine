using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Engine.Utility;

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
        ConcurrentQueue<string> readQueue, writeQueue;
        TcpClient client;

        /// <summary>
        /// Construct a client that manages concurrent reads/writes to a TcpClient
        /// </summary>
        /// <param name="baseClient"></param>
        public Client(TcpClient baseClient)
        {
            client = baseClient;
            IsAlive = false;

            readQueue = new ConcurrentQueue<string>();
            writeQueue = new ConcurrentQueue<string>();
            readThread = new Thread(new ThreadStart(ReadLoop));
            writeThread = new Thread(new ThreadStart(WriteLoop));
        }

        /// <summary>
        /// Writes a message to the client.
        /// Messages are queued and a thread proccesses them in the order they were enqueued
        /// </summary>
        /// <param name="msg"></param>
        public void Write(string msg)
        {
            writeQueue.Enqueue(msg);
        }

        /// <summary>
        /// Tries to read a message from the client.
        /// Returns null if there is no pending message from the client.
        /// Messages to be read are enqueued by a worker thread and added in the order they are received
        /// </summary>
        public string Read()
        {
            string msg;
            bool hasMsg = readQueue.TryDequeue(out msg);
            return hasMsg ? msg : null;
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
            client.GetStream().Close();
            client.Close();
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

        void ReadLoop()
        {
            string msg;
            var stream = client.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    msg = stream.ReadStringWithHeader();
                    readQueue.Enqueue(msg);
                }
                catch { break; }
            }
            IsAlive = false;
        }
        void WriteLoop()
        {
            bool needsWrite;
            string msg;
            var stream = client.GetStream();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    needsWrite = writeQueue.TryDequeue(out msg);
                    if (needsWrite) stream.WriteWithHeader(msg);
                }
                catch { break; }
            }
            IsAlive = false;
        }

    }
}
