using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Supercell.Laser.Server
{
    public static class Creators
    {
        private static string FilePath { get; }
        private static List<Creator> CreatorList { get; set; }

        static Creators()
        {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "creators.json");
            LoadCreators();
        }


        private static void LoadCreators()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                CreatorList = JsonConvert.DeserializeObject<List<Creator>>(json) ?? new List<Creator>();
            }
            else
            {
                CreatorList = new List<Creator>();
            }
        }




        private static void SaveCreators()
        {
            var json = JsonConvert.SerializeObject(CreatorList, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static void AddCreatorByName(string creatorName, long id)
        {
            if (!CreatorExists(creatorName))
            {
                CreatorList.Add(new Creator { Name = creatorName, Id = id, UsageCount = 0 });
                SaveCreators();
            }
        }

        public static void DeleteCreatorByName(string creatorName)
        {
            CreatorList.RemoveAll(c => c.Name.Equals(creatorName, StringComparison.OrdinalIgnoreCase));
            SaveCreators();
        }

        public static void IncreaseCreator(string creatorName)
        {
            var creator = CreatorList.Find(c => c.Name.Equals(creatorName, StringComparison.OrdinalIgnoreCase));
            if (creator != null)
            {
                creator.UsageCount++;
                SaveCreators();
            }
        }
        public static List<Creator> GetAllCreators()
        {
            return CreatorList;
        }
        public static void ReduceCreator(string creatorName)
        {
            var creator = CreatorList.Find(c => c.Name.Equals(creatorName, StringComparison.OrdinalIgnoreCase));
            if (creator != null && creator.UsageCount > 0)
            {
                creator.UsageCount--;
                SaveCreators();
            }
        }

        public static bool CreatorExists(string creatorName)
        {
            return CreatorList.Exists(c => c.Name.Equals(creatorName, StringComparison.OrdinalIgnoreCase));
        }

        public static string CreatorInfoByName(string creatorName)
        {
            var creator = CreatorList.Find(c => c.Name.Equals(creatorName, StringComparison.OrdinalIgnoreCase));
            return creator != null ? $"{creator.UsageCount}" : "Tüh! bulunamadı.";
        }

        public static long GetCreatorIdByName(string creatorName)
        {
            var creator = CreatorList.Find(c => c.Name.Equals(creatorName, StringComparison.OrdinalIgnoreCase));
            return creator.Id;
        }

        public class Creator
        {
            public string Name { get; set; }
            public long Id { get; set; }
            public int UsageCount { get; set; }
        }
    }

 
}
