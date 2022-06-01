using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Login : UI_Scene
{
    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        TitleText,
        PointText,
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

        Bind<Button>(typeof(Buttons)); // Buttons 의 enum 타입을 넘기겠다 는 의미
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);
    }


    public void OnButtonClicked(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.InGame);
    }
}
