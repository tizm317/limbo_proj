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

            if (_beginDragSlot && _beginDragSlot.HasItem)
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
                // 위치 복원?
                _beginDragIconTransform.position = _beginDragIconPoint;
                
                // UI 순서 복원
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                // 드래그 완료 처리 ?
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

        if(endDragSlot && endDragSlot.IsAccessible)
        {
            TrySwapItems(_beginDragSlot, endDragSlot);
        }
    }


    Inventory _inventory;

    private void TrySwapItems(UI_ItemSlot from, UI_ItemSlot to)
    {
        if (from == to) return;
        from.SwapIcon(to);
        _inventory.Swap(from.Index, to.Index);
    }

    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        //TODO
    }

    public void SetItemIcon(int idx, Sprite iconSprite)
    {

    }

    public void SetItemAmountText(int idx, int amount)
    {

    }

    public void HideItemAmountText(int idx)
    {

    }
    
    public void RemoveItem(int idx)
    {

    }
}
