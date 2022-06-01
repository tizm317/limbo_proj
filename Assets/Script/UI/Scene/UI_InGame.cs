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

    private void OnApplicationQuit()
    {
        // 꺼질 때에도 저장해야 함
        saveInven();
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
            {
                // 인벤토리 내용(변경사항) json 저장
                if (!ui_Inven.IsPeek())
                    return;
                saveInven();
                Managers.UI.ClosePopupUI(ui_Inven);
            }
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            // 미니맵 UI
            // off -> 최소 -> 중간 -> 최대 -> off
            if (miniMap && !miniMap.IsPeek()) // 미니맵은 켜져있으면서 팝업 첫번째 아니면 리턴
                return;

            ChangeMiniMapStep();
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            if (!miniMap.IsPeek())
                return;
            changeMinimapZoom();
        }
    }

    public void ChangeMiniMapStep()
    {

        switch (miniMapStep)
        {
            case (int)minimap.Off:
                if (Managers.Data.MapDict.Count == 0) // 처음에 안 생긴 경우만
                    Managers.Data.MakeMapDict();
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
                //miniMap.SizeControl(miniMapStep);
                miniMapStep = (int)minimap.Off;
                miniMapZoom = (int)zoom.defaultZoom; // 줌도 초기화
                Managers.UI.ClosePopupUI(miniMap);
                Debug.Log("Step : " + miniMapStep);
                break;
        }
    }

    public void changeMinimapZoom()
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

    public void saveInven()
    {
        // dictionary 변경 있으면 json 저장
        //state machine 써야하나..?

        Managers.Data.resetSaveData2(); // json에 덮여쓰이는거 막기 위함

        Dictionary<int, Data.Item> invenDict = Managers.Data.InvenDict;
        string json = "";

        for (int key = 0; key < invenDict.Count; key++)
        {
            // MakeList() 에서 List 만들어서 반환
            json = Managers.Data.MakeListInDict(invenDict[key]);
        }
        // List 최종본이 json에 저장된 채로 나옴
        Managers.Data.SaveJson(json, "InvenData.json");
    }
}
