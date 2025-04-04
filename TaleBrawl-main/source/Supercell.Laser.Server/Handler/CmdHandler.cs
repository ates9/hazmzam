namespace Supercell.Laser.Server.Handler
{
    using Discord;
    using Supercell.Laser.Logic.Avatar;
    using Supercell.Laser.Logic.Command.Home;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Data.Helper;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Logic.Home.Gatcha;
    using Supercell.Laser.Logic.Home.Items;
    using Supercell.Laser.Logic.Listener;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Message.Home;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Cache;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;
    using System.Diagnostics;
    using System.Reflection;
    using Ubiety.Dns.Core.Records;

    public static class CmdHandler
    {
        public static void Start()
        {
            while (true)
            {
                try
                {
                    string cmd = Console.ReadLine();
                    if (cmd == null) continue;
                    if (!cmd.StartsWith("/")) continue;
                    cmd = cmd.Substring(1);
                    string[] args = cmd.Split(" ");
                    if (args.Length < 1) continue;
                    switch (args[0])
                    {
                        case "premium":
                            ExecuteGivePremiumToAccount(args);
                            break;
                        case "ban":
                            ExecuteBanAccount(args);
                            break;
                        case "unban":
                            ExecuteUnbanAccount(args);
                            break;
                        case "changename":
                            ExecuteChangeNameForAccount(args);
                            break;
                        case "getvalue":
                            ExecuteGetFieldValue(args);
                            break;
                        case "changevalue":
                            ExecuteChangeValueForAccount(args);
                            break;
                        case "unlockall":
                            ExecuteUnlockAllForAccount(args);
                            break;
                        case "unlockallskins":
                            ExecuteUnlockAllSkinsForAccount(args);
                            break;
                        case "maintenance":
                            Console.WriteLine("Starting maintenance...");
                            ExecuteShutdown();
                            Console.WriteLine("Maintenance started!");
                            break;
                    }
                }
                catch (Exception) { }
            }
        }

        public static void ExecuteUnlockAllForAccount(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: /unlockall [TAG]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
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
        public static void ExecuteUnlockAllSkinsForAccount(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: /unlockall [TAG]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
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

        private static void ExecuteGivePremiumToAccount(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: /premium [TAG]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            account.Avatar.IsPremium = true;
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

        private static void ExecuteUnbanAccount(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: /unban [TAG]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            account.Avatar.Banned = false;
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

        private static void ExecuteBanAccount(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: /ban [TAG]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            account.Avatar.Banned = true;
            //account.Avatar.ResetTrophies();
            account.Avatar.Name = "Brawler";
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

        private static void ExecuteChangeNameForAccount(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: /changevalue [TAG] [NewName]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            account.Avatar.Name = args[2];
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

        private static void ExecuteGetFieldValue(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: /changevalue [TAG] [FieldName]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            Type type = typeof(ClientAvatar);
            FieldInfo field = type.GetField(args[2]);
            if (field == null)
            {
                Console.WriteLine($"Fail: LogicClientAvatar::{args[2]} not found!");
                return;
            }

            int value = (int)field.GetValue(account.Avatar);
            Console.WriteLine($"LogicClientAvatar::{args[2]} = {value}");
        }

        private static void ExecuteChangeValueForAccount(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: /changevalue [TAG] [FieldName] [Value]");
                return;
            }

            long id = LogicLongCodeGenerator.ToId(args[1]);
            Account account = Accounts.Load(id);
            if (account == null)
            {
                Console.WriteLine("Fail: account not found!");
                return;
            }

            Type type = typeof(ClientAvatar);
            FieldInfo field = type.GetField(args[2]);
            if (field == null)
            {
                Console.WriteLine($"Fail: LogicClientAvatar::{args[2]} not found!");
                return;
            }

            field.SetValue(account.Avatar, int.Parse(args[3]));
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

        private static void ExecuteShutdown()
        {
            Sessions.StartShutdown();
            AccountCache.SaveAll();
            AllianceCache.SaveAll();

            AccountCache.Started = false;
            AllianceCache.Started = false;
        }
    }
}
