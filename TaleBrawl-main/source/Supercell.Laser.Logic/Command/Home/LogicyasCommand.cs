﻿namespace Supercell.Laser.Logic.Command.Home
{
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Data.Helper;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Titan.DataStream;

    public class LogicyassecCommand : Command
    {
        public int yasgirme;

        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);
            stream.ReadVInt();
            stream.ReadVInt();
            stream.ReadVInt();
            yasgirme = stream.ReadVInt();
            Console.WriteLine("PORNOLAR? " + yasgirme);
        }

        public override int Execute(HomeMode homeMode)
        {
            return 0;
        }

        public override int GetCommandType()
        {
            return 530;
        }
    }
}
