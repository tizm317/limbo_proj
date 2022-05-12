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

    /*void Start()
    {
        Init();
    }
    */
    public override void Init()
    {


        // 바인딩
        Bind<GameObject>(typeof(GameObjects));

        // 텍스트 설정
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        // event 랑 바인딩해서 아이콘 클릭하면 로그 뜸 // ?
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => { _originalPos = PointerEventData.position; Debug.Log($"아이템 클릭 : {_name}"); });

        // Drag event 랑 아이콘이랑 연결하고, UI_Inven_Item 위치를 마우스 따라감
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
