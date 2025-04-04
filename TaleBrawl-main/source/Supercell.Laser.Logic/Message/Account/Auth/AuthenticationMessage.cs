using System.Net.Security;

namespace Supercell.Laser.Logic.Message.Account.Auth
{
    public class AuthenticationMessage : GameMessage
    {
        public AuthenticationMessage() : base()
        {
            AccountId = 0;
        }

        public long AccountId;
        public string PassToken;
        public string DeviceId;
        public string Device;
        public int DeviceLang;
        public string Sha;
        public string Android;
        public int Majorsurum;
        public int Minorsurum;
        public int Buildsurum;
        public string korunmalar;

        public override void Decode()
        {
            AccountId = Stream.ReadLong();
            PassToken = Stream.ReadString();
            Majorsurum = Stream.ReadInt();
            Buildsurum = Stream.ReadInt();
            Minorsurum = Stream.ReadInt();
            Sha = Stream.ReadString(); // fingerprint sha
            DeviceId = Stream.ReadString(); // device ismi
            Stream.ReadVInt(); // vint 1, int 17956864. ISANDROID(belki)
            DeviceLang = Stream.ReadVInt(); // device lang. 18 turkce
            korunmalar = Stream.ReadString(); // 5 yazdı? STRING DİLMİŞ...
            Android = Stream.ReadString(); // android surumu
            Stream.ReadVInt(); // vint 1 yazdı. int 16777216 yazdı.
            //Directory.CreateDirectory("diller");

            //File.WriteAllText(@"diller\" + AccountId + ".txt", DeviceLang);

        }

        public override int GetMessageType()
        {
            return 10101;
        }

        public override int GetServiceNodeType()
        {
            return 1;
        }
    }
}

/*namespace Supercell.Laser.Logic.Message.Account.Auth
{
    public class AuthenticationMessage : GameMessage
    {
        public AuthenticationMessage() : base()
        {
            AccountId = 0;
        }

        public long AccountId;
        public string PassToken;
        public string DeviceId;
        public string Device;
        public string DeviceLang;

        public override void Decode()
        {
            AccountId = Stream.ReadLong();
            PassToken = Stream.ReadString();
            Stream.ReadInt();
            Stream.ReadInt();
            Stream.ReadInt();
            Stream.ReadString();
            DeviceId = Stream.ReadString(); // sha benzeri uzun birsey
            //Stream.ReadInt(); // lang id
            DeviceLang = Stream.ReadString();
                        Directory.CreateDirectory("diller");

            File.WriteAllText(@"diller\" + AccountId + ".txt", DeviceLang);

        }

        public override int GetMessageType()
        {
            return 10101;
        }

        public override int GetServiceNodeType()
        {
            return 1;
        }
    }
}*/
