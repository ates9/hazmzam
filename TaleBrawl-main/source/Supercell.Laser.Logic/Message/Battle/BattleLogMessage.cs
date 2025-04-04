namespace Supercell.Laser.Logic.Message.Battle
{
    using Supercell.Laser.Logic.Battle;
    using Supercell.Laser.Logic.Helper;
    using Supercell.Laser.Titan.DataStream;

    public class BattleLogMessage : GameMessage
    {

        public BattleLogMessage() : base()
        {
        }

        public void Encode(ByteStream encodes)
        {
            ByteStream encoder = encodes;
            encoder.WriteBoolean(true);
             encoder.WriteVInt(10); //player count
              Console.WriteLine("battlelog test");
             for (int i = 0; i < 10; i++)
             {
                 //BattleLogPlayerEntry
                 encoder.WriteVInt(i); //Order of player?
                 encoder.WriteVLong(1); //PlayerID (yes in hex :skull:)
                 encoder.WriteBoolean(false); //IsStarPlayer

                 encoder.WriteVInt(1);
                 for (int iss = 0; iss < 10; iss++)
                 {
                     ByteStreamHelper.WriteDataReference(encoder, GlobalId.CreateGlobalId(16, 85)); //PlayerBrawlerID
                     encoder.WriteVInt(999); //BrawlerTrophies
                     encoder.WriteVInt(999); //BrawlerTrophiesForRank
                     encoder.WriteVInt(9); //BrawlerID
                 }

                 encoder.WriteVInt(i + 1); //order

                 //PlayerDisplayData
                 encoder.WriteString("" + i); //PlayerName
                 encoder.WriteVInt(100); //PlayerExperience
                 encoder.WriteVInt(280000); //PlayerThumbnail
                 encoder.WriteVInt(3000000); //PlayerNameColor
                 encoder.WriteVInt(-64); //BrawlPassNameColor
                                      //PlayerDisplayData end

                 //BattleLogPlayerEntry end
             }
           

        }

        public override int GetMessageType()
        {
            return 23458;
        }

        public override int GetServiceNodeType()
        {
            return 11;
        }
    }
}
