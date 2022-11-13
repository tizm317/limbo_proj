using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }


    public class DataManager
    {
        //public static Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
        public static Dictionary<int, PlayerStatInfo> PlayerStatDict { get; private set; } = new Dictionary<int, PlayerStatInfo>();
        public static void LoadData()
        {
            PlayerStatDict = LoadJson<PlayerStatData, int, PlayerStatInfo>("PlayerStatData").MakeDict();
        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }

    }
}
