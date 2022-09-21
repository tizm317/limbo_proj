using System;
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
        ScriptText,
        SpeakerNameText,
    }

    enum GameObjects
    {
        Panel,
    }

    enum Images
    {
    }


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

      
        // 대사 초기화
        GetText((int)Texts.ScriptText).text = "";
        GetText((int)Texts.SpeakerNameText).text = "";

    }

    public void startDialogue(PointerEventData data)
    {
        // 대화

        if (lineNum != -1)
        {
            Debug.Log("대화");
            npc.stateMachine(Define.Event.EVENT_PUSH_DIALOGUE);
        }

        // 대화 끝 : 버튼 비활성화
        //if (lineNum == -1)
        //    GetButton((int)Buttons.DialogueButton).interactable = false;
    }

    public void startTrade(PointerEventData data)
    {
        // 거래
        Debug.Log("거래");
        npc.stateMachine(Define.Event.EVENT_PUSH_SHOP);

    }

    public void endButtonClicked(PointerEventData data)
    {
        npc.stateMachine(Define.Event.EVENT_QUIT_DIALOGUE);
    }

    public override void ClosePopupUI()
    {
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

    public void dialogEnd()
    {
        // 대사 초기화
        GetText((int)Texts.ScriptText).text = "";
        GetText((int)Texts.SpeakerNameText).text = "";
        lineNum = 0;

        // 대화 버튼 활성화
        GetButton((int)Buttons.DialogueButton).gameObject.GetComponent<Button>().interactable = true;
    }

    public void dialogue()
    {
        //받아온 게 null 이면 대화 끝이라 가정 lineNum을 - 1로 세팅해서 종료시킴
        if (npc.getSpeakersNScripts(npc._id.ToString(), lineNum) != null)
        {
            // Speaker Name
            GetText((int)Texts.SpeakerNameText).text = (npc.getSpeakersNScripts(npc._id.ToString(), lineNum).Item1 + ":");
            // Script
            GetText((int)Texts.ScriptText).text = npc.getSpeakersNScripts(npc._id.ToString(), lineNum).Item2;
            lineNum++;
        }
        else
        {
            lineNum = -1;
            GetText((int)Texts.ScriptText).text = "대화를 마쳤습니다.";
            GetText((int)Texts.SpeakerNameText).text = "";
        }
    }

    public void setButtons(int state)
    {
        // Set Buttons Active or Not
        // At NPC's StateMachine

        // getButton 막 써도 괜찮은가..?
        switch(state)
        {
            case 1: // STATE_NPC_UI_POPUP
                if (GetButton((int)Buttons.DialogueButton) == null) // null 레퍼 오류
                    break;
                GetButton((int)Buttons.DialogueButton).interactable = true;
                GetButton((int)Buttons.TradeButton).interactable = true;
                GetButton((int)Buttons.EndButton).interactable = true;
                break;
            case 2: // STATE_DIALOGUE
                GetButton((int)Buttons.TradeButton).interactable = false;
                if (lineNum == -1)
                {
                    // 대사 끝
                    GetButton((int)Buttons.DialogueButton).interactable = false;
                    GetButton((int)Buttons.EndButton).interactable = true;
                }
                else
                {
                    GetButton((int)Buttons.DialogueButton).interactable = true;
                    GetButton((int)Buttons.EndButton).interactable = false;
                }
                break;
            case 3: // STATE_SHOP_UI_POPUP
                GetButton((int)Buttons.DialogueButton).interactable = false;
                GetButton((int)Buttons.TradeButton).interactable = false;
                GetButton((int)Buttons.EndButton).interactable = false;
                break;
            default: // STATE_IDLE,
                Debug.Log("디폴트");
                GetButton((int)Buttons.DialogueButton).interactable = true;
                GetButton((int)Buttons.TradeButton).interactable = true;
                GetButton((int)Buttons.EndButton).interactable = true;
                break;

        }
    }

}
