using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Skeleton2 : Enemy
{
    Stat _stat;

    [SerializeField] float _scanRange = 4;   //사정거리
    [SerializeField] float _attachRange = 2;  //적 공격 사정거리

    private Transform[] points;  //waypoints 배열
    private int nextIdx = 1;     // waypoints 인덱스
    private int theNextIdx = 0;   // 다음 waypoint 확인용 인덱스

    private Vector3 movePos;  // enemy 위치 정보
    private Transform tr;  //enemy 위치
    private Transform playerTr; //player 위치

    public override void Init()
    {
        // 스탯
        _stat = gameObject.GetComponent<Stat>();
            
        // 디폴트 애니메이션 
        State = Define.State.Patroll;

        // HPBar
        /*
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
        */

        // WayPoint
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>();
        nextIdx = Random.Range(1, points.Length);

        tr = GetComponent<Transform>();  //enemy 위치
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    }

    protected override void UpdateIdle()
    {
        State = Define.State.Idle;
    }

    protected override void Updatepatroll()
    {
        if (nextIdx >= points.Length)
        {
            nextIdx = 1;
        }
        movePos = points[nextIdx].position;  //다음 waypoint 위치
        Debug.Log(nextIdx);
        Quaternion quat = Quaternion.LookRotation(movePos - tr.position);  //가야할 방향벡터를 퀀터니언 타입의 각도로 변환
        tr.rotation = Quaternion.Slerp(tr.rotation, quat, _stat.TurnSpeed * Time.deltaTime);  //점진적 회전(smooth하게 회전)
        tr.Translate(Vector3.forward * Time.deltaTime * _stat.MoveSpeed);  //앞으로 이동

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)  //Player가 없으면 대기
            return;
        float distance = (playerTr.position - tr.position).magnitude;
        if (distance <= _scanRange)
        {
            Debug.Log("스캔랜지안에 in");
            lockTarget = player;
            State = Define.State.Moving;
        }
    }
    protected override void UpdateMoving()
    {
        Debug.Log("move");
    }

    protected override void UpdateSkill()
    {
        Debug.Log("때리기");
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    protected override void UpdateHit()
    {
        Debug.Log("맞음");
        State = Define.State.Hit;
    }

    protected override void UpdateDie()
    {
        Debug.Log("죽기");
        State = Define.State.Die;

    }

    void OnTriggerEnter(Collider coll)
    {      
        if (coll.tag == "WAY_POINT")
        {
            theNextIdx = Random.Range(1, points.Length);

            if(nextIdx != theNextIdx)
            {
                nextIdx = theNextIdx;
            }
            else if(nextIdx == theNextIdx)
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
    IEnumerator Idle()
    {
        State = Define.State.Idle;

        yield return new WaitForSeconds(2.0f);

        State = Define.State.Patroll;
    }

    void OnHitEvent()
    {
        //체력
        if (lockTarget != null)
        {
            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            float damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
            targetStat.Hp -= damage; //플레이어 데미지

            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float distance = (lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attachRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Patroll;
            }
        }
        else
        {
            State = Define.State.Patroll;
        }
    }
}

