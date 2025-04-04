namespace Supercell.Laser.Server
{
    using Supercell.Laser.Server.Handler;
    using Supercell.Laser.Server.Settings;
    using System.Drawing;

    static class Program
    {
        public const string SERVER_VERSION = "29";
        public const string BUILD_TYPE = "UPGRADED";

        private static void Main(string[] args)
        {
            Console.Title = "Tale Brawl - Server Emulator v" + SERVER_VERSION + " Build: " + BUILD_TYPE;
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            Colorful.Console.WriteWithGradient(
                @"
  ______      __        ____                      __
 /_  __/___ _/ /__     / __ )_________ __      __/ /
  / / / __ `/ / _ \   / __  / ___/ __ `/ | /| / / /
 / / / /_/ / /  __/  / /_/ / /  / /_/ /| |/ |/ / /
/_/  \__,_/_/\___/  /_____/_/   \__,_/ |__/|__/_/

       " + "\n\n\n", Color.DarkGreen, Color.Cyan, 8);

            Logger.Init();
            Configuration.Instance = Configuration.LoadFromFile("config.json");

            Resources.InitDatabase();
            Resources.InitLogic();
            Resources.InitNetwork();

            Logger.Print("Server started! Let's play Brawl Stars!");

            MyDiscordBot.RunBotAsync();
            ExitHandler.Init();
            CmdHandler.Start();



        }
    }
}