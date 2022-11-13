using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Data
{
    #region PlayerStat
    [Serializable]
    public class PlayerStatData : ILoader<int, PlayerStatInfo>
    {
        public List<PlayerStatInfo> playerstats = new List<PlayerStatInfo>();

        public Dictionary<int, PlayerStatInfo> MakeDict()
        {
            Dictionary<int, PlayerStatInfo> dict = new Dictionary<int, PlayerStatInfo>();
            foreach (PlayerStatInfo stat in playerstats)
                dict.Add(stat.Level, stat);
            return dict;
        }
    }
    #endregion

    #region Skill
    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float cooldown; // 쿨타임
        public int damage;
        //public SkillType skillType;
    }

    #endregion
}
