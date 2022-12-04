using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GameScene : BaseScene
{
    public UI_InGame UI_InGame { get { return _sceneUI; } }
    UI_InGame _sceneUI;

    // CAPTCHA system
    UI_Captcha uI_Captcha;
    Coroutine co;
    const float CaptchaDelaySeconds = 900.0f; // 15min

    //보스 15min마다 생성
    GameObject Boss;
    [SerializeField] private float cooldownTime = 900.0f;
    private float nextFireTime = 0f;
    void Start()
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
            case "InGameVillage":
                SceneType = Define.Scene.InGameVillage;
                break;
            default: // 나머지 던전
                SceneType = Define.Scene.Dungeon;
                break;
        }
        base.Init();

        // 화면 크기 설정 *** (멀티 플레이 테스트할 때 전체화면 불편)
        //Screen.SetResolution(640, 480, false);
        //Set_Resolution();//나중에 주석 해제해주시면 됨돠! (HY : BaseScene으로 옮김!)
        // �� UI
        if(_sceneUI == null)
            _sceneUI = Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");

        // InGame Scene BGM ����
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        if (SceneManager.GetActiveScene().name == "InGameVillage")
        {

        }
        else
        {
            GameObject go = new GameObject { name = "SpawningPool" };
            SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
            pool.SetKeepMonsterCount(5);

            GameObject goTest = new GameObject { name = "SpawningPoolTest" };
            PoolingTest poolTest = goTest.GetOrAddComponent<PoolingTest>();
            poolTest.SetKeepMonsterCount(4);

            GameObject goTest2 = new GameObject { name = "SpawningPoolTest2" };
            PoolingTest2 poolTest2 = goTest2.GetOrAddComponent<PoolingTest2>();
            poolTest2.SetKeepMonsterCount(2);
        }

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

        // NPC
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

    public override void Clear()
    {
        Debug.Log("InGameScene Clear");
    }
}
