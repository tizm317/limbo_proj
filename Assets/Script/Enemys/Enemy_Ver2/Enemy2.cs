using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : Enemy
{
    //boss
    Stat _stat;
    
    [SerializeField] float _scanRange = 50;   //사정거리
    [SerializeField] float _attachRange = 40;  //적 공격 사정거리
    
    public Transform[] points;  //waypoints 배열
    private int nextIdx = 1;     // waypoints 인덱스
    private int theNextIdx = 0;   // 다음 waypoint 확인용 인덱스

    private Vector3 movePos;  // enemy 위치 정보
    private Transform tr;  //enemy 위치
    private Transform playerTr; //player 위치

    Material mat;
    private int probability = 0;
    

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        // 스탯
        _stat = gameObject.GetComponent<Stat>();

        // 디폴트 애니메이션 
        State = Define.State.Idle;
        _stat.Hp = 300;

        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        //enemy & player 위치
        tr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    }

    //Idle 상태
    protected override void UpdateIdle()
    {
        //사정 거리 내에서 Tag를 이용하여 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)  //Player가 없으면 대기
            return;

        //player와 enemy의 거리 계산
        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _scanRange)
        {
            lockTarget = player;
            State = Define.State.Moving;
        }
    }

    //Moving 상태
    protected override void UpdateMoving()
    {
        //플레이어가 사정 거리보다 가까우면 공격
        if (lockTarget != null)
        {
            _destPos = lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attachRange)
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                State = Define.State.Skill;
                return;
            }
        }
        //이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);  //내가 가야할 타켓 지정
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
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
    }

    //Die 상태
    protected override void UpdateDie()
    {
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
                {
                    State = Define.State.Skill;
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
        else
        {
            State = Define.State.Moving;
        }
    }

}
