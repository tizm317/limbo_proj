using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpc : Npc
{
    protected UI_Quest _UI_Quest;
    protected int _questId = 0;

    public override void Awake()
    {
        //Init();
    }


    public override void Init(int id)
    {
        // NPC Default Setting
        base.Init(id);

        _questId = dict[id].questid;

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
        //Debug.Log("Show Quest UI");
        // Quest UI

        // 안 받은 경우+ 클리어 안 한 경우
        if (Managers.Data.QuestDict[_questId].get == 0 && Managers.Data.QuestDict[_questId].clear == 0)
        {
            _UI_Quest = Managers.UI.ShowPopupUI<UI_Quest>();
            _UI_Quest.getNpcInfo(this);
            _UI_Quest.getQuest(_questId);
        }
    }

    public override void stateMachine(Define.Event inputEvent)
    {
        // 이거만 호출
        // 외부에서 인자로 다음 이벤트 넣어줌
        Debug.Log($"현재 상태 : {curState} | 입력된 이벤트 : {inputEvent}");
        //Debug.Log($"입력된 이벤트 : {inputEvent}");

        if (turn_coroutine != null) StopCoroutine(turn_coroutine);
        //StopAllCoroutines(); // 그 전 코루틴 꺼주기 (모든 코루틴 꺼서 패트롤도 꺼버려서 수정)


        //  테이블에 정의된 각 행에 대해 비교
        for (int i = 0; i < table.Length; i++)
        {
            // 현재 상태와 일치하는지 검사
            if (curState == table[i]._curState)
            {
                // 입력된 이벤트와 일치하는지 검사
                if (inputEvent == table[i]._event)
                {
                    // 퀘스트 깬 상태에서 퀘스트 버튼눌리는 경우
                    if (inputEvent == Define.Event.EVENT_PUSH_QUEST && (Managers.Data.QuestDict[_questId].get == 1 || Managers.Data.QuestDict[_questId].clear == 1))
                    {
                        continue;
                    }

                    // 해당 트랜지션이 발생할 때 수행해야할 함수들을 실행시킴
                    // ...
                    if (table[i]._action != null)
                        table[i]._action.Invoke();
                    // 테이블에 정의된 다음 상태로 현재 상태 변경
                    curState = table[i]._nextState;

                    // 버튼 활성화 셋팅
                    if (_UI_Dialogue)
                        _UI_Dialogue.setButtons((int)curState);


                    break;
                }
            }
        }

        //Debug.Log("트랜지션 완료");
        Debug.Log($"변경된 상태 : {curState}");

        // Change Animation
        ChangeAnim();
        /*=======================================================================================================================*/
    }

    public void CloseQuestUI()
    {
        //Debug.Log("Close Quest UI");
        // Quest UI
        _UI_Quest.ClosePopupUI();
        _UI_Quest = null;
    }
}
