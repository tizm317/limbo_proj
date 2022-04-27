using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    Material mat;
    NavMeshAgent nav; //네비게이션메쉬
                      //오브젝트가 목표까지 최단 거리를 계산해 추적하는 역할을 하며 충돌을 회피하는 기능을 제공
    Stat _stat;
    [SerializeField] GameObject _lockTarget;
    [SerializeField] float scanRange = 10.0f;  //player 사정 거리
                                               // [SerializeField] float attackRange = 2.0f;  //공격 사정 거리

    void Awake()  //start 함수보다 먼저 호출되어 게임 상태 초기화
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();

    }
    void init()
    {
        _stat = gameObject.GetComponent<Stat>();  //EnemyState Script 받음
        GameObject player = GameObject.FindGameObjectWithTag("Player");  //Tag를 이용하여 "player" 인식
        if (player == null)
            return;  //player가 없으면 대기
        float distance = (player.transform.position - transform.position).magnitude;  // player와의 거리 체크
        if(distance < scanRange)  // player와의 거리가 사정거리보다 작은 경우
        {
            _lockTarget = player;
            nav.SetDestination(_lockTarget.transform.position);  // 도착할 목표 위치 지정 함수

            //State = Define.State.moving;
            return;
        }
    }

    void UpdateIdle()
    {
        Debug.Log("Enemy UpdateIdle");
    }
    void UpdateMoving()
    {
        Debug.Log("Enemy UpdateMoving");
    }
    void UpdateSkill()
    {
        Debug.Log("Enemy UpdateSkill");
    }

    void OnHitEvent()
    {
        Debug.Log("Enemy OnHitEvent");
    }
    void FreezeVelocity()
    {
        // 물리력이 NavAgent이동을 방해하지 않도록 로직 추가
        rigid.velocity = Vector3.zero;  // 속도
        rigid.angularVelocity = Vector3.zero;  // 회전력
    }
    void FixedUpdate()
    {
        FreezeVelocity();

    }
}
