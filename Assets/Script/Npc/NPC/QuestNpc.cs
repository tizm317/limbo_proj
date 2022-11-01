using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : Npc
{
    // UI_QUEST로 바꾸기(임시로)
    protected UI_Shop _UI_Quest;

    public override void Awake()
    {
        //Init();
    }


    public override void Init(int id)
    {
        // NPC Default Setting
        base.Init(id);

        // Plus Quest NPC Table Setting
        EventActionTable[] tempTable = new EventActionTable[8];
        for(int i = 0; i < table.Length; i++)
            tempTable[i] = table[i];

        tempTable[table.Length] = new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_QUEST, null, Define.NpcState.STATE_QUEST_UI_POPUP); // 퀘스트 대화 시작
        tempTable[table.Length + 1] = new EventActionTable(Define.NpcState.STATE_QUEST_UI_POPUP, Define.Event.EVENT_ACCEPT_QUEST, null, Define.NpcState.STATE_NPC_UI_POPUP); // 퀘스트 수락
        tempTable[table.Length + 2] = new EventActionTable(Define.NpcState.STATE_QUEST_UI_POPUP, Define.Event.EVENT_REJECT_QUEST, null, Define.NpcState.STATE_NPC_UI_POPUP); // 퀘스트 거절

        // 퀘스트 관련
        tempTable[table.Length]._action -= ShowQuestUI;
        tempTable[table.Length]._action += ShowQuestUI;

        tempTable[table.Length + 1]._action -= CloseQuestUI;
        tempTable[table.Length + 1]._action += CloseQuestUI;

        tempTable[table.Length + 2]._action -= CloseQuestUI;
        tempTable[table.Length + 2]._action += CloseQuestUI;

        table = new EventActionTable[8];
        table = (EventActionTable[])tempTable.Clone();
    }

    public void ShowQuestUI()
    {
        Debug.Log("Show Quest UI");
        // Quest UI
        _UI_Quest = Managers.UI.ShowPopupUI<UI_Shop>();
        _UI_Quest.getNpcInfo(this);
    }

    public void CloseQuestUI()
    {
        Debug.Log("Close Quest UI");
        // Quest UI
        _UI_Quest.ClosePopupUI();
        _UI_Quest = null;
    }
}
