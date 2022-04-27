using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    [SerializeField]
    float _turnSpeed = 20.0f;

    Vector3 _destPos;

    // Animation 을 state pattern 으로 관리
    // state pattern
    // state 를 enum 으로 정해둠
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
    }

    // state 변수
    PlayerState _state = PlayerState.Idle;

    void Start()
    {
        // 마우스 이벤트
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        // TEMP
        //Managers.UI.ShowPopupUI<UI_Button>();
        Managers.UI.ShowSceneUI<UI_Inven>();
    }

    void UpdateDie()
    {
        // State : Die
        // 아무것도 못함
    }

    void UpdateMoving()
    {
        // State : Moving

        Vector3 dir = _destPos - transform.position;

        if (dir.magnitude < 0.00001f)
        {
            _state = PlayerState.Idle;
        }
        else
        {
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
        }

        // Animation
        Animator anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨준다
        anim.SetFloat("speed", _speed);
    }

    void OnRunEvent()
    {
        Debug.Log("뚜벅뚜벅");
    }

    void UpdateIdle()
    {
        // State : Idle

        // Animation
        Animator anim = GetComponent<Animator>();

        // 현재 게임 상태에 대한 정보를 넘겨준다
        anim.SetFloat("speed", 0);
    }

    void Update()
    {
        switch (_state)
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
        }

    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destPos = hit.point;
            _state = PlayerState.Moving;
        }
    }
}


