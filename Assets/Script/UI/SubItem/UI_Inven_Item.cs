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

    string _name;

    Vector3 _originalPos;
    Vector3 _originalLocalPosition;

    GameObject gridPanel;


    /*void Start()
    {
        Init();
    }
    */

    bool tryRemoving = false;

    public override void Init()
    {
        // 바인딩
        Bind<GameObject>(typeof(GameObjects));

        // 텍스트 설정
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        // event 랑 바인딩해서 아이콘 클릭하면 로그 뜸 // ?
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => {  _originalPos = PointerEventData.position; Debug.Log($"아이템 클릭 : {_name}"); });

        

        // Drag event 랑 아이콘이랑 연결하고, UI_Inven_Item 위치를 마우스 따라감 -> 빼버림
        GameObject go = GetObject((int)GameObjects.ItemIcon).gameObject;

        //_originalLocalPosition = gameObject.transform.position;

        //BindEvent(go, (PointerEventData data) => { if (data.pointerId != -1) return; gameObject.transform.position = data.position; }, Define.UIEvent.Drag);

        gridPanel = gameObject.transform.parent.gameObject;
        RectTransform rectTransform = gridPanel.GetComponent<RectTransform>();
        float disX;
        float disY;

        // 드래그하면 boolean값 true로 바꿔주고, 커서컨트롤러 set함수에 전달
        CursorController cursorController = GameObject.Find("@Scene").GetComponent<CursorController>();
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => 
        {
            // UI : 좌클 아니면 리턴
            if (PointerEventData.pointerId != -1) return;

            disX = Mathf.Abs(PointerEventData.position.x - gridPanel.transform.position.x); disY = Mathf.Abs(PointerEventData.position.y - gridPanel.transform.position.y);
            if (disX > rectTransform.rect.width / 2 || disY > rectTransform.rect.height / 2)
                tryRemoving = true;
           else
                tryRemoving = false;
            cursorController.SetTryRemoving(tryRemoving);
        }, Define.UIEvent.Drag);


        BindEvent
            (
                go, (PointerEventData data) => 
                    {
                        // UI : 좌클 아니면 리턴
                        if (data.pointerId != -1) return;

                        disX = Mathf.Abs(data.position.x - gridPanel.transform.position.x); disY = Mathf.Abs(data.position.y - gridPanel.transform.position.y);
                        if (disX > rectTransform.rect.width / 2 || disY > rectTransform.rect.height / 2)//(Mathf.Abs(gameObject.GetComponent<RectTransform>().localPosition.x - _originalLocalPosition.x) > 100)
                        {
                            Managers.Resource.Destroy(gameObject); // pool로 반환
                        }
                        //Debug.Log("Delete"); 
                        //else
                        //gameObject.GetComponent<RectTransform>().localPosition = _originalLocalPosition; 

                        // 포인터 업 되면, tryRemoving false로 초기화
                        tryRemoving = false;
                        cursorController.SetTryRemoving(tryRemoving);
                    }, Define.UIEvent.PointerUp
            );

    }


    public void SetInfo(string name)
    {
        _name = name;
    }


    //public void SetPos(Vector3 InitPos)
    //{
    //    _originalLocalPosition = InitPos;
    //}
}
