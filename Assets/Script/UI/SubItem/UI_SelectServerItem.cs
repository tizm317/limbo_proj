using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerItem : UI_Base
{
    public ServerInfo Info { get; set; }

    enum Buttons
    {
        ServerButton
    }

    enum Texts
    {
        ServerNameText
    }

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.ServerButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null) return;
        GetText((int)Texts.ServerNameText).text = Info.Name;
    }

    void OnClickButton(PointerEventData data)
    {
        Managers.Network.ConnectToGame(Info);

        // Connect 실패할 때 load 하면 안되는데..? 
        Managers.Scene.LoadScene(Define.Scene.InGame);

        Managers.UI.ClosePopupUI();
    }

}
