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

    /*void Start()
    {
        Init();
    }
    */
    public override void Init()
    {


        // ���ε�
        Bind<GameObject>(typeof(GameObjects));

        // �ؽ�Ʈ ����
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        // event �� ���ε��ؼ� ������ Ŭ���ϸ� �α� �� // ?
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => { _originalPos = PointerEventData.position; Debug.Log($"������ Ŭ�� : {_name}"); });

        // Drag event �� �������̶� �����ϰ�, UI_Inven_Item ��ġ�� ���콺 ����
        GameObject go = GetObject((int)GameObjects.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { if (data.pointerId != -1) return; gameObject.transform.position = data.position; }, Define.UIEvent.Drag);

        // ?
        GameObject go2 = GetObject((int)GameObjects.ItemIcon).gameObject;
        BindEvent(go2, (PointerEventData data) => { if (Mathf.Abs(gameObject.transform.position.x - _originalPos.x) > 100) Debug.Log("Delete"); else gameObject.transform.position = _originalPos; }, Define.UIEvent.PointerUp);
    }


    public void SetInfo(string name)
    {
        _name = name;
    }
}
