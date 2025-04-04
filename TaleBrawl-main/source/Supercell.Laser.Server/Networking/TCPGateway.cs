namespace Supercell.Laser.Server.Networking
{
    using Supercell.Laser.Logic.Message;
    using Supercell.Laser.Server.Networking.Session;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;

    public static class TCPGateway
    {
        private static List<Connection> ActiveConnections;
        private static Socket Socket;
        private static Thread Thread;
        private static ManualResetEvent AcceptEvent;
        private static Timer affResetTimer;

        private static readonly string BlockedIpsFilePath = "blocked_ips.txt";
        private static readonly HashSet<string> BlockedIps = new HashSet<string>();
        private static readonly Dictionary<string, List<DateTime>> ConnectionAttempts = new Dictionary<string, List<DateTime>>();
        private static readonly int MaxAttempts = 150;
        private static readonly TimeSpan TimeWindow = TimeSpan.FromSeconds(5);

        public static void Init(string host, int port)
        {
            affResetTimer = new Timer(5000);
            affResetTimer.Elapsed += ResetAffCounter;
            affResetTimer.AutoReset = true;
            affResetTimer.Enabled = true;

            LoadBlockedIps();
            ActiveConnections = new List<Connection>();

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            Socket.Listen(10000);

            AcceptEvent = new ManualResetEvent(false);

            Thread = new Thread(TCPGateway.Update);
            Thread.Start();

            Logger.Print($"TCP Server started at {host}:{port}");
        }

        private static void ResetAffCounter(object sender, ElapsedEventArgs e)
        {
            ConnectionAttempts.Clear();
        }

        private static void Update()
        {
            while (true)
            {
                AcceptEvent.Reset();
                Socket.BeginAccept(new AsyncCallback(OnAccept), null);
                AcceptEvent.WaitOne();
            }
        }


        private static void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket client = Socket.EndAccept(ar);
                Connection connection = new Connection(client);
                ActiveConnections.Add(connection);
                Logger.Print("New connection!");
                Connections.AddConnection(connection);
                client.BeginReceive(connection.ReadBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceive), connection);
            }
            catch (Exception)
            {
                ;
            }

            AcceptEvent.Set();
        }

        private static void LoadBlockedIps()
        {
            if (File.Exists(BlockedIpsFilePath))
            {
                foreach (var line in File.ReadLines(BlockedIpsFilePath))
                {
                    BlockedIps.Add(line.Trim());
                }
            }
        }

        private static void SaveBlockedIp(string ip)
        {
            if (!BlockedIps.Contains(ip))
            {
                BlockedIps.Add(ip);
                File.AppendAllLines(BlockedIpsFilePath, new[] { ip });
                LoadBlockedIps();

            }
        }

        private static bool IsIpBlocked(string ip)
        {
            if (!ConnectionAttempts.ContainsKey(ip))
            {
                ConnectionAttempts[ip] = new List<DateTime>();
            }

            ConnectionAttempts[ip].Add(DateTime.UtcNow);

            ConnectionAttempts[ip].RemoveAll(attempt => (DateTime.UtcNow - attempt) > TimeWindow);

            if (ConnectionAttempts[ip].Count > MaxAttempts)
            {
                SaveBlockedIp(ip);
                Logger.Print($"IP {ip} has been banned.");
                return true;
            }

            return false;
        }

        private static void OnReceive(IAsyncResult ar)
        {
            Connection connection = (Connection)ar.AsyncState;
            if (connection == null) return;

            try
            {
                string clientIp = ((IPEndPoint)connection.Socket.RemoteEndPoint).Address.ToString();

                if (BlockedIps.Contains(clientIp) || IsIpBlocked(clientIp))
                {
                    Logger.Print("Blocked IP attempted connection: " + clientIp);
                    connection.Close();
                    return;
                }


                int r = connection.Socket.EndReceive(ar);
                if (r <= 0)
                {
                    Logger.Print("client disconnected.");
                    ActiveConnections.Remove(connection);
                    if (connection.MessageManager.HomeMode != null)
                    {
                        Sessions.Remove(connection.Avatar.AccountId);
                    }
                    connection.Close();
                    return;
                }

                if (r > 100000) //100kb
                {
                    Logger.Print($"IP {clientIp} büyük paket ban.");
                    SaveBlockedIp(clientIp);
                    BlockedIps.Add(clientIp);

                    connection.Close();
                    return;
                }
                // Logger.Print("Client IP: " + clientIp);

                connection.Memory.Write(connection.ReadBuffer, 0, r);
                if (connection.Messaging.OnReceive() != 0)
                {
                    ActiveConnections.Remove(connection);
                    if (connection.MessageManager.HomeMode != null)
                    {
                        Sessions.Remove(connection.Avatar.AccountId);
                    }
                    connection.Close();
                    Logger.Print("client disconnected.");
                    return;
                }

                connection.Socket.BeginReceive(connection.ReadBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceive), connection);
            }
            catch (SocketException)
            {
                ActiveConnections.Remove(connection);
                if (connection.MessageManager.HomeMode != null)
                {
                    Sessions.Remove(connection.Avatar.AccountId);
                }
                connection.Close();
                Logger.Print("client disconnected.");
            }
            catch (Exception exception)
            {
                connection.Close();
                Logger.Print("Unhandled exception: " + exception + ", trace: " + exception.StackTrace);
            }
        }

        public static void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndSend(ar);
            }
            catch (Exception)
            {
                ;
            }
        }
    }
}
