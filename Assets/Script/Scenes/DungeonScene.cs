using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : BaseScene
{
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
        // �ʱ�ȭ
        base.Init();

        SceneType = Define.Scene.Dungeon;

        // 화면 크기 설정 *** (멀티 플레이 테스트할 때 전체화면 불편)
        Screen.SetResolution(640, 480, false);


        // �� UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");


        // InGame Scene BGM ����
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);

        

        // DataManager test
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Dictionary<int, Data.Map> dict_map = Managers.Data.MapDict;

        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();


        // CAPTCHA System
        co = StartCoroutine("CoCaptcha", CaptchaDelaySeconds);

        //
        Managers.NPC.Init();
    }

    void Update()
    {

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