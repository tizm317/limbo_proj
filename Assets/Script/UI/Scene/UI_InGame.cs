using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InGame : UI_Scene
{
    // 연관된 팝업 UI 목록
    UI_Inventory uI_Inventory;
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

    enum EmoteAction
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

    GameObject playerGo;
    Player player;
    PlayerStat ps;

    #region UI업데이트
    GameObject Role;
    Image Hp, Mp,Exp_Left, Exp_Right;
    Text Level, Hp_text, Mp_text;
    #endregion

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UI_Update();
    }

    public override void Init()
    {
        base.Init();

        // 연결
        Managers.Input.KeyAction -= ControlPopUpUI;
        Managers.Input.KeyAction += ControlPopUpUI;

        playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null)
        {
            // Player 아직 생성 전이면 생긴 이후에 다시 Init 하도록 코루틴으로 대기함
            ProcessLater(() => GameObject.FindGameObjectWithTag("Player") != null, () => Init());
            return;
        }
        player = playerGo.GetComponent<MyWarrior>();
        miniMap = GetComponentInChildren<UI_MiniMap>();

        #region RadialMenu
        sprites_action = new Sprite[8];
        sprites_emoticon = new Sprite[8];

        string num;
        for (int i = 0; i < pieceCount; i++)
        {
            num = (i % emoticonCount).ToString("000");
            sprites_action[i] = Managers.Resource.Load<Sprite>($"Sprites/Action/{(EmoteAction)(i % (int)EmoteAction.COUNT)}");
            sprites_emoticon[i] = Managers.Resource.Load<Sprite>($"Sprites/Emoticon/Emoticon{num}");
        }

        radialMenu = Managers.UI.ShowPopupUI<UI_RadialMenu>();
        radialMenu.Init();
        radialMenu.SetPieceImageSprites(sprites_action);
        #endregion

        emoticon = GameObject.Find("@UI_Root").GetComponentInChildren<UI_Emoticon>();
        UI_Init();
    }

    void UI_Init()//UI적용에 사용할 오브젝트 찾기용
    {
        ps = player.GetPlayer().GetComponent<PlayerStat>();
        GameObject temp = gameObject.transform.GetChild(0).gameObject;
        
        Hp = temp.transform.GetChild(0).transform.Find("Fill").gameObject.GetComponent<Image>();
        Hp_text = temp.transform.GetChild(0).transform.Find("Text Group").gameObject.transform.GetChild(0).GetComponent<Text>();
        Mp = temp.transform.GetChild(1).transform.Find("Fill").gameObject.GetComponent<Image>();
        Mp_text = temp.transform.GetChild(1).transform.Find("Text Group").gameObject.transform.GetChild(0).GetComponent<Text>();
      
        //아바타 이미지 넣을거면 여기다 추가!
        Level = temp.transform.GetChild(3).transform.Find("Text").gameObject.GetComponent<Text>();
        Role = temp.transform.GetChild(4).gameObject;
        int Case = (int)GameObject.Find("@Scene").GetComponent<PlayerMgr>().job;
        switch(Case)
        {
            case 0:
                Role.transform.GetChild(0).gameObject.SetActive(false);
                Role.transform.GetChild(1).gameObject.SetActive(false);
                Role.transform.GetChild(2).gameObject.SetActive(true);
                break;
            case 1:
                Role.transform.GetChild(0).gameObject.SetActive(false);
                Role.transform.GetChild(1).gameObject.SetActive(true);
                Role.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 2:
                Role.transform.GetChild(0).gameObject.SetActive(true);
                Role.transform.GetChild(1).gameObject.SetActive(false);
                Role.transform.GetChild(2).gameObject.SetActive(false);
                break;
        }
        temp = gameObject.transform.GetChild(1).gameObject;
        Exp_Left = temp.transform.Find("XP Bar").gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        Exp_Right = temp.transform.Find("XP Bar").gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
        //여기에 경험치 채우는 거 구현(근데 왜 2개로 나눠져있지??)
    }

    void UI_Update()
    {
        if (Hp == null || ps == null) return;

        Hp.fillAmount = Mathf.Lerp(Hp.fillAmount, ps.Hp/ps.MaxHp, 0.1f);
        Hp_text.text = ((ps.Hp/ps.MaxHp) * 100).ToString();
        Mp.fillAmount = Mathf.Lerp(Mp.fillAmount, ps.Mana/ps.MaxMana, 0.1f);
        Mp_text.text = ((ps.Mana/ps.MaxMana) * 100).ToString();
        Level.text = ps.Level.ToString();
        float rate = (ps.Exp - ps.current_exp)/(ps.next_level_up - ps.current_exp);
        if(rate <= 0.5)
        {
            Exp_Left.fillAmount = Mathf.Lerp(Exp_Left.fillAmount,rate * 2f ,0.1f);
            Exp_Right.fillAmount = 0;
        }
        else
        {
            Exp_Left.fillAmount = 1;
            Exp_Right.fillAmount = Mathf.Lerp(Exp_Right.fillAmount,(rate - 0.5f) * 2f, 0.1f);
        }
    }

    private void OnApplicationQuit()
    {
        // 꺼질 때에도 저장해야 함
        // 인벤토리 내용(변경사항) json 저장
        
        //uI_Inventory.Save();
    }

    void ControlPopUpUI()
    {
        // NPC와 상호작용중이면 X
        if (player == null) return;
        if (player.IsInteractWithNPC)
            return;

        // 키보드 입력 -> 팝업UI On/Off
        if (Input.GetKeyDown(KeyCode.I))
        {
            // 인벤토리 UI

            if(!uI_Inventory)
            {
                ui_Equipment = Managers.UI.ShowPopupUI<UI_Equipment>();
                uI_Inventory = Managers.UI.ShowPopupUI<UI_Inventory>();
            }
            else
            {
                //if (!uI_Inventory.IsPeek())
                //    return;
                
                // 인벤토리 내용(변경사항) json 저장
                uI_Inventory.Save(); // TODO

                //saveInven();
                Managers.UI.ClosePopupUI(uI_Inventory);
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
                //if (!setting.IsPeek())
                //    return;

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
        // 여기 있을게 아닌거 같은디.. -> Not Use

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

    // 코루틴 래퍼(Wrapper) 메소드
    // predicate 조건이 충족되지 않은 경우, 대기한다.
    private void ProcessLater(Func<bool> predicate, Action job)
    {
        StartCoroutine(ProcessLaterRoutine());

        // Local
        IEnumerator ProcessLaterRoutine()
        {
            yield return new WaitUntil(predicate);
            job?.Invoke();
        }
    }

}
