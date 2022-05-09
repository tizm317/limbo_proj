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
    _InputManager input;
    [SerializeField]
    Stat stat;
    PathFinding pathfinding;
    private PlayerStat my_stat;
    private Camera cam;
    public GameObject player,my_enemy;
    private Vector3 player_pos;
    private List<Vector3> destination = new List<Vector3>();//이동하는 목적지를 저장하는 변수
    private bool isMove, isObstacle;//캐릭터가 이동중인지 확인하는 변수
    private float speed = PlayerSpeed;//플레이어의 이동속도
    private Vector3 dir;//이동방향을 위한 변수
    private bool dash_cool = true;//대쉬 스킬의 쿨타임을 확인하기위한 bool변수
    private Animator ani;
    void Start()
    {
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
        player = GameObject.Find("Player");
        cam = Camera.main;
        player_pos = player.GetComponent<Transform>().position;
        ani = player.GetComponent<Animator>();
        my_stat = player.GetComponent<PlayerStat>();
        pathfinding = GameObject.Find("A*").GetComponent<PathFinding>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Dash();

        if(my_enemy != null && Vector3.Distance(player.GetComponent<Transform>().position,my_enemy.GetComponent<Transform>().position) < 1)
        {
            if(!ani.GetBool("IsAttack"))//공격중에는 더 이상 실행안됨
                StartCoroutine(Attack(my_stat.Attack,AttackDelay));//현재 attack_delay는 1 공격속도는 2배로 늘어남 기본 1
        }
        else
            move(speed);
    }
    
    void OnMouseClicked(Define.MouseEvent evt)
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
    }

    #region 스킬
    IEnumerator SkillCool(string skill, float cool_time)//스킬의 이름을 string으로 입력하고 쿨타임 지정해주면 됨
    {
        while(cool_time > 0)
        {
            cool_time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        if(skill == "Dash")//스킬이 추가될 때마다 추가해주어야함, 반환형이 없기때문에 bool을 입력받아 직접 바꿔줄 수 없음, 반복함수라 ref사용 불가능
            dash_cool = true;
    }

    private void Lock_On(GameObject enemy)
    {
        my_enemy = enemy;
        stat = my_enemy.GetComponent<Stat>();
    }
    IEnumerator Attack(int damage,float attack_delay)
    {//damage는 한번 공격할때 감소하는 hp수, distance는 사거리->아직 미구현
         isMove = false;
        ani.SetBool("IsMove",false);
        ani.SetBool("IsAttack",true);
        ani.SetFloat("AttackSpeed",1/attack_delay);//공격 속도조절,attack_delay가 커질수록 공격속도가 느려짐, 반대로 작아지면 공격속도 빨라짐
        while(my_enemy != null)
        {
            stat.Hp = stat.Hp - damage;
            Debug.Log(stat.Hp);
            if(stat.Hp < 0)
            {
                Destroy(my_enemy);
                ani.SetBool("IsAttack",false);
            }
            yield return new WaitForSeconds(attack_delay);
        }
    }
    
    private void Dash()
    {
        if(dash_cool)
        {
            dash_cool = false;
            RaycastHit hit;//레이케스트 선언
            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);
            Vector3 dash_dir = new Vector3(hit.point.x - player.GetComponent<Transform>().position.x,0,hit.point.z- player.GetComponent<Transform>().position.z);//대쉬 방향은 현재 마우스 포인터의 방향
            player.GetComponent<Rigidbody>().AddForce(dash_dir.normalized * 5.0f,ForceMode.Impulse);
            isMove = false;//대쉬를 사용하면 이동을 멈춤
            StartCoroutine(SkillCool("Dash",1.0f));
        }
        
    }
    #endregion

    #region 이동
    private void Set_Destination(Vector3 dest)
    {
        destination.Clear();//리스트를 비워주고
        Vector3 pos = player.GetComponent<Transform>().position;
        isObstacle = Physics.Raycast(pos,new Vector3(dest.x - pos.x, 0, dest.z - pos.z),Vector3.Distance(pos,new Vector3(dest.x,pos.y,dest.z)),pathfinding.grid.unwalkableMask);
        if(isObstacle)
        {
            Debug.Log("player pos = " + pos);
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

    private void move(float speed)
    {
        if(destination.Count > 0)
        {
            dir = new Vector3(destination[0].x - player.GetComponent<Transform>().position.x,0,destination[0].z - player.GetComponent<Transform>().position.z);//플레이어가 이동하는 방향을 설정
            if(isMove)//움직여도 되는가?
            {
                if(!isObstacle)//장애물이 없다면
                {
                    if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<=0.7)//너무 가깝게 찍으면 움직일 필요 없음
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
                    if(Vector3.Distance(player.GetComponent<Transform>().position,destination[0])<=0.1)//너무 가깝게 찍으면 움직일 필요 없음
                    {
                        Debug.LogFormat("destination coordinate = {0}, count = {1}",destination[0],destination.Count);
                        if(destination.Count == 1)
                        {
                            isMove = false;
                            ani.SetBool("IsMove",false);
                            return;
                        }
                        else
                        {
                            if(destination.Count != 1)
                                destination.RemoveAt(0);
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