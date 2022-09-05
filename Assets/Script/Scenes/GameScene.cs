using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    // 게임씬
    // 게임씬의 선봉대 역할
    // @Scene 오브젝트 : 선봉대

    // 코루틴 관련
    /*
    //class Test
    //{
    //    public int Id = 0;
    //}

    //class CoroutineTest : IEnumerable
    //{
    //    public IEnumerator GetEnumerator()
    //    {
    //        //// return 타입 : System.Object 이라 모든거 리턴 가능
    //        //// yield return : 일시정지
    //        //yield return new Test() { Id = 1 }; 
    //        ////yield return null; // null 이용해서 나가기 밑에서 null 체크해줘야하긴함
    //        //yield break; // 진짜 종료하는법, 일반 함수에서의 return;

    //        //// Unreachable
    //        //yield return new Test() { Id = 2 };
    //        //yield return new Test() { Id = 3 };
    //        //yield return new Test() { Id = 4 };

    //        for(int i = 0; i < 1000000; i++)
    //        {
    //            if (i % 10000 == 0)
    //                yield return null; // 만번째 단위로 잠시 쉬기
    //        }


    //    }

    //    void GenerateItem()
    //    {
    //        // 아이템을 만들어준다
    //        // DB 저장

    //        // 멈춤
    //        // 로직
    //    }
    //}

    Coroutine co; // handle 역할
    */

    // CAPTCHA system
    UI_Captcha uI_Captcha;
    Coroutine co;
    const float CaptchaDelaySeconds = 3600.0f; // 1hour

    void Awake()
    {
        // cf) Awake : Start 보다 먼저, 컴포넌트 꺼져있어도 오브젝트가 들고 있으면 가능
        // 주의 : 오브젝트가 꺼져있으면 안 됨.
        Init();
    }

    protected override void Init()
    {
        // 초기화
        base.Init();

        // 씬타입 설정
        SceneType = Define.Scene.InGame;

        // 씬 UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");
        //Managers.UI.ShowSceneUI<MiniMap>("MiniMap");
        //Managers.UI.ShowSceneUI<UI_Inven>();

        // InGame Scene BGM 설정
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        // 풀매니저 테스트
        List<GameObject> list = new List<GameObject>();

        for (int i = 0; i < 2; i++)
            list.Add(Managers.Resource.Instantiate("Enemy_Skeleton"));

        //for (int i = 0; i < 5; i++)
        //    list.Add(Managers.Resource.Instantiate("Enemy_Spider"));
        //Managers.Resource.Instantiate("Enemy_Skeleton");

        // 다시 넣기
        //foreach (GameObject obj in list)
        //    Managers.Resource.Destroy(obj);


        /*
         * 코루틴 관련
           코루틴 테스트
        CoroutineTest test = new CoroutineTest();
        foreach(System.Object t in test)
        {
            // 여기서 판단해줌
            Test value = (Test)t;
            Debug.Log(value.Id);
        }

        co = StartCoroutine(CoExplodeAfterSeconds(4.0f));
        StartCoroutine("CoStopExplode", 2.0f);
        */


        // DataManager test - 외부에서 사용할 때
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Dictionary<int, Data.Map> dict_map = Managers.Data.MapDict;

        // 커서컨트롤러 @Scene
        gameObject.GetOrAddComponent<CursorController>();
        // @Scene으로 옮김
        //gameObject.GetOrAddComponent<UI_Setting>();
        gameObject.GetOrAddComponent<Player_Controller>();

        // CAPTCHA System
        co = StartCoroutine("CoCaptcha", CaptchaDelaySeconds);
    }

    IEnumerator CoCaptcha(float seconds)
    {
        while(true)
        {
            yield return new WaitForSeconds(seconds);
            uI_Captcha = Managers.UI.ShowPopupUI<UI_Captcha>();
            uI_Captcha.GenerateCaptcha();
        }
    }

    // 코루틴 관련
    /*
    IEnumerator CoStopExplode(float seconds)
    {
        Debug.Log("Stop Enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Stop Execute!!!");
        if(co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    IEnumerator CoExplodeAfterSeconds(float seconds)
    {
        //yield return null; // 한박자 쉬고 다음 틱에서 이어서

        Debug.Log("Explode Enter");

        // IEnumerator 는 게임오브젝트 반환해서 임의로 정의한 클래스 넣을수있음
        // waitforseconds 는 유니티가 만들어둔 클래스
        yield return new WaitForSeconds(seconds); // 다음부분은 seconds 후에 실행

        Debug.Log("Explode Execute!!");

        co = null;

    }
    */

    public override void Clear()
    {
        // 이 씬이 종료될 때 날려줘야 하는 부분 넣어줘야 함
        Debug.Log("InGameScene Clear");
    }



}
