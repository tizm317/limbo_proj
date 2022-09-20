    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    // ���Ӿ�
    // ���Ӿ��� ������ ����
    // @Scene ������Ʈ : ������

    // �ڷ�ƾ ����
    /*
    //class Test
    //{
    //    public int Id = 0;
    //}

    //class CoroutineTest : IEnumerable
    //{
    //    public IEnumerator GetEnumerator()
    //    {
    //        //// return Ÿ�� : System.Object �̶� ���� ���� ����
    //        //// yield return : �Ͻ�����
    //        //yield return new Test() { Id = 1 }; 
    //        ////yield return null; // null �̿��ؼ� ������ �ؿ��� null üũ������ϱ���
    //        //yield break; // ��¥ �����ϴ¹�, �Ϲ� �Լ������� return;

    //        //// Unreachable
    //        //yield return new Test() { Id = 2 };
    //        //yield return new Test() { Id = 3 };
    //        //yield return new Test() { Id = 4 };

    //        for(int i = 0; i < 1000000; i++)
    //        {
    //            if (i % 10000 == 0)
    //                yield return null; // ����° ������ ��� ����
    //        }


    //    }

    //    void GenerateItem()
    //    {
    //        // �������� ������ش�
    //        // DB ����

    //        // ����
    //        // ����
    //    }
    //}

    Coroutine co; // handle ����
    */

    // CAPTCHA system
    UI_Captcha uI_Captcha;
    Coroutine co;
    const float CaptchaDelaySeconds = 3600.0f; // 1hour

    void Awake()
    {
        // cf) Awake : Start ���� ����, ������Ʈ �����־ ������Ʈ�� ��� ������ ����
        // ���� : ������Ʈ�� ���������� �� ��.
        Init();
    }

    protected override void Init()
    {
        // �ʱ�ȭ
        base.Init();

        // ��Ÿ�� ����
        SceneType = Define.Scene.InGame;

        // �� UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");
        //Managers.UI.ShowSceneUI<MiniMap>("MiniMap");
        //Managers.UI.ShowSceneUI<UI_Inven>();

        // InGame Scene BGM ����
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        // Ǯ�Ŵ��� �׽�Ʈ
        List<GameObject> list = new List<GameObject>();

        //for (int i = 0; i < 2; i++)
        //    list.Add(Managers.Resource.Instantiate("Enemy_Skeleton"));

        //for (int i = 0; i < 5; i++)
        //    list.Add(Managers.Resource.Instantiate("Enemy_Spider"));
        //Managers.Resource.Instantiate("Enemy_Skeleton");

        //Managers.Game.Spawn(Define.WorldObject.Monster, "Enemy_Rabbit");

        
        GameObject go = new GameObject { name = "SpawningPool" };
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        pool.SetKeepMonsterCount(2);
        

        // �ٽ� �ֱ�
        //foreach (GameObject obj in list)
        //    Managers.Resource.Destroy(obj);


        /*
         * �ڷ�ƾ ����
           �ڷ�ƾ �׽�Ʈ
        CoroutineTest test = new CoroutineTest();
        foreach(System.Object t in test)
        {
            // ���⼭ �Ǵ�����
            Test value = (Test)t;
            Debug.Log(value.Id);
        }

        co = StartCoroutine(CoExplodeAfterSeconds(4.0f));
        StartCoroutine("CoStopExplode", 2.0f);
        */


        // DataManager test - �ܺο��� ����� ��
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Dictionary<int, Data.Map> dict_map = Managers.Data.MapDict;

        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();
        // @Scene���� �ű�
        //gameObject.GetOrAddComponent<UI_Setting>();
        //gameObject.GetOrAddComponent<Player_Controller>();

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

    // �ڷ�ƾ ����
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
        //yield return null; // �ѹ��� ���� ���� ƽ���� �̾

        Debug.Log("Explode Enter");

        // IEnumerator �� ���ӿ�����Ʈ ��ȯ�ؼ� ���Ƿ� ������ Ŭ���� ����������
        // waitforseconds �� ����Ƽ�� ������ Ŭ����
        yield return new WaitForSeconds(seconds); // �����κ��� seconds �Ŀ� ����

        Debug.Log("Explode Execute!!");

        co = null;

    }
    */

    public override void Clear()
    {
        // �� ���� ����� �� ������� �ϴ� �κ� �־���� ��
        Debug.Log("InGameScene Clear");
    }



}
