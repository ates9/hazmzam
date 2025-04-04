namespace Supercell.Laser.Logic.Message.Home
{
    public class orhanMessage : GameMessage
    {
        public string Name { get; set; }

        public override void Decode()
        {
            base.Decode();
        }

        public override int GetMessageType()
        {
            return 31111;
        }

        public override int GetServiceNodeType()
        {
            return 9;
        }
    }
}
