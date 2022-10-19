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

        //UpdateAccessibleStatesAll();
    }

    // ShopUI에서 호출
    public void SetInventoryUI(UI_Shop shopUI)
    {
        _UI_Shop = shopUI;
    }

    // 모든 슬롯 UI에 접근 가능 여부 업데이트
    public void UpdateAccessibleStatesAll()
    {
        _UI_Shop.SetAccessibleSlotRange(14);
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

    public void UpdateAllSlots()
    {
        for (int i = 0; i < itemList.Count; i++)
            UpdateSlot(i);
    }

    // 해당하는 인덱스의 슬롯 상태 및 UI 업데이트
    public void UpdateSlot(int idx)
    {
        //if (!IsValidIndex(idx)) return;

        //Item item = _items[idx];
        Item item = itemList[idx];

        // 1. 아이템이 슬롯에 존재하는 경우
        if (item != null)
        {
            // 아이콘 등록
            //_UI_inventory.SetItemIcon(idx, item.Data.IconSprite);
            _UI_Shop.SetItemIcon(idx, item.Data.IconSprite);

            _UI_Shop.SetItemName(idx, item.Data.Name);
            _UI_Shop.SetItemPrice(idx, item.Data.Price);

            // 1.1 셀 수 있는 아이템
            if (item is CountableItem ci)
            {
                // 1.1.1 수량이 0인 경우, 아이템 제거
                if (ci.IsEmpty)
                {
                    itemList[idx] = null;
                    RemoveIcon();
                    return;
                }
                else // 1.1.2 수량 텍스트 표시
                {
                    _UI_Shop.SetItemAmountText(idx, ci.Amount);
                }
            }
            else // 1.2 셀 수 없는 아이템, 수량 텍스트 제거
            {
                _UI_Shop.HideItemAmountText(idx);
            }
        }
        else // 2. 빈 슬롯, 아이템 제거
        {
            RemoveIcon();
        }

        // local : 아이템 제거
        void RemoveIcon()
        {
            _UI_Shop.RemoveItem(idx);
            _UI_Shop.HideItemAmountText(idx);
        }
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

        UpdateAllSlots();

        // Inventory UI
        _UI_Inventory = Managers.UI.ShowPopupUI<UI_Inventory>();

        // 인벤토리 UI를 거래용으로 씀
        _UI_Inventory.Trading = true;
    }

    public void CloseTradeUI()
    {
        // 거래용으로 쓰던 인벤토리 UI를 false
        _UI_Inventory.Trading = false;

        // Inventory UI
        _UI_Inventory.ClosePopupUI();
        _UI_Inventory = null;

        // Shop UI
        _UI_Shop.ClosePopupUI();
        _UI_Shop = null;
    }

    // 해당 슬롯의 아이템 정보 리턴
    public ItemData GetItemData(int idx)
    {
        //if (!IsValidIndex(idx)) return null;
        if (itemList[idx] == null) return null;

        return itemList[idx].Data;
    }
}
