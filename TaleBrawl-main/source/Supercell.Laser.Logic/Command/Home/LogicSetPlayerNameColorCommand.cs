﻿namespace Supercell.Laser.Logic.Command.Home
{
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Data.Helper;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Titan.DataStream;

    public class LogicSetPlayerNameColorCommand : Command
    {
        public int NameColorInstanceId;

        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);
            stream.ReadVInt();
            NameColorInstanceId = stream.ReadVInt();
        }

        public override int Execute(HomeMode homeMode)
        {
            if (NameColorInstanceId > 12) return 0;
            return 0;
        }

        public override int GetCommandType()
        {
            return 527;
        }
    }
}
