namespace Supercell.Laser.Logic.Message.Club
{
    using Supercell.Laser.Logic.Helper;

    public class CreateAllianceMessage : GameMessage
    {
        public string Name;
        public string Description;
        public int BadgeId;
        public int Type;
        public int RequiredTrophies;
        public int kluplocation;

        public override void Decode()
        {
            Name = Stream.ReadString();
            Description = Stream.ReadString();
            BadgeId = ByteStreamHelper.ReadDataReference(Stream);
            kluplocation = ByteStreamHelper.ReadDataReference(Stream);
            Type = Stream.ReadVInt();
            RequiredTrophies = Stream.ReadVInt();
            Console.WriteLine($"klupismi: {Name}\nklupaciklama: {Description}\nklupbadgesi: {BadgeId}\nkluplocation: {kluplocation}\nkluptype: {Type}\nklupgereklikupa: {RequiredTrophies}");
        }

        public override int GetMessageType()
        {
            return 14301;
        }

        public override int GetServiceNodeType()
        {
            return 11;
        }
    }
}
