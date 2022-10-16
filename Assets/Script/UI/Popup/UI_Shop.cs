using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop : UI_Popup
{
    enum GameObjects
    {
        //BG,
    }

    enum Buttons
    {
        ButtonClose,
    }
    enum Texts
    {
        //EndText,
    }

    Npc npc;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        //Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        //Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(endButtonClicked);
    }

    public void endButtonClicked(PointerEventData data)
    {
        // 거래 종료
        Debug.Log("거래 종료");
        npc.stateMachine(Define.Event.EVENT_QUIT_SHOP);
    }

    public void getNpcInfo(Npc clickedNpc)
    {
        // 대화창 UI 랑 대화하는 NPC 연결해주기 위함
        npc = clickedNpc;
    }
}
