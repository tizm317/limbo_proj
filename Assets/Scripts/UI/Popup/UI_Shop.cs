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

    // 플레이어
    Player player;
    Inventory inventory;

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


    List<Button> itemButtons = new List<Button>();

    public override void Init()
    {
        base.Init();
        //Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        //Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(endButtonClicked);

        // itemButtonList 
        //Button[] buttons = transform.GetComponentsInChildren<Button>();
        //foreach(Button button in buttons)
        //{
        //    if (button.gameObject.name.StartsWith("Item"))
        //    {
        //        itemButtons.Add(button);
        //        button.gameObject.BindEvent(Buy);
        //    }
        //}


        //
        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

        // 플레이어
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        inventory = GameObject.Find("@Scene").GetComponent<Inventory>();
    }



    public void endButtonClicked(PointerEventData data)
    {
        // 거래 종료
        //Debug.Log("거래 종료");
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

    internal void SetItemPrice(int idx, uint price)
    {
        Transform textGroup = _slotUIList[idx].transform.parent.GetChild(1);
        Text GoldText = textGroup.GetChild(1).GetChild(0).GetComponent<Text>();
        Text SilverText = textGroup.GetChild(1).GetChild(1).GetComponent<Text>();
        Text CopperText = textGroup.GetChild(1).GetChild(2).GetComponent<Text>();
        GoldText.text = price.ToString();
        SilverText.text = "";
        CopperText.text = "";
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;

        OnPointerEnter(_ped.position);
        OnPointerDown();
        OnPointerExit(_ped.position);
    }

    /* 아이템 드래그 앤 드롭 이동 */
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);
        if (_rrList.Count == 0) return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }

    UI_ItemDescription ui_tooltip;


    UI_ItemSlot itemSlot_tooltip;
    private void OnPointerEnter(Vector2 pointer)
    {
        // 중복 수행 방지
        if (itemSlot_tooltip == RaycastAndGetFirstComponent<UI_ItemSlot>())
            return;

        itemSlot_tooltip = RaycastAndGetFirstComponent<UI_ItemSlot>();
        if (itemSlot_tooltip != null && itemSlot_tooltip.HasItem)
        {
            ItemData itemData = npc.GetItemData(itemSlot_tooltip.Index);
            if (!ui_tooltip)
            {
                ui_tooltip = Managers.UI.ShowPopupUI<UI_ItemDescription>();
                ui_tooltip.setTooltip(itemData, pointer);
            }
            else
            {
                ui_tooltip.setTooltip(itemData, pointer);
            }
        }
    }

    private void OnPointerDown()
    {
        if(Input.GetMouseButtonDown(1)) // right click
        {
            // 아이템 슬롯 버튼 클릭 => Buy
            itemSlot_tooltip = RaycastAndGetFirstComponent<UI_ItemSlot>();
            if (itemSlot_tooltip != null && itemSlot_tooltip.HasItem)
            {
                ItemData itemData = npc.GetItemData(itemSlot_tooltip.Index);
                TryBuyItem(itemData);
            }
        }
        
    }

    private void TryBuyItem(ItemData item)
    {
        // 소유 금액 >= 아이템 가격
        if (inventory.Golds >= item.Price)
        {
            // 구매
            inventory.Buy(item);

            // 인벤토리에 아이템 추가 / 아이템 수량++
            int tempIdx;
            inventory.Add(item, idx: out tempIdx);

            //Debug.Log($"Buy {item.Name}");
        }
        else // if inventory.Gold < item.Price
        {
            //Debug.Log($"Can't Buy {item.Name}");
            Debug.Log($"You Need More Golds : {item.Price - inventory.Golds}");
        }
    }

    private void OnPointerExit(Vector2 pointer)
    {
        UI_ItemSlot itemSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();
        if ((itemSlot == null || !itemSlot.HasItem) && ui_tooltip)
            ui_tooltip.ClosePopupUI();
    }
}
