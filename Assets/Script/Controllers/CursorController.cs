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
        // ���콺 Ŀ�� ����

        // (��Ŭ) ���콺 ���� ���¿����� Ŀ�� ��ȭ x
        if (Input.GetMouseButton(1))
            return;

        // �κ��丮���� ���� �� removeĿ�� ǥ��
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

        // ������ Ȯ��
        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack) // �� �����Ӹ��� �ٲ�� �� ����
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto); // 2��° ���� : Ŀ�� �̹��� ���� ���� ���ؿ��� �󸶸�ŭ �̵�����
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
                    Cursor.SetCursor(_basicIcon, new Vector2(_basicIcon.width / 3, 0), CursorMode.Auto); // 3��° ���� : Auto(�ϵ���� ����ȭ) / ForceSoftware(SW������ �׸����۾�)
                    _cursorType = CursorType.Basic;

                }
            }

        }
    }

#region ������Ŀ��
    // ������� �巡���ϴ� ������ �Ǵ��ϱ� ���� bool��
    bool tryRemoving = false;
    public void SetTryRemoving(bool input)
    {
        // UI_Inven_Item���� üũ�ϱ� ���� boolean�� �ٲٱ� ���� �Լ�
        tryRemoving = input;
    }
#endregion
}
