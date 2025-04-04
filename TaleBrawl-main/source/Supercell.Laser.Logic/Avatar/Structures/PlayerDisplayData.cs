﻿namespace Supercell.Laser.Logic.Avatar.Structures
{
    using Supercell.Laser.Titan.DataStream;

    public class PlayerDisplayData
    {
        public int ThumbnailId;
        public int NameColorId;
        public int HighNameColorId;
        public string Name;

        public PlayerDisplayData()
        {
            ;
        }

        public PlayerDisplayData(bool hasPremiumPass, int thumbnail, int namecolor, string name)
        {
            ThumbnailId = thumbnail;
            NameColorId = namecolor;
            HighNameColorId = hasPremiumPass ? namecolor : 0;
            Name = name;
        }

        public void Encode(ByteStream stream)
        {
            stream.WriteString(Name);
            stream.WriteVInt(100);
            stream.WriteVInt(ThumbnailId);
            stream.WriteVInt(NameColorId);
            stream.WriteVInt(HighNameColorId);
        }
    }
}
