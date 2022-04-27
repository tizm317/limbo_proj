using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Animation 을 state pattern 으로 관리
    // state pattern
    // state 를 enum 으로 정해둠
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    PlayerStat _stat;
    Vector3 _destPos;
    GameObject _lockTarget;

    // state 변수
    [SerializeField]
    PlayerState _state = PlayerState.Idle;

    // _state 랑 animation 같이 바꾸기 위해서 프로퍼티로 만들어서 묶어서 관리
    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            // Animation
            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case PlayerState.Die:
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Idle:
                    // 현재 게임 상태에 대한 정보를 넘겨준다
                    anim.SetFloat("speed", 0);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Moving:
                    // 현재 게임 상태에 대한 정보를 넘겨준다
                    anim.SetFloat("speed", _stat.MoveSpeed);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Skill:
                    anim.SetBool("attack", true);
                    break;
            }
        }
    }

    void Start()
    {
        _stat = gameObject.GetComponent<PlayerStat>();

        // 마우스 이벤트
        // 구독 신청 : inputmanager에서 키 입력 있으면, OnMouseClicked 함수 실행
        Managers.Input.MouseAction -= OnMouseEvent; // 2번 호출 방지
        Managers.Input.MouseAction += OnMouseEvent;

    }

    void UpdateDie()
    {
        // State : Die
        // 아무것도 못함
    }

    void UpdateMoving()
    {
        // (타켓팅 된) 몬스터가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 1.0f)
            {
                State = PlayerState.Skill;
                return;
            }
        }

        // 이동
        // State : Moving

        Vector3 dir = _destPos - transform.position;

        if (dir.magnitude < 0.1f) //  0.00001f
        {
            // 거의 도착하면 Idle 상태로 변환

            State = PlayerState.Idle;
        }
        else
        {
            // 이동 부분

            // 네이게이션 이동 버전
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0.0f, dir.magnitude);

            //nma.CalculatePath // 갈수있는지 계산
            nma.Move(dir.normalized * moveDist); // 갈수 있는 곳 무브.. // 또 부들부들거리는 문제 why? Move는 완전히 가까이 이동시키지 않음 간접적인 방법이라 -> 도착 범위 수정
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _stat.TurnSpeed * Time.deltaTime);

            // 못 가는 곳(layor : Block) 도달하면 (레이저로 확인 후) 멈추기
            // 플레이어 앞으로 레이저 쏴서 블록인지 확인 후 리턴
            // 레이저 확인
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.blue);
            if (Physics.Raycast(transform.position, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false) // 마우스 누르는 중에는 Idle로 안 변하게(계속 달리게)
                    State = PlayerState.Idle;
                return;
            }
            // 기본 이동 버전
            /*
            // 도착해서 부들부들 거리는 거 해결 방법 
            // 1. Mathf.Clamp 활용 (min, max 사이)
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0.0f, dir.magnitude);

            // 2. if문으로 비교해서 dir.magnitude 넘으면 dir.magnitude 대입
            //float moveDist = _speed * Time.deltaTime;
            //if (moveDist >= dir.magnitude)
            //    moveDist = dir.magnitude;

            transform.position += dir.normalized * moveDist;

            //transform.LookAt(_destPos);
            // 뚝뚝 끊기는 거 해결
            // slerp 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);
            */
        }


    }

    void OnRunEvent()
    {
        Debug.Log("뚜벅뚜벅");
    }

    void UpdateIdle()
    {
        // State : Idle

    }


    void UpdateSkill()
    {

    }

    void OnHitEvent()
    {
        // hit 애니메이션 끝날 때 호출

        Debug.Log("OnHitEvent");

        //TODO
        if (_stopSkill == true)
        {
            // 아니? -> 멈춰
            State = PlayerState.Idle;
        }
        else
        {
            // 마우스 누르고 있? -> 계속 스킬
            State = PlayerState.Skill;
        }


        State = PlayerState.Idle;
    }


    void Update()
    {

        switch (State)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }

    }

    bool _stopSkill = false;
    void OnMouseEvent(Define.MouseEvent evt)
    {
        switch (State)
        {
            case PlayerState.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp)
                        _stopSkill = true;
                }
                break;
        }
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        // 공통 부분
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 레이저 확인
        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        // 디아블로 형식..
        // 몬스터 찍은 경우, 몬스터 타게팅해서 몬스터 방향으로 이동 (누른 상태에서 커서 이동해도 타겟팅한 몬스터쪽으로 이동)
        // 땅 찍은 경우, 땅 쪽으로 이동
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        State = PlayerState.Moving;
                        _stopSkill = false; //////////////////////////////////////////////////////////////////////////////// 이상해

                        // 몬스터를 처음으로 클릭하면, 몬스터를 타겟팅함.
                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    // 땅찍었을 때
                    if (_lockTarget == null && raycastHit)
                        _destPos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                // 살짝 클릭한 경우 - 1회용 스킬
                _stopSkill = false;
                break;
        }
    }
}

