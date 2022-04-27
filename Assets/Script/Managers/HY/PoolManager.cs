using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    // 풀매니저 : 리소스 매니저 보좌하는 역할
    // 오브젝트 풀링

    // 리소스 파일을 메모리에 불러오고, 그 메모리에 있는 것을 instantiate 하는 작업이 매우 느림
    // 개선법 : 오브젝트 풀링
    // 자주 사용하는 오브젝트를 꺼두고, 필요할 때 켜서 사용(씬에 있지만, 안보이게)

    // 풀링할 대상 어떻게 구분? 컴포넌트(Poolable) 이용해서 찾음
    // 프리팹 단위로 풀링

    // 풀매니저는 여러개 풀 가짐 -> class Pool
    // 풀매니저 : 딕셔너리 이용해서 풀 목록 들고있음 -> Dictionary<string, Pool> _pool
    // 각 오브젝트 종류별로 나눠서 관리하기 위함

    #region Pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        // 스택
        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            // 초기화

            Original = original;

            // @Pool_Root 산하 각 오브젝트의 루트
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root"; 

            // 초기화할 때 count 갯수만큼 들고 있음
            for(int i = 0; i < count; i++)
                Push(Create()); // create 하자마자 push
        }

        Poolable Create()
        {
            // 새로운 객체 생성 후 반환 

            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name; // 이름에서 (Clone) 제거
            return go.GetOrAddComponent<Poolable>(); // Poolable 컴포넌트 getOrAdd
        }

        public void Push(Poolable poolable)
        {
            // _poolStack 에 푸시하는 함수
            // 푸시하기 전에 해줘야할 작업들 : parent 연결, 비활성화 , IsUsing = false;

            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            // _poolStack 에서 Pop 하는 함수
            // 팝 이후에 해주는 작업들 : parent 연결, 활성화, IsUsing = true;

            Poolable poolable;

            // _poolStack에 있으면 (대기상태 있으면) Pop해서 꺼내옴
            // 없으면 새 객체 Create함
            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            // 활성화
            poolable.gameObject.SetActive(true);

            // DontDestroyOnLoad 해제 용도
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            
            // parent 연결
            poolable.transform.parent = parent;
            // IsUsing
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion


    // 풀매니저 : 딕셔너리로 Pool 관리
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();  // 여러개의 pool 관리하는 딕셔너리
    
    Transform _root; // cf) GameObject 도 상관 없음 (걍 Transform은 항상 포함되는 컴포넌트라)

    public void Init()
    {
        // 초기화
        // 자신의 루트 만들고, 그 루트가 풀링 필요한 오브젝트 들고 있게 만듦 (대기실 역할)

        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        // 풀매니저에 풀 처음 만드는 함수 (딕셔너리에 넣는 함수)

        // Pool 생성 과정
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root; // @Pool_Root와 연결 

        // Pool 을 풀매니저 딕셔너리에 넣기
        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        // Pool 에다 집어넣는 함수
        // 인풋 : Poolable 컴포넌트 가진 오브젝트
        // 다 사용하고 풀에 반환하는 함수

        string name = poolable.gameObject.name;

        // pool 이 없는데 푸시 하는 상황? (특수한 경우 ex : 에디터 상에서 드래그 드롭으로 만든 경우)
        if(_pool.ContainsKey(name) == false)
        {
            // 그냥 지워줌
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        
        // 풀 매니저에 name으로 찾아서 해당 풀에 푸시(반납)
        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        // 사용하기 위해 풀매니저 딕셔너리에서 Pool 찾은 후 Pop하는 함수
        // 풀링된 오브젝트 있으면 바로 사용하는 함수

        // original의 이름을 string key로 사용

        // (처음 시도한 경우) 풀매니저에 orginal.name 에 해당하는 Pool 없으면, Create 한다.
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        // 풀매니저에서 orignal.name 으로 딕셔너리에서 Pool 찾아서 Pop함수로 Poolable 리턴
        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        // 풀매니저(딕셔너리)에서 오리지널 반환해주는 함수

        // 풀매니저에 없는 경우 null 반환
        if (_pool.ContainsKey(name) == false)
            return null;

        // 풀매니저에 존재하는 경우, 딕셔너리에서 찾아서 리턴
        return _pool[name].Original;
    }

    public void Clear()
    {
        // 씬 이동할 때...
        // pool 날려야 하나? 유지해야 하나?
        // 대부분의 게임에서 굳이 날릴 필요는 없는데
        // mmoRPG 에서 지역마다 오브젝트가 많이 다른 경우 정도만?
        // 일단 만들어줌
        
        // @Pool_Root(_root) 산하 child 모두 지우기
        foreach (Transform child in _root)
        {
            GameObject.Destroy(child.gameObject);

            // 풀 매니저도 초기화
            _pool.Clear();
        }
    }
}
