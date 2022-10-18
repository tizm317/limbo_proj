using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNpc : Npc
{
    UI_Shop _UI_Shop;
    //UI_Inven _UI_Inven;
    UI_Inventory _UI_Inventory;


    // 판매 아이템 목록
    [SerializeField]
    public ItemData[] itemDatas = new ItemData[12];
    //Inventory inventory;

    public List<Item> itemList = new List<Item>();

    public override void Awake()
    {
        Init();
        itemListInit();
    }

    public void Start()
    {
        //inventory = this.gameObject.GetOrAddComponent<Inventory>();
        ////inventory.SetInventoryUI(_UI_Shop);
        //foreach (ItemData data in itemDatas)
        //{
        //    CountableItemData cid = data as CountableItemData;
        //    if (cid != null)
        //        inventory.Add(cid, cid.MaxAmount);
        //    else
        //        inventory.Add(data, 1);
        //}

        foreach(ItemData itemData in itemDatas)
        {
            itemList.Add(itemData.CreatItem());
        }
    }

    private void itemListInit()
    {
        // 판매 아이템 목록

        int count = 0;
        string armor = "Prefabs/Item/Armor/Item_Armor_";
        itemDatas[count++] = Managers.Resource.Load<ItemData>(armor + "Armor");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(armor + "Helmet");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(armor + "Pants");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(armor + "Shoes");

        string etc = "Prefabs/Item/ETC/Item_Etc_";
        itemDatas[count++] = Managers.Resource.Load<ItemData>(etc + "Arrow");

        string potion = "Prefabs/Item/Potion/Item_Potion_";
        itemDatas[count++] = Managers.Resource.Load<ItemData>(potion + "Both");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(potion + "HP");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(potion + "MP");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(potion + "Speed");

        string weapon = "Prefabs/Item/Weapon/Item_Weapon_";
        itemDatas[count++] = Managers.Resource.Load<ItemData>(weapon + "Axe");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(weapon + "Bow");
        itemDatas[count++] = Managers.Resource.Load<ItemData>(weapon + "Wand");
    }

    public override void Init()
    {
        // NPC Default Setting
        base.Init();

        // Plus Merchant NPC Table Setting
        EventActionTable[] tempTable = new EventActionTable[7];
        for (int i = 0; i < table.Length; i++)
            tempTable[i] = table[i];

        tempTable[table.Length] = new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_SHOP, null, Define.NpcState.STATE_SHOP_UI_POPUP);
        tempTable[table.Length + 1] = new EventActionTable(Define.NpcState.STATE_SHOP_UI_POPUP, Define.Event.EVENT_QUIT_SHOP, null, Define.NpcState.STATE_NPC_UI_POPUP);

        // 상점 관련
        tempTable[table.Length]._action -= ShowTradeUI;
        tempTable[table.Length]._action += ShowTradeUI;

        tempTable[table.Length + 1]._action -= CloseTradeUI;
        tempTable[table.Length + 1]._action += CloseTradeUI;

        table = new EventActionTable[7];
        table = (EventActionTable[])tempTable.Clone();
    }

    public void ShowTradeUI()
    {
        // Shop UI
        _UI_Shop = Managers.UI.ShowPopupUI<UI_Shop>();
        _UI_Shop.getNpcInfo(this);

        // Inventory UI
        _UI_Inventory = Managers.UI.ShowPopupUI<UI_Inventory>();
    }

    public void CloseTradeUI()
    {
        // Inventory UI
        _UI_Inventory.ClosePopupUI();
        _UI_Inventory = null;

        // Shop UI
        _UI_Shop.ClosePopupUI();
        _UI_Shop = null;
    }
}
