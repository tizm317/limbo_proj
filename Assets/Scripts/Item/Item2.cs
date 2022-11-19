using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item2
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
    public Item2(ItemType itemType)
    {
        ItemType = itemType;
    }

    // DB 에서 추출한 정보로 인메모리에서 관리할 수 있는 Item으로 만들어주는 함수
    public static Item2 MakeItem(ItemInfo itemInfo)
    {
        Item2 item = null;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(itemInfo.TemplateId, out itemData);

        if (itemData == null) return null;
        switch (itemData.itemType)
        {
            case ItemType.Weapon:
                item = new Weapon(itemInfo.TemplateId);
                break;
            case ItemType.Armor:
                item = new Armor(itemInfo.TemplateId);
                break;
            case ItemType.Potion:
                item = new Potion(itemInfo.TemplateId);
                break;
            case ItemType.Etc:
                item = new Etc(itemInfo.TemplateId);
                break;
            case ItemType.Quest:
                item = new Quest(itemInfo.TemplateId);
                break;
            case ItemType.Coin:
                item = new Coin(itemInfo.TemplateId);
                break;
        }

        // 아이템 정보 중에서 Init할 때 채워주는데,
        // DB에만 있고, 저장하지 않은 정보 DbId, Count 넣어줌
        if (item != null)
        {
            item.ItemDbId = itemInfo.ItemDbId;
            item.Count = itemInfo.Count;
        }
        return item;
    }
}

// Equipment Item
public class Weapon : Item2
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
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

        if (itemData.itemType != ItemType.Weapon) return;
        Data.WeaponData data = (Data.WeaponData)itemData;
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
public class Armor : Item2
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
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

        if (itemData.itemType != ItemType.Armor) return;
        Data.ArmorData data = (Data.ArmorData)itemData;
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
public class Potion : Item2
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
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

        if (itemData.itemType != ItemType.Potion) return;
        Data.PotionData data = (Data.PotionData)itemData;
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

public class Etc : Item2
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
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

        if (itemData.itemType != ItemType.Etc) return;
        Data.EtcData data = (Data.EtcData)itemData;
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

public class Quest : Item2
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
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

        if (itemData.itemType != ItemType.Quest) return;
        Data.QuestData data = (Data.QuestData)itemData;
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

public class Coin : Item2
{
    public int MaxAmount { get; private set; } //
    public Coin(int templateId) : base(ItemType.Coin)
    {
        // templateId를 이용해서 실제 값 찾아오기
        Init(templateId);
    }

    void Init(int templateId)
    {
        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);

        if (itemData.itemType != ItemType.Coin) return;
        Data.CoinData data = (Data.CoinData)itemData;
        {
            // 정보 채우기
            TemplateId = data.id;
            Count = 1; // 현재 내가 들고있는 Count 
            MaxAmount = data.maxAmount;
            Stackable = (data.maxAmount > 1); // maxAmount 1 이상이면 겹쳐지는것
        }
    }
}
