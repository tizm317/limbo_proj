using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    Stat _state;
    Vector3 _destPos;
    
    bool _moveToDest = false;

    void Start()
    {
        _state = gameObject.GetComponent<Stat>();    
    }
        
    public enum Stat  //Enemy variable
    {
        Idle,
        Moving,
        Skill,
        Die,
    }

    float wait_walk_ratio = 0.0f;


    void UpdateIdle()
    {
        //Animation
        wait_walk_ratio = Mathf.Lerp(wait_walk_ratio, 0, 10.0f * Time.deltaTime);
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("wait_walk_ratio", wait_walk_ratio);
        anim.Play("WAIT_WALK");
    }

    void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            _state = Stat.Idle;

        }
        else
        {
            //float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            //transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

        }

        //Animation
        wait_walk_ratio = Mathf.Lerp(wait_walk_ratio, 1, 10.0f * Time.deltaTime);
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("wait_walk_ratio", wait_walk_ratio);
        anim.Play("WAIT_WALK");
    }
    void UpdateDie()
    {
        //미정
    }
    void Update()
    {
        switch(_state)
        {
            case Stat.Idle:
                UpdateIdle();
                break;;
            case Stat.Moving:
                UpdateMoving();
                break;
            case Stat.Skill:
                break;
            case Stat.Die:
                break;
        }
    }
}
    