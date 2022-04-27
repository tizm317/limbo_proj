using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
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
    [SerializeField] float _scanRange = 10.0f;  //사정거리
    [SerializeField] float _attachRange = 2.0f;  //적 공격 사정거리

    public EnemyState state
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();

            switch (_state)
            {
                case EnemyState.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break; ;
                case EnemyState.Moving:
                    anim.CrossFade("WALK", 0.1f);
                    break;
                case EnemyState.Skill:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0);
                    break;
                case EnemyState.Die:
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

        // HPBar 여부 체크 후 생성 코드 필요
    }


    void UpdateIdle()
    {
        Debug.Log("UpdateIdle");

        //Tag를 이용하여 Player 찾기
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)  //Player가 없으면 대기
            return;

        //player와 enemy의 거리 계산
        float distance = (Player.transform.position - this.transform.position).magnitude;
        if (distance <= _scanRange)
        {
            _lockTarget = Player;
            _state = EnemyState.Moving;

            return;
        }



    }
    void UpdateMoving()
    {
        Debug.Log("UpdateMoving");

        //플레이어가 사정 거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - this.transform.position).magnitude;
            if (distance <= _attachRange)
            {
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(_destPos);  //타켓 지정
                _state = EnemyState.Skill;
                return;

            }
        }

        //이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude > 0.1f)
        {
            _state = EnemyState.Idle;

        }
        else
        {
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);  //타켓 지정
            nma.speed = _stat.MoveSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

        }


    }

    void UpdateSkill()
    {
        Debug.Log("UpdateSkill");

        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);

        }
    }
    void UpdateDie()
    {
        Debug.Log("UpdateDie");
        //미정
    }
    void OnHitEvent()
    {
        Debug.Log("OnHitEvent");
        if (_lockTarget != null)
        {
            //체력
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            Stat myStat = gameObject.GetComponent<Stat>();
            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attachRange)
                {
                    _state = EnemyState.Skill;

                }
                else
                {
                    _state = EnemyState.Moving;

                }
            }
            else
            {
                _state = EnemyState.Idle;
            }
        }
        else
        {
            _state = EnemyState.Idle;
        }


    }
    void Update()
    {
        switch (_state)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break; ;
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
