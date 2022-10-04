using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : UI_Popup
{
    enum GameObjects
    {
        Window,
        Borders,
        Header,
        Content,
    }

    enum Buttons
    {
        ButtonClose,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject window = Get<GameObject>((int)GameObjects.Window);
        GameObject header = Get<GameObject>((int)GameObjects.Header);
        BindEvent(header, (PointerEventData data) => {
            if (data.pointerId != -1) return;
            window.transform.position = data.position;
        }, Define.UIEvent.Drag);


        Bind<Button>(typeof(Buttons));
        Button buttonClose = GetButton((int)Buttons.ButtonClose);
        GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(Quit_Inventory);
    }


    public void Quit_Inventory(PointerEventData data)
    {
        this.ClosePopupUI();
    }
}
