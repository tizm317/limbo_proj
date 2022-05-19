using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    const float PlayerSpeed = 5.0f;
    const float AttackDelay = 1.0f;
    // Start is called before the first frame update
    [SerializeField]
    InputManager input;
    [SerializeField]
    Stat stat;
    [SerializeField]
    PathFinding pathfinding;
    private PlayerStat my_stat;
    private Camera cam;
    public GameObject player,my_enemy;
    private List<Vector3> destination = new List<Vector3>();//이동하는 목적지를 저장하는 변수
    private bool isMove, isObstacle;//캐릭터가 이동중인지 확인하는 변수
    private float speed = PlayerSpeed;//플레이어의 이동속도
    private Vector3 dir;//이동방향을 위한 변수
    private bool dash_cool = true;//대쉬 스킬의 쿨타임을 확인하기위한 bool변수
    private bool on_skill = false;//스킬 사용중 이동을 막기 위한 bool변수
    private Animator ani;

    void Start()
    {
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
        player = GameObject.Find("Player");
        cam = Camera.main;
        ani = player.GetComponent<Animator>();
        my_stat = player.GetComponent<PlayerStat>();
        pathfinding = GameObject.Find("A*").GetComponent<PathFinding>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Dash(5));//거리 5만큼 떨어진 곳으로 이동
        else if(my_enemy != null && Vector3.Distance(player.GetComponent<Transform>().position,my_enemy.GetComponent<Transform>().position) < 1)
        {
            StartCoroutine(Attack(my_stat.Attack,AttackDelay));//현재 attack_delay는 1 공격속도는 2배로 늘어남 기본 1
        }
        else
            move(speed);
    }


    void OnMouseClicked(Define.MouseEvent evt)
    {
        if(!on_skill)
        {
            if(evt == Define.MouseEvent.Right_Press || evt == Define.MouseEvent.Right_PointerDown)//마우스 오른쪽 클릭인 경우만 사용
            {
                RaycastHit hit;//레이케스트 선언
                bool raycastHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//카메라의 위치에서 마우스 포인터의 위치에서 쏜 레이에 맞는 오브젝트의 위치 찾기
                Debug.Log(raycastHit);
                if (!raycastHit) return; // raycast 실패하면 return
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "ground")
                {
                    Set_Destination(hit.point);//마우스에서 나간 광선이 도착한 위치를 목적지로 설정
                    my_enemy = null;//다시 땅 찍으면 타게팅을 풀어줌
                    stat = null;//저장해둔 스텟도 지워줌
                }
                else if(hit.collider.tag == "Enemy")//후에 공격과 자동 타게팅을 추가할 예정
                {
                    Lock_On(hit.transform.gameObject);//타게팅용
                    Set_Destination(my_enemy.GetComponent<Transform>().position);
                }
                else
                { 
                    return;
                }
            }
        }
    }

    #region 스킬
    IEnumerator SkillCool(string skill, float cool_time)//스킬의 이름을 string으로 입력하고 쿨타임 지정해주면 됨
    {
        switch(skill)
        {
            case "Dash":
                dash_cool = false;
                break;
            default:
                break;
        }

        while(cool_time > 0)
        {
            cool_time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        switch(skill)
        {
            case "Dash":
                dash_cool = true;
                break;
            default:
                break;
        }
    }

    private void Lock_On(GameObject enemy)
    {
        my_enemy = enemy;
        stat = my_enemy.GetComponent<Stat>();
    }
    IEnumerator Attack(int damage,float attack_delay)
    {
        isMove = false;
        ani.SetBool("IsMove",false);
        ani.CrossFade("Attack",0.3f);
        ani.SetFloat("AttackSpeed",1/attack_delay);//공격 속도조절,attack_delay가 커질수록 공격속도가 느려짐, 반대로 작아지면 공격속도 빨라짐
        while(my_enemy != null)
        {
            stat.Hp = stat.Hp - damage;
            if(stat.Hp <= 0)
            {
                Destroy(my_enemy);
            }
            yield return new WaitForSeconds(attack_delay);
        }
    }
    
    IEnumerator Dash(float x)
    {
        if(dash_cool)//쿨타임이 도는중인지 먼저 확인
        {
            on_skill = true;//스킬을 사용중에는 새로운 목적지를 설정할 수 없도록 설정
            dash_cool = false;
            isMove = true;
            RaycastHit hit;//레이케스트 선언
            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//스크린상에서 마우스 포인터의 위치
            Vector3 dash_dir = new Vector3(hit.point.x - player.GetComponent<Transform>().position.x,0,hit.point.z- player.GetComponent<Transform>().position.z);//대쉬 방향은 현재 마우스 포인터의 방향
            Vector3 dash_dst = player.GetComponent<Transform>().position + dash_dir.normalized*x;//현재 위치에서 마우스 포인터로 거리 x만큼 떨어진 위치로 이동
            destination.Clear();//목적지를 비워줌
            destination.Add(dash_dst);
            ani.CrossFade("Dash",0.3f);//"Dash"모션을 스테이트 머신이 아닌 크로스페이드로 지정해주고, 스테이트 머신으로 애니메이션 종료 후 IDle 혹은 Move로 이동하도록 구현

            ani.SetBool("IsAttack",false);
            ani.SetBool("IsMove",false);
            
            speed = PlayerSpeed*3;
            while(true)
            {
                if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<=0.4)
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

    private void move(float speed)
    {
        if(destination.Count > 0)
        {
            dir = new Vector3(destination[0].x - player.GetComponent<Transform>().position.x,0,destination[0].z - player.GetComponent<Transform>().position.z);//플레이어가 이동하는 방향을 설정
            if(isMove)//움직여도 되는가?
            {
                if(!isObstacle)//장애물이 없다면
                {
                    if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<=0.4)//너무 가깝게 찍으면 움직일 필요 없음
                    {
                        isMove = false;
                        ani.SetBool("IsMove",false);
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
                    if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<=0.4)//너무 가깝게 찍으면 움직일 필요 없음
                    {

                        if(destination.Count == 1)
                        {
                            isMove = false;
                            ani.SetBool("IsMove",false);
                            return;
                        }
                        else
                        {
                            if(destination.Count != 1)
                            {
                                destination.RemoveAt(0);
                            }
                        }
                    }
                    else
                    {
                        player.GetComponent<Transform>().position += dir.normalized * Time.deltaTime * speed;//시간동안에 걸쳐서 속도만큼의 빠르기로 이동
                        player.GetComponent<Transform>().forward = dir;
                    }
                }
            }
        }
    }
    #endregion

    public bool get_isObstacle()
    {
        // 미니맵에서 isObstacle 값 가져오기 위한 public 함수_HY
        return isObstacle;
    }
}
//삭제된 코드
/*private void Mouse_Right_Click()
{
    RaycastHit hit;//레이케스트 선언
    bool raycastHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//카메라의 위치에서 마우스 포인터의 위치에서 쏜 레이에 맞는 오브젝트의 위치 찾기
    if (!raycastHit) return; // raycast 실패하면 return
    if(hit.collider.tag == "ground")
    {
        Set_Destination(hit.point);//마우스에서 나간 광선이 도착한 위치를 목적지로 설정
        my_enemy = null;//다시 땅 찍으면 타게팅을 풀어줌
        stat = null;//저장해둔 스텟도 지워줌
        ani.SetBool("IsAttack",false);
    }
    else if(hit.collider.tag == "Enemy")//후에 공격과 자동 타게팅을 추가할 예정
    {
        Lock_On(hit.transform.gameObject);//타게팅용
        Set_Destination(my_enemy.GetComponent<Transform>().position);
    }
    else
        return;
}*/
/*void Camera_follow()
{
    cam.GetComponent<Transform>().position = player.GetComponent<Transform>().position + new Vector3(0,5,-10);//이동중에는 카메라도 따라다님
}
void start_camera_set()
{
    cam = Camera.main;
    player = GameObject.Find("Player");
    cam.GetComponent<Transform>().position = player.GetComponent<Transform>().position + new Vector3(0,5,-15);
    cam.GetComponent<Transform>().rotation = Quaternion.Euler(30,0,0);
}*/