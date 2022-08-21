using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InGame : UI_Scene
{
    // 수정(추가)해야 함

    // 연관된 팝업 UI 목록
    UI_Inven ui_Inven;
    UI_Equipment ui_Equipment;
    UI_MiniMap miniMap;
    UI_Setting setting;

    Player_Controller player;

    enum Buttons
    {
    }
    enum Texts
    {
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

        // 연결
        Managers.Input.KeyAction -= ControlPopUpUI;
        Managers.Input.KeyAction += ControlPopUpUI;

        player = GameObject.Find("@Scene").GetComponent<Player_Controller>();
    }

    private void OnApplicationQuit()
    {
        // 꺼질 때에도 저장해야 함
        saveInven();
    }

    void ControlPopUpUI()
    {
        // NPC와 상호작용중이면 X
        if (player.IsInteractWithNPC)
            return;

        // 키보드 입력 -> 팝업UI On/Off
        if (Input.GetKeyDown(KeyCode.I))
        {
            // 인벤토리 UI

            if(!ui_Inven)
            {
                ui_Equipment = Managers.UI.ShowPopupUI<UI_Equipment>();
                ui_Inven = Managers.UI.ShowPopupUI<UI_Inven>();
            }
            else
            {
                // 인벤토리 내용(변경사항) json 저장
                if (!ui_Inven.IsPeek())
                    return;
                saveInven();
                Managers.UI.ClosePopupUI(ui_Inven);
                ui_Equipment.ClosePopupUI();
            }
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            // 미니맵 UI
            // off -> 최소 -> 중간 -> 최대 -> off
            if (!miniMap)
                miniMap = GetComponentInChildren<UI_MiniMap>();

            if (miniMap.gameObject.activeInHierarchy == false)
                miniMap.gameObject.SetActive(true);
            
            miniMap.SizeControl(); /////////////////////////////////////////////////////////

            //if (!miniMap)
            //    miniMap = Managers.UI.ShowPopupUI<UI_MiniMap>();
            //else
            //    miniMap.SizeControl(); // 미니맵 크기 조절
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            if (!miniMap)
                return;
            // 미니맵 줌 조절
            miniMap.Zoom();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!setting)
                setting = Managers.UI.ShowPopupUI<UI_Setting>();
            else
            {
                // 인벤토리 내용(변경사항) json 저장
                if (!setting.IsPeek())
                    return;
                Managers.UI.ClosePopupUI(setting);
            }
        }
    }

    public void saveInven()
    {
        // 여기 있을게 아닌거 같은디..

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
