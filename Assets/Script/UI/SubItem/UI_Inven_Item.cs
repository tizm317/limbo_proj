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
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => {  _originalPos = PointerEventData.position; Debug.Log($"������ Ŭ�� : {_name}"); });

        // �� �� ��ġ�� �ƴϳ�
        

        // Drag event �� �������̶� �����ϰ�, UI_Inven_Item ��ġ�� ���콺 ����
        GameObject go = GetObject((int)GameObjects.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { if (data.pointerId != -1) return; gameObject.transform.position = data.position; }, Define.UIEvent.Drag);

        // ?
        GameObject go2 = GetObject((int)GameObjects.ItemIcon).gameObject;
        BindEvent
            (
                go2, (PointerEventData data) => 
                    { if (Mathf.Abs(gameObject.GetComponent<RectTransform>().localPosition.x - _originalLocalPosition.x) > 100)
                            Managers.Resource.Destroy(gameObject); // pool�� ��ȯ
                        //Debug.Log("Delete"); 
                        else gameObject.GetComponent<RectTransform>().localPosition = _originalLocalPosition; 
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
