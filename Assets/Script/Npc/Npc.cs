using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    /*
     * Npc
     * 
     */

    // attributes
    static int num_npc = 0; // static 변수 : 처음에 npc 정보 분배할 때 사용
    public int _id { get; private set; }            // id
    public string _name { get; private set; }       // 이름
    public string _job { get; private set; }

    string _type;       // 타입 : 거래, 물품 보관, 이벤트 진행, 퀘스트 제공 등.
    float _moveSpeed;   // 이동 속도
    bool _isPatrol;     // 패트롤여부

    GameObject clickedPlayer = null;
    float turnSpeed = 4.0f;
    public State curState { get; set; }

    Dictionary<int, Data.Dialog> dialogDict;

    UI_Dialogue _UI_Dialogue;
    public bool ui_dialogue_ison { get; private set; } = false; // UI 켜졌는지 Player_Controller에서 확인하기 위함
    UI_Shop _UI_Shop; 

    // state
    public enum State
    {
        // 기본
        STATE_IDLE,
        STATE_NPC_UI_POPUP,
        STATE_DIALOGUE,
        // 상점
        STATE_SHOP_UI_POPUP,
        //STATE_BUY_UI_POPUP,
        //STATE_SELL_UI_POPUP,
        //// 창고
        //STATE_STORAGE_UI_POPUP,
        //// 퀘스트
        //STATE_QUEST_UI_POPUP,
    }

    // Event
    public enum Event
    {
        // 기본 NPC EVENT
        EVENT_NPC_CLICKED_IN_DISTANCE,
        //EVENT_QUIT_UI_POPUP,
        EVENT_PUSH_DIALOGUE,
        //EVENT_OTHER_DIALOGUE,
        EVENT_QUIT_DIALOGUE,
        // 상점
        EVENT_PUSH_SHOP,
        //EVENT_PUSH_BUY,
        //EVENT_QUIT_BUY,
        //EVENT_PUSH_SELL,
        //EVENT_QUIT_SELL,
        EVENT_QUIT_SHOP,
        // 창고
        // ...
        // 퀘스트
        // ...
    }


    class EventActionTable
    {
        public State _curState { get; set; }
        public Event _event { get; set; }
        // actions
        public Action _action { get; set; }
        public State _nextState { get; set; }

        public EventActionTable(State cs, Event e, Action act, State ns)
        {
            _curState = cs;
            _event = e;
            //foreach(Action act in act_arr)
            //_action += act;
            _nextState = ns;
        }
    }



    EventActionTable[] table = new EventActionTable[]
    {
        new EventActionTable(State.STATE_IDLE, Event.EVENT_NPC_CLICKED_IN_DISTANCE, null, State.STATE_NPC_UI_POPUP),

        new EventActionTable(State.STATE_NPC_UI_POPUP, Event.EVENT_PUSH_DIALOGUE, null, State.STATE_DIALOGUE), // 대화 시작
        new EventActionTable(State.STATE_NPC_UI_POPUP, Event.EVENT_PUSH_SHOP,  null, State.STATE_SHOP_UI_POPUP),
        new EventActionTable(State.STATE_NPC_UI_POPUP, Event.EVENT_QUIT_DIALOGUE, null, State.STATE_IDLE),

        new EventActionTable(State.STATE_DIALOGUE, Event.EVENT_PUSH_DIALOGUE,  null, State.STATE_DIALOGUE), // 대화중
        new EventActionTable(State.STATE_DIALOGUE, Event.EVENT_QUIT_DIALOGUE,  null, State.STATE_NPC_UI_POPUP), // 대화 끝 : 초기화

        new EventActionTable(State.STATE_SHOP_UI_POPUP, Event.EVENT_QUIT_SHOP,  null, State.STATE_NPC_UI_POPUP),
    };


    public void Awake()
    {
        Init();
    }


    void Init()
    {
        //
        curState = State.STATE_IDLE;

        // action 등록

        // NPC 상호작용 시작
        table[0]._action -= lookAtPlayer;
        table[0]._action += lookAtPlayer;
        table[0]._action -= npcUIPopUp;
        table[0]._action += npcUIPopUp;
        table[0]._action -= showNpcInfo;
        table[0]._action += showNpcInfo;

        // NPC 거래 버튼 클릭
        table[2]._action -= shop;
        table[2]._action += shop;

        // NPC 상호작용 끝
        table[3]._action -= npcUIClose;
        table[3]._action += npcUIClose;

        // NPC 거래 끝
        table[6]._action -= closeShopUI;
        table[6]._action += closeShopUI;

        // Npc 정보 읽기
        Dictionary<int, Data.Npc> dict = Managers.Data.NpcDict;
        _id = dict[num_npc].id;
        _name = dict[_id].name;
        _job = dict[_id].job;

        //// 대사 (이건 지금 딱 1개 대사 딕셔너리만 가져오는 상황.. / 실제로는 여러개)
        //if (_id < Managers.Data.Dict_DialogDict.Count)
        //{
        //    dialogDict = Managers.Data.Dict_DialogDict[_id.ToString()];
        //}
        //else
        //    dialogDict = null;
        //switch (_id)
        //{
        //    case 0:
        //        dialogDict = Managers.Data.Dict_DialogDict[_id];
        //        //dialogDict = Managers.Data.DialogDict2;
        //        break;
        //    case 1:
        //        dialogDict = Managers.Data.Dict_DialogDict[_id];
        //        dialogDict = Managers.Data.DialogDict; // 대사 테스트
        //        break;
        //    default:
        //        dialogDict = null;
        //        break;
        //}

        num_npc++; // static 변수 이용
    }



    public void npcUIPopUp()
    {
        // 클릭되어서 맨 처음 npc UI 뜨는거
        _UI_Dialogue = Managers.UI.ShowPopupUI<UI_Dialogue>();
        if (_UI_Dialogue)
            ui_dialogue_ison = true;
        else
        {
            Debug.Log("Error : No UI_Dialogue");
            return;
        }

        // UI 쪽도 NPC 정보 필요
        _UI_Dialogue.getNpcInfo(this);

        // NPC 대화
        table[1]._action -= _UI_Dialogue.dialogue;
        table[1]._action += _UI_Dialogue.dialogue;
        table[4]._action -= _UI_Dialogue.dialogue;
        table[4]._action += _UI_Dialogue.dialogue;

        // NPC 대화 끝
        table[5]._action -= _UI_Dialogue.dialogEnd;
        table[5]._action += _UI_Dialogue.dialogEnd;
    }

    public void npcUIClose()
    {
        // npc UI 꺼짐
        _UI_Dialogue.ClosePopupUI();
        ui_dialogue_ison = false;
        _UI_Dialogue = null;
    }


    public Tuple<string, string> getSpeakersNScripts(string key, int lineNum)
    {
        // key 로 상황에 맞는 대사 가져옴
        // return Speaker's name and scripts using tuple
        
        if (Managers.Data.Dict_DialogDict.ContainsKey(key))
        {
            if (lineNum >= Managers.Data.Dict_DialogDict[key].Count)
                return null;

            return Tuple.Create(Managers.Data.Dict_DialogDict[key][lineNum].name, Managers.Data.Dict_DialogDict[key][lineNum].script);
        }
        return null;
    }

    public Tuple<string ,string> getSpeakersNScripts(int lineNum)
    {
        // return Speaker's name and scripts using tuple

        if (dialogDict != null)
        {
            if (lineNum >= dialogDict.Count)
                return null;

            return Tuple.Create(dialogDict[lineNum].name, dialogDict[lineNum].script);
        }
        return null;
    }
    // 여기서 하는 것보다 UI쪽에서 출력하는 것이 맞다 판단
    // 대사만 넘겨주는 게 맞아보임
    public void dialogue(int lineNum)
    {
        // 대화 시스템
        // json 파일 읽어서 진행
        // Debug.log 로 대사 출력하고
        // lineNum 증가시켜주고 리턴함
        // 다 읽으면 -1 리턴


        if (dialogDict != null)
        {
            //Debug.Log($"{dialogDict[lineNum].name} : {dialogDict[lineNum].script}");
            lineNum++;
            if (lineNum == dialogDict.Count)
            {
                
            }
                //return -1;
            else
            {

            }
                //return lineNum;
        }

        //return -1;
    }


    IEnumerator turnToPlayer()
    {
        float timeCount = 0.0f;
        while(timeCount < 1.0f)
        {
            Quaternion lookOnlook = Quaternion.LookRotation(clickedPlayer.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookOnlook, timeCount);
            timeCount = Time.deltaTime * turnSpeed;
            yield return null;
        }
    }

    public void lookAtPlayer()
    {
        Debug.Log("플레이어 쳐다보기");
        StartCoroutine(turnToPlayer());
    }

    private void showNpcInfo()
    {
        // 디버깅용
        Debug.Log(_id);
        Debug.Log(_name);
        Debug.Log(_job);
    }

    public void stateMachine(Event inputEvent)
    {
        // 이거만 호출
        // 외부에서 인자로 다음 이벤트 넣어줌
        Debug.Log($"현재 상태 : {curState}");
        Debug.Log($"입력된 이벤트 : {inputEvent}");
        
        StopAllCoroutines(); // 그 전 코루틴 꺼주기

        //  테이블에 정의된 각 행에 대해 비교
        for (int i = 0; i < table.Length; i++)
        {
            // 현재 상태와 일치하는지 검사
            if(curState == table[i]._curState)
            {
                // 입력된 이벤트와 일치하는지 검사
                if(inputEvent == table[i]._event)
                {
                    // 해당 트랜지션이 발생할 때 수행해야할 함수들을 실행시킴
                    // ...
                    if(table[i]._action != null)
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
        Debug.Log("트랜지션 완료");
        Debug.Log($"현재 상태 : {curState}");
    }
    

    public void shop()
    {
        _UI_Shop = Managers.UI.ShowPopupUI<UI_Shop>();
        _UI_Shop.getNpcInfo(this);
    }

    public void closeShopUI()
    {
        _UI_Shop.ClosePopupUI();
    }

    public void getPlayer(GameObject player)
    {
        // 상호작용하는 플레이어 정보 얻기 위함
        clickedPlayer = player;
    }

}
