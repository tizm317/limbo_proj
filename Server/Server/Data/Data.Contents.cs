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

    #region Item
    [Serializable]
    public class ItemData
    {
        public int id;
        public string name;
        public ItemType itemType;
    }

    public class WeaponData : ItemData
    {
        public WeaponType weaponType;
        public int damage;
    }

    public class ArmorData : ItemData
    {
        public ArmorType armorType;
        public int defence;
    }

    public class ConsumableData : ItemData
    {
        public ConsumableType consumableType;
        public int maxCount;
    }

    // TODO : QuestItem, ETCItem
    //public class QuestData : ItemData
    //{
    //    public QuestType questType;
    //}
    //public class EtcData : ItemData
    //{
    //    public EtcType etcType;
    //}

    [Serializable]
    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<ArmorData> armors = new List<ArmorData>();
        public List<ConsumableData> consumables = new List<ConsumableData>();
        //public List<ConsumableData> consumables = new List<ConsumableData>();
        //public List<ConsumableData> consumables = new List<ConsumableData>();

        // Dictionary 는 하나로 관리
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
            foreach (ItemData item in weapons)
            {
                item.itemType = ItemType.Weapon;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in armors)
            {
                item.itemType = ItemType.Armor;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in consumables)
            {
                item.itemType = ItemType.Consumable;
                dict.Add(item.id, item);
            }
            //foreach (ItemData item in weapons)
            //{
            //    item.itemType = ItemType.Consumable;
            //    dict.Add(item.id, item);
            //}
            //foreach (ItemData item in weapons)
            //{
            //    item.itemType = ItemType.Consumable;
            //    dict.Add(item.id, item);
            //}
            return dict;
        }
    }
    #endregion

}
