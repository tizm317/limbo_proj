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
    [SerializeField] float _scanRange = 10;  //사정거리
    [SerializeField] float _attachRange = 2;  //적 공격 사정거리

    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mat = GetComponent<Material>();
    }
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
                    break;
                case EnemyState.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break; ;
                case EnemyState.Moving:
                    anim.CrossFade("WALK", 0.1f);
                    break;
                case EnemyState.Skill:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0);
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
    void UpdateDie()
    {
        //Debug.Log("UpdateDie");
        //미정
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
        Debug.Log("OnHitEvent");

        if (_lockTarget != null)
        {
            //체력
            PlayerStat targetStat = _lockTarget.GetComponent<PlayerStat>();
            Stat myStat = gameObject.GetComponent<Stat>();
            int damage = Mathf.Max(0, myStat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance >= _attachRange)
                {
                    State = EnemyState.Moving;
                    
                }
                else
                {
                    State = EnemyState.Skill;
                }
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
