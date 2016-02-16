using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using GameCreator;

namespace FinalProjectServer
{
    public partial class ServerForm : Form
    {
        private const int UPDATE_RATE = 60;
        private const int PORT = 54446;

        private UdpClient server;
        private Thread listenThread;
        private Thread updateThread;

        private ConcurrentQueue<Packet> outboundPacketQueue = new ConcurrentQueue<Packet>();
        private ConcurrentDictionary<PlayerInfo, ConcurrentQueue<PacketData>> connectedUsers = new ConcurrentDictionary<PlayerInfo, ConcurrentQueue<PacketData>>();
        private List<GameTeam> teamList = new List<GameTeam>();
        private List<Bullet> bulletList = new List<Bullet>();

        private int idCount;
        private int bulletIdCount;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            teamList.Add(new GameTeam(0));
            teamList.Add(new GameTeam(1));
            teamList.Add(new GameTeam(2));
            teamList.Add(new GameTeam(3));
        }

        private void Listen(IPEndPoint client)
        {
            string receivedMessage;
            byte[] receivedBytes;

            Line("Waiting for connection...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (true)
            {
                try
                {
                    receivedBytes = server.Receive(ref client);
                    receivedMessage = Encoding.ASCII.GetString(receivedBytes);

                    PacketData packet = Packet.Parse(receivedMessage);

                    if (packet != null && !(packet.Packet is PacketLogin))
                    {
                        foreach (var player in connectedUsers)
                        {
                            if (player.Key.Address.Equals(client.Address) && player.Key.Port.Equals(client.Port))
                            {
                                player.Value.Enqueue(packet);
                            }
                        }
                    }

                    if (packet.Packet is PacketLogin)
                    {
                        foreach (var team in teamList)
                        {
                            foreach (var teamPlayer in team.Players)
                            {
                                if (!connectedUsers.Keys.Contains(teamPlayer))
                                {
                                    team.Players.Remove(teamPlayer);
                                    break;
                                }
                            }
                        }

                        idCount++;
                        Player playerObj = new Player(idCount, 0, 0, 80, 80);
                        playerObj.Team = teamList[GetLowestTeamIndex()];
                        playerObj.Name = packet.Parameters[0].ToString();

                        if (playerObj.Team.Identifier == 0)
                        {
                            playerObj.Area.X = World.RedSpawn.X;
                            playerObj.Area.Y = World.RedSpawn.Y;
                        }
                        if (playerObj.Team.Identifier == 1)
                        {
                            playerObj.Area.X = World.BlueSpawn.X;
                            playerObj.Area.Y = World.BlueSpawn.Y;
                        }
                        else if (playerObj.Team.Identifier == 2)
                        {
                            playerObj.Area.X = World.GreenSpawn.X;
                            playerObj.Area.Y = World.GreenSpawn.Y;
                        }
                        else if (playerObj.Team.Identifier == 3)
                        {
                            playerObj.Area.X = World.YellowSpawn.X;
                            playerObj.Area.Y = World.YellowSpawn.Y;
                        }

                        PlayerInfo player = new PlayerInfo(client.Address, client.Port, packet.Parameters[0].ToString(), playerObj);
                        teamList[GetLowestTeamIndex()].Players.Add(player);

                        List<Player> mObjects = new List<Player>();
                        foreach (var item in connectedUsers)
                        {
                            mObjects.Add(item.Key.Object);
                            outboundPacketQueue.Enqueue(new PacketAddPlayer(player.Object, new IPEndPoint(item.Key.Address, item.Key.Port)));
                            outboundPacketQueue.Enqueue(new PacketAddPlayer(item.Key.Object, client));
                        }

                        AddPlayer(player, packet);
                        Line("Connected with player " + player.Username);

                        outboundPacketQueue.Enqueue(new PacketID(idCount, playerObj.Team, new IPEndPoint(player.Address, player.Port)));
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private int GetLowestTeamIndex()
        {
            GameTeam lowest = null;
            foreach (var team in teamList)
            {
                if (lowest == null)
                    lowest = team;
                else if (lowest.Players.Count > team.Players.Count)
                    lowest = team;
            }
            return teamList.IndexOf(lowest);
        }

        private void HandlePacket(PlayerInfo player, PacketData packet)
        {
            if (packet.Packet is PacketPing)
            {
                foreach (var user in connectedUsers)
                {
                    if (user.Key.Address == player.Address && user.Key.Port == player.Port)
                    {
                        user.Key.Restart = true;
                    }
                }
                player.Stopwatch.Reset();
            }

            if (packet.Packet is PacketDisconnect)
            {
                RemovePlayer(player);
                foreach (var user in connectedUsers)
                {
                    outboundPacketQueue.Enqueue(new PacketDisconnect(player.Object.ID, new IPEndPoint(user.Key.Address, user.Key.Port)));
                }
                if (player != null)
                    Line("Disconnected with user " + player.Username);
            }

            if (packet.Packet is PacketMove)
            {
                PacketMove move = (PacketMove)packet.Packet;
                foreach (var item in connectedUsers)
                {
                    if (item.Key.Address == player.Address && item.Key.Port == player.Port && item.Key.Object.ID == move.ID && item.Key.Object.Health > 0)
                        item.Key.Object.Area = new RectangleFloat(move.NewPosition.X, move.NewPosition.Y, move.NewPosition.Width, move.NewPosition.Height);

                    if (item.Key.Object.Health > 0)
                        outboundPacketQueue.Enqueue(new PacketMove(move.ID, move.NewPosition, new IPEndPoint(item.Key.Address, item.Key.Port)));
                }
            }

            if (packet.Packet is PacketShoot)
            {
                PacketShoot shoot = (PacketShoot)packet.Packet;
                shoot.Object.ID = ++bulletIdCount;
                bulletList.Add(shoot.Object);

                foreach (var item in connectedUsers)
                {
                    outboundPacketQueue.Enqueue(new PacketShoot(shoot.Object, new IPEndPoint(item.Key.Address, item.Key.Port)));
                }

                shoot.Object.Shoot();
            }
        }

        private void AddPlayer(PlayerInfo player, PacketData loginPacket)
        {
            var packets = new ConcurrentQueue<PacketData>();
            connectedUsers.AddOrUpdate(player, packets, (oldKey, oldValue) => packets);
        }

        private void RemovePlayer(PlayerInfo player)
        {
            try
            {
                ConcurrentQueue<PacketData> ignoreForGarbage;
                connectedUsers.TryRemove(player, out ignoreForGarbage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
            }
        }

        private void UpdateServer()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (true)
            {
                if (sw.ElapsedMilliseconds >= 1000 / UPDATE_RATE - 1)
                {
                    foreach (var bullet in bulletList)
                    {
                        bullet.Update();
                    }
                    foreach (var bullet in bulletList)
                    {
                        if (!bullet.InTranslation)
                        {
                            bulletList.Remove(bullet);
                            break;
                        }
                    }

                    foreach (var player in connectedUsers)
                    {
                        if (player.Key.Object.Team.Identifier == -1)
                        {

                        }
                        for (int i = 0; i < player.Value.Count; i++)
                        {
                            PacketData packet;
                            player.Value.TryDequeue(out packet);
                            HandlePacket(player.Key, packet);
                        }
                        foreach (var bullet in bulletList)
                        {
                            if (bullet.IsCollided(World.ObjectList.Keys.ToArray()))
                            {
                                bulletList.Remove(bullet);
                                break;
                            }
                            Player shooter = null;
                            foreach (var item in connectedUsers)
                            {
                                if (item.Key.Object.ID == bullet.Shooter)
                                    shooter = item.Key.Object;
                            }
                            if (shooter != null && player.Key.Object.ID != bullet.Shooter && player.Key.Object.Team != shooter.Team && player.Key.Object.IsCollided(bullet))
                            {
                                foreach (var p in connectedUsers)
                                {
                                    outboundPacketQueue.Enqueue(new PacketHit(bullet.ID, bullet.BulletType, player.Key.Object.ID, new IPEndPoint(p.Key.Address, p.Key.Port)));
                                }
                                player.Key.Object.Health -= Bullet.GetDamage(bullet.BulletType);
                                bulletList.Remove(bullet);
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < outboundPacketQueue.Count; i++)
                    {
                        Packet toSend;
                        outboundPacketQueue.TryDequeue(out toSend);
                        if (toSend != null)
                        {
                            toSend.SendPacket(server);
                        }
                    }

                    sw.Reset();
                    sw.Start();
                }
            }
        }

        private void Line(string s)
        {
            this.Invoke((MethodInvoker)delegate { AddToConsole(s); });
        }

        private void AddToConsole(string s)
        {
            consoleBox.Text += consoleBox.Text == "" ? s : Environment.NewLine + s;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (startButton.Text == "Stop")
            {
                this.Dispose();
                this.Close();
                Application.Exit();
                return;
            }

            server = new UdpClient(PORT);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                listenThread = new Thread(() => Listen(endPoint));
                listenThread.IsBackground = true;
                listenThread.Start();

                updateThread = new Thread(UpdateServer);
                updateThread.IsBackground = true;
                updateThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException());
            }

            startButton.Text = "Stop";
        }
    }
}
