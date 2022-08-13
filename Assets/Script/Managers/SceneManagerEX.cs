using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    // Unity 제공 SceneManager 있기 때문에 Extended 붙임
    // 씬 관리, 씬 이동

    // 선봉대 BaseScene 출력
    // BaseScene 컴포넌트 들고있는 오브젝트를 BaseScene으로 출력(제너릭형식)
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        // 씬 이동
        // 유니티 제공 버전에서는 string 받았는데
        // 우리는 Define.Scene 으로 enum 값 활용하니까 바꿈

        Managers.Clear(); // 불필요한 메모리 싹 날림
        
        // 유니티 제공 LoadScene 함수 활용 - string 값 넣기 위해 GetSceneName 함수 사용
        SceneManager.LoadScene(GetSceneName(type)); // 다음 씬 이동  
    }

    string GetSceneName(Define.Scene type)
    {
        // Define.Scene 값(enum)을 string으로 변경하는 함수
        // 유니티 제공 SceneManager.LoadScene 은 string 을 입력 받기 때문.

        string name = System.Enum.GetName(typeof(Define.Scene), type); // 실제 이름 추출 (C# 리플렉션 기능 활용)
        return name;
    }

    public void Clear()
    {
        // 현재 씬 클리어 작업
        // 현재 사용하는 씬 찾아서 클리어
        CurrentScene.Clear(); 
        
    }
}
