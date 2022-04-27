using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    // 로그인 씬
    // @Scene

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        //List<GameObject> list = new List<GameObject>();

        //// 풀매니저 테스트
        //for (int i = 0; i < 5; i++)
        //    list.Add(Managers.Resource.Instantiate("UnityChan"));

        // 다시 넣기
        //foreach (GameObject obj in list)
        //    Managers.Resource.Destroy(obj);
    }

    private void Update()
    {
        // 특정 키 누르면(하드코딩) 다른 씬으로 이동 test
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // 유니티에서 씬 매니저 제공
            // 기능 많음
            //SceneManager.LoadScene("Game");
            // Async 계열 함수 : 비동기로 하는 작업
            // MMORPG 에서 씬 변경 할 때, 다 로딩하려면 개 오래걸림
            // 이럴때 로그인 하는 도중이나, 로비 들어오는 중에, 다음 씬 로드 조금조금씩 로드하는 방식
            // async 이용하면 백그라운드에서 데이터 로드 가능
            // 로딩 화면 만들 때 유용


            // -> 근데 BaseScene, GameScene, LoginScene 등에서 Init, Clear 할 때 중요 작업 하니까
            // 그런 작업 한번에 관리하기 위해서 Scene Manager 하나 새로 만듦 / 선택사항
            // -> 새로 만든 매니저로
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear");
    }

}
