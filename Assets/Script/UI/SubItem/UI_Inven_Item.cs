using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    // 인벤토리 내 아이템

    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
    }

    // item 정보 (묶어서 정리할것)
    public int _id;
    public string _name;
    public string _type;
    public string _grade;
    public int _count;
    //

    // 인벤토리 관련
    GameObject gridPanel;
    RectTransform panelRect;
    float disX;
    float disY;

    UI_Item_Remove_Caution uI_Item_Remove_Caution;

    CursorController cursorController;
    bool tryRemoving = false;

    public override void Init()
    {
        // 바인딩
        Bind<GameObject>(typeof(GameObjects));

        // 텍스트 설정
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

#region 아이템 버리기
        // 인벤토리 RectTransform
        gridPanel = gameObject.transform.parent.gameObject;
        panelRect = gridPanel.GetComponent<RectTransform>();
        // 커서
        cursorController = GameObject.Find("@Scene").GetComponent<CursorController>();

        // 드래그하면 boolean값 true로 바꿔주고, 커서컨트롤러 set함수에 전달
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => 
        {
            // UI : 좌클 아니면 리턴
            if (PointerEventData.pointerId != -1) return;

            // 인벤토리 범위 넘어가면 버리기
            disX = Mathf.Abs(PointerEventData.position.x - gridPanel.transform.position.x); 
            disY = Mathf.Abs(PointerEventData.position.y - gridPanel.transform.position.y);
            if (disX > panelRect.rect.width / 2 || disY > panelRect.rect.height / 2)
                tryRemoving = true;
           else
                tryRemoving = false;

            // 커서 : 버리기 커서
            cursorController.SetTryRemoving(tryRemoving);
        }, Define.UIEvent.Drag);

        // 드래그 후 포인터업
        GameObject go = GetObject((int)GameObjects.ItemIcon).gameObject;
        BindEvent
            (
                go, (PointerEventData data) => 
                    {
                        // UI : 좌클 아니면 리턴
                        if (data.pointerId != -1) return;

                        disX = Mathf.Abs(data.position.x - gridPanel.transform.position.x); disY = Mathf.Abs(data.position.y - gridPanel.transform.position.y);
                        if (disX > panelRect.rect.width / 2 || disY > panelRect.rect.height / 2)//(Mathf.Abs(gameObject.GetComponent<RectTransform>().localPosition.x - _originalLocalPosition.x) > 100)
                        {
                            // 경고문 팝업
                            uI_Item_Remove_Caution = Managers.UI.ShowPopupUI<UI_Item_Remove_Caution>();
                            // 경고문 확인 버튼 클릭 event 연결
                            uI_Item_Remove_Caution.buttonClicked -= ConfirmButtonClicked;
                            uI_Item_Remove_Caution.buttonClicked += ConfirmButtonClicked;
                            
                            // 커서 원래대로
                            tryRemoving = false;
                            cursorController.SetTryRemoving(tryRemoving);
                        }
                        Debug.Log($"아이템 이름 : {_name}, 타입 : {_type}, 등급 : {_grade}, 갯수 : {_count}");
                    }, Define.UIEvent.PointerUp
            );
        #endregion
    }

    public void SetInfo(int key)
    {
        Dictionary<int, Data.Item> invenDict = Managers.Data.InvenDict;

        // item 정보 저장
        _id = invenDict[key].id;
        _name = invenDict[key].name;
        _type = invenDict[key].type;
        _grade = invenDict[key].grade;
        _count = invenDict[key].count;
    }

    // 아이템 버리기
    public void changeCount(int id)
    {
        Dictionary<int, Data.Item> invenDict = Managers.Data.InvenDict;

        invenDict[id].count = _count;

        if (invenDict[id].count == 0)
        {
            for(int i = id; i < invenDict.Count -1; i++)
            {
                invenDict.Remove(i);
                invenDict.Add(i, invenDict[i + 1]);
                invenDict[i].id = i; // id 수정
            }
            invenDict.Remove(invenDict.Count - 1); // 마지막꺼 지우기
        }

    }
    public void ConfirmButtonClicked()
    {
        // UI_Item_Remove_Caution 경고창 UI 확인(Yes/No) 버튼 클릭 시 호출
        if (uI_Item_Remove_Caution.RemoveItem())
        {
            // count 갯수 줄이기
            if (_count > 1)
            {
                _count--;
                changeCount(_id);
                //invenDict[_id].count = _count;
            }
            else
            {
                _count--;
                changeCount(_id);
                //invenDict[_id].count = _count;
                Managers.Resource.Destroy(gameObject); // pool로 반환
            }
        }
        Debug.Log($"아이템 이름 : {_name}, 타입 : {_type}, 등급 : {_grade}, 갯수 : {_count}");
        
        uI_Item_Remove_Caution.ClosePopupUI();
    }

}
