using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : MonoBehaviour
{
    //enemyController2는 waypoint를 순차대로 무빙하다가 player가 추적 사정거리 안으로 들어올 때 추적하고 멀어지면 다시 waypoint 무빙
    public Transform[] points;  //waypoints 배열 세팅
    public int nextIdx = 1;  //다음 waypoint를 가리킬 인덱스 & index가 0이 아닌 1인 이유는 부모 WayPointGroup 포함하여 get하였기에 부모 제외 하위 point만 움직이게 하기 위해

    public float speed = 3.0f;
    public float damping = 5.0f; //rotation 속도

    private Transform tr;  //enemy 위치
    private Transform playerTr;

    private Vector3 movePos;
    private bool isAttack = false; //공격 여부를 판단할 변수
    private Animator anim;

    [SerializeField] GameObject _lockTarget;


    void init()
    {
        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        //랜덤하게 point를 이동하도록
        nextIdx = Random.Range(1, points.Length);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();  //enemy 위치
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();  //player Tag를 가진 게임오브젝트 위치
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>();  //waypointgroup 안 (부모포함) 게임오브젝트 위치
        anim = GetComponent<Animator>();
        GameObject _lockTarget = GameObject.FindGameObjectWithTag("Player");
        init();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);  //enemy와 player와의 추적 사정  거리

        if (dist <= 1.0f)
        {
            isAttack = true; //아래 추적 루틴을 건너뛰기 위하여
        }
        else if (dist <= 5.0f) //else if로 해야함! if문으로 하면 공격을 안하고 player 주위만 돔
        {
            movePos = playerTr.position; //player의 위치로 이동
            isAttack = false;
        }
        else
        {
            movePos = points[nextIdx].position;  // 5.0f 이상(거리가 멀리 떨어진)의 경우 다음 waypoint 위치로 이동
            isAttack = false;
        }

        anim.SetBool("isAttack", isAttack);


        if (!isAttack) //공격하지 않을 떄
        {
            Quaternion rot = Quaternion.LookRotation(movePos - tr.position);  //가야할 방향벡터를 퀀터니언 타입의 각도로 변환
            tr.rotation = Quaternion.Slerp(tr.rotation, rot, damping * Time.deltaTime);  //점진적 회전(smooth하게 회전)
            tr.Translate(Vector3.forward * Time.deltaTime * speed);  //앞으로 이동
        }

    }

    //enemy는 way point 순서대로 움직이게 만드는 함수
    //만약 player와의 거리가 5.0f 이내가 된다면 way point가 아닌 palyer를 추적하는 로직
    //player와의 거리가 5.0f 이상의 경우 다시 다음 way point로 이동
    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "WAY_POINT")  //Tag가 WAY_POINT인 인덱스 순으로 이동
        {
            //nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
            nextIdx = (++nextIdx >= points.Length) ? 1 : Random.Range(1, points.Length);

            //nextIdx = Random.Range(1, points.Length);
        }
    }
    void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Debug.Log("ONHITEVENT");
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<Stat>();
            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            if (targetStat.Hp > 0)
            {
                float dist = (_lockTarget.transform.position - transform.position).magnitude;
                if (dist <= 1.0f)
                    isAttack = true;
                else
                    isAttack = false; //어택
            }
        }
    }
}
