using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy4 : Enemy
{
    
    [SerializeField] float _scanRange = 12.0f;   //플레이서 인식 사정거리
    [SerializeField] float _attachRange = 3;  //적 근거리공격 사정거리

    public Transform[] points;  //waypoints 배열
    private int nextIdx = 1;     // waypoints 인덱스
    private int theNextIdx = 0;   // 다음 waypoint 확인용 인덱스

    private Vector3 movePos;  // enemy 위치 정보
    private Transform tr;  //enemy 위치

    public Transform bombPos; // 무기가 생성될 발사 위치 지정
    public GameObject bomb; // 무기 오브젝트(프리팹)
    public int idx;
    //public float throwPower = 15.0f; //던지는 힘

    public float cooldownTime = 5.0f;
    private float nextFireTime = 0f;


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
        State = Define.EnemyState.Moving;

        // HPBar
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);


        // WayPoint
        points = GameObject.Find("WayPointGroup").GetComponentsInChildren<Transform>();
        nextIdx = Random.Range(1, points.Length);

        //enemy & player 위치
        tr = GetComponent<Transform>();  

    }

    //Idle 상태
    protected override void UpdateIdle()
    {
        State = Define.EnemyState.Idle;
    }

    //Moving 상태
    protected override void UpdateMoving()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        //GameObject player = Managers.Game.GetPlayer();
        lockTarget = player;
        if (lockTarget == null) return;
        _destPos = lockTarget.transform.position;
        float dist = (_destPos - tr.position).magnitude;
        Vector3 dir = _destPos - transform.position;
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        nma.speed = _stat.MoveSpeed;

        if (dist <= _attachRange)
        {
            nma.SetDestination(tr.position);
            State = Define.EnemyState.Skill;
            return;
        }
        else if (dist <= _scanRange)
        {
            idx = Random.Range(1, 11);
            if (idx > 9)
            {
                shot();
                
                return;
            }
            if (dir.magnitude < 0.1f)
            {
                State = Define.EnemyState.Moving;
            }
            else
            {
                nma.SetDestination(_destPos);  //내가 가야할 타켓 지정
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
        else
        {
            if (nextIdx >= points.Length)
            {
                nextIdx = 1;
            }
            movePos = points[nextIdx].position;  //다음 waypoint 위치
            nma.SetDestination(movePos);

            Quaternion quat = Quaternion.LookRotation(movePos - tr.position);  //가야할 방향벡터를 퀀터니언 타입의 각도로 변환
            tr.rotation = Quaternion.Slerp(tr.rotation, quat, _stat.TurnSpeed * Time.deltaTime);  //점진적 회전(smooth하게 회전)
            tr.Translate(Vector3.forward * Time.deltaTime * _stat.MoveSpeed);  //앞으로 이동
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

    protected override void UpdateJumpSkill()
    {

    }

    //Hit 상태
    protected override void UpdateHit()
    {
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    //Die 상태
    protected override void UpdateDie()
    {
    }

    void OnTriggerEnter(Collider coll)
    {
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        nma.SetDestination(tr.position);

        if (coll.tag == "WAY_POINT")
        {
            theNextIdx = Random.Range(1, points.Length);

            if(nextIdx != theNextIdx)
            {
                nextIdx = theNextIdx;
            }
            else if(nextIdx == theNextIdx)
            {
                nextIdx += 1;

                if (nextIdx >= points.Length)
                {
                    nextIdx = 1;
                }
            }
            StartCoroutine("Idle");
        }
    }
    
    void OnHitEvent()
    {
        if (_stat.Hp <= 0)
        {
            return;
        }

        if (lockTarget != null)
        {
            if (State == Define.EnemyState.Die) return;
            PlayerStat targetStat = lockTarget.GetComponent<PlayerStat>();
            targetStat.OnAttacked(_stat);

            //죽었는지 여부 체크 
            if (targetStat.Hp > 0)
            {
                float dist = (lockTarget.transform.position - tr.position).magnitude;
                if (dist <= _attachRange)
                {
                    State = Define.EnemyState.Skill;
                }
                else
                    State = Define.EnemyState.Moving;
            }
            else
            {
                State = Define.EnemyState.Moving;
            }
        }
        else
        {
            State = Define.EnemyState.Moving;
        }
    }

    IEnumerator Idle()
    {
        State = Define.EnemyState.Idle;

        yield return new WaitForSeconds(2.0f);

        State = Define.EnemyState.Moving;
    }


    public void shot()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + cooldownTime;

            transform.forward = (lockTarget.transform.position - transform.position).normalized;

            GameObject instantBomb = Instantiate(bomb, bombPos.transform.position, bombPos.transform.rotation);

            StartCoroutine(shot_(instantBomb));

        }
    }
    IEnumerator shot_(GameObject gameobject)
    {

        bool hit = false;
        float time = 0;

        Vector3 target_pos = lockTarget.transform.position;
        while (!hit)
        {
            yield return new WaitForEndOfFrame();

            if (lockTarget != null)
                target_pos = lockTarget.transform.position;

            time += Time.deltaTime;

            if (Vector3.Distance(gameObject.transform.position, target_pos) < 0.5f)
            {
                hit = true;
                OnHitEvent();
            }
            else if (time > 5f)
                hit = true;
        }
        Destroy(gameobject);

    }
}
