namespace Supercell.Laser.Logic.Message.Account
{
    public class DebugMenuMessage : GameMessage
    {

        public override void Encode()
        {

        }

        public override int GetMessageType()
        {
            return 20500;
        }

        public override int GetServiceNodeType()
        {
            return 11;
        }
    }
}
