namespace Supercell.Laser.Logic.Club
{
    using Supercell.Laser.Logic.Avatar;
    using Supercell.Laser.Logic.Data.Helper;
    using Supercell.Laser.Logic.Helper;
    using Supercell.Laser.Titan.DataStream;
    using Supercell.Laser.Logic.Club;
    using Supercell.Laser.Logic.Avatar;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Logic.Data;
    public class AllianceHeader
    {
        private long Id;

        private string Name;
        private int Badge;
        private int PlayersCount;
        private int Trophies;
        private int RequiredTrophies;
        private int OnlineMembers;

        private int Type;

        public int porn;


        public AllianceHeader(long id, string name, int badge, int playersCount, int trophies, int requiredTrophies, int type, int onlineMembers)
        {
            Id = id;
            Name = name;
            Badge = badge;
            PlayersCount = playersCount;
            Trophies = trophies;
            RequiredTrophies = requiredTrophies;
            OnlineMembers = onlineMembers;
            Type = type;
        }

        public void Encode(ByteStream stream)
        {
            stream.WriteLong(Id);
            stream.WriteString(Name);
            ByteStreamHelper.WriteDataReference(stream, Badge);
            stream.WriteVInt(Type); // Type
            stream.WriteVInt(PlayersCount);
            stream.WriteVInt(Trophies);
            stream.WriteVInt(RequiredTrophies); // trophies required
            ByteStreamHelper.WriteDataReference(stream, null);
            stream.WriteString("TR");
            stream.WriteVInt(OnlineMembers); // online oyuncu
            stream.WriteVInt(0);
            Console.WriteLine(Type);
        }
    }
}
