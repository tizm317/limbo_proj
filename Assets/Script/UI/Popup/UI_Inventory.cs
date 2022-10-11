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
    }

    void Start()
    {
        Init();

        _inventory.test();
    }

    public void Awake()
    {
        _inventory = GameObject.Find("Player").GetComponent<Inventory>();
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
        GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(Quit_Inventory);


        /**/
        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

    }

    public void Quit_Inventory(PointerEventData data)
    {
        this.ClosePopupUI();
    }


    /* 아이템 드래그 앤 드롭 이동 */
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private UI_ItemSlot _beginDragSlot;         // 현재 드래그 시작한 슬롯
    private Transform _beginDragIconTransform;  // 해당 슬롯의 아이콘 트랜스폼

    private Vector3 _beginDragIconPoint;        // 드래그 시작시 슬롯 위치
    private Vector3 _beginDragCursorPoint;      // 드래그 시작시 커서 위치
    private int _beginDragSlotSiblingIndex;

    private void Update()
    {
        _ped.position = Input.mousePosition;

        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
    }


    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);
        if (_rrList.Count == 0) return null;
        return _rrList[0].gameObject.GetComponent<T>();
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
            // 아이템 사용
            UI_ItemSlot itemSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();
            if(itemSlot != null && itemSlot.HasItem && itemSlot.IsAccessible)
            {
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

    private void EndDrag()
    {
        UI_ItemSlot endDragSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();

        if(endDragSlot != null && endDragSlot.IsAccessible)
        {
            TrySwapItems(_beginDragSlot, endDragSlot);
        }
        else if(endDragSlot == null) // 버리기
        {
            _inventory.Remove(_beginDragSlot.Index);
        }
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
}
