using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    // UI Manager : 팝업 관리

    // Canvas의 sort order 관리하기 위함
    // popup on, off 요청
    // sort order 저장했다가 연동하는 역할

    // 최근 사용 sort order
    // 시작을 0으로 하면, sorting 하지 않는 order 하고 같아지니까
    // 10부터 시작하도록 수정(어차피 UI_Popup 끼리의 order니까 상관x)
    // 0~9 는 더 먼저 뜨고 싶은거 예약할 수 있음
    int _order = 10;

    // 팝업 목록을 들고 있음.
    // stack 구조
    // 가장 마지막에 띄운 팝업을 꺼야하니까
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); // UI_Popup 컴포넌트 들고 있는 것들

    // 씬 UI 들고 있는 변수
    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        // 하이러라키 창 정리하기 위한 Root

        get
        {
            GameObject root = GameObject.Find("@UI_Root"); // 아무리 각각의 객체가 호출한다고 하더라도 전역에 저장되는건 root 원본 하나
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        // 외부에서 popup ui 켜질 때 , 역으로 UI Manager 한테 setcanvas 요청해서 canvas 내 order 변경
        // UI_Scene, UI_Popup 시작할 때 호출

        // Canvas
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // 중첩 캔버스가 있을 때 , 부모가 어떤 값을 가지던 자신의 sorting order를 가지는 옵션

        // sorting 요청 확인
        if (sort)
        {
            // sorting 요청 O => _order 넣어주고 증가시킴.
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            // sorting 요청 X => 팝업이랑 관계 없는 일반 UI
            canvas.sortingOrder = 0;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        // 서브아이템 만들기

        // name 옵션
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        // parent 연결 옵션
        if (parent != null)
            go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        // 씬 UI 용 인터페이스

        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }


    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        // T : 스크립트
        // name : 프리팹 이름 
        // 이름 최대한 맞춰서 name 옵션으로 사용.

        // 프리팹 이름 안 넣어주면 스크립트 이름 넣어줌.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        // 팝업 UI 프리팹 instantiate
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

        // 실제 기능 담당하는 스크립트 빼오기 
        T popup = Util.GetOrAddComponent<T>(go);

        // 팝업 스택에 푸시
        _popupStack.Push(popup);

        // 하이어라키 창에서 정리하기 위해 parent 를 root 로 설정
        go.transform.SetParent(Root.transform);

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        // 더 안전한 버전
        // 내가 입력으로 넣은 것이 실제로 삭제되는 것이 맞는지 확인 후 삭제

        if (_popupStack.Count == 0)
            return;

        // 확인 과정
        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        // 삭제 과정
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        // 닫기 : 스택에서 하나씩 뺌

        // 스택 확인
        if (_popupStack.Count == 0)
            return;

        // 스택 pop
        UI_Popup popup = _popupStack.Pop();

        // 프리팹 지우기
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;

        // sort order 관리
        _order--;
    }
    public void CloseAllPopupUI()
    {
        // 모든 팝업 지우기
        // 스택 통채로 지움

        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        // 팝업스택 날리기
        CloseAllPopupUI();
        // UI_Scene 날리기
        _sceneUI = null;
    }
}
