using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    void Start()
    {
        Init();
    }

    public override void Init()
    {
        // 바인딩
        Bind<GameObject>(typeof(GameObjects));

        // 텍스트 설정
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        // event 랑 바인딩
        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => { Debug.Log($"아이템 클릭 : {_name}"); });
    }


    public void SetInfo(string name)
    {
        _name = name;
    }
}
