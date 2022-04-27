using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    // 매니저 관리

    //static Managers Instance; // 유일성 보장
    //public static Managers GetInstance() { Init();  return s_instance; } // 유일한 매니저 가져옴 / 외부에서 사용할 때는 이거

    // 프로퍼티로 개선
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    // 각 매니저 연결
    DataManager _data = new DataManager();
    _InputManager _input = new _InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEX _scene = new SceneManagerEX();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    // 전역으로 사용
    public static DataManager Data { get { return Instance._data; } }
    public static _InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }

    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEX Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    void Start()
    {
        Init();
    }

    void Update()
    {
        // 입력 확인
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

            // DataManager 초기화
            s_instance._data.Init();

            // PoolManager 초기화
            s_instance._pool.Init();

            // 사운드매니저 초기화
            s_instance._sound.Init();

        }


        // 아직 문제 남음
        // 만약 Managers 가 없어서 못 찾으면 NUll...
        // Instance 가 null 인지 확인해야함
    }

    public static void Clear()
    {
        // 씬 이동할 때 날려줘야 할 것들
        // 각 매니저 클리어 함수
        // data는 클리어 할 이유 없음

        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        UI.Clear();

        // 다른 곳에서 사용할 수도 있어서 마지막에 clear
        Pool.Clear();
    }
}
