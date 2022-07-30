using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Login : UI_Scene
{
    // 로그인 씬 용 UI

    enum Buttons
    {
        StartButton,
        EndButton,
    }

    enum Texts
    {
        TitleText,
        StartText,
        EndText,
    }

    enum GameObjects
    {
    }

    enum Images
    {
        BackGroundImage
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons)); 
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.StartButton).gameObject.BindEvent(StartButtonClicked);
        GetButton((int)Buttons.EndButton).gameObject.BindEvent(EndButtonClicked);

    }


    private void StartButtonClicked(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.InGame);
    }
    private void EndButtonClicked(PointerEventData data)
    {
        Util.Quit();
    }
}
