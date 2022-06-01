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
    public State nextState { get; set; }

    public Event curEvent { get; set; }

    // state
    public enum State
    {
        // 기본
        STATE_READY,
        STATE_NPC_UI_POPUP,
        STATE_DIALOGUE,
        // 상점
        STATE_SHOP_UI_POPUP,
        STATE_BUY_UI_POPUP,
        STATE_SELL_UI_POPUP,
        // 창고
        STATE_STORAGE_UI_POPUP,
        // 퀘스트
        STATE_QUEST_UI_POPUP,
    }

    // Event
    public enum Event
    {
        // 기본 NPC EVENT
        EVENT_NPC_CLICKED_IN_DISTANCE,
        EVENT_PUSH_DIALOGUE,
        EVENT_OTHER_DIALOGUE,
        EVENT_QUIT_DIALOGUE,
        // 상점
        EVENT_PUSH_SHOP,
        EVENT_PUSH_BUY,
        EVENT_QUIT_BUY,
        EVENT_PUSH_SELL,
        EVENT_QUIT_SELL,
        EVENT_QUIT_SHOP,
        // 창고
        // ...
        // 퀘스트
        // ...
    }

    class EventActionTable
    {
        State _curState;
        Event _event;
        //action
        State _nextState;
    }


    // method

    void start()
    {
        Init();
    }

    void Init()
    {
        //
        curState = State.STATE_READY;
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
}
