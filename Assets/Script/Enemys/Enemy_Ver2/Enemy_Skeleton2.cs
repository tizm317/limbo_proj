using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Skeleton2 : Enemy
{
    Stat _stat;

    [SerializeField] float _scanRange = 3;   //사정거리
    [SerializeField] float _attachRange = 2;  //적 공격 사정거리

    public Transform[] points;  //waypoints 배열
    public int nextIdx = 1;     // waypoints 인덱스

    private Vector3 movePos;
    private Transform tr;  //enemy 위치

    public override void Init()
    {
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
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>();
        nextIdx = Random.Range(1, points.Length);

        tr = GetComponent<Transform>();  //enemy 위치

    }

    protected override void UpdateIdle()
    {
        State = Define.State.Idle;
    }

    protected override void UpdateMoving()
    {

        movePos = points[nextIdx].position;  // 다음 waypoint 위치로 이동

        Debug.Log(nextIdx + " 움직이기");
       
        Quaternion rot = Quaternion.LookRotation(movePos - tr.position);  //가야할 방향벡터를 퀀터니언 타입의 각도로 변환
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, _stat.TurnSpeed * Time.deltaTime);  //점진적 회전(smooth하게 회전)
        tr.Translate(Vector3.forward * Time.deltaTime * _stat.MoveSpeed);  //앞으로 이동

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
        Debug.Log("맞기");
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
            if(++nextIdx == nextIdx && nextIdx >= points.Length)
            {
                nextIdx = 1;
            }
            else
            {
                nextIdx = Random.Range(1, points.Length);
            }

            //nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
            //nextIdx = (++nextIdx >= points.Length) ? 1 : Random.Range(1, points.Length);
            StartCoroutine("Idle");
        }
    }
    IEnumerator Idle()
    {
        State = Define.State.Idle;

        yield return new WaitForSeconds(2.0f);

        State = Define.State.Moving;

    }

    void OnHitEvent()
    {
        //체력
        if (lockTarget != null)
        {
            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            int damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
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
                State = Define.State.Idle;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}

