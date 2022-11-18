using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NoticeBox : UI_Popup
{
    private Text _noticeText;

    enum Texts
    {
        NoticeText,
    }

    enum Buttons
    {
        CloseButton,
    }

    private void Start()
    {
        if (_noticeText == null)
            Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseButtonClicked);
        _noticeText = GetText((int)Texts.NoticeText);
    }

    private void CloseButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }

    public void Notice(int noticeType, bool isSuccess)
    {
        if (_noticeText == null)
            Init();

        // 계정 생성 : 1
        // 로그인    : 2
        switch(noticeType)
        {
            case 1:
                _noticeText.text = "Create Account ";
                break;
            case 2:
                _noticeText.text = "Login ";
                break;
        }

        // 성공 실패 여부
        if(isSuccess == true)
        {
            _noticeText.text += "Success";
        }
        else
        {
            _noticeText.text += "Fail";
        }
    }
}
