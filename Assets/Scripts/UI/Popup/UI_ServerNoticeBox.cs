﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ServerNoticeBox : UI_Popup
{
    // 서버 점검 공지
    // TODO: 점검 시간 추가?
    // 서버 안 열렸을 때 팝업

    enum Buttons
    {
        CloseButton,
        SinglePlayButton,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseButtonClicked);
        GetButton((int)Buttons.SinglePlayButton).gameObject.BindEvent(SinglePlayButtonClicked);
    }

    private void CloseButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }
    private void SinglePlayButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
        LoadingScene.LoadScene(Define.Scene.Lobby);
        //Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
