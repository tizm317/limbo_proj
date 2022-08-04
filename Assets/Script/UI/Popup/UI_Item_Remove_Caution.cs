using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item_Remove_Caution : UI_Popup
{
    enum GameObjects
    {
        Panel,
    }
    enum Texts
    {
        CautionText,
        YesText,
        NoText,
    }
    enum Buttons
    {
        YesButton,
        NoButton,
    }

    // event
    public delegate void OnClicked();
    public event OnClicked buttonClicked;
    bool remove; // Y/N button

    private void Start()
    {
        Init();
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.YesButton).gameObject.BindEvent(YesButtonClick, Define.UIEvent.Click);
        GetButton((int)Buttons.NoButton).gameObject.BindEvent(NoButtonClick, Define.UIEvent.Click);

        remove = false;
    }

    private void YesButtonClick(PointerEventData data)
    {
        remove = true;
        buttonClicked();
    }
    private void NoButtonClick(PointerEventData data)
    {
        remove = false;
        buttonClicked();
    }

    public bool RemoveItem()
    {
        // UI_Inven_Item 에서 어떤 버튼(Y/N) 눌렸는지 확인용
        return remove;
    }
}
