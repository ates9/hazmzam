﻿namespace Supercell.Laser.Server.Message
{
    using Supercell.Laser.Server.Networking;
    using Supercell.Laser.Server.Message;
    using System.Collections.Concurrent;
    using Supercell.Laser.Logic.Message;
    using System.Diagnostics;
    using static System.Net.Mime.MediaTypeNames;

    public static class Processor
    {
        private static ConcurrentQueue<QueueItem> IncomingQueue;
        private static ConcurrentQueue<QueueItem> OutgoingQueue;

        private static ManualResetEvent ReceiveEvent;
        private static ManualResetEvent SendEvent;

        private static Thread ReceiveThread;
        private static Thread SendThread;

        private struct QueueItem
        {
            public readonly Connection Connection;
            public readonly GameMessage Message;

            public QueueItem(Connection connection, GameMessage message)
            {
                Connection = connection;
                Message = message;
            }
        }

        public static void Init()
        {
            IncomingQueue = new ConcurrentQueue<QueueItem>();
            OutgoingQueue = new ConcurrentQueue<QueueItem>();

            ReceiveEvent = new ManualResetEvent(false);
            SendEvent = new ManualResetEvent(false);

            ReceiveThread = new Thread(UpdateReceive);
            SendThread = new Thread(UpdateSend);

            ReceiveThread.Start();
            SendThread.Start();
        }

        public static bool Receive(Connection connection, GameMessage message)
        {
            if (message == null) return false;

            if (IncomingQueue.Count >= 724)
            {
                int messageType = message.GetMessageType();
                Logger.Print($"Processor: Incoming message queue full. Removing messages of type {messageType}.");

                var tempList = new List<Processor.QueueItem>();

                while (IncomingQueue.TryDequeue(out var queueItem))
                {
                    if (queueItem.Message.GetMessageType() != messageType)
                    {
                        tempList.Add(queueItem);
                    }
                }

                foreach (var item in tempList)
                {
                    IncomingQueue.Enqueue(item);
                }

                if (IncomingQueue.Count >= 724)
                {
                    Logger.Print($"hay nk {messageType}.");
                    return false;
                }

                Logger.Print($"Processor:{messageType} kaldırıldı.");
            }


            IncomingQueue.Enqueue(new QueueItem(connection, message));
            ReceiveEvent.Set();
            return true;
        }

        public static void Send(Connection connection, GameMessage message)
        {
            if (message == null) return;

            if (OutgoingQueue.Count >= 1024)
            {
                return;
            }

            OutgoingQueue.Enqueue(new QueueItem(connection, message));
            SendEvent.Set();
        }

        public static void UpdateReceive()
        {
            while (true)
            {
                ReceiveEvent.WaitOne();

                while (IncomingQueue.TryDequeue(out QueueItem item))
                {
                    try
                    {
                        item.Connection.MessageManager.ReceiveMessage(item.Message);
                    }
                    catch (Exception) { }
                }

                ReceiveEvent.Reset();
            }
        }

        public static void UpdateSend()
        {
            while (true)
            {
                SendEvent.WaitOne();

                while (OutgoingQueue.TryDequeue(out QueueItem item))
                {
                    item.Connection.Messaging.EncryptAndWrite(item.Message);
                }

                SendEvent.Reset();
            }
        }
    }
}
