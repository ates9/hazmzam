using Discord.WebSocket;
using Discord;
using System.Net.NetworkInformation;
using Supercell.Laser.Logic.Command.Home;
using Supercell.Laser.Logic.Home;
using Supercell.Laser.Logic.Message.Home;
using Supercell.Laser.Logic.Util;
using Supercell.Laser.Server.Database.Models;
using Supercell.Laser.Server.Database;
using Supercell.Laser.Server.Networking.Session;
using Supercell.Laser.Server.Networking;
using Ubiety.Dns.Core;
using MySql.Data.MySqlClient;
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
//using Supercell.Laser.Logic.Message.Api;
using Ubiety.Dns.Core;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using MySqlX.XDevAPI.Relational;
using System.Security.Principal;
using Newtonsoft.Json;
using System.Text;
using MySqlX.XDevAPI;
using MySql.Data.MySqlClient.Memcached;
using Supercell.Laser.Logic.Notification;
using System.Net;
using Org.BouncyCastle.Asn1.X509;
using System.Globalization;
using Supercell.Laser.Server.Message;
using Supercell.Laser.Server.Handler;
using Supercell.Laser.Logic.Home.Gatcha;
using System.Drawing;
using Color = Discord.Color;
using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Drawing.Printing;
using System.Diagnostics.Metrics;
using static System.Net.Mime.MediaTypeNames;
using Ubiety.Dns.Core.Records.General;

namespace Supercell.Laser.Server
{
    public static class MyDiscordBot
    {
        public static HomeMode HomeMode;
        private static DiscordSocketClient _client;
        private static string prefix = "tb!";

        static void Main(string[] args) => MyDiscordBot.RunBotAsync().GetAwaiter().GetResult();

        private static ulong _roleId = 1272549674062184498;
        private static ulong _guildId = 1272547700587368520;



        private static readonly List<ulong> allowedUserIds = new List<ulong>
        {
            816413258260414524,
            1255457403114360963,
            1273310549345833000,
            789364673803190313,
            1232281411415969884, //hazar
            //1090293287505645638, // jakethedog
            //1258930652447768618,
            //789364673803190313,
            //1050467389382336522,
        };

        private static readonly List<ulong> allowedUserIdsGems = new List<ulong>
        {
            816413258260414524,
            1255457403114360963,
            1273310549345833000,
            789364673803190313,
            1232281411415969884, //hazar
           // 1090293287505645638 //jakethedog
            //1258930652447768618,
            //789364673803190313,
            //1050467389382336522,
        };

        public static async Task RunBotAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
            });



            _client.Log += Log;

            string token = "Bot_token_here";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += MessageReceivedAsync;

            #region button ve selectmenu events
            _client.ButtonExecuted += async (component) =>
            {
                try
                {
                    if (component.Data.CustomId.StartsWith("creatorlist_prev_") || component.Data.CustomId.StartsWith("creatorlist_next_"))
                    {
                        int currentPage = int.Parse(component.Data.CustomId.Split('_')[2]);

                        int newPage = component.Data.CustomId.StartsWith("creatorlist_prev_") ? currentPage - 1 : currentPage + 1;

                        var creators = Creators.GetAllCreators();
                        int itemsPerPage = 6;
                        int totalPages = (int)Math.Ceiling((double)creators.Count / itemsPerPage);

                        if (newPage < 1 || newPage > totalPages)
                        {
                            await component.RespondAsync("Geçersiz sayfa numarası!", ephemeral: true);
                            return;
                        }

                        int startIndex = (newPage - 1) * itemsPerPage;
                        var pageCreators = creators.Skip(startIndex).Take(itemsPerPage);

                        var embedBuilder = new EmbedBuilder()
                            .WithTitle("Tara Brawl Creators")
                            .WithColor(new Color(0, 255, 187))
                            .WithFooter($"Sayfa {newPage}/{totalPages}");

                        foreach (var creator in pageCreators)
                        {
                            embedBuilder.AddField(
                                $"<:CREATOR:1306984878939705394> {creator.Name}",
                                $"Kullanım Sayısı: {creator.UsageCount}",
                                true
                            );
                        }

                        var menuBuilder = new SelectMenuBuilder()
                            .WithCustomId("creatorlist_menu")
                            .WithPlaceholder("Bir creator seçin");

                        foreach (var creator in pageCreators)
                        {
                            menuBuilder.AddOption(new SelectMenuOptionBuilder()
                                .WithLabel(creator.Name)
                                .WithValue(creator.Name));
                        }

                        var buttons = new ComponentBuilder()
                            .WithButton("←", customId: $"creatorlist_prev_{newPage}", style: ButtonStyle.Primary, disabled: newPage == 1)
                            .WithButton("→", customId: $"creatorlist_next_{newPage}", style: ButtonStyle.Primary, disabled: newPage == totalPages)
                            .WithSelectMenu(menuBuilder);

                        await component.UpdateAsync(msg =>
                        {
                            msg.Embed = embedBuilder.Build();
                            msg.Components = buttons.Build();
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Button interaction error: {ex.Message}");
                    await component.RespondAsync("Bir hata oluştu!", ephemeral: true);
                }
            };


            _client.SelectMenuExecuted += async (component) =>
            {
                try
                {
                    if (component.Data.CustomId == "creatorlist_menu")
                    {
                        string selectedCreator = component.Data.Values.First();

                        await component.DeferAsync();

                        if (Creators.CreatorExists(selectedCreator))
                        {
                            long ids = Creators.GetCreatorIdByName(selectedCreator);
                            Account targetAccountGems = Accounts.Load(ids);
                            int thumbnailId = targetAccountGems.Home.ThumbnailId;
                            string thumbnaillink = "";
                            if (thumbnailId == 28000055) { thumbnaillink = "https://cdn.brawlify.com/profile-icons/regular/28000186.png";} else { thumbnaillink = $"https://cdn.brawlify.com/profile-icons/regular/{thumbnailId}.png"; }
                            
                            var embedBuilder = new EmbedBuilder()
                                .WithTitle("Creator Bilgileri")
                                .WithColor(Color.Blue)
                                .WithThumbnailUrl(thumbnaillink)
                                .AddField("Creator Ad", selectedCreator, true)
                                .AddField("Hesap Ad", targetAccountGems.Avatar.Name, true)
                                .AddField("Hesap ID", LogicLongCodeGenerator.ToCode(ids), true)
                                .AddField("Kullanım Sayısı", Creators.CreatorInfoByName(selectedCreator), true);

                            await component.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
                        }
                        else
                        {
                            await component.FollowupAsync($"{selectedCreator} adlı creator bulunamadı.", ephemeral: true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Menu interaction error: {ex.Message}");
                }
            };
            #endregion
            await Task.Delay(-1);
        }



        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
        [JsonProperty] public static List<OfferBundle> OfferBundles;


        private static async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            if (message is SocketUserMessage userMessage && !string.IsNullOrEmpty(userMessage.Content))
            {
                Console.WriteLine($"Message received: {userMessage.Content}");

                //MESAJ BİLGİLERİ
                string username = message.Author.Username;
                string userid = message.Author.Id.ToString();
                string mesajid = message.Id.ToString();


                if (userMessage.Content == prefix + "ping")
                {
                    await userMessage.Channel.SendMessageAsync("Pong!");
                }
                if (userMessage.Content.StartsWith(prefix + "addword"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string name = args[1];

                        var wordlist = new Wordlist("words.json");
                        if (wordlist.WordExists(name))
                        {
                            await userMessage.Channel.SendMessageAsync($"{name} zaten var");
                        }
                        else
                        {
                            wordlist.AddWord(name);
                            await userMessage.Channel.SendMessageAsync($"{name} eklendi");
                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addword kelime");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "deleteword"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string name = args[1];

                        var wordlist = new Wordlist("words.json");
                        if (wordlist.WordExists(name))
                        {
                            wordlist.DeleteWord(name);
                            await userMessage.Channel.SendMessageAsync($"{name} silindi");

                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync($"{name} kelimesi listede bulunamadı");
                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: deleteword kelime");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "addcreator"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string name = args[1];
                        string id = args[2];

                        long ids = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(ids);

                        Creators.AddCreatorByName(name, ids);
                        targetAccountGems.Avatar.ContentCreator = true;
                        await userMessage.Channel.SendMessageAsync($"{name} isimli creator eklendi. Creator Hesap ID: {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addcreator name #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "deletecreator"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string name = args[1];
                        if (Creators.CreatorExists(name))
                        {
                            Creators.DeleteCreatorByName(name);
                            long crid = Creators.GetCreatorIdByName(name);
                            Account targetAccountGems = Accounts.Load(crid);
                            targetAccountGems.Avatar.ContentCreator = false;
                            await userMessage.Channel.SendMessageAsync($"{name} isimli creator silindi.");
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync($"{name} isminde creator bulunamadı.");
                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: deletecreator name");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "creatorinfo"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string name = args[1];

                        if (Creators.CreatorExists(name))
                        {
                            long ids = Creators.GetCreatorIdByName(name);
                            Account targetAccountGems = Accounts.Load(ids);

                            int thumbnailId = targetAccountGems.Home.ThumbnailId;
                            string thumbnaillink = "";
                            if (thumbnailId == 28000055) { thumbnaillink = "https://cdn.brawlify.com/profile-icons/regular/28000186.png"; } else { thumbnaillink = $"https://cdn.brawlify.com/profile-icons/regular/{thumbnailId}.png"; }
                            var embedBuilder = new EmbedBuilder()
                              .WithTitle("Creator Bilgileri")
                              .WithColor(Color.Blue)
                              .WithThumbnailUrl(thumbnaillink);

                            embedBuilder.AddField("Creator Ad", name, true);
                            embedBuilder.AddField("Hesap Ad", targetAccountGems.Avatar.Name, true);
                            embedBuilder.AddField("Hesap ID", LogicLongCodeGenerator.ToCode(ids), true);
                            embedBuilder.AddField("Kullanım Sayısı", Creators.CreatorInfoByName(name), true);

                            await userMessage.Channel.SendMessageAsync(embed: embedBuilder.Build());
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("Bu creator bulunamadı.");
                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: creatorinfo name");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "creatorlist") || userMessage.Content.StartsWith(prefix + "creators"))
                {
                   /* if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }*/

                    int page = 1;
                    var args = userMessage.Content.Split(' ');
                    if (args.Length == 2 && int.TryParse(args[1], out int qqq))
                    {
                        page = qqq;
                    }

                    int itemlerporno = 6;
                    var creators = Creators.GetAllCreators();
                    int toplamsayfa = (int)Math.Ceiling((double)creators.Count / itemlerporno);

                    if (page < 1 || page > toplamsayfa)
                    {
                        return;
                    }

                    var embedBuilder = new EmbedBuilder()
                        .WithTitle("Tara Brawl Creators")
                        .WithColor(new Color(0, 255, 187));

                    int startIndex = (page - 1) * itemlerporno;
                    var sxxxxcreators = creators.Skip(startIndex).Take(itemlerporno);

                    foreach (var creator in sxxxxcreators)
                    {
                        embedBuilder.AddField(
                            $"<:CREATOR:1306984878939705394> {creator.Name}",
                            $"Kullanım Sayısı: {creator.UsageCount}",
                            true
                        );
                    }

                    embedBuilder.WithFooter($"Sayfa {page}/{toplamsayfa}");

                    var menuBuilder = new SelectMenuBuilder()
                        .WithCustomId("creatorlist_menu")
                        .WithPlaceholder("Bir creator seçin");

                    foreach (var creator in sxxxxcreators)
                    {
                        menuBuilder.AddOption(new SelectMenuOptionBuilder()
                            .WithLabel(creator.Name)
                            .WithValue(creator.Name)
                            //.WithEmote(new Emoji("<:CREATOR:1306984878939705394>"))
                        );
                    }

                    var buttons = new ComponentBuilder()
                        .WithButton("←", customId: $"creatorlist_prev_{page}", style: ButtonStyle.Primary, disabled: page == 1)
                        .WithButton("→", customId: $"creatorlist_next_{page}", style: ButtonStyle.Primary, disabled: page == toplamsayfa)
                        .WithSelectMenu(menuBuilder);

                    await userMessage.Channel.SendMessageAsync(embed: embedBuilder.Build(), components: buttons.Build());
                }

                if (userMessage.Content.StartsWith(prefix + "addbackup"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }
                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string name = args[1];

                        Random random = new Random();
                        string karakterler = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        string sayilarlakrakt = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                        string birinci = "";
                        for (int i = 0; i < 3; i++)
                        {
                            birinci += karakterler[random.Next(karakterler.Length)];
                        }

                        string ikinci = "";
                        for (int i = 0; i < 5; i++)
                        {
                            ikinci += sayilarlakrakt[random.Next(sayilarlakrakt.Length)];
                        }

                        string randomCode = $"{birinci}-{ikinci}";

                        File.WriteAllText(randomCode, name + "");
                        await userMessage.Channel.SendMessageAsync("TB ID backup oluşturuldu: `/backup " + randomCode + "`\nID:" + name);
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("...");
                    }

                }
                if (userMessage.Content.StartsWith(prefix + "status"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    long megabytesUsed = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);
                    DateTime now = Process.GetCurrentProcess().StartTime;
                    DateTime futureDate = DateTime.Now;

                    TimeSpan timeDifference = futureDate - now;

                    string formattedTime = string.Format("{0}{1}{2}{3}",
                    timeDifference.Days > 0 ? $"{timeDifference.Days} Days, " : string.Empty,
                    timeDifference.Hours > 0 || timeDifference.Days > 0 ? $"{timeDifference.Hours} Hours, " : string.Empty,
                    timeDifference.Minutes > 0 || timeDifference.Hours > 0 ? $"{timeDifference.Minutes} Minutes, " : string.Empty,
                    timeDifference.Seconds > 0 ? $"{timeDifference.Seconds} Seconds" : string.Empty);

                    string statusmessage = $"Tara Brawl Server Status:\n" +
                        $"Server Game Version: v29.258\n" +
                        $"Server Build: v1.2b from 03.06.2024\n" +
                        $"Resources Sha: {Fingerprint.Sha}\n" +
                        $"Environment: Prod\n" +
                        $"Server Time: {DateTime.Now} EEST\n" +
                        $"Players Online: {Sessions.Count}\n" +
                        $"Memory Used: {megabytesUsed} MB\n" +
                        $"Uptime: {formattedTime}\n";

                    await userMessage.Channel.SendMessageAsync(statusmessage);
                    
                }
                if (userMessage.Content.StartsWith(prefix + "gems"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"Gems Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = Convert.ToInt32(number),
                            MessageEntry = $"<c6>DC: {username} tarafından {number} elmas hediye edildi.</c>"
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

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1357657857356464128/nj0hutzpeg-1xgKHMa3ZB3dk4YbIsTcmg7osog4orFOJ9mY5r9Fp9p5res2CcCo7YI8n", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " elmas gönderdi.");
                        await userMessage.Channel.SendMessageAsync($"gems komutu test {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: gems #id number");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "dev"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                 
                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        if (targetAccountGems.Avatar.IsDev == true)
                        {
                            targetAccountGems.Avatar.IsDev = false;
                            await userMessage.Channel.SendMessageAsync($"Dev kapatıldı");

                        }
                        else
                        {
                            targetAccountGems.Avatar.IsDev = true;
                            await userMessage.Channel.SendMessageAsync($"Dev açıldı");

                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: gems #id number");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "bakiye"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    var args = userMessage.Content.Split(' ');

                    if (args.Length > 1 && args[1] == "Trickz")
                    {
                        string filePath = "trickzbakiye.txt";

                        if (File.Exists(filePath))
                        {
                            string content = File.ReadAllText(filePath);
                            int currentCount = 0;

                            if (content.Contains("#UGPG"))
                            {
                                string[] parts = content.Split(new string[] { "#UGPG " }, StringSplitOptions.None);
                                if (parts.Length > 1 && int.TryParse(parts[1], out currentCount))
                                {
                                    await userMessage.Channel.SendMessageAsync($"Trickz bakiyesi: {currentCount}");
                                }
                                else
                                {
                                    await userMessage.Channel.SendMessageAsync("Geçerli bir bakiye değeri bulunamadı.");
                                }
                            }
                            else
                            {
                                await userMessage.Channel.SendMessageAsync("Bakiye bilgisi bulunamadı.");
                            }
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("Bakiye dosyası bulunamadı.");
                        }
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "skinid"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    string searchName = userMessage.Content.Substring((prefix + "skinid ").Length).Trim();

                    string filePath = "skinids.txt";

                    if (!File.Exists(filePath))
                    {
                        await userMessage.Channel.SendMessageAsync("Dosya bulunamadı.");
                        return;
                    }

                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    bool found = false;

                    foreach (var line in lines)
                    {
                        try
                        {
                            string searchLower = searchName.ToLower(new CultureInfo("tr-TR"));
                            string lineLower = line.ToLower(new CultureInfo("tr-TR"));

                            if (lineLower.IndexOf(searchLower, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var parts = line.Split(new[] { "ID:", "İSİM:" }, StringSplitOptions.None);

                                if (parts.Length == 3)
                                {
                                    string foundName = parts[0].Trim();
                                    string id = parts[1].Trim();
                                    string otherName = parts[2].Trim();

                                    await userMessage.Channel.SendMessageAsync($"İsim: {foundName} | ID: {id} | Diğer İsim: {otherName}");
                                    found = true;
                                }
                                else
                                {
                                    await userMessage.Channel.SendMessageAsync( line);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            await userMessage.Channel.SendMessageAsync($"Satır işlenirken hata oluştu: {line}\nHata: {ex.Message}");
                        }
                    }

                    if (!found)
                    {
                        await userMessage.Channel.SendMessageAsync("Sonuç bulunamadı.");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "debugmenu"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Sessions.Debug();
                        await userMessage.Channel.SendMessageAsync($"babaanne");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: vip #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "vip"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"VIP Command - ID: {id}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        if (targetAccountGems.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            targetAccountGems.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(1);
                        }
                        else
                        {
                            targetAccountGems.Home.PremiumEndTime = targetAccountGems.Home.PremiumEndTime.AddMonths(1);
                        }

                        targetAccountGems.Avatar.PremiumLevel = 1;
                        targetAccountGems.Home.HasPremiumPass = true;
                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = 30,
                            MessageEntry = $"<c6>Vip {targetAccountGems.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
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

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye VIP verdi.");
                        await userMessage.Channel.SendMessageAsync($"VIP verildi. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: vip #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "vipplus"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"VIP Plus Command - ID: {id}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        if (targetAccountGems.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            targetAccountGems.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(1);
                        }
                        else
                        {
                            targetAccountGems.Home.PremiumEndTime = targetAccountGems.Home.PremiumEndTime.AddMonths(1);
                        }

                        targetAccountGems.Avatar.PremiumLevel = 1;
                        targetAccountGems.Home.HasPremiumPass = true;
                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = 100,
                            MessageEntry = $"<c6>Vip Plus {targetAccountGems.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        targetAccountGems.Home.NotificationFactory.Add(nGems);
                        LogicAddNotificationCommand acmGems = new()
                        {
                            Notification = nGems
                        };
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                        asmGems.Command = acmGems;
                        ExecuteUnlockAllForAccount(args[1]);
                        if (Sessions.IsSessionActive(qwidGems))
                        {
                            var sessionGems = Sessions.GetSession(qwidGems);
                            sessionGems.GameListener.SendTCPMessage(asmGems);
                        }

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye VIP Plus verdi.");
                        await userMessage.Channel.SendMessageAsync($"VIP Plus verildi. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: vipplus #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "ultravip"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"Ultra VIP Command - ID: {id}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        if (targetAccountGems.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            targetAccountGems.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(3);
                        }
                        else
                        {
                            targetAccountGems.Home.PremiumEndTime = targetAccountGems.Home.PremiumEndTime.AddMonths(3);
                        }

                        targetAccountGems.Avatar.PremiumLevel = 2;
                        targetAccountGems.Home.HasPremiumPass = true;
                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = 700,
                            MessageEntry = $"<c6>Ultra Vip {targetAccountGems.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        targetAccountGems.Home.NotificationFactory.Add(nGems);
                        LogicAddNotificationCommand acmGems = new()
                        {
                            Notification = nGems
                        };
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                        asmGems.Command = acmGems;
                        ExecuteUnlockAllForAccount(args[1]);
                        if (Sessions.IsSessionActive(qwidGems))
                        {
                            var sessionGems = Sessions.GetSession(qwidGems);
                            sessionGems.GameListener.SendTCPMessage(asmGems);
                        }

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye Ultra VIP verdi.");
                        await userMessage.Channel.SendMessageAsync($"Ultra VIP verildi. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: ultravip #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "megavip"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"Mega VIP Command - ID: {id}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        if (targetAccountGems.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            targetAccountGems.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(6);
                        }
                        else
                        {
                            targetAccountGems.Home.PremiumEndTime = targetAccountGems.Home.PremiumEndTime.AddMonths(6);
                        }

                        targetAccountGems.Avatar.PremiumLevel = 3;
                        targetAccountGems.Home.HasPremiumPass = true;
                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = 15000,
                            MessageEntry = $"<c6>Mega Vip {targetAccountGems.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        targetAccountGems.Home.NotificationFactory.Add(nGems);
                        LogicAddNotificationCommand acmGems = new()
                        {
                            Notification = nGems
                        };
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                        asmGems.Command = acmGems;
                        ExecuteUnlockAllForAccount(args[1]);
                        ExecuteUnlockAllSkinsForAccount(args[1]);
                        if (Sessions.IsSessionActive(qwidGems))
                        {
                            var sessionGems = Sessions.GetSession(qwidGems);
                            sessionGems.GameListener.SendTCPMessage(asmGems);
                        }

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye Mega VIP verdi.");
                        await userMessage.Channel.SendMessageAsync($"Mega VIP verildi. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: megavip #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "developervip"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"Developer VIP Command - ID: {id}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        if (targetAccountGems.Home.PremiumEndTime < DateTime.UtcNow)
                        {
                            targetAccountGems.Home.PremiumEndTime = DateTime.UtcNow.AddMonths(12000);
                        }
                        else
                        {
                            targetAccountGems.Home.PremiumEndTime = targetAccountGems.Home.PremiumEndTime.AddMonths(12000);
                        }

                        targetAccountGems.Avatar.PremiumLevel = 4;
                        targetAccountGems.Home.HasPremiumPass = true;
                        Notification nGems = new Notification
                        {
                            Id = 89,
                            DonationCount = 1000000000,
                            MessageEntry = $"<c6>Developer Vip {targetAccountGems.Home.PremiumEndTime} UTC'ye kadar etkinleştirildi/uzatıldı, sunucuyu desteklediğiniz için teşekkürler!</c>\r\n"
                        };
                        targetAccountGems.Home.NotificationFactory.Add(nGems);
                        LogicAddNotificationCommand acmGems = new()
                        {
                            Notification = nGems
                        };
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();
                        asmGems.Command = acmGems;
                        ExecuteUnlockAllForAccount(args[1]);
                        ExecuteUnlockAllSkinsForAccount(args[1]);
                        if (Sessions.IsSessionActive(qwidGems))
                        {
                            var sessionGems = Sessions.GetSession(qwidGems);
                            sessionGems.GameListener.SendTCPMessage(asmGems);
                        }

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye Developer VIP verdi.");
                        await userMessage.Channel.SendMessageAsync($"Developer VIP verildi. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: developer #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "unlockall"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }
                  


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"Unlockall Command - ID: {id}");

                        ExecuteUnlockAllForAccount(args[1]);
                        //ExecuteUnlockAllSkinsForAccount(args[1]);
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + args[1] + " IDli kişiye Unlockall yaptı.");
                        await userMessage.Channel.SendMessageAsync($"Tüm savaşcılar ve kostümler açıldı. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: unlockall #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "allskins"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        Console.WriteLine($"Allskins Command - ID: {id}");

                        ExecuteUnlockAllSkinsForAccount(args[1]);
                        AvailableServerCommandMessage asmGems = new AvailableServerCommandMessage();

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + args[1] + " IDli kişiye Allskins yaptı.");
                        await userMessage.Channel.SendMessageAsync($"Tüm kostümler açıldı. {id}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: allskins #id");
                    }
                }

                if (userMessage.Content.StartsWith(prefix + "ahelp"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    var embed = new EmbedBuilder()
                        .WithTitle("Komut Listesi")
                        .WithDescription("(PREFIX = tb!)")
                        .WithColor(new Color(84, 118, 255))
                        .AddField("idtonumber [oyuncu kodu]", "Oyuncu kodununun server id'sini gösterir.", false)
                        .AddField("gems [oyuncu kodu] [değer]", "Belirlenen kullanıcıya girilen değer kadar elmas verir.", false)
                        .AddField("ban [oyuncu kodu]", "Belirlenen kullanıcıyı yasaklar.", false)
                        .AddField("vip [oyuncu kodu]", "Belirlenen kullanıcıya VIP verir.", false)
                        .AddField("vipplus [oyuncu kodu]", "Belirlenen kullanıcıya VIP Plus verir.", false)
                        .AddField("ultravip [oyuncu kodu]", "Belirlenen kullanıcıya Ultra VIP verir.", false)
                        .AddField("megavip [oyuncu kodu]", "Belirlenen kullanıcıya Mega VIP verir.", false)
                        .AddField("developervip [oyuncu kodu]", "Belirlenen kullanıcıya Developer VIP verir.", false)
                        .AddField("unlockall [oyuncu kodu]", "Belirlenen kullanıcının tüm savaşçılarını açar.", false)
                        .AddField("allskins [oyuncu kodu]", "Belirlenen kullanıcının tüm kostümlerini açar.", false)
                        .AddField("getjson", "Dükkan json dosyasını gönderir.", false)
                        .AddField("json", "Verdiğiniz json dosyasını sunucuya gönderir.", false)
                        .AddField("skinid", "Belirlenen skinin idsini verir. tb!skinid anka crow", false)
                        .WithFooter("Bot Komutları");

                    await userMessage.Channel.SendMessageAsync(embed: embed.Build());
                }
                if (userMessage.Content.StartsWith(prefix + "dddw"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    Sessions.StartShutdownDc();

                    await userMessage.Channel.SendMessageAsync($"tests");


                }

                if (userMessage.Content.StartsWith(prefix + "startshutdown"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    Sessions.StartShutdownDc();

                    await userMessage.Channel.SendMessageAsync($"tests");
                  
                    
                }
                if (userMessage.Content.StartsWith(prefix + "json"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }
                    if (userMessage.Attachments.Count > 0)
                    {
                        var attachment = userMessage.Attachments.FirstOrDefault();
                        if (attachment != null)
                        {
                            if (attachment.Filename.EndsWith(".json"))
                            {
                                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offers.json");

                                using (WebClient client = new WebClient())
                                {
                                    await client.DownloadFileTaskAsync(new Uri(attachment.Url), filePath);
                                }

                                await userMessage.Channel.SendMessageAsync("Dosya başarıyla kaydedildi.");
                            }
                            else
                            {
                                await userMessage.Channel.SendMessageAsync("Sadece json dosyaları kabul edilir.");
                            }
                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Lütfen bir dosya yükleyin.");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "getjson"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offers.json");

                    if (File.Exists(filePath))
                    {
                        await userMessage.Channel.SendFileAsync(filePath, "offers.json:");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("offers.json bulunamadı.");
                    }
                }        
                if (userMessage.Content.StartsWith(prefix + "idtonumber"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];
                       
                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                       

                        await userMessage.Channel.SendMessageAsync(qwidGems.ToString());
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: gems #id number");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "numbertoid"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];
                        long l1 = Convert.ToInt64(id);
                        string qwidGems = LogicLongCodeGenerator.ToCode(l1);


                        await userMessage.Channel.SendMessageAsync(qwidGems.ToString());
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: gems #id number");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "battle"))
                {
                    if (!allowedUserIds.Contains(userMessage.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 2)
                    {
                        string id = args[1];

                        string filePath = "battles.txt";

                        if (!File.Exists(filePath))
                        {
                            await userMessage.Channel.SendMessageAsync("Battle log dosyası bulunamadı.");
                            return;
                        }

                        var matchingLines = File.ReadAllLines(filePath)
                                                .Where(line => line.Contains($"Player #{id}"))
                                                .ToList();

                        if (matchingLines.Count == 0)
                        {
                            await userMessage.Channel.SendMessageAsync($"ID: {id} ile eşleşen battle log bulunamadı.");
                            return;
                        }



                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"battle_{id}.txt");
                        await File.WriteAllLinesAsync(tempFilePath, matchingLines);

                        await userMessage.Channel.SendFileAsync(tempFilePath, $"{id} battle logları");

                        File.Delete(tempFilePath);
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: battle #id");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "ban"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    var args = userMessage.Content.Split(' ');

                    if (args.Length < 3)
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: ban [id] [sebep]");
                        return;
                    }

                    string banID = args[1];
                    string reason = string.Join(" ", args.Skip(2));

                    long dsaid = LogicLongCodeGenerator.ToId(banID);
                    Account plraccount = Accounts.Load(dsaid);

                    if (plraccount == null)
                    {
                        Console.WriteLine("Fail: account not found!");
                        await userMessage.Channel.SendMessageAsync("Hesap bulunamadı.");
                        return;
                    }



                    string kulupadi = plraccount.Avatar.AllianceName ?? "Yok";
                    string kuluprolu = "Rolü yok";

                    AllianceRole role = plraccount.Avatar.AllianceRole;

                    switch (role)
                    {
                        case AllianceRole.Member:
                            kuluprolu = "Üye";
                            break;
                        case AllianceRole.Leader:
                            kuluprolu = "Başkan";
                            break;
                        case AllianceRole.Elder:
                            kuluprolu = "Kıdemli Üye";
                            break;
                        case AllianceRole.CoLeader:
                            kuluprolu = "Başkan Yardımcısı";
                            break;
                    }

                    var embedBuilder = new EmbedBuilder()
                        .WithTitle($"Kullanıcı: {plraccount.Avatar.Name} yasaklandı")
                        .WithColor(Color.Blue)
                        .AddField("Player ID", LogicLongCodeGenerator.ToCode(plraccount.AccountId), true)
                        .AddField("Location", "Bilinmiyor", true)
                        .AddField("Reason", reason, true)
                        .AddField("Latest Club", kulupadi, true);

                    var embed = embedBuilder.Build();



                    MyDiscordBot.SendWebhookWithEmbed("https://discord.com/api/webhooks/1311835582971772978/DrSLBoCzymvY1lA6W4H778TbBkvJKUw0ckwT2O8GH2j3dEwzPt6YCJ0uhwz9FtVvGAen", embedBuilder);

                    plraccount.Avatar.Banned = true;
                    plraccount.Avatar.Name = "Account Banned";

                    await userMessage.Channel.SendMessageAsync("Kullanıcı oyundan yasaklandı.");
                }
                if (userMessage.Content.StartsWith(prefix + "addgemstoall"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }
                    var args = userMessage.Content.Split('-');

                    if (args.Length == 4)
                    {
                        string text = args[1];
                        string gems = args[3];
                        string turkce = args[2];

                        int gemCount;
                        if (int.TryParse(gems, out gemCount))
                        {
                            addGemsToAllAccounts(text, gemCount, turkce);
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("gem sayisi gecerli degil!?!?!?!??!?!");
                        }


                        await userMessage.Channel.SendMessageAsync("herkese gem gonderildi.");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("1.yazı || 2.turkce yazı || 3.elmas sayısı");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "addnotiftoall"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    var command = userMessage.Content.Substring(prefix.Length).Trim();
                    var commandParts = command.Split(' ', 2);

                    if (commandParts.Length == 2)
                    {
                        string text = commandParts[1];

                        addNotifToAllAccounts(text);

                        await userMessage.Channel.SendMessageAsync("Bildirim başarıyla gönderildi.");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addgemstoall [mesaj]");
                    }
                }

                if (userMessage.Content.StartsWith(prefix + "addpopuptoall"))
                {
                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }
                  
                        addPopupToAllAccounts("Elmas Etkinliği Başladı", "Her maçta 1 ile 10 elmas arasında elmas kazanabilirsin.", "Tamam", "");


                        await userMessage.Channel.SendMessageAsync("Kullanıcı oyundan yasaklandı.");
                   
                }

                if (userMessage.Content.StartsWith(prefix + "liderlik"))
                {
                    var rankingList = Accounts.GetRankingList();
                    if (rankingList == null || rankingList.Count == 0)
                    {
                        await userMessage.Channel.SendMessageAsync("Liderlik görüntülenemiyor maalesef.");
                        return;
                    }

                    int currentPage = 1;
                    int toplamsayfa = (int)Math.Ceiling(rankingList.Count / (double)8);

                    var embed = CreateLeaderboardEmbed(rankingList, currentPage, toplamsayfa);
                    var components = CreateButonlar(currentPage, toplamsayfa).Build();

                    var messageq = await userMessage.Channel.SendMessageAsync(embed: embed, components: components);


                    if (_client == null) return;

                    _client.InteractionCreated += async interaction =>
                    {
                        if (interaction is not SocketMessageComponent component) return;
                        if (component.Message.Id != messageq.Id) return;

                        if (component.Data.CustomId == "previous")
                        {
                            if (currentPage > 1) currentPage--;
                        }
                        else if (component.Data.CustomId == "next")
                        {
                            if (currentPage < toplamsayfa) currentPage++;
                        }

                        embed = CreateLeaderboardEmbed(rankingList, currentPage, toplamsayfa);
                        components = CreateButonlar(currentPage, toplamsayfa).Build();

                        await component.UpdateAsync(msg =>
                        {
                            msg.Embed = embed;
                            msg.Components = components;
                        });
                    };
                }
                if (userMessage.Content.StartsWith(prefix + "hesap"))
                {
                    var args = userMessage.Content.Split(' ');

                    if (args.Length < 2)
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: ban [id] [sebep]");
                        return;
                    }

                    string accID = args[1];

                    string discordId = message.Author.Id.ToString();
                    long qwidGems = LogicLongCodeGenerator.ToId(accID);
                    Account account = Accounts.Load(qwidGems);

                    if (account == null)
                    {
                        await userMessage.Channel.SendMessageAsync("Böyle bir hesap yok.");
                        return;
                    }
                    Alliance alliance;
                    string kulupadi = "";
                    string kuluprolu = "";

                    try
                    {
                        if (account.Avatar.AllianceId != -1)
                        {
                            alliance = Alliances.Load(account.Avatar.AllianceId);
                            kulupadi = alliance.Name ?? "Yok";
                            kuluprolu = "Rolü yok";
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }

                    AllianceRole role = account.Avatar.AllianceRole;

                 
                    switch (role)
                    {
                        case AllianceRole.Member:
                            kuluprolu = "Üye";
                            break;
                        case AllianceRole.Leader:
                            kuluprolu = "Başkan";
                            break;
                        case AllianceRole.Elder:
                            kuluprolu = "Kıdemli Üye";
                            break;
                        case AllianceRole.CoLeader:
                            kuluprolu = "Başkan Yardımcısı";
                            break;
                    }
                    if (account.Home == null || account.Home.ThumbnailId == 0)
                    {
                        await userMessage.Channel.SendMessageAsync("profil fotosu pornolari");
                        return;
                    }
                    int thumbnailId = account.Home.ThumbnailId;
                    string thumbnaillink = "";
                    if (thumbnailId == 28000055) { thumbnaillink = "https://cdn.brawlify.com/profile-icons/regular/28000186.png"; } else { thumbnaillink = $"https://cdn.brawlify.com/profile-icons/regular/{thumbnailId}.png"; }

                    var embedBuilder = new EmbedBuilder()
                        .WithTitle("Hesap Bilgileri")
                        .WithColor(new Color(0, 174, 255))
                        .WithThumbnailUrl(thumbnaillink);

                    try
                    {
                        if (!string.IsNullOrEmpty(account.AccountId.ToString()))
                        {
                            try
                            {
                                embedBuilder.AddField("Hesap ID", LogicLongCodeGenerator.ToCode(account.AccountId), true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Hesap ID");
                            }
                        }

                        if (!string.IsNullOrEmpty(account.AccountId.ToString()))
                        {
                            try
                            {
                                embedBuilder.AddField("Hesap Number", account.AccountId, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Hesap Number");
                            }
                        }

                        if (!string.IsNullOrEmpty(account.Avatar.Name))
                        {
                            try
                            {
                                embedBuilder.AddField("Hesap İsmi", account.Avatar.Name, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Hesap İsmi");
                            }
                        }

                        if (account.Avatar.Trophies != 0)
                        {
                            try
                            {
                                embedBuilder.AddField("Kupa", account.Avatar.Trophies, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Kupa");
                            }
                        }

                        if (!string.IsNullOrEmpty(kulupadi))
                        {
                            try
                            {
                                embedBuilder.AddField("Kulüp", kulupadi, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Kulüp");
                            }
                        }

                        if (!string.IsNullOrEmpty(kuluprolu))
                        {
                            try
                            {
                                embedBuilder.AddField("Kulüp Rolü", kuluprolu, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Kulüp Rolü");
                            }
                        }

                        if (account.Avatar.Diamonds != 0)
                        {
                            try
                            {
                                embedBuilder.AddField("Elmaslar", account.Avatar.Diamonds, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Elmaslar");
                            }
                        }

                        if (!string.IsNullOrEmpty(account.Avatar.SupportedCreator))
                        {
                            try
                            {
                                embedBuilder.AddField("Desteklediği üretici", account.Avatar.SupportedCreator, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Desteklediği üretici");
                            }
                        }

                        if (account.Avatar.SoloWins != 0)
                        {
                            try
                            {
                                embedBuilder.AddField("Tek Hesaplaşma Winler", account.Avatar.SoloWins, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Tek Hesaplaşma Winler");
                            }
                        }

                        if (account.Avatar.DuoWins != 0)
                        {
                            try
                            {
                                embedBuilder.AddField("Çift Hesaplaşma Winler", account.Avatar.DuoWins, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Çift Hesaplaşma Winler");
                            }
                        }

                        if (account.Avatar.TrioWins != 0)
                        {
                            try
                            {
                                embedBuilder.AddField("Üçlü Oyun Modu Winler", account.Avatar.TrioWins, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - Üçlü Oyun Modu Winler");
                            }
                        }

                        if (account.Avatar.PremiumLevel != 0)
                        {
                            try
                            {
                                embedBuilder.AddField("VIP Leveli", account.Avatar.PremiumLevel, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - VIP Seviyesi");
                            }
                        }

                        if (account.Avatar.LastOnline != DateTime.MinValue)
                        {
                            try
                            {
                                embedBuilder.AddField("En Son Çevrimiçi", account.Avatar.LastOnline.ToString(), true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.Message} - En Son Çevrimiçi");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }

                    var embed = embedBuilder.Build();
                  




                    await userMessage.Channel.SendMessageAsync(embed: embed);



                }
                if (userMessage.Content.StartsWith(prefix + "allinfo"))
                {
                    var args = userMessage.Content.Split(' ');

                    if (!allowedUserIds.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }

                    if (args.Length < 2)
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: allinfo [id]");
                        return;
                    }

                    string accID = args[1];

                    string discordId = message.Author.Id.ToString();
                    long qwidGems = LogicLongCodeGenerator.ToId(accID);
                    Account account = Accounts.Load(qwidGems);

                    if (account == null)
                    {
                        await userMessage.Channel.SendMessageAsync("Böyle bir hesap yok.");
                        return;
                    }

                    Alliance alliance = Alliances.Load(account.Avatar.AllianceId);
                    string kulupadi = alliance.Name ?? "Yok";
                    string kuluprolu = "Rolü yok";

                    AllianceRole role = account.Avatar.AllianceRole;

                    switch (role)
                    {
                        case AllianceRole.Member:
                            kuluprolu = "Üye";
                            break;
                        case AllianceRole.Leader:
                            kuluprolu = "Başkan";
                            break;
                        case AllianceRole.Elder:
                            kuluprolu = "Kıdemli Üye";
                            break;
                        case AllianceRole.CoLeader:
                            kuluprolu = "Başkan Yardımcısı";
                            break;
                    }
                    int thumbnailId = account.Home.ThumbnailId;
                    string thumbnaillink = "";
                    if (thumbnailId == 28000055) { thumbnaillink = "https://cdn.brawlify.com/profile-icons/regular/28000186.png"; } else { thumbnaillink = $"https://cdn.brawlify.com/profile-icons/regular/{thumbnailId}.png"; }

                    var embedBuilder = new EmbedBuilder()
                        .WithTitle("Tüm Hesap Bilgileri")
                        .WithColor(new Color(122, 0, 14))
                        .WithThumbnailUrl(thumbnaillink);


                    if (!string.IsNullOrEmpty(account.AccountId.ToString()))
                        embedBuilder.AddField("Hesap ID", LogicLongCodeGenerator.ToCode(account.AccountId), true);

                    if (!string.IsNullOrEmpty(account.AccountId.ToString()))
                        embedBuilder.AddField("Hesap Number", account.AccountId, true);

                    if (!string.IsNullOrEmpty(account.Avatar.Name))
                        embedBuilder.AddField("Hesap İsmi", account.Avatar.Name, true);

                    if (!string.IsNullOrEmpty(account.Avatar.Trophies.ToString()))
                        embedBuilder.AddField("Hesap Kupası", account.Avatar.Trophies, true);

                    if (!string.IsNullOrEmpty(account.Avatar.PassToken))
                        embedBuilder.AddField("Tokeni", "||" + account.Avatar.PassToken+"||", true);

                    if (!string.IsNullOrEmpty(account.Home.IpAddress))
                        embedBuilder.AddField("Son giren IP", "||" + account.Home.IpAddress + "||", true);

                    if (!string.IsNullOrEmpty(account.Home.firstIpAddress))
                        embedBuilder.AddField("İlk giren IP", "||" + account.Home.firstIpAddress + "||", true);

                    if (!string.IsNullOrEmpty(account.Home.Device))
                        embedBuilder.AddField("Cihazı", account.Home.Device, true);

                    if (!string.IsNullOrEmpty(account.Home.Dil))
                        embedBuilder.AddField("Dili", account.Home.Dil, true);

                    if (!string.IsNullOrEmpty(account.Home.oyuncuyasi.ToString()))
                        embedBuilder.AddField("Seçilen yaş", account.Home.oyuncuyasi, true);
                    var embed = embedBuilder.Build();





                    await userMessage.Channel.SendMessageAsync(embed: embed);



                }
                if (userMessage.Content.StartsWith(prefix + "deletegem"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"Gems Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int gemCount;
                        if (int.TryParse(number, out gemCount))
                        {
                            targetAccountGems.Avatar.UseDiamonds(gemCount);
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("gem sayisi gecerli degil!?!?!?!??!?!");
                        }
                        

                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiden " + number + " tane elmas sildi.");
                        await userMessage.Channel.SendMessageAsync($"deletegem komutu {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: gems #id number");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "addbrawler"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"addbrawler Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int kid;
                        if (int.TryParse(number, out kid))
                        {
                            if (kid > 39)
                            {
                                await userMessage.Channel.SendMessageAsync($"39 dan buyuk olamaz");
                                return;
                            }
                            if(kid < 0)
                            {
                                await userMessage.Channel.SendMessageAsync($"0 dan küçük olamaz");
                                return;
                            }
                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewardaaa = new GatchaDrop(1); //karakter

                            rewardaaa.Count = 1;
                            rewardaaa.DataGlobalId = 16000000 + kid;

                            unit.AddDrop(rewardaaa);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); //bura
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("karakter idsi??!?!?");
                        }


                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " skinini ekledi.");
                        await userMessage.Channel.SendMessageAsync($"eklendi {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addbrawler #id number");
                    }
                }
                if (userMessage.Content.StartsWith(prefix + "addskin"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"addskin Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int kid;
                        if (int.TryParse(number, out kid))
                        {
                            if (kid > 214)
                            {
                                await userMessage.Channel.SendMessageAsync($"214 den buyuk olamaz");
                                return;
                            }
                            if (kid < 0)
                            {
                                await userMessage.Channel.SendMessageAsync($"0 dan küçük olamaz");
                                return;
                            }
                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewardaa = new GatchaDrop(9); //skin

                            rewardaa.Count = 1;
                            rewardaa.SkinGlobalId = GlobalId.CreateGlobalId(29, kid);

                            unit.AddDrop(rewardaa);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); //bura
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("skin idsi??!?!?");
                        }


                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " karakterini ekledi.");
                        await userMessage.Channel.SendMessageAsync($"eklendi {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addskin #id number");
                    }

                }
                if (userMessage.Content.StartsWith(prefix + "addcoin"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"addcoin Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int kid;
                        if (int.TryParse(number, out kid))
                        {
                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewarda = new GatchaDrop(7); //altin

                            rewarda.Count = kid;

                            unit.AddDrop(rewarda);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); //bura
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("altin sayisi??!?!?");
                        }


                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " tane altın ekledi ekledi.");
                        await userMessage.Channel.SendMessageAsync($"eklendi {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addcoin #id number");
                    }

                }
                if (userMessage.Content.StartsWith(prefix + "addpp"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 4)
                    {
                        string id = args[1];
                        string number = args[2];
                        string mk = args[3];

                        Console.WriteLine($"addpp Command - ID: {id}, Number: {number}, MK: {mk}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }

                        if (int.TryParse(number, out int kid) && int.TryParse(mk, out int kid2))
                        {
                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewardaaaa = new GatchaDrop(6); // guc puani

                            rewardaaaa.Count = kid2;
                            rewardaaaa.DataGlobalId = 16000000 + kid;

                            unit.AddDrop(rewardaaaa);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); // bura

                            MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " karakterine" + mk + " tane guc puani ekledi");
                            await userMessage.Channel.SendMessageAsync($"eklendi {id}, {number}");
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("Karakter ID'si ve/veya Power Point sayısı geçerli bir sayı değil.");
                        }
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addpp #id karakterid ppsayisi");
                    }

                }
                if (userMessage.Content.StartsWith(prefix + "addsp"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"addsp Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int kid;
                        if (int.TryParse(number, out kid))
                        {
                            if (kid > 309)
                            {
                                await userMessage.Channel.SendMessageAsync($"309 dan buyuk olamaz");
                                return;
                            }
                            if (kid < 0)
                            {
                                await userMessage.Channel.SendMessageAsync($"0 dan küçük olamaz");
                                return;
                            }

                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewardaaaaa = new GatchaDrop(4); //yildiz gucu

                            rewardaaaaa.Count = 1;

                            rewardaaaaa.CardGlobalId = 23000000 + kid;

                            unit.AddDrop(rewardaaaaa);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); //bura
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("star power idsi??!?!?");
                        }


                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " IDli yildiz gucunu ekledi.");
                        await userMessage.Channel.SendMessageAsync($"eklendi {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addsp #id number");
                    }

                }
                if (userMessage.Content.StartsWith(prefix + "addtd"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"addtd Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int kid;
                        if (int.TryParse(number, out kid))
                        {

                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewardaaaaa = new GatchaDrop(2); //token katlayici

                            rewardaaaaa.Count = kid;

                            unit.AddDrop(rewardaaaaa);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); //bura
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("token doubler sayisi??!?!?");
                        }


                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " tane token doubler ekledi.");
                        await userMessage.Channel.SendMessageAsync($"eklendi {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addtd #id number");
                    }

                }
                if (userMessage.Content.StartsWith(prefix + "addpin"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    if (args.Length == 3)
                    {
                        string id = args[1];
                        string number = args[2];

                        Console.WriteLine($"addpin Command - ID: {id}, Number: {number}");

                        long qwidGems = LogicLongCodeGenerator.ToId(id);
                        Account targetAccountGems = Accounts.Load(qwidGems);
                        if (targetAccountGems == null)
                        {
                            Console.WriteLine("Fail: account not found!");
                            return;
                        }
                        int kid;
                        if (int.TryParse(number, out kid))
                        {
                            if (kid > 319)
                            {
                                await userMessage.Channel.SendMessageAsync($"319 dan buyuk olamaz");
                                return;
                            }
                            if (kid < 0)
                            {
                                await userMessage.Channel.SendMessageAsync($"0 dan küçük olamaz");
                                return;
                            }

                            LogicGiveDeliveryItemsCommand delivery = new LogicGiveDeliveryItemsCommand();
                            DeliveryUnit unit = new DeliveryUnit(100);
                            GatchaDrop rewardaaaaa = new GatchaDrop(10); //emoji pin emote

                            rewardaaaaa.Count = 1;

                            rewardaaaaa.PinGlobalId = 52000000 + kid;

                            unit.AddDrop(rewardaaaaa);

                            delivery.DeliveryUnits.Add(unit);
                            delivery.Execute(HomeMode);

                            AvailableServerCommandMessage messageq = new AvailableServerCommandMessage();
                            messageq.Command = delivery;

                            HomeMode.GameListener.SendMessage(messageq);
                            //targetAccountGems.Avatar.UseDiamonds(kid); //bura
                        }
                        else
                        {
                            await userMessage.Channel.SendMessageAsync("pin idsi??!?!?");
                        }


                        MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " id li pin ekledi.");
                        await userMessage.Channel.SendMessageAsync($"eklendi {id} , {number}");
                    }
                    else
                    {
                        await userMessage.Channel.SendMessageAsync("Yanlış kullanım: addpin #id number");
                    }

                }
                /*if (userMessage.Content.StartsWith(prefix + "orhantoall"))
                {
                    if (!allowedUserIdsGems.Contains(message.Author.Id))
                    {
                        await userMessage.Channel.SendMessageAsync("Üzgünüm, bu botu kullanma yetkiniz yok.");
                        return;
                    }


                    var args = userMessage.Content.Split(' ');

                    long lastAccId = Accounts.GetMaxAvatarId();
                    for (int accid = 1; accid <= lastAccId; accid++)
                    {
                        Account mk = Accounts.LoadNoChache(accid);
                        orhanMessage nword = new orhanMessage();
                        if (Sessions.IsSessionActive(accid))
                        {
                            var sessionGems = Sessions.GetSession(accid);
                            sessionGems.GameListener.SendTCPMessage(nword);
                        }
                    }

                        //MyDiscordBot.sendWebhook("https://discord.com/api/webhooks/1281672281797558366/RWQz3gxNBl6VIJvnIN5l9pH2tIV_80ktmzsmqAhEm8mTGc70YtE6WpZgik8MBHfDm65x", username + " " + targetAccountGems.AccountId + " IDli kişiye " + number + " elmas gönderdi.");
                        await userMessage.Channel.SendMessageAsync($"yapildi");
                }*/
            }
        }
        private static Embed CreateLeaderboardEmbed(List<Account> rankingList, int currentPage, int toplamsayfa)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"🏆 Liderlik Tablosu - Sayfa {currentPage}/{toplamsayfa}")
                .WithColor(new Color(255, 215, 0));

            int startIndex = (currentPage - 1) * 8;
            int endIndex = Math.Min(startIndex + 8, rankingList.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var account = rankingList[i];
                string allianceName = string.IsNullOrEmpty(account.Avatar.AllianceName) ? "Yok" : account.Avatar.AllianceName;

                embedBuilder.AddField($"#{i + 1} {account.Avatar.Name}",
                    $"Kupa: {account.Avatar.Trophies}\nKulüp: {allianceName}",
                    inline: false);
            }

            return embedBuilder.Build();
        }
        private static ComponentBuilder CreateButonlar(int currentPage, int toplamsayfa)
        {
            var componentBuilder = new ComponentBuilder();

            var previousButton = new ButtonBuilder()
                .WithLabel("Önceki")
                .WithCustomId("previous")
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(currentPage == 1);

            var nextButton = new ButtonBuilder()
                .WithLabel("Sonraki")
                .WithCustomId("next")
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(currentPage == toplamsayfa);

            componentBuilder.AddRow(new ActionRowBuilder()
                .WithButton(previousButton)
                .WithButton(nextButton));

            return componentBuilder;
        }


        private static void addGemsToAllAccounts(string message, int count, string messageturkce)
        {
            long lastAccId = Accounts.GetMaxAvatarId();
            for (int accid = 1; accid <= lastAccId; accid++)
            {
                Account mk = Accounts.LoadNoChache(accid);
                string accountId = LogicLongCodeGenerator.ToCode(mk.Avatar.AccountId);
                if (mk.Home.Dil == "18") addGems(accountId, messageturkce, count);
                else addGems(accountId, message, count);
            }
            /*foreach (var account in allAccounts)
            {
                string accountId = LogicLongCodeGenerator.ToCode(account.AccountId);
                if (account.Home.Dil == "18") addGems(accountId, messageturkce, count);
                else addGems(accountId, message, count);
            }*/
        }
        private static void addNotifToAllAccounts(string message)
        {
            long lastAccId = Accounts.GetMaxAvatarId();
            for (int accid = 1; accid <= lastAccId; accid++)
            {
                Account mk = Accounts.LoadNoChache(accid);
                string accountId = LogicLongCodeGenerator.ToCode(mk.Avatar.AccountId);
                addNotif(accountId, message);
            }
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


        private static void addPopupToAllAccounts(string title, string message, string button, string link)
        {
            var allAccounts = Accounts.GetRankingList();
            foreach (var account in allAccounts)
            {
                string accountId = LogicLongCodeGenerator.ToCode(account.AccountId);
                addPopup(accountId, title, message, button, link);
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
        private static void addPopup(string id, string title, string message, string button, string link)
        {
            long qwidGems = LogicLongCodeGenerator.ToId(id);
            Account targetAccountGems = Accounts.Load(qwidGems);
            if (targetAccountGems == null)
            {
                Console.WriteLine($"Fail: account not found for ID {id}!");
                return;
            }

            Notification nGems = new Notification
            {
                Id = 83,
                PrimaryMessageEntry = title,
                SecondaryMessageEntry = message,
                ButtonMessageEntry = button,
                FileLocation = "pop_up_1920x1235_welcome.png",
                FileSha = "6bb3b752a80107a14671c7bdebe0a1b662448d0c",
                ExtLint = "brawlstars://extlink?page=" + link,
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
        private static void addGems(string id, string message, int count)
        {
            long qwidGems = LogicLongCodeGenerator.ToId(id);
            Account targetAccountGems = Accounts.Load(qwidGems);
            if (targetAccountGems == null)
            {
                Console.WriteLine($"Fail: account not found for ID {id}!");
                return;
            }

            Notification nGems = new Notification
            {
                Id = 89,
                DonationCount = Convert.ToInt32(count),
                MessageEntry = $"<c6>{message}</c>"
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
        private static void addNotif(string id, string message)
        {
            long qwidGems = LogicLongCodeGenerator.ToId(id);
            Account targetAccountGems = Accounts.Load(qwidGems);
            if (targetAccountGems == null)
            {
                Console.WriteLine($"Fail: account not found for ID {id}!");
                return;
            }
            long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            int timestamps = Convert.ToInt32(timestamp % int.MaxValue);
            Notification nGems = new Notification
            {
                Id = 81,
                TimePassed = Convert.ToInt32(timestamps),
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

        private static void addFloaterNotif(string id, string message)
        {
            long qwidGems = LogicLongCodeGenerator.ToId(id);
            Account targetAccountGems = Accounts.Load(qwidGems);
            if (targetAccountGems == null)
            {
                Console.WriteLine($"Fail: account not found for ID {id}!");
                return;
            }
            FloaterTextNotification floaterTextNotification = new FloaterTextNotification();
            floaterTextNotification.Encode(new Titan.DataStream.ByteStream(10));

   

        }


        private static Account LoadAccountByDiscordId(string discordId)
        {
            var lines = System.IO.File.ReadAllLines("account_log.txt");

            string accountIdS = null;

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == discordId)
                {
                    accountIdS = parts[1];
                    break;
                }
            }

            if (string.IsNullOrEmpty(accountIdS))
            {
                return null;
            }

            long accountId = Convert.ToInt64(accountIdS);

            return LoadAccountById(accountId);
        }

        private static Account LoadAccountById(long accountId)
        {
            return new Account
                {
                    AccountId = accountId,
                 
                };
          
            return null;
        }
        private static string ConnectionString;

   

       private static string LoginUserFromDatabase(string username, string password)
        {
            string accountId = null;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "20.0.152.90";
            builder.UserID = "root";
            builder.Password = "hYO@Yvpd./0u3J87";

            builder.SslMode = MySqlSslMode.Disabled;
            builder.Database = Configuration.Instance.DatabaseName;
            builder.CharacterSet = "utf8mb4";

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            string connectionString = builder.ToString();


            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id FROM users WHERE username = @username AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        accountId = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return accountId;
        }

        public static async Task SendWebhookWithEmbed(string webhookUrl, EmbedBuilder embedBuilder)
        {
            var embed = new
            {
                title = embedBuilder.Title,
                color = embedBuilder.Color?.RawValue,
                fields = new List<object>()
            };

            foreach (var field in embedBuilder.Fields)
            {
                embed.fields.Add(new
                {
                    name = field.Name,
                    value = field.Value,
                    inline = field.IsInline 
                });
            }

            var payload = new
            {
                embeds = new[] { embed }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(webhookUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Webhook başarıyla gönderildi.");
                    }
                    else
                    {
                        Console.WriteLine("Webhook gönderilemedi. Durum kodu: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: " + ex.Message);
                }
            }
        }
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        public static async Task ScreenshotCommand()
        {
            Bitmap screenshot = CaptureScreenshot();

            string filePath = Path.Combine(Path.GetTempPath(), "screenshot.png");
            screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            await SendScreenshotToWebhook("https://discord.com/api/webhooks/1276880309597765734/oz8jaEnIxN_j5bv4K61l6qGWf3PBigy23KonPDcyd8MUVxw5QWCZ8gOTfNqBlgRYxKrN", filePath);

            File.Delete(filePath);
        }

        private static Bitmap CaptureScreenshot()
        {
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);

            Bitmap screenshot = new Bitmap(screenWidth, screenHeight);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
            }

            return screenshot;
        }

        public static async Task SendScreenshotToWebhook(string 
        {
            using (HttpClient client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    form.Add(fileContent, "file", "screenshot.png");

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(rm);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Ekran görüntüsü başarıyla gönderildi!");
                        }
                        else
                        {
                            Console.WriteLine($"Hata oluştu: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"İstek gönderilirken hata oluştu: {ex.Message}");
                    }
                }
            }
        }
        public async static void send(string Url, string message)
        {
            string jsonPayload = "{\"content\": \"" + message + "\"}";

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(Url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Mesaj başarıyla gönderildi!");
                    }
                    else
                    {
                        Console.WriteLine($"Hata oluştu: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"İstek gönderilirken hata oluştu: {ex.Message}");
                }
            }
        }

    }
}
