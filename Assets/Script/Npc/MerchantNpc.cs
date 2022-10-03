using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNpc : Npc
{
    UI_Shop _UI_Shop;
    UI_Inven _UI_Inven;

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
        _UI_Inven = Managers.UI.ShowPopupUI<UI_Inven>();
    }

    public void CloseTradeUI()
    {
        // Inventory UI
        _UI_Inven.ClosePopupUI();
        _UI_Inven = null;

        // Shop UI
        _UI_Shop.ClosePopupUI();
        _UI_Shop = null;
    }
}
