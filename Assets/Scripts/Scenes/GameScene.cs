using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GameScene : BaseScene
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

    public UI_InGame UI_InGame { get { return _sceneUI; } }
    UI_InGame _sceneUI;

    // CAPTCHA system
    UI_Captcha uI_Captcha;
    Coroutine co;
    const float CaptchaDelaySeconds = 3600.0f; // 1hour

    //보스 1시간마다 생성
    GameObject Boss;
    public float cooldownTime = 3600.0f;
    private float nextFireTime = 0f;
    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "InGameNature":
                SceneType = Define.Scene.InGameNature;
                break;
            case "InGameDesert":
                SceneType = Define.Scene.InGameDesert;
                break;
            case "InGameCemetery":
                SceneType = Define.Scene.InGameCemetery;
                break;
        }
        base.Init();

        // 화면 크기 설정 *** (멀티 플레이 테스트할 때 전체화면 불편)
        //Screen.SetResolution(640, 480, false);
        Set_Resolution();//나중에 주석 해제해주시면 됨돠!
        // �� UI
        _sceneUI = Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");

        //Managers.UI.ShowSceneUI<MiniMap>("MiniMap");
        //Managers.UI.ShowSceneUI<UI_Inven>();

        // InGame Scene BGM ����
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        GameObject go = new GameObject { name = "SpawningPool" };
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        pool.SetKeepMonsterCount(3);


        //List<GameObject> list = new List<GameObject>();
        //for (int i = 0; i < 3; i++)
        //{
        //    list.Add(Managers.Resource.Instantiate("Enemy_Wizard"));
        //}


        //foreach (GameObject obj in list)
        //{
        //    Managers.Resource.Destroy(obj);
        //}

        // DataManager test - �ܺο��� ����� ��
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Dictionary<int, Data.Map> dict_map = Managers.Data.MapDict;

        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();


        // CAPTCHA System
        co = StartCoroutine("CoCaptcha", CaptchaDelaySeconds);

        //
        Managers.NPC.Init(SceneType);
    }
    private void Update()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + cooldownTime;


            if (SceneManager.GetActiveScene().name == "InGameCemetery")
            {
                if (Boss != null)
                    return;
                Boss = Managers.Resource.Instantiate("Warrok W Kurniawan");
                NavMeshAgent nma0 = Boss.GetComponent<NavMeshAgent>();
            }
            else if (SceneManager.GetActiveScene().name == "InGameNature")
            {
                if (Boss != null)
                    return;
                Boss = Managers.Resource.Instantiate("Enemy_Rhino");
                NavMeshAgent nma0 = Boss.GetComponent<NavMeshAgent>();
            }
            else if (SceneManager.GetActiveScene().name == "InGameDesert")
            {
                if (Boss != null)
                    return;
                Boss = Managers.Resource.Instantiate("Vanguard");
                NavMeshAgent nma0 = Boss.GetComponent<NavMeshAgent>();
            }

        }
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

    public void Set_Resolution()
    {
        int set_Width = 2560;
        int set_Height = 1440;
        int device_Width = Screen.width;
        int device_Height = Screen.height;
        
        // Screen.SetResolution(set_Width,(int)((float)device_Height/device_Width) * set_Width, false);
        // if((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        // {
        //     float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
        //     Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, 1f); // 새로운 Rect 적용
        // }
        // else // 게임의 해상도 비가 더 큰 경우
        // {
        //     float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
        //     Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        // }
        if(set_Width > device_Width)
        {
            set_Height = (int)(set_Height * ((float)device_Width/set_Width));
            set_Width = device_Width;
        }
        if(set_Height > device_Height)
        {
            set_Width = (int)(set_Width * ((float)device_Height/set_Height));
            set_Height = device_Height;
        }
        Debug.LogFormat("set_Width = {0}, set_Height = {1}",set_Width, set_Height);
        Screen.SetResolution(set_Width,set_Height, false);
        if((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        {
            float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
            Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, set_Height); // 새로운 Rect 적용
            //Debug.LogFormat("Current Resolution = {0} * {1}",new_Width,set_Height);
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, set_Width, newHeight); // 새로운 Rect 적용
            //Debug.LogFormat("Current Resolution = {0} * {1}",set_Width,newHeight);
        }
    }
}
