using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    // ItemData 와 달리 진짜 서버메모리에 들고 있는 아이템 의미함.
    public class Item
    {
        public ItemInfo Info = new ItemInfo();

        #region Wrapping 함수들(자주 쓰는 ItemInfo)
        public int ItemDbId
        {
            get { return Info.ItemDbId; }
            set { Info.ItemDbId = value; }
        }
        public int TemplateId
        {
            get { return Info.TemplateId; }
            set { Info.TemplateId = value; }
        }
        public int Count
        {
            get { return Info.Count; }
            set { Info.Count = value; }
        }
        #endregion
    
        public ItemType ItemType { get; private set; }
        public bool Stackable { get; protected set; } // 아이템이 겹쳐지는지 (캐싱용도)

        // 생성자
        public Item(ItemType itemType)
        {
            ItemType = itemType;
        }

        // DB 에서 추출한 정보로 인메모리에서 관리할 수 있는 Item으로 만들어주는 함수
        public static Item MakeItem(ItemDb itemDb)
        {
            Item item = null;

            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(itemDb.TemplateId, out itemData);
            
            if (itemData == null) return null;
            switch(itemData.itemType)
            {
                case ItemType.Weapon:
                    item = new Weapon(itemDb.TemplateId);
                    break;
                case ItemType.Armor:
                    item = new Armor(itemDb.TemplateId);
                    break;
                case ItemType.Potion:
                    item = new Potion(itemDb.TemplateId);
                    break;
                case ItemType.Etc:
                    item = new Etc(itemDb.TemplateId);
                    break;
                case ItemType.Quest:
                    item = new Quest(itemDb.TemplateId);
                    break;
                case ItemType.Coin:
                    item = new Coin(itemDb.TemplateId);
                    break;
            }

            // 아이템 정보 중에서 Init할 때 채워주는데,
            // DB에만 있고, 저장하지 않은 정보 DbId, Count 넣어줌
            if(item != null)
            {
                item.ItemDbId = itemDb.ItemDbId;
                item.Count = itemDb.Count;
            }
            return item;
        }
    }

    // Equipment Item
    public class Weapon : Item
    {
        public ClassType ClassType { get; private set; }
        public int Damage { get; private set; }
        public int MaxDurablity { get; private set; }   //
        public Weapon(int templateId) : base(ItemType.Weapon)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);
            
            if (itemData.itemType != ItemType.Weapon) return;
            WeaponData data = (WeaponData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 무기는 안 겹치기 때문에
                ClassType = data.classType;
                Damage = data.damage;
                Stackable = false; // 안 겹침
                MaxDurablity = data.maxDurability;
            }
        }
    }
    public class Armor : Item
    {
        public ArmorType ArmorType { get; private set; }
        public int Defence { get; private set; }
        public int MaxDurablity { get; private set; }   //
        public Armor(int templateId) : base(ItemType.Armor)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);
            
            if (itemData.itemType != ItemType.Armor) return;
            ArmorData data = (ArmorData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 안 겹침
                ArmorType = data.armorType;
                Defence = data.defence;
                Stackable = false; // 안 겹침
                MaxDurablity = data.maxDurability;
            }
        }
    }

    // Countable Item
    public class Potion : Item
    {
        public int MaxAmount { get; private set; } //
        public int Value { get; private set; }
        public Potion(int templateId) : base(ItemType.Potion)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);

            if (itemData.itemType != ItemType.Potion) return;
            PotionData data = (PotionData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 현재 내가 들고있는 Count 
                MaxAmount = data.maxAmount;
                Stackable = (data.maxAmount > 1); // maxAmount 1 이상이면 겹쳐지는것
                Value = data.value;
            }
        }
    }

    public class Etc : Item
    {
        public int MaxAmount { get; private set; } //
        public int Value { get; private set; }

        public Etc(int templateId) : base(ItemType.Etc)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);

            if (itemData.itemType != ItemType.Etc) return;
            EtcData data = (EtcData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 현재 내가 들고있는 Count 
                MaxAmount = data.maxAmount;
                Stackable = (data.maxAmount > 1); // maxAmount 1 이상이면 겹쳐지는것
                Value = data.value;
            }
        }
    }

    public class Quest : Item
    {
        public int MaxAmount { get; private set; } //
        public int Value { get; private set; }
        public Quest(int templateId) : base(ItemType.Quest)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);

            if (itemData.itemType != ItemType.Quest) return;
            QuestData data = (QuestData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 현재 내가 들고있는 Count 
                MaxAmount = data.maxAmount;
                Stackable = (data.maxAmount > 1); // maxAmount 1 이상이면 겹쳐지는것
                Value = data.value;
            }
        }
    }

    public class Coin : Item
    {
        public int MaxAmount { get; private set; } //
        public Coin(int templateId) : base(ItemType.Coin)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);

            if (itemData.itemType != ItemType.Coin) return;
            CoinData data = (CoinData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 현재 내가 들고있는 Count 
                MaxAmount = data.maxAmount;
                Stackable = (data.maxAmount > 1); // maxAmount 1 이상이면 겹쳐지는것
            }
        }
    }
}
