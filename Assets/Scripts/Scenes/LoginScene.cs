using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    // �α��� ��
    // @Scene

    UI_Login _sceneUI;

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        SceneType = Define.Scene.Login;
        base.Init();

        Managers.Web.BaseUrl = "https://localhost:5001/api";

        //Screen.SetResolution(640, 480, false);

        // �� UI
        if(_sceneUI == null)
            _sceneUI = Managers.UI.ShowSceneUI<UI_Login>("UI_Login");

        // Login Scene BGM
        Managers.Sound.Play("Sound/BGM/BGM_Ambient_Version", Define.Sound.Bgm);
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear");
    }

}
