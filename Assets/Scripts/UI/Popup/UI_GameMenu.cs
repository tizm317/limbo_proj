using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using System;

public class UI_GameMenu : UI_Popup
{
    UI_Settings setting;

    enum Buttons
    {
        ButtonResume,
        ButtonOptions,
        ButtonLogout,
        ButtonExit,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ButtonResume).gameObject.BindEvent(Resume);
        GetButton((int)Buttons.ButtonOptions).gameObject.BindEvent(Options);
        GetButton((int)Buttons.ButtonLogout).gameObject.BindEvent(Logout);
        GetButton((int)Buttons.ButtonExit).gameObject.BindEvent(Exit);
    }

    public void Resume(PointerEventData data)
    {
        ClosePopupUI();
    }

    public void Options(PointerEventData data)
    {
        setting = Managers.UI.ShowPopupUI<UI_Settings>();
    }

    public void Logout(PointerEventData data)
    {
        Managers.Scene.LoadScene(Define.Scene.Login);
    }

    public void Exit(PointerEventData data)
    {
        Util.Quit();
    }

    internal UI_Settings getSettingUI()
    {
        return setting;
    }
}
