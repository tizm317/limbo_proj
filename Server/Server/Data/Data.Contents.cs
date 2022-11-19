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
        public string grade;
        public uint price;
        //public string iconSprite;
        public string tooltip;
        public ItemType itemType;
    }

    public class EquipmentData : ItemData
    {
        public int maxDurability;
    }

    public class WeaponData : EquipmentData
    {
        public ClassType classType;
        public int damage;
    }

    public class ArmorData : EquipmentData
    {
        public ArmorType armorType;
        public int defence;
    }

    //
    public class CountableData : ItemData
    {
        public int maxAmount;
    }

    public class PotionData : CountableData
    {
        public int value;
    }

    public class EtcData : CountableData
    {
        public int value;
    }

    public class QuestData : CountableData
    {
        public int value;
    }

    public class CoinData : CountableData
    {

    }

    [Serializable]
    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<ArmorData> armors = new List<ArmorData>();
        public List<PotionData> potions = new List<PotionData>();
        public List<EtcData> etcs = new List<EtcData>();
        public List<QuestData> quests = new List<QuestData>();
        public List<CoinData> coins = new List<CoinData>();

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
            foreach (ItemData item in potions)
            {
                item.itemType = ItemType.Potion;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in quests)
            {
                item.itemType = ItemType.Quest;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in etcs)
            {
                item.itemType = ItemType.Etc;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in coins)
            {
                item.itemType = ItemType.Coin;
                dict.Add(item.id, item);
            }

            return dict;
        }
    }
    #endregion

}
