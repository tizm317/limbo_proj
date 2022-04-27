using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    // 유니티에서 제공되는 클래스에 함수 추가하는 기능 : ExtensionMethod
    // static class
    // Extension 해줄 함수 추가

    // ex) GetImage((int)Images.gameObject.AddUIEvent(); // 이런 식으로 사용자 정의 함수인 AddUIEvent를 사용하도록 만드는 방법


    // 함수 복사해와서 this 만 추가하면 됨. -> go.BindEvent(); 가능
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        // 기능은 추가해야 함.
        UI_Base.BindEvent(go, action, type);
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }
}
