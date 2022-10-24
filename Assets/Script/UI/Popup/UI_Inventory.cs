using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : UI_Popup
{
    enum GameObjects
    {
        Window,
        Borders,
        Header,
        Content,
    }

    enum Buttons
    {
        ButtonClose,
        ButtonTrim,
        ButtonSort,
    }

    

    void Start()
    {
        Init();

        // 테스트용 임시
        //_inventory.test();

        if (_inventory.LoadFinish == false)
            _inventory.InvenLoad();
        else
        {
            _inventory.UpdateAllSlot();
            _inventory.UpdateCurrency();
        }

    }

    Player player_State;
    private GameObject Scene;

    public bool Trading { get { return _isTrading; } set { _isTrading = value; } }
    bool _isTrading = false;

    public void Awake()
    {
        //Scene = GameObject.Find("@Scene");
        //player_State = Scene.GetComponent<Player>();
        //_inventory = player_State.GetPlayer().GetComponent<Inventory>();

        _inventory = GameObject.Find("@Scene").GetComponent<Inventory>();
        SlotInit();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject window = Get<GameObject>((int)GameObjects.Window);
        GameObject header = Get<GameObject>((int)GameObjects.Header);
        BindEvent(header, (PointerEventData data) => {
            if (data.pointerId != -1) return;
            window.transform.position = data.position;
        }, Define.UIEvent.Drag);


        Bind<Button>(typeof(Buttons));
        Button buttonClose = GetButton((int)Buttons.ButtonClose);
        buttonClose.gameObject.BindEvent(Quit_Inventory);
        //GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(Quit_Inventory);

        Button buttonTrim = GetButton((int)Buttons.ButtonTrim);
        buttonTrim.gameObject.BindEvent(Trim_Items);

        Button buttonSort = GetButton((int)Buttons.ButtonSort);
        buttonSort.gameObject.BindEvent(SortItems);

        /**/
        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

    }

    private void SortItems(PointerEventData obj)
    {
        _inventory.SortAll();
    }

    private void Trim_Items(PointerEventData obj)
    {
        // 인벤토리 내 아이템 사이 빈칸 없이 앞에서부터 채우기
        _inventory.trimItems();
    }

    internal void SetMyGolds(uint myGolds)
    {
        HorizontalLayoutGroup CurrenciesGroup = transform.GetComponentInChildren<HorizontalLayoutGroup>();
        Text[] curenciesTexts = CurrenciesGroup.GetComponentsInChildren<Text>();

        // Gold, Silver, Copper Init
        curenciesTexts[0].text = myGolds.ToString();
        curenciesTexts[1].text = "0";
        curenciesTexts[2].text = "0";
    }


    public void Quit_Inventory(PointerEventData data)
    {
        this.ClosePopupUI();
    }


    /* 아이템 드래그 앤 드롭 이동 */
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    [SerializeField]
    private UI_ItemSlot _beginDragSlot;         // 현재 드래그 시작한 슬롯
    private Transform _beginDragIconTransform;  // 해당 슬롯의 아이콘 트랜스폼
    private UI_ItemSlot _removeTargetSlot;      // 버리기 대상 슬롯

    private Vector3 _beginDragIconPoint;        // 드래그 시작시 슬롯 위치
    private Vector3 _beginDragCursorPoint;      // 드래그 시작시 커서 위치
    private int _beginDragSlotSiblingIndex;

    private void Update()
    {
        _ped.position = Input.mousePosition;

        OnPointerEnter(_ped.position);
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
        OnPointerExit(_ped.position);
    }


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
        if(itemSlot_tooltip != null && itemSlot_tooltip.HasItem)
        {
            ItemData itemData = _inventory.GetItemData(itemSlot_tooltip.Index);
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

    private void OnPointerExit(Vector2 pointer)
    {
        UI_ItemSlot itemSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();
        if ((itemSlot == null || !itemSlot.HasItem) && ui_tooltip)
            ui_tooltip.ClosePopupUI();
    }


    private void OnPointerDown()
    {
        if(Input.GetMouseButtonDown(0)) // left click
        {
            _beginDragSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();

            // 아이템 갖고 있는 슬롯
            if (_beginDragSlot != null && _beginDragSlot.HasItem)
            {
                // 위치 기억
                _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                // 맨 위에 보이기
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();
            }
            else _beginDragSlot = null;
        }
        else if(Input.GetMouseButtonDown(1)) // right click
        {
            UI_ItemSlot itemSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();
            // 판매
            if (Trading == true)
            {
                if (itemSlot != null && itemSlot.HasItem && itemSlot.IsAccessible)
                    _inventory.Sell(itemSlot.Index);
            }
            else // 아이템 사용
            {
                if (itemSlot != null && itemSlot.HasItem && itemSlot.IsAccessible)
                    _inventory.Use(itemSlot.Index);
            }
        }
    }

    private void OnPointerDrag()
    {
        if (!_beginDragSlot) return;
        if(Input.GetMouseButton(0))
        {
            // 위치 이동
            _beginDragIconTransform.position = _beginDragIconPoint + Input.mousePosition - _beginDragCursorPoint;
        }
    }

    private void OnPointerUp()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(_beginDragSlot)
            {
                // 위치 복원
                _beginDragIconTransform.position = _beginDragIconPoint;
                
                // UI 순서 복원
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                // 드래그 완료 처리
                EndDrag();

                // 참조 제거
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }

    UI_Item_Remove_Caution uI_Item_Remove_Caution;

    private void EndDrag()
    {
        UI_ItemSlot endDragSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();

        if (endDragSlot != null && endDragSlot.IsAccessible && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            // 빈 슬롯이 아닐 경우 그냥 Swap
            if (endDragSlot.HasItem) 
                TrySwapItems(_beginDragSlot, endDragSlot);
            else
                TrySplitItems(_beginDragSlot, endDragSlot);
        }
        else if (endDragSlot != null && endDragSlot.IsAccessible)
        {
            TrySwapItems(_beginDragSlot, endDragSlot);
        }
        else if(endDragSlot == null) // 버리기
        {
            _removeTargetSlot = _beginDragSlot; // null 오류 해결

            // 분할 버리기
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
            {
                trySplitRemove = true;
                TrySplitItems(_beginDragSlot, endDragSlot, trySplitRemove);
            }
            else
            {
                // 경고문 팝업
                uI_Item_Remove_Caution = Managers.UI.ShowPopupUI<UI_Item_Remove_Caution>();
                // 경고문 확인 버튼 클릭 event 연결
                uI_Item_Remove_Caution.buttonClicked -= ConfirmButtonClicked;
                uI_Item_Remove_Caution.buttonClicked += ConfirmButtonClicked;
            }
            
        }
    }

    UI_Item_Split uI_Item_Split;
    UI_ItemSlot _splitTargetSlotA;
    UI_ItemSlot _splitTargetSlotB;
    bool trySplitRemove = false;
    private void TrySplitItems(UI_ItemSlot beginDragSlot, UI_ItemSlot endDragSlot, bool tryRemove = false)
    {
        if(tryRemove == false)
        {
            trySplitRemove = false;

            if (beginDragSlot == endDragSlot) return;

            int currentAmount = 0;
            currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.Index);
            if (currentAmount > 1)
            {
                _splitTargetSlotA = beginDragSlot;
                _splitTargetSlotB = endDragSlot;

                // UI 팝업
                uI_Item_Split = Managers.UI.ShowPopupUI<UI_Item_Split>();
                // 경고문 확인 버튼 클릭 event 연결
                uI_Item_Split.buttonClicked -= SplitConfirmButtonClicked;
                uI_Item_Split.buttonClicked += SplitConfirmButtonClicked;
            }
            else
            {
                TrySwapItems(_beginDragSlot, endDragSlot);
            }
        }
        else // 분할 버리기
        {
            trySplitRemove = true;

            if (beginDragSlot == endDragSlot) return;
            int currentAmount = 0;
            currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.Index);
            if (currentAmount > 1)
            {
                _splitTargetSlotA = beginDragSlot;
                //_splitTargetSlotB = endDragSlot; // null

                // UI 팝업
                uI_Item_Split = Managers.UI.ShowPopupUI<UI_Item_Split>();
                // 경고문 확인 버튼 클릭 event 연결
                uI_Item_Split.buttonClicked -= SplitConfirmButtonClicked;
                uI_Item_Split.buttonClicked += SplitConfirmButtonClicked;
            }
            else
            {
                //TrySwapItems(_beginDragSlot, endDragSlot);
                if(currentAmount == 1) // 하나면 그냥 버림
                {
                    // 경고문 팝업
                    uI_Item_Remove_Caution = Managers.UI.ShowPopupUI<UI_Item_Remove_Caution>();
                    // 경고문 확인 버튼 클릭 event 연결
                    uI_Item_Remove_Caution.buttonClicked -= ConfirmButtonClicked;
                    uI_Item_Remove_Caution.buttonClicked += ConfirmButtonClicked;
                }
            }
        }
    }

    public void SplitConfirmButtonClicked()
    {
        if(trySplitRemove == false)
        {
            if (uI_Item_Split.SplitItems() > 0)
            {
                _inventory.SplitItems(_splitTargetSlotA.Index, _splitTargetSlotB.Index, uI_Item_Split.SplitItems());
            }

            // 참조 제거
            _splitTargetSlotA = null;
            _splitTargetSlotB = null;

            uI_Item_Split.ClosePopupUI();
        }
        else
        {
            if (uI_Item_Split.SplitItems() > 0)
            {
                //_splitTargetSlotB 가 null 이라서
                //_splitTargetSlotB.Index 대신 -1 넣음(어차피 안 씀) 
                _inventory.SplitItems(_splitTargetSlotA.Index, -1, uI_Item_Split.SplitItems(), trySplitRemove);
            }

            // 참조 제거
            _splitTargetSlotA = null;
            _splitTargetSlotB = null;

            uI_Item_Split.ClosePopupUI();
        }

        trySplitRemove = false;
    }


    public void ConfirmButtonClicked()
    {
        // UI_Item_Remove_Caution 경고창 UI 확인(Yes/No) 버튼 클릭 시 호출
        if (uI_Item_Remove_Caution.RemoveItem())
        {
            // count 갯수 줄이기
            _inventory.Remove(_removeTargetSlot.Index); // 왜 null 이지? -> _removeTargetSlot (다른변수에 저장) 해결

            //if (_count > 1)
            //{
            //    _count--;
            //    changeCount(_id);
            //    //invenDict[_id].count = _count;
            //}
            //else
            //{
            //    _count--;
            //    changeCount(_id);
            //    //invenDict[_id].count = _count;
            //    Managers.Resource.Destroy(gameObject); // pool로 반환
            //}
        }

        // 참조 제거
        _removeTargetSlot = null;

        uI_Item_Remove_Caution.ClosePopupUI();
    }

    Inventory _inventory;

    public void OnEnable()
    {
        _inventory.SetInventoryUI(this);
    }

    // Inventory에서 호출
    //public void SetInventory(Inventory inventory)
    //{
    //    _inventory = inventory;
    //}

    private void TrySwapItems(UI_ItemSlot from, UI_ItemSlot to)
    {
        if (from == to) return;

        // 슬롯 아이콘 이미지 교환
        from.SwapIcon(to);

        // 실제 아이템 교환
        _inventory.Swap(from.Index, to.Index);
    }


    List<UI_ItemSlot> _slotUIList;

    public void SlotInit()
    {
        _slotUIList = new List<UI_ItemSlot>(GetComponentsInChildren<UI_ItemSlot>(true));
        int index = 0;
        foreach(UI_ItemSlot slot in _slotUIList)
        {
            slot.SetSlotIndex(index++);
        }
    }

    // 접근 가능한 슬롯 범위 설정
    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
            _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
    }

    public void SetItemIcon(int idx, Sprite iconSprite)
    {
        _slotUIList[idx].SetItem(iconSprite);
    }

    public void SetItemAmountText(int idx, int amount)
    {
        _slotUIList[idx].SetItemAmount(amount);
    }

    public void HideItemAmountText(int idx)
    {
        _slotUIList[idx].SetItemAmount(0);
    }
    
    public void RemoveItem(int idx)
    {
        _slotUIList[idx].RemoveItem();
    }

    internal bool Equip(EquipmentItem equipmentItem, out EquipmentItem exchangedItem)
    {
        // 교체된 아이템
        exchangedItem = null;

        UI_Equipment _UI_Equipment = transform.parent.GetComponentInChildren<UI_Equipment>();
        bool success = _UI_Equipment.Equip(equipmentItem, out exchangedItem);
        return success;
    }
}
