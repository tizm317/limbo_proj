using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBoss : Enemy
{
    [SerializeField] float _scanRange = 20;   //사정거리
    [SerializeField] float _attachRange = 6;  //적 공격 사정거리
    [SerializeField] int skillIndex;

    public GameObject _effect; //이펙트 변수 생성
    public Transform _effectTransfrom;
    public float destroyTime = 1.5f; //효과 제거될 시간 변수
    public float currentTime = 0; //경과 시간 측정 변수


    // Start is called before the first frame update
    private void OnEnable()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Monster;

        // 스탯은 상속받아서 사용 : _stat
        _stat.Hp = 500.0f;
        _stat.MaxHp = 500.0f;
        _stat.Attack = 20.0f;
        // 디폴트 애니메이션 
        State = Define.EnemyState.Idle;
        
        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    //Idle 상태
    protected override void UpdateIdle()
    {
        State = Define.EnemyState.Idle;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        if(distance < _scanRange)
        {
            lockTarget = player;
            State = Define.EnemyState.Moving;
            return;
        }
        
    }

    //Moving 상태
    protected override void UpdateMoving()
    {
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        skillIndex = Random.Range(1, 11);
        //플레이어가 사정 거리보다 가까우면 공격
        if (lockTarget != null)
        {
            _destPos = lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attachRange)
            {
                nma.SetDestination(transform.position); //움직이지 않고 본인 위치에서 어택하도록 
                if (skillIndex <= 9)
                {
                    State = Define.EnemyState.Skill;
                    return;

                }
                else
                {
                    State = Define.EnemyState.JumpSkill;
                    return;

                }

            }
        }

        //이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.EnemyState.Idle;
        }
        else
        {
            _destPos = lockTarget.transform.position;
            _stat.MoveSpeed = nma.speed = 1.5f;
            //NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
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

    protected override void UpdateJumpSkill()
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
    }

    //Die 상태
    protected override void UpdateDie()
    {
    }
    void OnHitEvent()
    {
        //체력
        if (lockTarget != null)
        {
            if (State == Define.EnemyState.Die) return;
            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            targetStat.OnAttacked(_stat);

            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float distance = (lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _attachRange)
                {
                    skillIndex = Random.Range(1, 11);
                    if(skillIndex >= 5)
                    {
                        State = Define.EnemyState.Skill;
                        
                    }
                    else
                    {
                        State = Define.EnemyState.JumpSkill;

                    }
                }
                else
                    State = Define.EnemyState.Moving;
            }
            else
            {
                State = Define.EnemyState.Idle;
            }
        }
        else
        {
            State = Define.EnemyState.Idle;
        }
    }

    IEnumerator Effect()
    {
        GameObject eff = Instantiate(_effect);
        eff.transform.position = _effectTransfrom.position;
        yield return new WaitForSeconds(1.0f);

    }
}
