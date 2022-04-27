using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class _InputManager
{
    public Action KeyAction = null;

    public Action<Define.MouseEvent> MouseAction = null;
    bool _pressed = false;

    public void OnUpdate()
    {
        // UI 클릭이면 리턴
        // 임시로 주석처리
        //if (EventSystem.current.IsPointerOverGameObject())
        //    return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if (MouseAction != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;

                // 드래그 추가 가능
                // 시간 재서 몇초 이상일 때 드래그 상태...
            }
            else
            {
                if (_pressed)
                    MouseAction.Invoke(Define.MouseEvent.Click);
                _pressed = false;
            }
        }
    }
}
