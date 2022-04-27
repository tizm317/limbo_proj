using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    Texture2D _attackIcon;
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;

    void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Attack");
        _handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursors/Hand");
    }

    void Update()
    {
        // 마우스 커서 관리

        // 마우스 누른 상태에서는 커서 변화 x
        if (Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 레이저 확인
        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack) // 매 프레임마다 바뀌는 거 방지
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto); // 2번째 인자 : 커서 이미지 끝점 왼쪽 기준에서 얼마만큼 이동할지
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto); // 3번째 인자 : Auto(하드웨어 최적화) / ForceSoftware(SW적으로 그리는작업)
            }
        }
    }
}
