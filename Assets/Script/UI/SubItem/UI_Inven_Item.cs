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

    // item ���� (��� �����Ұ�)
    public int _id;
    public string _name;
    public string _type;
    public string _grade;
    public int _count;
    //

    Vector3 _originalPos;
    Vector3 _originalLocalPosition;

    GameObject gridPanel;

    //Dictionary<int, Data.Item> invenDict = Managers.Data.InvenDict;

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
        //Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => 
        //{  _originalPos = PointerEventData.position; Debug.Log($"������ �̸� : {_name}, Ÿ�� : {_type}, ��� : {_grade}, ���� : {_count}"); });
        

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
                            // count ���� ���̱�
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
                                Managers.Resource.Destroy(gameObject); // pool�� ��ȯ
                            }
                        }
                        Debug.Log($"������ �̸� : {_name}, Ÿ�� : {_type}, ��� : {_grade}, ���� : {_count}");
                        //Debug.Log("Delete"); 
                        //else
                        //gameObject.GetComponent<RectTransform>().localPosition = _originalLocalPosition; 

                        // ������ �� �Ǹ�, tryRemoving false�� �ʱ�ȭ
                        tryRemoving = false;
                        cursorController.SetTryRemoving(tryRemoving);
                    }, Define.UIEvent.PointerUp
            );

    }

    public void SetInfo(int key)
    {
        Dictionary<int, Data.Item> invenDict = Managers.Data.InvenDict;

        // item ���� ����
        _id = invenDict[key].id;
        _name = invenDict[key].name;
        _type = invenDict[key].type;
        _grade = invenDict[key].grade;
        _count = invenDict[key].count;
    }

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
                invenDict[i].id = i; // id ����
            }
            invenDict.Remove(invenDict.Count - 1); // �������� �����
        }

    }

    //public void SetPos(Vector3 InitPos)
    //{
    //    _originalLocalPosition = InitPos;
    //}
}
