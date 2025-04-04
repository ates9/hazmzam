namespace Supercell.Laser.Logic.Message.Club
{
    using Supercell.Laser.Logic.Club;
    using Supercell.Laser.Logic.Helper;
    using static System.Reflection.Metadata.BlobBuilder;

    public class AllianceListMessage : GameMessage
    {
        public List<Alliance> clubs;

        public AllianceListMessage() : base()
        {
            clubs = new List<Alliance>();
        }

        public string query;

        public override void Encode()
        {
            Stream.WriteString(query);
            Stream.WriteVInt(clubs.Count);

            foreach (Alliance alliance in clubs)
            {

                    Stream.WriteLong(alliance.Id);
                Stream.WriteString(alliance.Name);
                ByteStreamHelper.WriteDataReference(Stream, alliance.AllianceBadgeId);
                Stream.WriteVInt(alliance.Type);
                Stream.WriteVInt(alliance.Members.Count);
                Stream.WriteVInt(alliance.Trophies);
                Stream.WriteVInt(alliance.RequiredTrophies);
                ByteStreamHelper.WriteDataReference(Stream, GlobalId.CreateGlobalId(0, 0));
                Stream.WriteString("TR");

                Stream.WriteVInt(0);
                    Stream.WriteVInt(1);


                
            }
        }

        public override int GetMessageType()
        {
            return 24310;
        }

        public override int GetServiceNodeType()
        {
            return 11;
        }
    }
}
