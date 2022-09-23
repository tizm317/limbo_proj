using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : Enemy
{
    //wizard
    Stat _stat;

    [SerializeField] float _scanRange = 8;   //사정거리
    [SerializeField] float _attachRange = 3;  //적 공격 사정거리
    
    public Transform[] points;  //waypoints 배열
    private int nextIdx = 1;     // waypoints 인덱스
    private int theNextIdx = 0;   // 다음 waypoint 확인용 인덱스

    private Vector3 movePos;  // enemy 위치 정보
    private Transform tr;  //enemy 위치
    private Transform playerTr; //player 위치

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        // 스탯
        _stat = gameObject.GetComponent<Stat>();

        // 디폴트 애니메이션 
        State = Define.State.Moving;

        // HPBar
        /*
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
        */

        // WayPoint
        points = GameObject.Find("WayPointGroup1").GetComponentsInChildren<Transform>();
        nextIdx = Random.Range(1, points.Length);

        //enemy & player 위치
        tr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();


    }

    //Idle 상태
    protected override void UpdateIdle()
    {
        State = Define.State.Idle;
    }

    //Moving 상태
    protected override void UpdateMoving()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //GameObject player = Managers.Game.GetPlayer();
        lockTarget = player;
        _destPos = lockTarget.transform.position;
        float dist = (_destPos - tr.position).magnitude;
        Vector3 dir = _destPos - transform.position;
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        _stat.MoveSpeed = 1.5f;
        nma.speed = _stat.MoveSpeed;

        if (dist <= _attachRange)
        {
            nma.SetDestination(tr.position);

            State = Define.State.Skill;
            return;
        }
        else if (dist <= _scanRange)
        {
            if (dir.magnitude < 0.1f)
            {
                State = Define.State.Moving;
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
            //Debug.Log(nextIdx);
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
        Debug.Log("맞음");
        State = Define.State.Hit;
    }

    //Die 상태
    protected override void UpdateDie()
    {
        Debug.Log("죽기");
        State = Define.State.Die;

    }

    void OnTriggerEnter(Collider coll)
    {
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        nma.SetDestination(tr.position);

        if (coll.tag == "WAY_POINT1")
        {
            theNextIdx = Random.Range(1, points.Length);

            if (nextIdx != theNextIdx)
            {
                nextIdx = theNextIdx;
            }
            else if (nextIdx == theNextIdx)
            {
                nextIdx += 1;

                if (nextIdx >= points.Length)
                {
                    nextIdx = 1;
                }
            }
            StartCoroutine("Idle");
        }
    }

    void OnHitEvent()
    {
        if (lockTarget != null)
        {

            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            targetStat.OnAttacked(_stat);
            /*
            float damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
            targetStat.Hp -= damage; //플레이어 데미지

            
            if(targetStat.Hp <= 0)
            {
                Managers.Game.Despawn(targetStat.gameObject);
            }
            */
            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float dist = (lockTarget.transform.position - tr.position).magnitude;
                if (dist <= _attachRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Moving;
            }
        }
        else
        {
            State = Define.State.Moving;
        }
    }
    IEnumerator Idle()
    {
        State = Define.State.Idle;

        yield return new WaitForSeconds(2.0f);

        State = Define.State.Moving;
    }
}
