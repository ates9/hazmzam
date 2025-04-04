namespace Supercell.Laser.Logic.Message.Team
{
    public class TeamSetLocationMessage : GameMessage
    {
        public int istenenmap { get; set; }

        public override void Decode()
        {
            base.Decode();

            Stream.ReadVInt();
            istenenmap = Stream.ReadVInt();
        }

        public override int GetMessageType()
        {
            return 14363;
        }

        public override int GetServiceNodeType()
        {
            return 9;
        }
    }
}
