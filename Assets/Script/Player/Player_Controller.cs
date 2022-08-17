using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    const float PlayerSpeed = 4.0f;
    const float AttackDelay = 2.6f;
    // Start is called before the first frame update
    [SerializeField]
    InputManager input;
    [SerializeField]
    public List<Stat> stat = new List<Stat>();
    [SerializeField]
    PathFinding pathfinding;
    [SerializeField]
    Effect effect;
    private PlayerStat my_stat;
    private Camera cam;
    public GameObject player;
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> my_enemy = new List<GameObject>();
    public List<GameObject> my_enemy_for_skill = new List<GameObject>();
    public List<Vector3> destination = new List<Vector3>();//이동하는 목적지를 저장하는 변수
    private bool isMove, isObstacle;//캐릭터가 이동중인지 확인하는 변수
    private float speed = PlayerSpeed;//플레이어의 이동속도
    private Vector3 dir;//이동방향을 위한 변수
    private bool dash_cool = true, Ult_cool = true;//대쉬 스킬의 쿨타임을 확인하기위한 bool변수
    private bool on_skill = false;//스킬 사용중 이동을 막기 위한 bool변수
    private bool isAttack = false;
    private Animator ani;
    public GameObject potal;

    // npc 한테 이동
    private float audibleDistance = 3.0f; // NPC 대화 가능 거리 (HY)
    private UI_Dialogue ui_Dialogue;
    private bool toNpc = false;
    private Vector3 npcPos;
    private Npc npc;
    private float turnSpeed = 4.0f;
    private float turnTimeCount = 0.0f;
    private bool isTurning = false;
    private Coroutine co_turn;
    private IEnumerator enumerator;

    void Start()
    {
        
        Init();
        enumerator = turn();
    }

    void Update()
    {
        if(isTurning)
        {
            StartCoroutine(enumerator);
            //if (co_turn == null)
            //    co_turn = StartCoroutine("turn");
        }
        else
        {
            StopCoroutine(enumerator);
            //if (co_turn != null)
            //{
            //    StopCoroutine(co_turn);
            //    co_turn = null;
            //}
        }

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Dash(6));//거리 5만큼 떨어진 곳으로 이동
        else if(Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Ultimate_Skill());
        else if(my_enemy.Count != 0) 
        {
            if(my_enemy[0] != null&& Vector3.Distance(player.GetComponent<Transform>().position,my_enemy[0].GetComponent<Transform>().position) < 3)
                if(!isAttack)
                {
                    StartCoroutine(Attack(my_stat.Attack,AttackDelay));//현재 attack_delay는 1 공격속도는 2배로 늘어남 기본 1
                }

        }
        else
        {

            if (ui_Dialogue && ui_Dialogue.isOn)
            {
                cam.GetComponent<Camera_Controller>().FOV_Control(45);
                isTurning = true;
            }
            else
            {
                cam.GetComponent<Camera_Controller>().FOV_Control(60);
                isTurning = false;
            }

            if (toNpc)
                move2Npc(speed, audibleDistance);
            else
                move(speed);
        }
    }

    public void Init()
    {
        Enemy_Update();
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
        player = GameObject.Find("Player");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        ani = player.GetComponent<Animator>();
        my_stat = player.GetComponent<PlayerStat>();
        pathfinding = GameObject.Find("A*").GetComponent<PathFinding>();
    }

    private void Enemy_Update()
    {
        enemies.Clear();
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject i in temp)
                if(!enemies.Contains(i))
                    enemies.Add(i);//나중에 다시 소환 될때를 대비해서 만듬
    }


    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (ui_Dialogue)
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
                    my_enemy.Clear();//다시 땅 찍으면 타게팅을 풀어줌
                    stat.Clear();//저장해둔 스텟도 지워줌

                    toNpc = false;

                }
                else if(hit.collider.tag == "Enemy")//후에 공격과 자동 타게팅을 추가할 예정
                {
                    Lock_On(hit.transform.gameObject);//타게팅용
                    Set_Destination(my_enemy[0].GetComponent<Transform>().position);

                    toNpc = false;

                }
                else if (hit.collider.tag == "NPC")
                {
                    my_enemy.Clear();
                    stat.Clear();

                    npc = hit.collider.GetComponent<Npc>();

                    float dist = Vector3.Distance(player.transform.position, hit.collider.transform.position);
                    if(dist > audibleDistance)
                    {
                        Set_Destination(hit.collider.transform.position);
                        toNpc = true;
                    }
                    else
                    {
                        //player.transform.LookAt(hit.collider.transform.position); // npc 쳐다보기
                        //isTurning = true;
                        //Debug.Log(isTurning);
                        npcPos = hit.collider.transform.position;
                        toNpc = false;
                        if (!ui_Dialogue)
                        {
                            npc.clickedPlayer = player; // npc한테 플레이어 넘겨줌
                            npc.stateMachine(Npc.Event.EVENT_NPC_CLICKED_IN_DISTANCE);
                            ui_Dialogue = Managers.UI.ShowPopupUI<UI_Dialogue>();
                            ui_Dialogue.getNpcInfo(npc);
                            npc.connectUI(ui_Dialogue);
                        }

                    }
                }
                else
                {
                    toNpc = false;
                    return;
                }
            }
        }
    }

    #region 스킬
    IEnumerator SkillCool(string skill, float cool_time)//스킬의 이름을 string으로 입력하고 쿨타임 지정해주면 됨
    {
        Image img;
        img = GameObject.Find(skill).GetComponent<Image>();
        float max = cool_time;
        switch(skill)
        {
            case "Dash":
                
                dash_cool = false;
                break;
            case "Ultimate_Skill":
                Ult_cool = false;
                break;
            default:
                break;
        }

        while(cool_time > 0)
        {   
            img.fillAmount = (max - cool_time)/max;
            cool_time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(cool_time < 0)
            img.fillAmount = 1;
        
        switch(skill)
        {
            case "Dash":
                dash_cool = true;
                break;
            case "Ultimate_Skill":
                Ult_cool = true;
                break;
            default:
                break;
        }
    }

    void Get_Enemy()
    {
        if(!on_skill)
        {
            Enemy_Update();
            if(enemies.Count > 0 && my_enemy.Count == 0)//적이 다 죽은거 아니면
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

    void Get_Enemy(Vector3 point, float distance)
    {
        Enemy_Update();
        my_enemy.Clear();
        if(enemies.Count > 0)//적이 다 죽은거 아니면
        {
            foreach(GameObject i in enemies)
            {
                if(Vector3.Distance(i.GetComponent<Transform>().position,point) < distance)
                {
                    my_enemy_for_skill.Add(i);
                }
            }
        }
        
    }

    private void Lock_On(GameObject enemy)
    {
        my_enemy.Clear();
        stat.Clear();
        my_enemy.Add(enemy);
        stat.Add(my_enemy[0].GetComponent<Stat>());
    }
    IEnumerator Attack(int damage,float attack_delay)
    {
        if(!on_skill)
        {
            isAttack = true;
            isMove = false;
            ani.SetBool("IsAttack", true);
            ani.SetBool("IsMove",false);
            ani.SetFloat("AttackSpeed",1/attack_delay);//공격 속도조절,attack_delay가 커질수록 공격속도가 느려짐, 반대로 작아지면 공격속도 빨라짐
            while(my_enemy.Count != 0)
            {
                if(on_skill)
                    break;
                stat[0].Hp = stat[0].Hp - damage;
                
                player.GetComponent<Transform>().forward = new Vector3(my_enemy[0].GetComponent<Transform>().position.x - player.GetComponent<Transform>().position.x,0,my_enemy[0].GetComponent<Transform>().position.z - player.GetComponent<Transform>().position.z);
                if(stat[0].Hp <= 0)
                {
                    if(my_enemy.Count == 0)
                        ani.SetBool("IsAttack", false);

                    Animator enemy_Ani = my_enemy[0].GetComponent<Animator>();


                    enemy_Ani.SetTrigger("isDead");
                    Managers.Resource.Destroy(my_enemy[0]);
                    //Destroy(my_enemy[0], 2);
                    GameObject temp = my_enemy[0];
                    bool check = true;;
                    my_enemy.Clear();
                    Enemy_Update();
                    foreach(GameObject i in enemies)
                        if(i != temp && Vector3.Distance(i.GetComponent<Transform>().position,player.GetComponent<Transform>().position) < 5)
                        {
                            Lock_On(i);
                            check = false;
                            break;
                        }
                    if(check)
                    {
                        break;
                    }         
                }
                Managers.Sound.Play("Sound/Attack Jump & Hit Damage Human Sounds/Jump & Attack 2",Define.Sound.Effect);
                yield return new WaitForSeconds(attack_delay);
            }
            ani.SetBool("IsAttack", false);
            isAttack = false;
        }
    }


    IEnumerator Dash(float x)
    {
        if(dash_cool)//쿨타임이 도는중인지 먼저 확인
        {
            on_skill = true;//스킬을 사용중에는 새로운 목적지를 설정할 수 없도록 설정
            dash_cool = false;
            my_enemy.Clear();
            if(isMove)
                ani.CrossFade("Dash",0.5f);//"Dash"모션을 스테이트 머신이 아닌 크로스페이드로 지정해주고, 스테이트 머신으로 애니메이션 종료 후 IDle 혹은 Move로 이동하도록 구현
            else
                ani.CrossFade("Dash",0.3f);
            isMove = true;
            RaycastHit hit;//레이케스트 선언
            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//스크린상에서 마우스 포인터의 위치
            Vector3 dash_dir = new Vector3(hit.point.x - player.GetComponent<Transform>().position.x,0,hit.point.z- player.GetComponent<Transform>().position.z);//대쉬 방향은 현재 마우스 포인터의 방향
            Vector3 dash_dst = player.GetComponent<Transform>().position + dash_dir.normalized*x;//현재 위치에서 마우스 포인터로 거리 x만큼 떨어진 위치로 이동
            destination.Clear();//목적지를 비워줌
            destination.Add(dash_dst);
            ani.SetBool("IsMove",false);
            speed = PlayerSpeed*2;
            float deltaTime = 0.0f;
            while(true)
            {
                deltaTime += Time.deltaTime;
                if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<=0.4||deltaTime > 1f)
                {
                    speed = PlayerSpeed;
                    on_skill  = false;
                    destination.Clear();
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine(SkillCool("Dash",1.0f));
        }   
    }

    IEnumerator Ultimate_Skill()
    {
        if(Ult_cool)
        {
            on_skill = true;
            RaycastHit hit;//레이케스트 선언
            bool raycastHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//카메라의 위치에서 마우스 포인터의 위치에서 쏜 레이에 맞는 오브젝트의 위치 찾기
            Debug.Log(hit.collider.tag);
            if(hit.collider.tag == "ground")
            {
                Vector3 dir = Vector3.Normalize(hit.point - player.GetComponent<Transform>().position);
                Set_Destination(hit.point);
                
                while(true)
                {
                    if(Vector3.Distance(player.GetComponent<Transform>().position,hit.point) < 0.4)
                    {
                        Get_Enemy(player.GetComponent<Transform>().position,5);
                        if(isMove)
                            ani.CrossFade("Ult_Attack",0.5f);
                        else
                            ani.CrossFade("Ult_Attack",0.3f);
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(1.5f);
                foreach(GameObject a in my_enemy_for_skill)
                {
                    a.GetComponent<Stat>().Hp -= 50;
                    if(a.GetComponent<Stat>().Hp <= 0)
                    {
                        Animator enemy_Ani = a.GetComponent<Animator>();
                        enemy_Ani.SetTrigger("isDead");
                        Managers.Resource.Destroy(a);
                        //Destroy(a, 2);
                        
                    }
                        
                }
                my_enemy_for_skill.Clear();
                StartCoroutine(SkillCool("Ultimate_Skill",5.0f));
            }
            on_skill = false;
        }
    }
    #endregion

    #region 이동
    public void Set_Destination(Vector3 dest)
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
        ani.SetBool("IsMove",true);
    }

    public float magicNumber = 100.0f;

    public Vector3 Get_Destination()
    {
        // 목적지가 없을 때(클릭 없을 때) 리스트가 아예 없어서 y값을 Magic Number 로 설정해두고, 그 경우 미니맵 안 나타나도록
        if (destination.Count == 0)
            return new Vector3(0, magicNumber, 0);

        return destination[destination.Count -1];
    }

    private void move(float speed, float arrivalRange = 0.4f)
    {
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
            ani.SetBool("IsMove",false);
            return;
        }
        else
            Get_Enemy();//이동 안할때는 주변에 적을 탐색
    }
    
    private void move2Npc(float speed, float arrivalRange = 0.4f)
    {
        // move npc 한테 가는 버전
        // 도착 범위 다름

        float dist = Vector3.Distance(player.transform.position, destination[0]);
        if (dist < arrivalRange)
        {
            // 도착
            //player.transform.LookAt(destination[0]); // npc 쳐다보기
            npcPos = destination[0];
            toNpc = false;
            destination.Clear();
            if (!ui_Dialogue)
            {
                npc.clickedPlayer = player; // npc한테 플레이어 넘겨줌
                npc.stateMachine(Npc.Event.EVENT_NPC_CLICKED_IN_DISTANCE);
                ui_Dialogue = Managers.UI.ShowPopupUI<UI_Dialogue>();
                ui_Dialogue.getNpcInfo(npc);
                npc.connectUI(ui_Dialogue);
            }
        }
        else
            move(speed, arrivalRange);
    }
    #endregion

    IEnumerator turn()
    {
        //isTurning = true;
        Debug.Log("코루틴 돌기 시작");
        turnTimeCount = 0.0f;
        while (turnTimeCount < 1.0f)
        {
            Quaternion lookOnlook = Quaternion.LookRotation(npcPos - player.transform.position);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookOnlook, Time.deltaTime * turnSpeed);
            turnTimeCount = Time.deltaTime * turnSpeed;
            yield return null;
        }
        isTurning = false;
        Debug.Log("코루틴 끝");
    }


    public bool get_isObstacle()
    {
        // 미니맵에서 isObstacle 값 가져오기 위한 public 함수_HY
        return isObstacle;
    }

}
