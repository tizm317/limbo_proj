﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Player : MonoBehaviour
{
    protected Vector3 start_pos = new Vector3(1.2f,1f,-62.6f);
    
    #region 애니메이션
    private Animator ani;
    public State Ani_State
    {
        get { return  curState;}
        set{
            curState = value;

            switch(curState)
            {
                case State.STATE_IDLE :
                    ani.CrossFade("Idle", 0.2f);
                    break;
                case State.STATE_MOVE :
                    ani.CrossFade("Move", 0.2f);
                    break;
                case State.STATE_ATTACK :
                    ani.CrossFade(job + "_Attack", 0.2f);
                    break;
                case State.STATE_DIE :
                    ani.CrossFade("Die", 0.2f);
                    break;
                case State.STATE_SKILL :
                    switch(skill)
                    {
                        case HotKey.Q :
                            ani.CrossFade(job + "_Q", 0.2f);
                            break;
                        case HotKey.W :
                            ani.CrossFade(job + "_W", 0.2f);
                            break;
                        case HotKey.E :
                            ani.CrossFade(job + "_E", 0.2f);
                            break;
                        case HotKey.R :
                            ani.CrossFade(job + "_R", 0.2f);
                            break;
                    }
                    break;
            }
        
        }
    }

    public void Ani_State_Change()
    {
        Ani_State = curState;
    }

    #endregion

    #region 플레이어 관련

    protected PlayerStat my_stat;
    GameObject player;
    protected string job;

    public enum State
    {
        STATE_IDLE,
        STATE_MOVE,
        STATE_ATTACK,
        STATE_SKILL,
        STATE_DIE,
    }
    public State curState { get; set; }
    [SerializeField]
    private float speed { get { return my_stat.MoveSpeed; } set { speed = value; } }
    private float arrivalRange = 0.4f;
    protected float attackRange = 3f;

    #endregion

    #region 외부입력

    InputManager input;
    PathFinding pathfinding;

    #endregion

    #region 스킬 및 공격 관련

    public enum HotKey
    {
        Q,
        W,
        E,
        R,
    }
    protected HotKey skill;
    [SerializeField]
    protected float[] cool = new float[4];
    protected float[] cool_max = new float[4];
    public Image[] Skill_img = new Image[4];
    protected List<GameObject> enemies = new List<GameObject>();
    private List<Stat> stat = new List<Stat>();
    private GameObject my_enemy;
    private Stat my_enemy_stat;
    private List<GameObject> my_enemy_for_skill = new List<GameObject>();
    public bool on_skill = false;//스킬 사용중 이동을 막기 위한 bool변수
    private bool isAttack = false;

    #endregion  
    
    protected Camera cam;
    
    #region 이동관련

    private List<Vector3> destination = new List<Vector3>();//이동하는 목적지를 저장하는 변수
    private bool isMove, isObstacle;//캐릭터가 이동중인지 확인하는 변수
    private Vector3 dir;//이동방향을 위한 변수
    
    #endregion

    #region NPC관련

    // go to NPC
    private float audibleDistance = 3.0f; // NPC 대화 가능 거리 (HY)
    private bool toNpc = false;
    private Vector3 npcPos;
    private Npc npc;
    // turn To NPC
    private float turnSpeed = 4.0f;
    private float turnTimeCount = 0.0f;
    private bool isTurning = false;
    private IEnumerator enumerator; // turnToNPC 코루틴용

    // Camera FOV Value for NPC Interact
    private const int FOV_Interact = 45;
    private const int FOV_Normal = 60;

    // NPC하고 상호작용중인지 확인용
    public bool IsInteractWithNPC { get; private set; }

    #endregion

    #region 미니맵 관련

    // 미니맵 경로 그리기
    UI_MiniMap ui_MiniMap;

    #endregion

    void Start()
    {
        Init();
        abstract_Init();
        Passive();
    }

    void Update()
    {
        switch(curState)
        {
            case State.STATE_IDLE :
                Idle();
                break;
            case State.STATE_MOVE :
                Move();
                break;
            case State.STATE_ATTACK :
                Attack();
                break;
            case State.STATE_DIE :
                Die();
                break;
            case State.STATE_SKILL :
                Run_Skill();
                break;
        }
        
    }

    void Init()
    {
        curState = State.STATE_IDLE;
        player = GameObject.Find("Player");
        ani = player.GetComponent<Animator>();
        my_stat = player.GetComponent<PlayerStat>();
    
        Enemy_Update();

        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        Managers.Input.KeyAction -= OnKeyClicked;
        Managers.Input.KeyAction += OnKeyClicked;

        cam = Camera.main;
        
        pathfinding = GameObject.Find("A*").GetComponent<PathFinding>();
        enumerator = turnToNPC(); // 코루틴

        // 미니맵
        ui_MiniMap = GameObject.Find("@UI_Root").GetComponentInChildren<UI_MiniMap>(); 
    }

    public abstract void abstract_Init();//여기서 직업 정보, 스킬 쿨, 사거리등을 지정해줌
    void Idle()
    {
        Get_Enemy();
        if(my_enemy != null)
            curState = State.STATE_ATTACK;
    }

    #region 이동

    void Move()
    {
        if(my_enemy != null)
        {
            if(Vector3.Distance(player.transform.position, my_enemy.transform.position) < attackRange)
            {
                destination.Clear();
            }
        }
        if(destination.Count > 0)
        {
            dir = new Vector3(destination[0].x - player.GetComponent<Transform>().position.x, 0f,destination[0].z - player.GetComponent<Transform>().position.z);//플레이어가 이동하는 방향을 설정
            if(isMove)//움직여도 되는가?
            {
                if(!isObstacle)//장애물이 없다면
                {
                    if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<= arrivalRange)//너무 가깝게 찍으면 움직일 필요 없음
                    {
                        destination.RemoveAt(0);
                        return;
                    }
                    else
                    {
                        player.GetComponent<Transform>().position += dir.normalized * Time.deltaTime * speed;//시간동안에 걸쳐서 속도만큼의 빠르기로 이동
                        player.GetComponent<Transform>().forward = dir;
                    }
                }
                else
                {
                    if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<= arrivalRange && destination.Count != 0)//너무 가깝게 찍으면 움직일 필요 없음
                    {
                        destination.RemoveAt(0);    
                    }
                    else
                    {
                        player.GetComponent<Transform>().position += dir.normalized * Time.deltaTime * speed;//시간동안에 걸쳐서 속도만큼의 빠르기로 이동
                        player.GetComponent<Transform>().forward = dir;
                    }
                }
            }
            Managers.Sound.Play("Sound/Footsteps - Essentials/Footsteps_Grass/Footsteps_Grass_Run/Footsteps_Grass_Run_02",Define.Sound.Effect, 1.0f, true);
        }
        else if(destination.Count == 0 && isMove == true)
        {
            isMove = false;
            if(my_enemy != null)
                curState = State.STATE_ATTACK;
            else
                curState = State.STATE_IDLE;
            Ani_State_Change();
            ui_MiniMap.clearLine(); // 미니맵 경로 지우기
            return;
        }
    }

    private void Set_Destination(Vector3 dest)
    {
        destination.Clear();//리스트를 비워주고
        Vector3 pos = player.GetComponent<Transform>().position;
        isObstacle = Physics.Raycast(pos,new Vector3(dest.x - pos.x, 0, dest.z - pos.z),Vector3.Distance(pos,new Vector3(dest.x,pos.y,dest.z)),pathfinding.grid.unwalkableMask);
        if(isObstacle)
        {
            pathfinding.FindPath(pos,new Vector3(dest.x,pos.y,dest.z));
            destination = pathfinding.Return_Path(player.GetComponent<Transform>());
            destination.Add(new Vector3(dest.x,pos.y,dest.z));
            
        }
        else
        {
            destination.Add(dest);//새 목적지를 첫번째로
        }
        isMove = true;//움직여도 되는지판별
        curState = State.STATE_MOVE;
        Ani_State_Change();

        // 미니맵
        ui_MiniMap.drawLine();
    }

    #endregion

    #region 공격
    public virtual void Attack()//공격 함수 조건부
    {
        if(Vector3.Distance(player.transform.position, my_enemy.transform.position) > attackRange)//적이 사거리 이내에 있는지 확인 조건, 아니라면 적 방향으로 이동
        {
            Vector3 dir = (player.transform.position - my_enemy.transform.position).normalized * attackRange;
            Set_Destination(my_enemy.transform.position - dir);
            curState = State.STATE_MOVE;
            Ani_State_Change();
        }
        else
        {
            if(!isAttack)//공격을 이미 실행중이지 않은 경우에만 작동
            {
                StartCoroutine(Attack(my_stat.AttackSpeed));
            }
        }
    }

    IEnumerator Attack(float attack_speed)//공격 함수 구현부
    {
        if(!on_skill)//스킬을 사용중이라면 공격할 수 없음
        {
            isAttack = true;
            isMove = false;

            curState = State.STATE_ATTACK;
            Managers.Sound.Play("Sound/Attack Jump & Hit Damage Human Sounds/Jump & Attack 2",Define.Sound.Effect);
            Ani_State_Change();
            player.transform.LookAt(my_enemy.transform);
            yield return new WaitForSeconds(0.867f);//공격 애니메이션 시간
            //my_enemy_stat.Hp -= damage;
            my_enemy_stat.OnAttacked(my_stat);  //stat 스크립트에 hp 함수 만듬
            if (my_enemy_stat.Hp <= 0)//적 체력이 0보다 작거나 같다면
            {
                my_enemy = null;
                //Managers.Resource.Destroy(my_enemy);
            }
            curState = State.STATE_IDLE;
            Ani_State_Change();
            yield return new WaitForSeconds(1/attack_speed);//1초를 공격속도로 나눈 값만큼 기다렸다가 다음 공격을 수행
            isAttack = false;
        }
    }

    private void Lock_On(GameObject enemy)
    {
        my_enemy = enemy;
        my_enemy_stat = enemy.GetComponent<Stat>();
    }

    private void Enemy_Update()
    {
        enemies.Clear();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject i in temp)
                if(!enemies.Contains(i))
                    enemies.Add(i);//나중에 다시 소환 될때를 대비해서 만듬
    }

    void Get_Enemy()
    {
        if(!on_skill)
        {
            Enemy_Update();
            if(enemies.Count > 0 && my_enemy == null)//적이 다 죽은거 아니면
            {
                foreach(GameObject i in enemies)
                {
                    if(Vector3.Distance(i.GetComponent<Transform>().position,player.GetComponent<Transform>().position) < 5)
                    {
                        Lock_On(i);
                        break;
                    }
                }
            }
        }
    }


    #endregion

    #region 스킬

    public virtual void Q()
    {

    }

    public virtual void W()
    {

    }

    public virtual void E()
    {

    }

    public virtual void R()
    {

    }

    public virtual void Passive()
    {

    }

    void Run_Skill()//스킬 사용 함수
    {
        if(!on_skill)//스킬 사용 조건부, 만약 스킬을 쓰고 있는 중이 아니라면 idle한 상태로 변환
        {
            curState = State.STATE_IDLE;
            Ani_State_Change();
        }
    }

    void Cool()
    {
        for(int i = 0; i < cool.Length; i++)
        {
            if(cool[i] > 0)
                cool[i] -= Time.deltaTime;
            else if(cool[i] < 0)
                cool[i] = 0;
            //Skill_img[i].fillAmount = cool_max[i] - cool[i]/cool_max[i];
        }
    }

    #endregion

    #region 사망

    void Die()//사망 처리
    {
        StartCoroutine(Die(start_pos));
    }

    IEnumerator Die(Vector3 pos)//사망 처리 구현부
    {
        curState = State.STATE_DIE;
        Ani_State_Change();
        yield return new WaitForSeconds(3.9f);//사망 애니메이션 시간
        curState = State.STATE_IDLE;
        Ani_State_Change();
        my_stat.Hp = my_stat.MaxHp;//체력을 만땅으로 체워주고
        player.transform.position = pos;//초기 위치로 이동시켜줌
    }
    
    #endregion

    #region 입력

    void OnKeyClicked()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(!on_skill&&cool[0] == 0)
            {
                on_skill = true;
                Q();
            }
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            if(!on_skill&&cool[1] == 0)
            {
                on_skill = true;
                W();
            }
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!on_skill&&cool[2] == 0)
            {
                on_skill = true;
                E();
            }
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!on_skill&&cool[3] == 0)
            {
                on_skill = true;
                R();
            }
        }
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        // NPC와 상호작용 중
        if (npc && npc.ui_dialogue_ison)
            return;

        if(!on_skill)
        {
            if (evt == Define.MouseEvent.Right_Press || evt == Define.MouseEvent.Right_PointerDown)//마우스 오른쪽 클릭인 경우만 사용
            {
                RaycastHit hit;//레이케스트 선언
                if (!cam) return; // 호영 : 씬 전환 시 missing 레퍼 오류로 인해 없을 때 return 시킴
                bool raycastHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//카메라의 위치에서 마우스 포인터의 위치에서 쏜 레이에 맞는 오브젝트의 위치 찾기
                if (!raycastHit) return; // raycast 실패하면 return
                if(hit.collider.tag == "ground")
                {
                    Set_Destination(hit.point);//마우스에서 나간 광선이 도착한 위치를 목적지로 설정
                    my_enemy = null;//다시 땅 찍으면 타게팅을 풀어줌
                    my_enemy_stat = null;//저장해둔 스텟도 지워줌
                }
                else if(hit.collider.tag == "Enemy")//후에 공격과 자동 타게팅을 추가할 예정
                {
                    Lock_On(hit.transform.gameObject);//타게팅용
                    Set_Destination(my_enemy.GetComponent<Transform>().position);
                }
                else if (hit.collider.tag == "NPC")
                {
                    my_enemy = null;
                    stat = null;

                    npc = hit.collider.GetComponent<Npc>();

                    float dist = Vector3.Distance(player.transform.position, hit.collider.transform.position);
                    if(dist > audibleDistance)
                    {
                        // 대화 가능 범위보다 멀리 있는 경우
                        // NPC2Move 로 이동
                        Set_Destination(hit.collider.transform.position);
                        toNpc = true;
                    }
                    else
                    {
                        // 대화 가능 범위 내
                        // 바로 NPC와 대화 상호작용
                        npcPos = hit.collider.transform.position;
                        toNpc = false;

                        npc.getPlayer(player); // npc한테 플레이어 넘겨줌
                        npc.stateMachine(Define.Event.EVENT_NPC_CLICKED_IN_DISTANCE);
                    }
                }
                else
                    return;
            }
        }
    }

    #endregion

    #region 호영이형
    public float magicNumber = 100.0f;
    public Vector3 Get_Destination()
    {
        // 목적지가 없을 때(클릭 없을 때) 리스트가 아예 없어서 y값을 Magic Number 로 설정해두고, 그 경우 미니맵 안 나타나도록
        if (destination.Count == 0)
            return new Vector3(0, magicNumber, 0);

        return destination[destination.Count - 1];
    }

    public bool get_isObstacle()
    {
        // 미니맵에서 isObstacle 값 가져오기 위한 public 함수_HY
        return isObstacle;
    }

    public void emotion(int num)
    {
        ani.SetInteger("Emotion", num);
        ani.SetTrigger("EmotionTrigger");
    }

    private void initEmotion()
    {
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ani.SetInteger("Emotion", -1);
        }
    }

    IEnumerator turnToNPC()
    {
        // 플레이어가 NPC 바라보는 코루틴
        //Debug.Log("코루틴 돌기 시작");
        turnTimeCount = 0.0f;
        while (turnTimeCount < 1.0f)
        {
            Quaternion lookOnlook = Quaternion.LookRotation(npcPos - player.transform.position);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookOnlook, Time.deltaTime * turnSpeed);
            turnTimeCount = Time.deltaTime * turnSpeed;
            yield return null;
        }
        isTurning = false;
        //Debug.Log("코루틴 끝");
    }

    private void move2Npc(float speed, float arrivalRange = 0.4f)
    {
        // move npc 한테 가는 버전
        // 도착 범위 다름

        float dist = Vector3.Distance(player.transform.position, destination[0]);
        if (dist < arrivalRange)
        {
            // 도착
            npcPos = destination[0];
            toNpc = false;
            destination.Clear();

            npc.getPlayer(player); 
            npc.stateMachine(Define.Event.EVENT_NPC_CLICKED_IN_DISTANCE);
        }
        else
            Move();
    }
    #endregion
}