using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop : UI_Popup
{
    enum GameObjects
    {
        //BG,
    }

    enum Buttons
    {
        ButtonClose,
    }
    enum Texts
    {
        //EndText,
    }

    //Npc npc;
    MerchantNpc npc;

    private void Awake()
    {
        SlotInit();
    }

    void Start()
    {
        Init();
    }

    public void OnEnable()
    {
        SetAccessibleSlotRange(14);

        //npc.SetInventoryUI(this);
    }

    public override void Init()
    {
        base.Init();
        //Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        //Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(endButtonClicked);
    }

    public void endButtonClicked(PointerEventData data)
    {
        // 거래 종료
        Debug.Log("거래 종료");
        npc.stateMachine(Define.Event.EVENT_QUIT_SHOP);
    }

    public void getNpcInfo(Npc clickedNpc)
    {
        // 대화창 UI 랑 대화하는 NPC 연결해주기 위함
        npc = clickedNpc as MerchantNpc;
    }

    List<UI_ItemSlot> _slotUIList;
    public void SlotInit()
    {
        _slotUIList = new List<UI_ItemSlot>(GetComponentsInChildren<UI_ItemSlot>(true));
        int index = 0;
        foreach (UI_ItemSlot slot in _slotUIList)
        {
            slot.SetSlotIndex(index++);
        }
    }

    // 접근 가능한 슬롯 범위 설정
    internal void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
            _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
    }

    internal void SetItemIcon(int idx, Sprite iconSprite)
    {
        _slotUIList[idx].SetItem(iconSprite);
    }

    internal void SetItemAmountText(int idx, int amount)
    {
        _slotUIList[idx].SetItemAmount(amount);
    }

    internal void RemoveItem(int idx)
    {
        _slotUIList[idx].RemoveItem();
    }

    internal void HideItemAmountText(int idx)
    {
        _slotUIList[idx].SetItemAmount(0);
    }

    internal void SetItemName(int idx, string name)
    {
        _slotUIList[idx].transform.parent.GetChild(1).GetChild(0).GetComponent<Text>().text = name;
    }

    internal void SetItemPrice(int idx, int price)
    {
        Transform textGroup = _slotUIList[idx].transform.parent.GetChild(1);
        Text GoldText = textGroup.GetChild(1).GetChild(0).GetComponent<Text>();
        Text SilverText = textGroup.GetChild(1).GetChild(1).GetComponent<Text>();
        Text CopperText = textGroup.GetChild(1).GetChild(2).GetComponent<Text>();
        GoldText.text = price.ToString();
        SilverText.text = "";
        CopperText.text = "";
    }
}
