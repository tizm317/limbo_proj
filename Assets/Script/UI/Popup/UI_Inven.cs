using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inven : UI_Popup
{
    // �κ��丮
    // Popup UI

    enum GameObjects
    {
        Panel,
        GridPanel,
    }


    void Start()
    {
        Init();
    }

    GameObject gridPanel;

    Dictionary<int, Data.Item> invenDict;

    public override void Init()
    {
        base.Init();

        // ���ε� ����
        Bind<GameObject>(typeof(GameObjects));

        GameObject panel = Get<GameObject>((int)GameObjects.Panel);


        // girdPanel ������ (child ��ȸ�ؼ�) ����
        gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        //gridPanel.GetComponent<GridLayout>().


        // �巡�� event �߰�
        //GetObject((int)GameObjects.GridPanel).gameObject.BindEvent(OnMouseDrag, Define.UIEvent.Drag);
        //GameObject go = GetObject((int)GameObjects.GridPanel).gameObject; // gridPanel �ϰ� ���ļ� ������
        // pointerId : 0, -1, -2, -3 (��ġ, ��Ŭ, ��Ŭ, ��Ŭ��)
        BindEvent(gridPanel, (PointerEventData data) => { if (data.pointerId != -1) return;
            gridPanel.transform.position = data.position; panel.transform.position = data.position; }, Define.UIEvent.Drag);

        invenDict = Managers.Data.InvenDict;

        // ������ ������ŭ ���̱�
        for (int i = 0; i < invenDict.Count; i++) // ���� �κ��丮 ���� �����ؼ� � ������ ���
        {
            // 1. inven_item �����ؼ� GridPanel ���Ͽ� ����
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform).gameObject;

           // gridPanel.GetComponent<GridLayout>().

            // �����տ� UI_Inven_Item ������Ʈ ���� �� �Ǿ��־ ����� ���� �ذ��
            // 1��° ��� - �ڵ�� �߰� : Util.GetOrAddComponent<UI_Inven_Item>(item);
            // (2��° ��� : ������ ������ �� add component �ؼ� UI_Inven_Item ������Ʈ �߰�)
            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>(); // Extension ���

            //item.GetComponent<UI_Inven_Item>().SetPos(item.GetComponent<RectTransform>().localPosition);

            invenItem.SetInfo(i);
            //invenItem.SetInfo($"�����{i}��");
        }
    }


}
