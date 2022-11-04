using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    // 모든 씬 클래스 부모 역할
    // 누군가는 선봉대 역할을 해야함
    // 그 역할을 누가? 특정 오브젝트가 하면 안되겠지
    // 이걸 위한 씬 이라는 새로운 놈 만들고 씬 관련 모든 초기화 담당하도록 만듦

    // abstract class

    // 씬 타입으로 Define에서 관리
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    { 
        // 씬 시작할 때 eventSystem 확인 과정
        // eventsystem 있어야 UI 가능
        // 프리팹 만들어서 초기화
        // 있는지 체크 후 없으면 만들어줌.
        Object eventSystem = GameObject.FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    // base에서 구현 안 할 거니까 abstaract
    // 씬 날리기 전에 해야하는 작업들 수행하는 함수
    public abstract void Clear();

}
