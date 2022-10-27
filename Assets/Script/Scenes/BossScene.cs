using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScene : BaseScene
{
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
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.InGame;

        // 화면 크기 설정 *** (멀티 플레이 테스트할 때 전체화면 불편)
        Screen.SetResolution(640, 480, false);


        // �� UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");
        //Managers.UI.ShowSceneUI<MiniMap>("MiniMap");
        //Managers.UI.ShowSceneUI<UI_Inven>();

        // InGame Scene BGM ����
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        GameObject go = new GameObject { name = "SpawningPool" };
        SpawningPool_Boss pool = go.GetOrAddComponent<SpawningPool_Boss>();
        pool.SetKeepMonsterCount(1);

        // DataManager test - �ܺο��� ����� ��
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Dictionary<int, Data.Map> dict_map = Managers.Data.MapDict;

        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();


        // CAPTCHA System
        co = StartCoroutine("CoCaptcha", CaptchaDelaySeconds);

        //
        Managers.NPC.Init();
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
        Debug.Log("InGameScene Clear");
    }



}
