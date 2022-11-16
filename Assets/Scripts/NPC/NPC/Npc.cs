using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    #region Attributes
    [field: SerializeField]
    public int _id { get; protected set; }            // id
    [field: SerializeField]
    public string _name { get; protected set; }       // 이름

    // 수정할 수도 있는 것들
    [SerializeField]
    protected Vector3 _position;      // 시작 위치
    static protected int num_npc = 0; // static 변수 : 처음에 npc 정보 분배할 때 사용
    [field: SerializeField]
    public string _job { get; protected set; }
    //string _type;       // 타입 : 거래, 물품 보관, 이벤트 진행, 퀘스트 제공 등.

    #endregion

    public virtual void Awake()
    {
        //Init();
    }
    public virtual void Init(int id)
    {
        #region Read NPC Info from Dictionary

        Dictionary<int, Data.Npc> dict = Managers.Data.NpcDict;
        //_id = dict[num_npc].id;
        _id = id;
        _name = dict[_id].name;
        _job = dict[_id].job;
        _patrolable = dict[_id].patrol;
        num_npc++; // static 변수 이용

        #endregion
        #region State Init
        
        // State init
        curState = Define.NpcState.STATE_IDLE;

        // Table init
        table = new EventActionTable[]
        {
            new EventActionTable(Define.NpcState.STATE_IDLE, Define.Event.EVENT_NPC_CLICKED_IN_DISTANCE, null, Define.NpcState.STATE_NPC_UI_POPUP),
            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_DIALOGUE, null, Define.NpcState.STATE_DIALOGUE), 
            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_QUIT_DIALOGUE, null, Define.NpcState.STATE_IDLE),
            new EventActionTable(Define.NpcState.STATE_DIALOGUE, Define.Event.EVENT_PUSH_DIALOGUE,  null, Define.NpcState.STATE_DIALOGUE), 
            new EventActionTable(Define.NpcState.STATE_DIALOGUE, Define.Event.EVENT_QUIT_DIALOGUE,  null, Define.NpcState.STATE_NPC_UI_POPUP), 
        };


        // Action Register

        // 1. NPC Interact Begin
        table[0]._action -= ClosePopupBeforeInteract;
        table[0]._action += ClosePopupBeforeInteract;
        table[0]._action -= startTurnToPlayer;
        table[0]._action += startTurnToPlayer;
        table[0]._action -= npcUIPopUp;
        table[0]._action += npcUIPopUp;
        table[0]._action -= showNpcInfo4Debug;
        table[0]._action += showNpcInfo4Debug;

        // 2. NPC Interact end
        table[2]._action -= npcUIClose;
        table[2]._action += npcUIClose;

        // 3. NPC Patrol
        if (_patrolable)
        {
            //NavMeshAgent

            table[2]._action -= startPatrol;
            table[2]._action += startPatrol;

            table[0]._action -= stopPatrol;
            table[0]._action += stopPatrol;

            _moveSpeed = 0.5f;
            startPatrol();
        }
        else _moveSpeed = 0.0f;

        #endregion

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


    #region StateMachine
    [field: SerializeField]
    public Define.NpcState curState { get; set; }
    protected EventActionTable[] table;
    /*=======================================================================================================================*/
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

        // Change Animation
        ChangeAnim();
    /*=======================================================================================================================*/
    }
    #endregion
    #region Turn To Player
    [SerializeField]
    protected float _turnSpeed = 2.0f;
    protected Coroutine turn_coroutine;
    // turnToPlayer 에서 사용, 상호작용하는 player를 getPlayer() 이용해서 가져옴
    protected GameObject clickedPlayer = null;
    /*=======================================================================================================================*/
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
        //Vector3 npcVector3 = this.transform.rotation.eulerAngles;
        while (timeCount < 1.0f)
        {
            // position.y 는 같게 해서 기울지 않도록
            //Vector3 playerVector = new Vector3(clickedPlayer.transform.position.x, this.transform.position.y, clickedPlayer.transform.position.z);
            Quaternion lookOnlook = Quaternion.LookRotation(clickedPlayer.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookOnlook, timeCount);
            //this.transform.rotation.eulerAngles.Set(0, this.transform.rotation.eulerAngles.y, 0);
            timeCount = Time.deltaTime * _turnSpeed;
            yield return null;
        }
    }
    #endregion
    #region Interact With NPC
    /* Dialogue */
    protected Dictionary<int, Data.Dialog> dialogDict;
    /* UI */
    protected UI_Dialogue _UI_Dialogue;
    public bool ui_dialogue_ison { get; protected set; } = false; // UI 켜졌는지 Player_Controller에서 확인하기 위함
    
    /*=======================================================================================================================*/

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
    [SerializeField]
    public bool _patrolable;       // 패트롤여부
    [SerializeField]
    protected float _moveSpeed;       // 이동 속도
    [SerializeField]
    protected bool isPatroling = false;
    public Transform[] wayPoints;
    [SerializeField]
    protected int destPoint = 0;
    protected NavMeshAgent agent;
    protected Coroutine patrol_coroutine;
    /*=======================================================================================================================*/

    protected void startPatrol()
    {
        GameObject wayPointGroup = GameObject.Find("WayPointGroup1").gameObject;
        SphereCollider[] wayPointColliders = wayPointGroup.GetComponentsInChildren<SphereCollider>();
        wayPoints = new Transform[wayPointColliders.Length];
        for(int i = 0; i < wayPointColliders.Length; i++)
        {
            wayPoints[i] = wayPointColliders[i].transform;
        }

        if (isPatroling) return;

        agent = Util.GetOrAddComponent<NavMeshAgent>(this.gameObject);
        agent.autoBraking = false;
        agent.speed = _moveSpeed;
        
        // 정지되어있을 경우 다시 재생
        if (agent.isStopped == true)
            agent.isStopped = false;

        GotoNextPoint();
        if (_patrolable) patrol_coroutine = StartCoroutine(patrol());
    }
    protected IEnumerator patrol()
    {
        ChangeAnim();

        isPatroling = true;
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
        ChangeAnim();
        
        // NevMeshAgent 정지
        agent.isStopped = true;
    }
    private void GotoNextPoint()
    {
        if (wayPoints == null || wayPoints.Length == 0) return;

        agent.destination = wayPoints[destPoint].position;
        destPoint = (destPoint + 1) % wayPoints.Length; // cycling
    }

    /* Animation */
    // IDLE(PATROL) , ETC
    protected void ChangeAnim()
    {
        Animator anim = gameObject.GetComponent<Animator>();
        switch (curState)
        {
            case Define.NpcState.STATE_IDLE:
                {
                    if (_patrolable)
                        anim.CrossFade("WALK", 0.2f);
                    else
                        anim.CrossFade("WAIT", 0.2f);
                }
                break;
            default:
                anim.CrossFade("WAIT", 0.2f);
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
