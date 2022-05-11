using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InGame : UI_Scene
{
    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
    }

    enum GameObjects
    {
    }

    enum Images
    {
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons)); // Buttons 의 enum 타입을 넘기겠다 는 의미
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);
    }

    UI_Inven ui_Inven;

    public void OnButtonClicked(PointerEventData data)
    {
        if (data.pointerId != -1) return;

        if (!ui_Inven)
            ui_Inven = Managers.UI.ShowPopupUI<UI_Inven>();
        else
            Managers.UI.ClosePopupUI(ui_Inven);
    }
}
