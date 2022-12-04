using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonScene : GameScene
{
   // CAPTCHA system
    //UI_Captcha uI_Captcha;
    Coroutine co;
    const float CaptchaDelaySeconds = 3600.0f; // 1hour
    
    UI_Dungeon _sceneUI;

    void Awake()
    {
        //Init();
    }

    protected override void Init()
    {
        SceneType = Define.Scene.Dungeon;
        //base.Init();


        // 화면 크기 설정 *** (멀티 플레이 테스트할 때 전체화면 불편)
        //Screen.SetResolution(640, 480, false);


        // �� UI
        if(_sceneUI == null)
            _sceneUI = Managers.UI.ShowSceneUI<UI_Dungeon>("UI_Dungeon");


        // InGame Scene BGM ����
        //Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        if (SceneManager.GetActiveScene().name == "DungeonCemetery")
        {
            Managers.Sound.Play("Sound/BGM/BGM_DungeonCemetery", Define.Sound.Bgm);

        }
        else if (SceneManager.GetActiveScene().name == "DungeonNature")
        {
            Managers.Sound.Play("Sound/BGM/BGM_DungeonNature", Define.Sound.Bgm);
        }
        else if (SceneManager.GetActiveScene().name == "DungeonDesert")
        {
            Managers.Sound.Play("Sound/BGM/BGM_DungeonDesert", Define.Sound.Bgm);
        }

        // DataManager test
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //Dictionary<int, Data.Map> dict_map = Managers.Data.MapDict;

        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();


        // CAPTCHA System
        //co = StartCoroutine("CoCaptcha", CaptchaDelaySeconds);

        //
        //Managers.NPC.Init();
    }


    //IEnumerator CoCaptcha(float seconds)
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(seconds);
    //        uI_Captcha = Managers.UI.ShowPopupUI<UI_Captcha>();
    //        uI_Captcha.GenerateCaptcha();
    //    }
    //}

    public override void Clear()
    {
        Debug.Log("InGameScene Clear");
    }

}
