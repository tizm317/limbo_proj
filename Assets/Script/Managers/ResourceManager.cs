using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    // 프리팹을 코드 상에서 만들어주기 위한 리소스 매니저
    // 3가지 인터페이스 제공
    // Load
    // Instantiate
    // Destroy

    // 유니티 툴로 프리팹 연결하면, 복잡하기 때문에 코드로 관리
    // Resources.Load<GameObject>("Prefabs/Tank") 
    // 경로 : 에셋 밑에 리소스 폴더 기준으로 경로 입력

    // + 풀매니저 연관

    public T Load<T>(string path) where T : Object
    {
        // 개선사항1 : original 이미 들고 있으면 바로 사용 (OK)
        if (typeof(T) == typeof(GameObject))// 프리팹일 확률 높음
        {
            // path : /knight 에서 '/' 자르는 작업 (substring으로 / 다음만)
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            // Pool 에 있는지 확인 , 있으면 바로 반환
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        // Pool에 없으면 리소스에서 로드
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        // 1) 경로 찾아서 로드해서 원본을 메모리에 로드 - 씬에서 아직 보이지 않음
        GameObject original = Load<GameObject>($"Prefabs/{path}"); // 프리팹 산하 경로 (필요시 경로 수정하면 됨)
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // 개선사항2 : 혹시 풀링된 애가 있는지 체크 -> 있으면 그걸 반환
        // Poolable 컴포넌트 갖고 있어야 풀링 대상이므로 체크 , 맞으면 Pop (처음실행 : 풀 생성하면서 return, 아니면 대기중인거 리턴)
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        // Poolable 대상 아니면,
        // 2) Instantiate : 원본을 복사해서 씬에 배치
        GameObject go = Object.Instantiate(original, parent);

        // (Clone) 지우기2 더 간단한 방법 : 원본 이름 넣음
        go.name = original.name;
        return go;

        /*
         * instantiate할 때, 뒤에 붙은 (Clone) 지우기1
            int index = go.name.IndexOf("(Clone)");
            if (index > 0)
                go.name = go.name.Substring(0, index);
        */
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        // 개선 사항 : 만약 풀링이 필요한 오브젝트? -> 풀링 매니저한테 위탁
        // Poolable 컴포넌트로 판단
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            // 풀링 대상이면, Push 해서 반환
            Managers.Pool.Push(poolable);
            return;
        }

        // 풀링 대상 아니면 destroy
        Object.Destroy(go);
    }
}
