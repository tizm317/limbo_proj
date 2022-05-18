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
        GameObject go = GetObject((int)GameObjects.GridPanel).gameObject;
        BindEvent(go, (PointerEventData data) => { if (data.pointerId != -1) return;
            go.transform.position = data.position; panel.transform.position = data.position; }, Define.UIEvent.Drag);


        // ������ ������ŭ ���̱�
        for (int i = 0; i < 8; i++) // ���� �κ��丮 ���� �����ؼ� � ������ ���
        {
            // 1. inven_item �����ؼ� GridPanel ���Ͽ� ����
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform).gameObject;

           // gridPanel.GetComponent<GridLayout>().

            // �����տ� UI_Inven_Item ������Ʈ ���� �� �Ǿ��־ ����� ���� �ذ��
            // 1��° ��� - �ڵ�� �߰� : Util.GetOrAddComponent<UI_Inven_Item>(item);
            // (2��° ��� : ������ ������ �� add component �ؼ� UI_Inven_Item ������Ʈ �߰�)
            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>(); // Extension ���
            
            //item.GetComponent<UI_Inven_Item>().SetPos(item.GetComponent<RectTransform>().localPosition);
            
            invenItem.SetInfo($"�����{i}��");
        }
    }


}
