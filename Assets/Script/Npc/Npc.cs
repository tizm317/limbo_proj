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
    static int _id;     // id
    string _name;       // 이름
    string _type;       // 타입 : 거래, 물품 보관, 이벤트 진행, 퀘스트 제공 등.
    float _moveSpeed;   // 이동 속도
    bool _isPatrol;     // 패트롤여부

    public State curState { get; set; }

    // state
    public enum State
    {
        // 기본
        STATE_READY,
        STATE_NPC_UI_POPUP,
        STATE_DIALOGUE,
        // 상점
        //STATE_SHOP_UI_POPUP,
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
        EVENT_PUSH_DIALOGUE,
        //EVENT_OTHER_DIALOGUE,
        EVENT_QUIT_DIALOGUE,
        // 상점
        //EVENT_PUSH_SHOP,
        //EVENT_PUSH_BUY,
        //EVENT_QUIT_BUY,
        //EVENT_PUSH_SELL,
        //EVENT_QUIT_SELL,
        //EVENT_QUIT_SHOP,
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
        new EventActionTable(State.STATE_READY, Event.EVENT_NPC_CLICKED_IN_DISTANCE, null, State.STATE_NPC_UI_POPUP),
        new EventActionTable(State.STATE_NPC_UI_POPUP, Event.EVENT_PUSH_DIALOGUE, null, State.STATE_DIALOGUE),
        new EventActionTable(State.STATE_NPC_UI_POPUP, Event.EVENT_QUIT_DIALOGUE, null, State.STATE_READY),
        new EventActionTable(State.STATE_DIALOGUE, Event.EVENT_QUIT_DIALOGUE,  null, State.STATE_READY),
    };



    public void Start()
    {
        Init();
    }

    void Init()
    {
        //
        curState = State.STATE_READY;

        // action 등록...
        table[0]._action -= lookAtPlayer;
        table[0]._action += lookAtPlayer;
    }



    public void npcUIPopUp()
    {
        // 클릭되어서 npc UI 뜨는거
        // 맨 처음
        // 여기서 어떤 UI 뜰지 확인
    }

    public void dialogue()
    {
        // 대화 시스템
        // json 파일 읽어서 진행
    }

    public void lookAtPlayer()
    {
        Debug.Log("플레이어 쳐다보기");
        //Transform player;
        //Quaternion lookOnlook = Quaternion.LookRotation(player.position - this.transform.position);
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookOnlook, Time.deltaTime * 5);
    }


    public void stateMachine(Event inputEvent)
    {
        // 이거만 호출
        // 외부에서 인자로 다음 이벤트 넣어줌
        Debug.Log($"현재 상태 : {curState}");
        Debug.Log($"입력된 이벤트 : {inputEvent}");


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
                    break;
                }
            }
        }
        Debug.Log("트랜지션 완료");
        Debug.Log($"현재 상태 : {curState}");
    }
}
