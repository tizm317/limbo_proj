using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    #region Attributes
    // Attributes
    [field: SerializeField]
    public int _id { get; protected set; }            // id
    [field: SerializeField]
    public string _name { get; protected set; }       // 이름
    
    [SerializeField]
    protected bool _patrolable;       // 패트롤여부
    [SerializeField]
    protected float _moveSpeed;       // 이동 속도
    [SerializeField]
    protected float _turnSpeed = 2.0f;
    [SerializeField]
    protected Vector3 _position;      // 시작 위치

    // StateMachine
    [field: SerializeField]
    public Define.NpcState curState { get;  set; }
    protected EventActionTable[] table;

    // Dialogue
    protected Dictionary<int, Data.Dialog> dialogDict;

    // UI
    protected UI_Dialogue _UI_Dialogue;
    public bool ui_dialogue_ison { get; protected set; } = false; // UI 켜졌는지 Player_Controller에서 확인하기 위함

    // 수정할 수도 있는 것들
    static protected int num_npc = 0; // static 변수 : 처음에 npc 정보 분배할 때 사용
    public string _job { get; protected set; }
    //string _type;       // 타입 : 거래, 물품 보관, 이벤트 진행, 퀘스트 제공 등.


    // turnToPlayer 에서 사용, 상호작용하는 player를 getPlayer() 이용해서 가져옴
    protected GameObject clickedPlayer = null;


    /* Patrol */
    public Transform[] wayPoints;
    [SerializeField]
    protected int destPoint = 0;
    protected NavMeshAgent agent;
    [SerializeField]
    protected bool isPatroling = false;
    protected Coroutine patrol_coroutine;
    #endregion

    protected Coroutine turn_coroutine;


    // EventActionTable Class
    // State, Event -> Define.NpcState, Define.Event

    // state
    //public enum State
    //{
    //    // 기본
    //    STATE_IDLE,
    //    STATE_NPC_UI_POPUP,
    //    STATE_DIALOGUE,
    //    // 상점
    //    STATE_SHOP_UI_POPUP,
    //    //STATE_BUY_UI_POPUP,
    //    //STATE_SELL_UI_POPUP,
    //    //// 창고
    //    //STATE_STORAGE_UI_POPUP,
    //    //// 퀘스트
    //    //STATE_QUEST_UI_POPUP,
    //}

    //// Event
    //public enum Event
    //{
    //    // 기본 NPC EVENT
    //    EVENT_NPC_CLICKED_IN_DISTANCE,
    //    //EVENT_QUIT_UI_POPUP,
    //    EVENT_PUSH_DIALOGUE,
    //    //EVENT_OTHER_DIALOGUE,
    //    EVENT_QUIT_DIALOGUE,
    //    // 상점
    //    EVENT_PUSH_SHOP,
    //    //EVENT_PUSH_BUY,
    //    //EVENT_QUIT_BUY,
    //    //EVENT_PUSH_SELL,
    //    //EVENT_QUIT_SELL,
    //    EVENT_QUIT_SHOP,
    //    // 창고
    //    // ...
    //    // 퀘스트
    //    // ...
    //}


    //class EventActionTable
    //{
    //    public State _curState { get; set; }
    //    public Event _event { get; set; }
    //    // actions
    //    public Action _action { get; set; }
    //    public State _nextState { get; set; }

    //    public EventActionTable(State cs, Event e, Action act, State ns)
    //    {
    //        _curState = cs;
    //        _event = e;
    //        //foreach(Action act in act_arr)
    //        //_action += act;
    //        _nextState = ns;
    //    }
    //}


    public virtual void Awake()
    {
        Init();
    }
    public virtual void Init()
    {
        //
        curState = Define.NpcState.STATE_IDLE;

        // table init
        table = new EventActionTable[]
        {
            new EventActionTable(Define.NpcState.STATE_IDLE, Define.Event.EVENT_NPC_CLICKED_IN_DISTANCE, null, Define.NpcState.STATE_NPC_UI_POPUP),

            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_DIALOGUE, null, Define.NpcState.STATE_DIALOGUE), // 대화 시작
            //new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_SHOP,  null,    Define.NpcState.STATE_SHOP_UI_POPUP),
            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_QUIT_DIALOGUE, null, Define.NpcState.STATE_IDLE),

            new EventActionTable(Define.NpcState.STATE_DIALOGUE, Define.Event.EVENT_PUSH_DIALOGUE,  null, Define.NpcState.STATE_DIALOGUE), // 대화중
            new EventActionTable(Define.NpcState.STATE_DIALOGUE, Define.Event.EVENT_QUIT_DIALOGUE,  null, Define.NpcState.STATE_NPC_UI_POPUP), // 대화 끝 : 초기화
                                    
            //new EventActionTable(Define.NpcState.STATE_SHOP_UI_POPUP, Define.Event.EVENT_QUIT_SHOP,  null, Define.NpcState.STATE_NPC_UI_POPUP),

            new EventActionTable(Define.NpcState.STATE_PATROL, Define.Event.EVENT_NPC_CLICKED_IN_DISTANCE,  null, Define.NpcState.STATE_IDLE),
            new EventActionTable(Define.NpcState.STATE_IDLE, Define.Event.EVENT_PATROL,  null, Define.NpcState.STATE_PATROL),

        };

        // action 등록

        // NPC 상호작용 시작
        table[0]._action -= ClosePopupBeforeInteract;
        table[0]._action += ClosePopupBeforeInteract;
        table[0]._action -= startTurnToPlayer;
        table[0]._action += startTurnToPlayer;
        table[0]._action -= npcUIPopUp;
        table[0]._action += npcUIPopUp;
        table[0]._action -= showNpcInfo4Debug;
        table[0]._action += showNpcInfo4Debug;
        

        

        // NPC 상호작용 끝
        table[2]._action -= npcUIClose;
        table[2]._action += npcUIClose;

        if(_patrolable)
        {
            table[2]._action -= startPatrol;
            table[2]._action += startPatrol;

            // 패트롤
            table[5]._action -= stopPatrol;
            table[5]._action += stopPatrol;
        }


        // Npc 정보 읽기
        Dictionary<int, Data.Npc> dict = Managers.Data.NpcDict;
        _id = dict[num_npc].id;
        _name = dict[_id].name;
        _job = dict[_id].job;
        _patrolable = dict[_id].patrol;
        if (_patrolable) _moveSpeed = 2.0f;
        else _moveSpeed = 0.0f;

        if (_patrolable) startPatrol();

        num_npc++; // static 변수 이용

        #region Not Use
        /*
         대사 (이건 지금 딱 1개 대사 딕셔너리만 가져오는 상황.. / 실제로는 여러개)
        if (_id < Managers.Data.Dict_DialogDict.Count)
        {
            dialogDict = Managers.Data.Dict_DialogDict[_id.ToString()];
        }
        else
            dialogDict = null;
        switch (_id)
        {
            case 0:
                dialogDict = Managers.Data.Dict_DialogDict[_id];
                //dialogDict = Managers.Data.DialogDict2;
                break;
            case 1:
                dialogDict = Managers.Data.Dict_DialogDict[_id];
                dialogDict = Managers.Data.DialogDict; // 대사 테스트
                break;
            default:
                dialogDict = null;
                break;
        }
        */
        #endregion
    }

    public void stateMachine(Define.Event inputEvent)
    {
        // 이거만 호출
        // 외부에서 인자로 다음 이벤트 넣어줌
        Debug.Log($"현재 상태 : {curState} | 입력된 이벤트 : {inputEvent}");
        //Debug.Log($"입력된 이벤트 : {inputEvent}");

        if(turn_coroutine != null)  StopCoroutine(turn_coroutine);
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
    }


    #region Turn To Player
    public void startTurnToPlayer()
    {
        //Debug.Log("플레이어 쳐다보기");
        turn_coroutine = StartCoroutine(turnToPlayer());
    }
    public void getPlayer(GameObject player)
    {
        // 상호작용하는 플레이어 정보 얻기 위함
        clickedPlayer = player;
    }
    protected IEnumerator turnToPlayer()
    {
        float timeCount = 0.0f;
        while (timeCount < 1.0f)
        {
            Quaternion lookOnlook = Quaternion.LookRotation(clickedPlayer.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookOnlook, timeCount);
            timeCount = Time.deltaTime * _turnSpeed;
            yield return null;
        }
    }
    #endregion
    #region Interact With NPC
    public void ClosePopupBeforeInteract()
    {
        // Before interact with NPC
        // check Other Popup UIs
        // and Close them

        Managers.UI.CloseAllPopupUI();
    }
    public virtual void npcUIPopUp()
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
        table[3]._action -= _UI_Dialogue.dialogue;
        table[3]._action += _UI_Dialogue.dialogue;

        // NPC 대화 끝
        table[4]._action -= _UI_Dialogue.dialogEnd;
        table[4]._action += _UI_Dialogue.dialogEnd;
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
    #endregion

    #region Patrol
    protected void startPatrol()
    {
        if (isPatroling) return;
        isPatroling = true;

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = _moveSpeed;

        GotoNextPoint();
        if (_patrolable) patrol_coroutine = StartCoroutine(patrol());
    }
    protected IEnumerator patrol()
    {
        stateMachine(Define.Event.EVENT_PATROL);
        //ChangeAnim();

        while (_patrolable)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                GotoNextPoint();
            yield return null;
        }
    }
    protected void stopPatrol()
    {
        isPatroling = false;
        StopCoroutine(patrol_coroutine);
        //ChangeAnim();
    }
    private void GotoNextPoint()
    {
        if (wayPoints.Length == 0) return;

        agent.destination = wayPoints[destPoint].position;
        destPoint = (destPoint + 1) % wayPoints.Length; // cycling
    }

    /* Animation */
    // IDLE(DEFAULT) , PATROL
    protected void ChangeAnim()
    {
        Animator anim = gameObject.GetComponent<Animator>();
        switch (curState)
        {
            case Define.NpcState.STATE_IDLE:
                anim.CrossFade("Idle", 0.2f);
                break;
            case Define.NpcState.STATE_PATROL:
                anim.CrossFade("Move", 0.2f);
                break;
            default:
                anim.CrossFade("Idle", 0.2f);
                break;
        }
    }

    #endregion

    #region Debug
    protected void showNpcInfo4Debug()
    {
        // 디버깅용
        Debug.Log($"{_id}) {_name} : {_job}");
        //Debug.Log(_name);
        //Debug.Log(_job);
    }
    #endregion
    #region Not Use
    public Tuple<string, string> getSpeakersNScripts(int lineNum)
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
    #endregion
}
