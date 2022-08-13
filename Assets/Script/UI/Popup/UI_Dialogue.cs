﻿using System;
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

    public bool isOn = false;

    Npc npc;
    int lineNum = 0;

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

        isOn = true;

    }

    public void startDialogue(PointerEventData data)
    {
        // 대화

        if (lineNum != -1)
        {
            Debug.Log("대화");
            npc.stateMachine(Npc.Event.EVENT_PUSH_DIALOGUE);
            lineNum = npc.dialogue(lineNum);
        }

        // 대화 끝 : 버튼 비활성화
        if (lineNum == -1)
            GetButton((int)Buttons.DialogueButton).gameObject.GetComponent<Button>().interactable = false;
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
        isOn = false;
        npc.stateMachine(Npc.Event.EVENT_QUIT_DIALOGUE);
        Managers.UI.ClosePopupUI(this);
    }

    public override bool IsPeek()
    {
        return Managers.UI.IsPeek(this);
    }

    public void getNpcInfo(Npc clickedNpc)
    {
        // 대화창 UI 랑 대화하는 NPC 연결해주기 위함
        npc = clickedNpc;
    }

}
