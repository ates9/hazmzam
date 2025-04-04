namespace Supercell.Laser.Logic.Command.Avatar
{
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Titan.DataStream;

    public class LogicChangeAvatarNameCommand : Command
    {
        public string Name;
        public int ChangeNameCost;

        public override void Encode(ByteStream stream)
        {
            stream.WriteString(Name);
            stream.WriteVInt(ChangeNameCost);
        }

        public override int Execute(HomeMode homeMode)
        {
            homeMode.Avatar.Name = Name;
            homeMode.Avatar.NameSetByUser = true;
            /*   var wordlist = new Wordlist("words.json");
            if (wordlist.ContainsBlockedWord(command.Name))
            {
                return false;
            }
            else
            {
                HomeMode.Avatar.Name = command.Name;
                HomeMode.Avatar.NameSetByUser = true;
                AvatarNameCheckRequestMessage message = new AvatarNameCheckRequestMessage();
                Connection.Send(message);
            }*/
            return 0;
        }

        public override int GetCommandType()
        {
            return 201;
        }
    }
}
