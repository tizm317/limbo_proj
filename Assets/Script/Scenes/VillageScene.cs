using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageScene : BaseScene
{
    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        // 초기화

        base.Init();

        // 씬타입 설정
        SceneType = Define.Scene.InGameVillage;

        // 씬 UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");

        // Village Scene BGM 설정
        Managers.Sound.Play("Sound/BGM/BGM_Dramatic_Theme", Define.Sound.Bgm);


        // 커서컨트롤러 @Scene
        gameObject.GetOrAddComponent<CursorController>();
        // @Scene으로 옮김
        //gameObject.GetOrAddComponent<UI_Setting>();
        //gameObject.GetOrAddComponent<Player_Controller>();


        //
        Managers.Data.MapDict.Clear();
        Managers.Data.MapTestSceneMapDataLoad();
    }
    public override void Clear()
    {
        // 이 씬이 종료될 때 날려줘야 하는 부분 넣어줘야 함
        Debug.Log("Village Scene Clear");
    }
}
