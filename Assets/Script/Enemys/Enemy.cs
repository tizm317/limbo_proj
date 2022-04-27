using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav; //네비게이션메쉬
                      //오브젝트가 목표까지 최단 거리를 계산해 추적하는 역할을 하며 충돌을 회피하는 기능을 제공

    void Awake()  //start 함수보다 먼저 호출되어 게임 상태 초기화
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();

    }


    void Update()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, 10.0f); // 특정 범위 내의 콜라이더 감지하여 거리 체크

        if(col.Length > 0)  //콜라이더가 0보다 큰 경우
        {
            for(int i = 0; i < col.Length; i++)
            {
                Transform target = col[i].transform;  //target = player인지 체크
                if (target.name == "Player")
                {
                    NavMeshPath path = new NavMeshPath();  //좌표값을 담고 있는 클래스
                    nav.CalculatePath(target.position, path);  //경로 환산하여 path에 집어 넣는다

                    Vector3[] wayPoints = new Vector3[path.corners.Length + 2];  //코너의 길이 값은 기억
                    wayPoints[0] = transform.position;  //배열 처음 = 자신의 위치 기억
                    wayPoints[wayPoints.Length - 1] = target.position;  //배열 마지막 = player(target)의 위치 기억

                    float _distance = 0.0f; 

                    for (int j = 0; j < path.corners.Length; j++) 
                    {
                        wayPoints[j + 1] = path.corners[j];  //자신의 위치 다음 번째에 넣는다.
                        _distance += Vector3.Distance(wayPoints[j], wayPoints[j + 1]);  //거리의 차 구한다
                    }

                    if(_distance <= 10.0f)  //10이하이면 추적
                    {
                        nav.SetDestination(target.position);  // 도착할 목표 위치 지정 함수
                    }
                }
            }
        }

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
