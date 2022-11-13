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

        // DB 에서 추출한 정보로 인메모리에서 관리할 수 있는 Item으로 만들어줌
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
                case ItemType.Consumable:
                    item = new Consumable(itemDb.TemplateId);
                    break;
            }

            if(item != null)
            {
                item.ItemDbId = itemDb.ItemDbId;
                item.Count = itemDb.Count;
            }
            return item;
        }
    }

    public class Weapon : Item
    {
        public WeaponType WeaponType { get; private set; }
        public int Damage { get; private set; }
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
                Count = 1; // 안 겹침
                WeaponType = data.weaponType;
                Damage = data.damage;
                Stackable = false; // 안 겹침
            }
        }
    }public class Armor : Item
    {
        public WeaponType WeaponType { get; private set; }
        public int Defence { get; private set; }
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
                WeaponType = data.armorType;
                Defence = data.defence;
                Stackable = false; // 안 겹침
            }
        }
    }

    public class Consumable : Item
    {
        public ConsumableType ConsumableType { get; private set; }
        public int MaxCount { get; private set; }
        public Consumable(int templateId) : base(ItemType.Consumable)
        {
            // templateId를 이용해서 실제 값 찾아오기
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);

            if (itemData.itemType != ItemType.Weapon) return;
            ConsumableData data = (ConsumableData)itemData;
            {
                // 정보 채우기
                TemplateId = data.id;
                Count = 1; // 내가 들고있는 Count 
                MaxCount = data.maxCount;
                ConsumableType = data.consumableType;
                Stackable = (data.maxCount > 1); // maxCount 1 이상이면 겹쳐지는것
            }
        }
    }
}
