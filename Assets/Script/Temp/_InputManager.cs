using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class _InputManager
{
    // 입력 관리 매니저

    // 액션 (delegate) 
    // 키 입력
    public Action KeyAction = null;

    // 마우스 입력
    public Action<Define.MouseEvent> MouseAction = null;
    bool _pressed = false;
    float _pressedTime = 0;


    // input manager 대표로 입력 체크해서 (유일하게)
    // 실제 입력있으면 , event로 전파함
    // listener pattern
    // 어디서 실행되는지 디버깅으로 찾기 쉬워짐
    public void OnUpdate() // monobehavior 아니여서 누군가 실행시켜줘야 하므로 이름 바꿈
    {
        // UI 클릭이면 리턴시켜서 UI 클릭시 밑에꺼 무시하도록 만듦 (플레이어 이동 등)
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // 1. 어떤 키 입력 있고, KeyAction이 null 이 아니면,
        // KeyAction event를 구독한 곳으로 전파
        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        // 마우스 입력 있으면,
        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    // 그 전에 누른 적 없다
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;

                // 드래그 추가 가능
                // 시간 재서 몇초 이상일 때 드래그 상태...
            }
            else
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    else
                        MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        // KeyAction, MouseAction 도 씬마다 다를 수 있으므르 날리는 함수
        KeyAction = null;
        MouseAction = null;
    }
}
