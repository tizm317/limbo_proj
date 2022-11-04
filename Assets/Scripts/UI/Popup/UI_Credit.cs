using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Credit : UI_Popup
{
    // Credit

    enum Buttons
    {
        CloseButton,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseButtonClicked);
    }

    private void CloseButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }
}
