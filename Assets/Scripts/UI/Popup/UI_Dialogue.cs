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
        QuestButton,
        MapButton,
    }

    enum Texts
    {
        DialogueText,
        TradeText,
        EndText,
        ScriptText,
        SpeakerNameText,
        QuestText,
        MapText,
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

    Dictionary<int, Button> buttonDict = new Dictionary<int, Button>();

    private void Start()
    {
        Init();
    }

    public void SetJobButtons()
    {
        if (buttonDict.Count == 0) return;

        foreach (Button button in buttonDict.Values)
        {
            button.gameObject.SetActive(false);
        }
        switch (npc._job)
        {
            case "MerchantNpc":
                buttonDict[(int)Buttons.TradeButton].gameObject.SetActive(true);
                break;
            case "MapNpc":
                buttonDict[(int)Buttons.MapButton].gameObject.SetActive(true);
                break;
            case "QuestNpc":
                buttonDict[(int)Buttons.QuestButton].gameObject.SetActive(true);
                break;
        }
        buttonDict[(int)Buttons.DialogueButton].gameObject.SetActive(true);
        buttonDict[(int)Buttons.EndButton].gameObject.SetActive(true);

        // 대사 초기화
        GetText((int)Texts.ScriptText).text = "";
        GetText((int)Texts.SpeakerNameText).text = "";
    }

    List<Button> buttons;
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        // Dictionary로 저장
        buttonDict.Clear();
        buttonDict.Add((int)Buttons.DialogueButton, GetButton((int)Buttons.DialogueButton));
        buttonDict.Add((int)Buttons.TradeButton, GetButton((int)Buttons.TradeButton));
        buttonDict.Add((int)Buttons.EndButton, GetButton((int)Buttons.EndButton));
        buttonDict.Add((int)Buttons.QuestButton, GetButton((int)Buttons.QuestButton));
        buttonDict.Add((int)Buttons.MapButton, GetButton((int)Buttons.MapButton));

        //
        GetButton((int)Buttons.DialogueButton).gameObject.BindEvent(startDialogue);
        GetButton((int)Buttons.TradeButton).gameObject.BindEvent(startTrade);
        GetButton((int)Buttons.EndButton).gameObject.BindEvent(endButtonClicked);
        GetButton((int)Buttons.QuestButton).gameObject.BindEvent(startQuest);

        GetButton((int)Buttons.MapButton).gameObject.BindEvent(startMap);

        GridLayoutGroup buttons = GetComponentInChildren<GridLayoutGroup>();
        foreach(Button button in buttons.GetComponentsInChildren<Button>())
        {
            button.gameObject.SetActive(false);
        }

        switch (npc._job)
        {
            case "MerchantNpc":
                GetButton((int)Buttons.TradeButton).gameObject.SetActive(true);
                break;
            case "MapNpc":
                GetButton((int)Buttons.MapButton).gameObject.SetActive(true);
                break;
            case "QuestNpc":
                GetButton((int)Buttons.QuestButton).gameObject.SetActive(true);
                break;
        }
        GetButton((int)Buttons.DialogueButton).gameObject.SetActive(true);
        GetButton((int)Buttons.EndButton).gameObject.SetActive(true);

        // 대사 초기화
        GetText((int)Texts.ScriptText).text = "";
        GetText((int)Texts.SpeakerNameText).text = "";

    }

    public void startDialogue(PointerEventData data)
    {
        // 대화
        if (lineNum != -1)
        {
            npc.stateMachine(Define.Event.EVENT_PUSH_DIALOGUE);
        }
    }

    public void startTrade(PointerEventData data)
    {
        // 거래
        npc.stateMachine(Define.Event.EVENT_PUSH_SHOP);

    }

    public void startQuest(PointerEventData data)
    {
        npc.stateMachine(Define.Event.EVENT_PUSH_QUEST);
    }

    public void startMap(PointerEventData data)
    {
        npc.stateMachine(Define.Event.EVENT_PUSH_MAP);
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
        SetJobButtons();
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
                switch (npc._job)
                {
                    case "MerchantNpc":
                        GetButton((int)Buttons.TradeButton).interactable = true;
                        break;
                    case "MapNpc":
                        GetButton((int)Buttons.MapButton).interactable = true;
                        break;
                    case "QuestNpc":
                        GetButton((int)Buttons.QuestButton).interactable = true;
                        break;
                }
                GetButton((int)Buttons.DialogueButton).interactable = true;
                GetButton((int)Buttons.EndButton).interactable = true;
                break;
            case 2: // STATE_DIALOGUE
                switch (npc._job)
                {
                    case "MerchantNpc":
                        GetButton((int)Buttons.TradeButton).interactable = false;
                        break;
                    case "MapNpc":
                        GetButton((int)Buttons.MapButton).interactable = false;
                        break;
                    case "QuestNpc":
                        GetButton((int)Buttons.QuestButton).interactable = false;
                        break;
                }
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
                switch(npc._job)
                {
                    case "MerchantNpc":
                        GetButton((int)Buttons.TradeButton).interactable = true;
                        break;
                    case "MapNpc":
                        GetButton((int)Buttons.MapButton).interactable = true;
                        break;
                    case "QuestNpc":
                        GetButton((int)Buttons.QuestButton).interactable = true;
                        break;
                }
                Debug.Log("디폴트");
                GetButton((int)Buttons.DialogueButton).interactable = true;
                GetButton((int)Buttons.EndButton).interactable = true;
                break;

        }
    }

}
