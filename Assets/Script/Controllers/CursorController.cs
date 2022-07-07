using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster) | (1 << (int)Define.Layer.NPC);

    Texture2D _attackIcon;
    Texture2D _handIcon;
    Texture2D _removeIcon;
    Texture2D _basicIcon;


    enum CursorType
    {
        None,
        Attack,
        Hand,
        Remove,
        Basic,
    }

    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Hand");
        _removeIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Remove");
        _basicIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Basic");
    }

    void Update()
    {
        // 마우스 커서 관리

        // (우클) 마우스 누른 상태에서는 커서 변화 x
        if (Input.GetMouseButton(1))
            return;

        // 인벤토리에서 버릴 때 remove커서 표시
        if (tryRemoving)
        {
            if (_cursorType != CursorType.Remove)
            {
                Cursor.SetCursor(_removeIcon, new Vector2(_removeIcon.width / 2, _removeIcon.height / 2), CursorMode.Auto);
                _cursorType = CursorType.Remove;
            }
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 레이저 확인
        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack) // 매 프레임마다 바뀌는 거 방지
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto); // 2번째 인자 : 커서 이미지 끝점 왼쪽 기준에서 얼마만큼 이동할지
                    _cursorType = CursorType.Attack;
                }
            }
            else if(hit.collider.gameObject.layer == (int)Define.Layer.NPC)
            {
                if (_cursorType != CursorType.Hand) 
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
            else
            {
                if (_cursorType != CursorType.Basic)
                {
                    Cursor.SetCursor(_basicIcon, new Vector2(_basicIcon.width / 3, 0), CursorMode.Auto); // 3번째 인자 : Auto(하드웨어 최적화) / ForceSoftware(SW적으로 그리는작업)
                    _cursorType = CursorType.Basic;

                }
            }

        }
    }

#region 버리기커서
    // 지우려고 드래그하는 중인지 판단하기 위한 bool값
    bool tryRemoving = false;
    public void SetTryRemoving(bool input)
    {
        // UI_Inven_Item에서 체크하기 위한 boolean값 바꾸기 위한 함수
        tryRemoving = input;
    }
#endregion
}
