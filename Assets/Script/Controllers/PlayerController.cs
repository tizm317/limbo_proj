using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Animation �� state pattern ���� ����
    // state pattern
    // state �� enum ���� ���ص�
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

    // state ����
    [SerializeField]
    PlayerState _state = PlayerState.Idle;

    // _state �� animation ���� �ٲٱ� ���ؼ� ������Ƽ�� ���� ��� ����
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
                    // ���� ���� ���¿� ���� ������ �Ѱ��ش�
                    anim.SetFloat("speed", 0);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Moving:
                    // ���� ���� ���¿� ���� ������ �Ѱ��ش�
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

        // ���콺 �̺�Ʈ
        // ���� ��û : inputmanager���� Ű �Է� ������, OnMouseClicked �Լ� ����
        Managers.Input.MouseAction -= OnMouseEvent; // 2�� ȣ�� ����
        Managers.Input.MouseAction += OnMouseEvent;

    }

    void UpdateDie()
    {
        // State : Die
        // �ƹ��͵� ����
    }

    void UpdateMoving()
    {
        // (Ÿ���� ��) ���Ͱ� �� �����Ÿ����� ������ ����
        Debug.Log("나 도는중!");
        if (_lockTarget != null)
        {
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 1.0f)
            {
                State = PlayerState.Skill;
                return;
            }
        }

        // �̵�
        // State : Moving

        Vector3 dir = _destPos - transform.position;

        if (dir.magnitude < 0.1f) //  0.00001f
        {
            // ���� �����ϸ� Idle ���·� ��ȯ

            State = PlayerState.Idle;
        }
        else
        {
            // �̵� �κ�

            // ���̰��̼� �̵� ����
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();

            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0.0f, dir.magnitude);

            //nma.CalculatePath // �����ִ��� ���
            nma.Move(dir.normalized * moveDist); // ���� �ִ� �� ����.. // �� �ε�ε�Ÿ��� ���� why? Move�� ������ ������ �̵���Ű�� ���� �������� ����̶� -> ���� ���� ����
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _stat.TurnSpeed * Time.deltaTime);

            // �� ���� ��(layor : Block) �����ϸ� (�������� Ȯ�� ��) ���߱�
            // �÷��̾� ������ ������ ���� �������� Ȯ�� �� ����
            // ������ Ȯ��
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.blue);
            if (Physics.Raycast(transform.position, dir, 1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false) // ���콺 ������ �߿��� Idle�� �� ���ϰ�(��� �޸���)
                    State = PlayerState.Idle;
                return;
            }
            // �⺻ �̵� ����
            /*
            // �����ؼ� �ε�ε� �Ÿ��� �� �ذ� ��� 
            // 1. Mathf.Clamp Ȱ�� (min, max ����)
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0.0f, dir.magnitude);

            // 2. if������ ���ؼ� dir.magnitude ������ dir.magnitude ����
            //float moveDist = _speed * Time.deltaTime;
            //if (moveDist >= dir.magnitude)
            //    moveDist = dir.magnitude;

            transform.position += dir.normalized * moveDist;

            //transform.LookAt(_destPos);
            // �Ҷ� ����� �� �ذ�
            // slerp 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);
            */
        }


    }

    void OnRunEvent()
    {
        Debug.Log("�ѹ��ѹ�");
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
        // hit �ִϸ��̼� ���� �� ȣ��

        Debug.Log("OnHitEvent");

        //TODO
        if (_stopSkill == true)
        {
            // �ƴ�? -> ����
            State = PlayerState.Idle;
        }
        else
        {
            // ���콺 ������ ��? -> ��� ��ų
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
        // ���� �κ�
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // ������ Ȯ��
        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        // ��ƺ��� ����..
        // ���� ���� ���, ���� Ÿ�����ؼ� ���� �������� �̵� (���� ���¿��� Ŀ�� �̵��ص� Ÿ������ ���������� �̵�)
        // �� ���� ���, �� ������ �̵�
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        State = PlayerState.Moving;
                        _stopSkill = false; //////////////////////////////////////////////////////////////////////////////// �̻���

                        // ���͸� ó������ Ŭ���ϸ�, ���͸� Ÿ������.
                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    // ������� ��
                    if (_lockTarget == null && raycastHit)
                        _destPos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                // ��¦ Ŭ���� ��� - 1ȸ�� ��ų
                _stopSkill = false;
                break;
        }
    }
}

