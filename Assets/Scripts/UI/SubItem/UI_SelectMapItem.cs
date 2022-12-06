using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectMapItem : UI_Base
{
    public MapInfo Info { get; set; }

    enum Buttons
    {
        MapButton
    }

    enum Texts
    {
        MapNameText
    }

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.MapButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if (Info == null) return;
        GetText((int)Texts.MapNameText).text = Info.Name;
    }

    void OnClickButton(PointerEventData data)
    {
        //Managers.Network.ConnectToGame(Info);

        //StopAllCoroutines();

        // 서버... 맵 이동....
        //LoadingScene.LoadScene(Info.scene.ToString());
        Managers.Scene.LoadScene(Info.scene);
        Managers.UI.ClosePopupUI();
    }
}
