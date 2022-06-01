using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController1 : MonoBehaviour
{
    //enemyController1은 대기하다가 추적 사정거리 안에 player가 들어오면 무빙하여 attack
    public enum EnemyState  //Enemy variable
    {
        Idle, 
        Moving,
        Skill,
        Die,
        
    }
    [SerializeField] Vector3 _destPos;  //타켓 위치
    [SerializeField] EnemyState _state = EnemyState.Idle;  //상태 초기값 = wait(idle)
    [SerializeField] GameObject _lockTarget;  //타켓

    Stat _stat;
    [SerializeField] float _scanRange = 10;  //사정거리
    [SerializeField] float _attachRange = 2;  //적 공격 사정거리

    public EnemyState State
    {
        get { return _state; }
        set
        {
            _state = value;
            Animator anim = GetComponent<Animator>();

            switch (_state)
            {
                case EnemyState.Die:
                    anim.CrossFade("DIE", 0.2f);
                    break;
                case EnemyState.Idle:
                    anim.CrossFade("WAIT", 0.2f);
                    break; ;
                case EnemyState.Moving:
                    anim.CrossFade("WALK", 0.2f);
                    break;
                case EnemyState.Skill:
                    anim.CrossFade("ATTACK", 0.2f, -1, 0.0f);
                    break;                   
            }
        }
    }
    void Start()
    {
        init();
    }
    void init()
    {
        _stat = gameObject.GetComponent<Stat>();

        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }
    void UpdateDie()
    {
        //Stat myStat = gameObject.GetComponent<Stat>();

        /*
        if (myStat.Hp <= 0)
        {
            myStat.Hp = 0;
            State = EnemyState.Die;
            Destroy(this, 3);
            GetComponent <EnemyController1> ().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<UI_HPBar>().enabled = false;
        }
        */

    }
    void UpdateIdle()
    {
       
        //사정 거리 내에서 Tag를 이용하여 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)  //Player가 없으면 대기
            return;

        //player와 enemy의 거리 계산
        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _scanRange)
        {
            _lockTarget = player;
            State = EnemyState.Moving;
            return;
        }

    }

    void UpdateMoving()
    {        
        //플레이어가 사정 거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attachRange)
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                State = EnemyState.Skill;
                return;
            }
        }

        //이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = EnemyState.Idle;
        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);  //내가 가야할 타켓 지정
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
        
    }

    void UpdateSkill()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    void OnHitEvent()
    {
        //체력
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<Stat>();
            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attachRange)
                    State = EnemyState.Skill;
                else
                    State = EnemyState.Moving;
            }
            else
            {
                State = EnemyState.Idle;
            }
        }
        else
        {
            State = EnemyState.Idle;
        }
    }
    

    void Update()
    {
        switch (State)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Moving:
                UpdateMoving();
                break;
            case EnemyState.Skill:
                UpdateSkill();
                break;
            case EnemyState.Die:
                UpdateDie();
                break;
        }
    }
}
