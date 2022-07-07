using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Dialogue : UI_Popup
{
    // 대화창 UI

    enum Buttons
    {
        DialogueButton,
        TradeButton,
        EndButton,
    }

    enum Texts
    {
        DialogueText,
        TradeText,
        EndText,
    }

    enum GameObjects
    {
        Panel,
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

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.DialogueButton).gameObject.BindEvent(startDialogue);
        GetButton((int)Buttons.TradeButton).gameObject.BindEvent(startTrade);
        GetButton((int)Buttons.EndButton).gameObject.BindEvent(endButtonClicked);
    }

    public void startDialogue(PointerEventData data)
    {
        // 대화
        Debug.Log("대화");
    }

    public void startTrade(PointerEventData data)
    {
        // 거래
        Debug.Log("거래");
    }

    public void endButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }

    public override void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
