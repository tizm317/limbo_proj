using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected Vector3 _destPos;  //타켓 위치
    [SerializeField] protected Define.EnemyState state = Define.EnemyState.Moving;  //상태 초기값 
    [SerializeField] protected GameObject lockTarget;  //타켓
    protected Stat _stat;
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown; 
    public virtual Define.EnemyState State
    {
        get { return state; }
        set
        {
            state = value;

            Animator anim = GetComponent<Animator>();

            switch (state)
            {
                case Define.EnemyState.Idle:
                    anim.CrossFade("WAIT", 0.2f);
                    break; ;
                case Define.EnemyState.Moving:
                    anim.CrossFade("WALK", 0.2f);
                    break;
                case Define.EnemyState.Skill:
                    anim.CrossFade("ATTACK", 0.2f, -1, 0f);
                    break;
                case Define.EnemyState.JumpSkill:
                    anim.CrossFade("JUMPATTACK", 0.2f, -1, 0f);
                    break;
                case Define.EnemyState.Hit:
                    anim.CrossFade("DAMAGE", 0.2f);
                    break;
                case Define.EnemyState.Die:
                    anim.CrossFade("DIE", 0.2f);
                    break;
            }
        }
    }

    void Start()
    {
        Init();

    }



    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case Define.EnemyState.Idle:
                UpdateIdle();
                break;
            case Define.EnemyState.Moving:
                UpdateMoving();
                break;
            case Define.EnemyState.Skill:
                UpdateSkill();
                break;
            case Define.EnemyState.JumpSkill:
                UpdateJumpSkill();
                break;
            case Define.EnemyState.Hit:
                UpdateHit();
                break;
            case Define.EnemyState.Die:
                UpdateDie();
                break;
        }
    }
    protected virtual void Init()
    {
        _stat = gameObject.GetComponent<Stat>();
        if(_stat.Hp <= 0)
        {
            _stat.Hp = _stat.MaxHp;
        }
    }

    protected virtual void UpdateIdle()
    {
    }
    protected virtual void UpdateMoving()
    {
    }
    protected virtual void UpdateSkill()
    {
    }
    protected virtual void UpdateJumpSkill()
    {
    }
    protected virtual void UpdateHit()
    {
    }
    protected virtual void UpdateDie()
    {
    }

    public void set_target(GameObject target)
    {
        lockTarget = target;
    }
}
