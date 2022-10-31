using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : Enemy
{
    //enemyController1은 대기하다가 추적 사정거리 안에 player가 들어오면 무빙하여 attack
    [SerializeField] float _scanRange = 10;  //사정거리
    [SerializeField] float _attachRange = 2;  //적 공격 사정거리

    GameObject player;

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
        State = Define.State.Idle;

        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        player = GameObject.FindGameObjectWithTag("Player");
        lockTarget = player;

    }

    protected override void UpdateIdle()
    {

        //사정 거리 내에서 Tag를 이용하여 플레이어 찾기
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)  //Player가 없으면 대기
            return;

        //player와 enemy의 거리 계산
        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _scanRange)
        {
            lockTarget = player;
            State = Define.State.Moving;
            return;
        }

    }

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
            _destPos = lockTarget.transform.position;
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);  //내가 가야할 타켓 지정
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

    }
    protected override void UpdateSkill()
    {
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    protected override void UpdateHit()
    {
        State = Define.State.Hit;

    }

    protected override void UpdateDie()
    {
        State = Define.State.Die;

    }

    void OnHitEvent()
    {
        //체력
        if (lockTarget != null)
        {
            if (State == Define.State.Die) return;
            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            targetStat.OnAttacked(_stat);

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

