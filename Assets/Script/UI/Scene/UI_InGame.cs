using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InGame : UI_Scene
{
    enum Buttons
    {
        //PointButton,
    }

    enum Texts
    {
        //PointText,
    }

    enum GameObjects
    {
    }

    enum Images
    {
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        //Bind<Button>(typeof(Buttons)); // Buttons 의 enum 타입을 넘기겠다 는 의미
        //Bind<Text>(typeof(Texts));

        //GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);

        // 연결
        Managers.Input.KeyAction -= ControlPopUpUI;
        Managers.Input.KeyAction += ControlPopUpUI;
    }

    UI_Inven ui_Inven;
    MiniMap miniMap;

    int miniMapStep = (int)minimap.Off;
    enum minimap
    {
        Off,
        Min,
        Middle,
        Max,
    }

    int miniMapZoom = (int)zoom.defaultZoom;
    enum zoom
    {
        defaultZoom,
        secondZoom,
        MaxZoom,
    }

    void ControlPopUpUI()
    {
        // 키보드 입력 -> 팝업UI On/Off
        if (Input.GetKeyDown(KeyCode.I))
        {
            // 인벤토리 UI

            if(!ui_Inven)
                ui_Inven = Managers.UI.ShowPopupUI<UI_Inven>();
            else
               Managers.UI.ClosePopupUI(ui_Inven);
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            // 미니맵 UI
            // off -> 최소 -> 중간 -> 최대 -> off

            switch (miniMapStep)
            {
                case (int)minimap.Off:
                    miniMap = Managers.UI.ShowPopupUI<MiniMap>();
                    miniMapStep = (int)minimap.Min;
                    miniMap.SizeControl(miniMapStep);
                    Debug.Log("Step : " + miniMapStep);
                    break;
                case (int)minimap.Min:
                    miniMapStep = (int)minimap.Middle;
                    Debug.Log("Step : " + miniMapStep);
                    miniMap.SizeControl(miniMapStep);
                    break;
                case (int)minimap.Middle:
                    miniMapStep = (int)minimap.Max;
                    Debug.Log("Step : " + miniMapStep);
                    miniMap.SizeControl(miniMapStep);
                    break;
                case (int)minimap.Max:
                    miniMap.SizeControl(miniMapStep);
                    Managers.UI.ClosePopupUI(miniMap);
                    miniMapStep = (int)minimap.Off;
                    miniMapZoom = (int)zoom.defaultZoom; // 줌도 초기화
                    Debug.Log("Step : " + miniMapStep);
                    break;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            if (!miniMap)
                return;

            switch (miniMapZoom)
            {
                case (int)zoom.defaultZoom:
                    miniMapZoom++;
                    miniMap.Zoom(miniMapZoom);
                    break;
                case (int)zoom.secondZoom:
                    miniMapZoom++;
                    miniMap.Zoom(miniMapZoom);
                    break;
                case (int)zoom.MaxZoom:
                    miniMapZoom = (int)zoom.defaultZoom;
                    miniMap.Zoom(miniMapZoom);
                    break;
            }
        }
    }
}
