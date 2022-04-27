using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    //static Managers Instance; // 유일성 보장
    static Managers s_instance; // 프로퍼티로 개선
    //public static Managers GetInstance() { Init();  return s_instance; } // 유일한 매니저 가져옴 / 외부에서 사용할 때는 이거
    static Managers Instance { get { Init(); return s_instance; } } // 프로퍼티 형식으로 개선

    _InputManager _input = new _InputManager();
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();

    public static _InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        // 초기화
        //Instance = this; // 누군가(본인) 채워줘 -> 문제 해결 x // Managers 여러개 일때 각 객체마다 start 실행해서 덮어쓰는 문제

        // 다른 방법
        // 무조건 원본

        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers"); // 아무리 각각의 객체가 호출한다고 하더라도 전역에 저장되는건 Managers 원본 하나
            if (go == null)
            {
                // 코드상으로 새로 만들어줌
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
        }


        // 아직 문제 남음
        // 만약 Managers 가 없어서 못 찾으면 NUll...
        // Instance 가 null 인지 확인해야함
    }
}
