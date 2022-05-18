using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    // �κ��丮 �� ������

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
        // ���ε�
        Bind<GameObject>(typeof(GameObjects));

        // �ؽ�Ʈ ����
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        // event �� ���ε��ؼ� ������ Ŭ���ϸ� �α� �� // ?
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => {  _originalPos = PointerEventData.position; Debug.Log($"������ Ŭ�� : {_name}"); });

        

        // Drag event �� �������̶� �����ϰ�, UI_Inven_Item ��ġ�� ���콺 ���� -> ������
        GameObject go = GetObject((int)GameObjects.ItemIcon).gameObject;

        //_originalLocalPosition = gameObject.transform.position;

        //BindEvent(go, (PointerEventData data) => { if (data.pointerId != -1) return; gameObject.transform.position = data.position; }, Define.UIEvent.Drag);

        gridPanel = gameObject.transform.parent.gameObject;
        RectTransform rectTransform = gridPanel.GetComponent<RectTransform>();
        float disX;
        float disY;

        // �巡���ϸ� boolean�� true�� �ٲ��ְ�, Ŀ����Ʈ�ѷ� set�Լ��� ����
        CursorController cursorController = GameObject.Find("@Scene").GetComponent<CursorController>();
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => 
        {
            // UI : ��Ŭ �ƴϸ� ����
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
                        // UI : ��Ŭ �ƴϸ� ����
                        if (data.pointerId != -1) return;

                        disX = Mathf.Abs(data.position.x - gridPanel.transform.position.x); disY = Mathf.Abs(data.position.y - gridPanel.transform.position.y);
                        if (disX > rectTransform.rect.width / 2 || disY > rectTransform.rect.height / 2)//(Mathf.Abs(gameObject.GetComponent<RectTransform>().localPosition.x - _originalLocalPosition.x) > 100)
                        {
                            Managers.Resource.Destroy(gameObject); // pool�� ��ȯ
                        }
                        //Debug.Log("Delete"); 
                        //else
                        //gameObject.GetComponent<RectTransform>().localPosition = _originalLocalPosition; 

                        // ������ �� �Ǹ�, tryRemoving false�� �ʱ�ȭ
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
