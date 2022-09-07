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

    #region RadialMenu
    private UI_RadialMenu radialMenu;
    private KeyCode key_emoticon = KeyCode.T;
    private KeyCode key_action = KeyCode.G;

    UI_Emoticon emoticon;

    private Sprite[] sprites_action;
    private Sprite[] sprites_emoticon;
    Coroutine co;

    enum Action
    {
        angry,
        pray,
        card_shuffle,
        twerk,
        air_guitar,
        dance,
        hi,
        surprise,
        COUNT,
    }

    int emoticonCount = 9;

    int pieceCount = 8;
    #endregion

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
        miniMap = GetComponentInChildren<UI_MiniMap>();

        #region RadialMenu
        sprites_action = new Sprite[8];
        sprites_emoticon = new Sprite[8];

        string num;
        for (int i = 0; i < pieceCount; i++)
        {
            num = (i % emoticonCount).ToString("000");
            sprites_action[i] = Managers.Resource.Load<Sprite>($"Sprites/Action/{(Action)(i % (int)Action.COUNT)}");
            sprites_emoticon[i] = Managers.Resource.Load<Sprite>($"Sprites/Emoticon/Emoticon{num}");
        }

        radialMenu = Managers.UI.ShowPopupUI<UI_RadialMenu>();
        radialMenu.Init();
        radialMenu.SetPieceImageSprites(sprites_action);
        #endregion

        player = GameObject.Find("@Scene").GetComponent<Player_Controller>();
        emoticon = GameObject.Find("@UI_Root").GetComponentInChildren<UI_Emoticon>();

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
            
            miniMap.SizeControl();

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

        if (Input.GetKeyDown(key_action))
        {
            radialMenu.SetPieceImageSprites(sprites_action);
            radialMenu.Show();
            co = StartCoroutine(coKeyUpCheck());
        }

        if (Input.GetKeyDown(key_emoticon))
        {
            radialMenu.SetPieceImageSprites(sprites_emoticon);
            radialMenu.Show();
            co = StartCoroutine(coKeyUpCheck());
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

    public IEnumerator coKeyUpCheck()
    {
        while (true)
        {
            if (Input.GetKeyUp(key_action))
            {
                int selected = radialMenu.Hide();
                Debug.Log($"Selected : {selected}");

                // 감정표현
                player.emotion(selected);
                StopCoroutine(co);
            }

            if (Input.GetKeyUp(key_emoticon))
            {
                int selected = radialMenu.Hide();
                Debug.Log($"Selected : {selected}");

                // 이모티콘
                emoticon.transform.GetChild(0).gameObject.SetActive(true);
                emoticon.setEmoticon(selected);
                StopCoroutine(co);
            }

            yield return null;
        }
    }
}
