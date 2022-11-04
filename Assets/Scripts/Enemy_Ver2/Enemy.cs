using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected Vector3 _destPos;  //타켓 위치
    [SerializeField] protected Define.State state = Define.State.Moving;  //상태 초기값 
    [SerializeField] protected GameObject lockTarget;  //타켓
    protected Stat _stat;

    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown; 
    public virtual Define.State State
    {
        get { return state; }
        set
        {
            state = value;

            Animator anim = GetComponent<Animator>();

            switch (state)
            {
                case Define.State.Idle:
                    anim.CrossFade("WAIT", 0.2f);
                    break; ;
                case Define.State.Moving:
                    anim.CrossFade("WALK", 0.2f);
                    break;
                case Define.State.Skill:
                    anim.CrossFade("ATTACK", 0.1f);
                    break;
                case Define.State.JumpSkill:
                    anim.CrossFade("JUMPATTACK", 0.1f);
                    break;
                case Define.State.Hit:
                    anim.CrossFade("DAMAGE", 0.2f);
                    break;
                case Define.State.Die:
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
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
            case Define.State.JumpSkill:
                UpdateJumpSkill();
                break;
            case Define.State.Hit:
                UpdateHit();
                break;
            case Define.State.Die:
                UpdateDie();
                break;
        }
    }
    protected virtual void Init()
    {
        _stat = gameObject.GetComponent<Stat>();
        //_stat.Hp = 100;
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
