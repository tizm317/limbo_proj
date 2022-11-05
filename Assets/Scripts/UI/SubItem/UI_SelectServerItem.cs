using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerItem : UI_Base
{
    public ServerInfo Info { get; set; }

    [SerializeField]
    private Sprite[] _networkIcons;

    enum Buttons
    {
        ServerButton
    }

    enum Texts
    {
        ServerNameText
    }

    enum Images
    {
        NetworkIcon
    }


    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.ServerButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null) return;
        GetText((int)Texts.ServerNameText).text = Info.Name;

        if (Info.Open == 1) // Open
        {
            GetButton((int)Buttons.ServerButton).interactable = true;
            GetImage((int)Images.NetworkIcon).sprite = _networkIcons[0];
        }
        else // Close
        {
            GetButton((int)Buttons.ServerButton).interactable = false;
            GetImage((int)Images.NetworkIcon).sprite = _networkIcons[1];
        }
    }

    void OnClickButton(PointerEventData data)
    {
        Managers.Network.ConnectToGame(Info);

        if (Info.Open == 1)
        {
            Managers.Scene.LoadScene(Define.Scene.InGameVillage);
            Managers.UI.ClosePopupUI();
        }
        
    }
}
