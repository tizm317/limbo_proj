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
    Inventory inventory;


    public override void Awake()
    {
        Init();
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
