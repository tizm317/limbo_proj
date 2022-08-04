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

    // �κ��丮 ����
    GameObject gridPanel;
    RectTransform panelRect;
    float disX;
    float disY;

    UI_Item_Remove_Caution uI_Item_Remove_Caution;

    CursorController cursorController;
    bool tryRemoving = false;

    public override void Init()
    {
        // ���ε�
        Bind<GameObject>(typeof(GameObjects));

        // �ؽ�Ʈ ����
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

#region ������ ������
        // �κ��丮 RectTransform
        gridPanel = gameObject.transform.parent.gameObject;
        panelRect = gridPanel.GetComponent<RectTransform>();
        // Ŀ��
        cursorController = GameObject.Find("@Scene").GetComponent<CursorController>();

        // �巡���ϸ� boolean�� true�� �ٲ��ְ�, Ŀ����Ʈ�ѷ� set�Լ��� ����
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => 
        {
            // UI : ��Ŭ �ƴϸ� ����
            if (PointerEventData.pointerId != -1) return;

            // �κ��丮 ���� �Ѿ�� ������
            disX = Mathf.Abs(PointerEventData.position.x - gridPanel.transform.position.x); 
            disY = Mathf.Abs(PointerEventData.position.y - gridPanel.transform.position.y);
            if (disX > panelRect.rect.width / 2 || disY > panelRect.rect.height / 2)
                tryRemoving = true;
           else
                tryRemoving = false;

            // Ŀ�� : ������ Ŀ��
            cursorController.SetTryRemoving(tryRemoving);
        }, Define.UIEvent.Drag);

        // �巡�� �� �����;�
        GameObject go = GetObject((int)GameObjects.ItemIcon).gameObject;
        BindEvent
            (
                go, (PointerEventData data) => 
                    {
                        // UI : ��Ŭ �ƴϸ� ����
                        if (data.pointerId != -1) return;

                        disX = Mathf.Abs(data.position.x - gridPanel.transform.position.x); disY = Mathf.Abs(data.position.y - gridPanel.transform.position.y);
                        if (disX > panelRect.rect.width / 2 || disY > panelRect.rect.height / 2)//(Mathf.Abs(gameObject.GetComponent<RectTransform>().localPosition.x - _originalLocalPosition.x) > 100)
                        {
                            // ��� �˾�
                            uI_Item_Remove_Caution = Managers.UI.ShowPopupUI<UI_Item_Remove_Caution>();
                            // ��� Ȯ�� ��ư Ŭ�� event ����
                            uI_Item_Remove_Caution.buttonClicked -= ConfirmButtonClicked;
                            uI_Item_Remove_Caution.buttonClicked += ConfirmButtonClicked;
                            
                            // Ŀ�� �������
                            tryRemoving = false;
                            cursorController.SetTryRemoving(tryRemoving);
                        }
                        Debug.Log($"������ �̸� : {_name}, Ÿ�� : {_type}, ��� : {_grade}, ���� : {_count}");
                    }, Define.UIEvent.PointerUp
            );
        #endregion
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

    // ������ ������
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
    public void ConfirmButtonClicked()
    {
        // UI_Item_Remove_Caution ���â UI Ȯ��(Yes/No) ��ư Ŭ�� �� ȣ��
        if (uI_Item_Remove_Caution.RemoveItem())
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
        
        uI_Item_Remove_Caution.ClosePopupUI();
    }

}
