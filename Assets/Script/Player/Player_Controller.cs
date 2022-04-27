using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    const float PlayerSpeed = 5.0f;
    // Start is called before the first frame update
    [SerializeField]
    InputManager input;
    [SerializeField]
    Stat stat;
    private Camera cam;
    private GameObject player,my_enemy;
    private Vector3 destination;//이동하는 목적지를 저장하는 변수
    private bool isMove;//캐릭터가 이동중인지 확인하는 변수
    private float speed = PlayerSpeed;//플레이어의 이동속도
    private Vector3 dir;//이동방향을 위한 변수
    private bool dash_cool = true;//대쉬 스킬의 쿨타임을 확인하기위한 bool변수
    private Animator ani;
    void Start()
    {
        start_camera_set();//카메라의 위치를 플레이어를 기준으로 y축 5만큼, z축 -10만큼, x각 30도만큼 변경
        ani = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(input.mouse_right_btn)
        {
            Mouse_Right_Click();
        }

        Run();
        if(Input.GetKeyDown(KeyCode.Space))
            Dash();

        if(my_enemy != null && Vector3.Distance(player.GetComponent<Transform>().position,my_enemy.GetComponent<Transform>().position) < 1)
        {
            if(!ani.GetBool("IsAttack"))//공격중에는 더 이상 실행안됨
                StartCoroutine(Attack(5,0.5f));//현재 attack_delay는 0.5 공격속도는 2배로 늘어남 기본 1
        }
        else
            move(speed);
        Camera_follow();
    }
    private void Mouse_Right_Click()
    {
        RaycastHit hit;//레이케스트 선언
        bool raycastHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit);//카메라의 위치에서 마우스 포인터의 위치에서 쏜 레이에 맞는 오브젝트의 위치 찾기
        if (!raycastHit) return; // raycast 실패하면 return
        if(hit.collider.tag == "ground")
        {
            Set_Destination(hit.point);//마우스에서 나간 광선이 도착한 위치를 목적지로 설정
            my_enemy = null;//다시 땅 찍으면 타게팅을 풀어줌
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
        Debug.Log(stat.Hp);
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
    
    private void Run()
    {
        if(isMove)
        {
            if(input.LeftShift)
                speed = 2*PlayerSpeed;
            else
                speed = PlayerSpeed;
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
        destination = dest;
        isMove = true;//움직여도 되는지판별
        ani.SetBool("IsMove",true);
    }

    private void move(float speed)
    {
        dir = new Vector3(destination.x - player.GetComponent<Transform>().position.x,0,destination.z - player.GetComponent<Transform>().position.z);//플레이어가 이동하는 방향을 설정
        if(isMove)//움직여도 되는가?
        {
            if(Vector3.Distance(player.GetComponent<Transform>().position,destination)<=0.7)//너무 가깝게 찍으면 움직일 필요 없음
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
    }
    #endregion
    #region 카메라 관리
    void Camera_follow()
    {
        cam.GetComponent<Transform>().position = player.GetComponent<Transform>().position + new Vector3(0,5,-10);//이동중에는 카메라도 따라다님
    }
    void start_camera_set()
    {
        cam = Camera.main;
        player = GameObject.Find("Player");
        cam.GetComponent<Transform>().position = player.GetComponent<Transform>().position + new Vector3(0,5,-10);
        cam.GetComponent<Transform>().rotation = Quaternion.Euler(30,0,0);
    }
    #endregion
}