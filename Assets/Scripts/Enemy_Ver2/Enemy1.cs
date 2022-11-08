using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : Enemy
{

    [SerializeField] float _scanRange = 15;   //사정거리
    [SerializeField] float _attachRange = 2;  //적 공격 사정거리

    public Transform[] points;  //waypoints 배열
    private int nextIdx = 1;     // waypoints 인덱스
    private int theNextIdx = 0;   // 다음 waypoint 확인용 인덱스

    private Vector3 movePos;  // enemy 위치 정보
    private Transform tr;  //enemy 위치
    //private Transform playerTr; //player 위치

    //private void Start()
    //{
    //    Init();
    //}

    private void OnEnable()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Monster;

        // 스탯은 상속받아서 사용 : _stat
            
        // 디폴트 애니메이션 
        State = Define.EnemyState.Moving;

        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);


        // WayPoint
        points = GameObject.Find("WayPointGroup1").GetComponentsInChildren<Transform>();
        nextIdx = Random.Range(1, points.Length);

        //enemy & player 위치
        tr = GetComponent<Transform>();  
        //playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();


    }

    //Idle 상태
    protected override void UpdateIdle()
    {
        State = Define.EnemyState.Idle;
    }

    //Moving 상태
    protected override void UpdateMoving()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        //GameObject player = Managers.Game.GetPlayer();
        lockTarget = player;
        if (lockTarget == null) return;
        _destPos = lockTarget.transform.position;
        float dist = (_destPos - tr.position).magnitude;
        Vector3 dir = _destPos - transform.position;
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        nma.speed = _stat.MoveSpeed;

        if (dist <= _attachRange)
        {
            nma.SetDestination(tr.position);

            State = Define.EnemyState.Skill;
            return;
        }
        else if (dist <= _scanRange)
        {
            if (dir.magnitude < 0.1f)
            {
                State = Define.EnemyState.Moving;
            }
            else
            {
                nma.SetDestination(_destPos);  //내가 가야할 타켓 지정
                //nma.speed = _stat.MoveSpeed;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
        else
        {
            if (nextIdx >= points.Length)
            {
                nextIdx = 1;
            }
            movePos = points[nextIdx].position;  //다음 waypoint 위치
//            Debug.Log(nextIdx);
            nma.SetDestination(movePos);
            //nma.speed = _stat.MoveSpeed;

            Quaternion quat = Quaternion.LookRotation(movePos - tr.position);  //가야할 방향벡터를 퀀터니언 타입의 각도로 변환
            tr.rotation = Quaternion.Slerp(tr.rotation, quat, _stat.TurnSpeed * Time.deltaTime);  //점진적 회전(smooth하게 회전)
            tr.Translate(Vector3.forward * Time.deltaTime * _stat.MoveSpeed);  //앞으로 이동
        }
        
    }

    //skill 상태
    protected override void UpdateSkill()
    {
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - tr.position;
            Quaternion quat = Quaternion.LookRotation(dir); //바라보고 싶은 방향
            //tr.rotation = Quaternion.Lerp(tr.rotation, quat, 20 * Time.deltaTime);
            tr.rotation = Quaternion.Slerp(tr.rotation, quat, _stat.TurnSpeed * Time.deltaTime);  //점진적 회전(smooth하게 회전)
        }
    }

    //Hit 상태
    protected override void UpdateHit()
    {
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    //Die 상태
    protected override void UpdateDie()
    {
    }

    void OnTriggerEnter(Collider coll)
    {
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        nma.SetDestination(transform.position);

        if (coll.tag == "WAY_POINT1")
        {
            theNextIdx = Random.Range(1, points.Length);

            if(nextIdx != theNextIdx)
            {
                nextIdx = theNextIdx;
            }
            else if(nextIdx == theNextIdx)
            {

                if (nextIdx >= points.Length)
                {
                    nextIdx = 1;
                }
                nextIdx += 1;
                return;
            }
            StartCoroutine("Idle");
        }
    }
    
    void OnHitEvent()
    {
        if (_stat.Hp <= 0)
        {
            return;
        }

        if (lockTarget != null)
        {
            if (State == Define.EnemyState.Die) return;
            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            targetStat.OnAttacked(_stat);

            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float dist = (lockTarget.transform.position - tr.position).magnitude;
                if (dist <= _attachRange)
                {
                    State = Define.EnemyState.Skill;
                }
                else
                    State = Define.EnemyState.Moving;
            }
            else
                State = Define.EnemyState.Die;
        }
        else
            State = Define.EnemyState.Moving;
    }
    IEnumerator Attack()
    {

        State = Define.EnemyState.Idle;

        yield return new WaitForSeconds(1.0f);

        State = Define.EnemyState.Skill;
    }
    IEnumerator Idle()
    {
        State = Define.EnemyState.Idle;

        yield return new WaitForSeconds(2.0f);

        State = Define.EnemyState.Moving;
    }
}
