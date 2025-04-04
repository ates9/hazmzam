namespace Supercell.Laser.Logic.Message.Home
{
    public class namefailed : GameMessage
    {
        public string Name { get; set; }

        public override void Decode()
        {
            base.Decode();

            Stream.WriteVInt(0);
        }

        public override int GetMessageType()
        {
            return 20205;
        }

        public override int GetServiceNodeType()
        {
            return 9;
        }
    }
}
