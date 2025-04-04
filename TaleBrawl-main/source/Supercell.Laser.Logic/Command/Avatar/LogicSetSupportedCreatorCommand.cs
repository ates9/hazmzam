﻿namespace Supercell.Laser.Logic.Command.Home
{
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Titan.DataStream;

    public class LogicSetSupportedCreatorCommand : Command
    {
        public string Name;
        public override void Encode(ByteStream stream)
        {
            stream.WriteVInt(1);
            stream.WriteString(Name);
            stream.WriteVInt(1);
            Console.WriteLine("cmd:" + Name);
            base.Encode(stream);

        }

        public override int Execute(HomeMode homeMode)
        {
            return 0;
        }

        public override int GetCommandType()
        {
            return 215;
        }
    }
}
