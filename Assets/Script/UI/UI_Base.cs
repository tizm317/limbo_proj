using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UI_Base : MonoBehaviour
{
    // 모든 UI 의 최상위 베이스 클래스
    // Bind 함수들, Get 함수들

    // 딕셔너리로 저장
    // 각 Type 별로 list로 들고 있음
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        // UI 자동화 기능 : 유니티 툴로 연결지어주는 기능 자동화 
        // 맵핑 기능
        
        // enum 값을 어떻게 넘겨받지?
        // C# 의 reflection 기능 사용
        // reflection : 오브젝트 엑스레이 찍어서 정보 추출
        // Type 을 이용해서 입력받음, typeof() 이용해서 넣음.

        // 하는 일
        // enum 산하에서 해당되는 이름을 찾아서 오브젝트 찾아서 연동
        // 정확히는 컴포넌트를 들고 있는 오브젝트
        // generic 으로 컴포넌트 넣어서 ...

        // 1. C# 기능으로 이름 변환 enum -> string 변환
        string[] names = Enum.GetNames(type); 

        // 2. 개수만큼 리스트 생성
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        
        // 3. 타입별로 리스트를 딕셔너리에 넣기
        _objects.Add(typeof(T), objects);

        // 4. 자동으로 넣기 (이름하고 같은 오브젝트 찾아서)
        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject)) // 게임 오브젝트면
                objects[i] = Util.FindChild(gameObject, names[i], true); // GameObject 전용 버전 FindChild
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true); // 컴포넌트 버전 FindChild

            // 바인드 실패
            if (objects[i] == null)
                Debug.Log($"Failed to Bind :  {names[i]}");
        }


    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        // Get 함수 : 딕셔너리에서 꺼내서 쓰는 함수

        // 1. 저장할 리스트 선언
        UnityEngine.Object[] objects = null;

        // 2. 딕셔너리에서 찾아서 리스트 만듦
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        // 3. T로 casting 해서 반환
        return objects[idx] as T;
    }

    // Get 다른 버전들 : GetObject, GetText, GetButton, GetImage
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }

    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        // 게임 오브젝트 받아서, UI_EventHandler 컴포넌트 추출하고, 뭔가 추가하는 기능
        // 입력 : 게임 오브젝트 , 콜백으로 받을 연동할 함수 action으로 받음 , UI_Event 종류

        // 1. UI_EventHandler 가져오기 (없으면 추가)
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        // 2. type에 따라 연동
        switch(type)
        {
            // Click type이면, 해당 action 이 OnClickHandler 구독신청
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;

                // Drag type이면, 해당 action 이 OnDragHandler 구독신청
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }

}
