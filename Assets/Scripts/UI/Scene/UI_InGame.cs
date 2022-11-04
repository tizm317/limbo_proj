using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InGame : UI_Scene
{
    // 수정(추가)해야 함

    // 연관된 팝업 UI 목록
    //UI_Inven ui_Inven;
    UI_Inventory uI_Inventory;
    UI_Equipment ui_Equipment;
    UI_MiniMap miniMap;
    UI_Setting setting;
    PlayerMgr pm;
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

    //Player_Controller player;
    //Player_State player;
    Player player;
    PlayerStat ps;

    #region UI업데이트
    GameObject Role;
    Image Hp, Mp,Exp_Left, Exp_Right;
    Text Level, Hp_text, Mp_text;
    #endregion
    enum Buttons
    {
        //UI_SkillUpButton_P,
        UI_SkillUpButton_Q,
        UI_SkillUpButton_W,
        UI_SkillUpButton_E,
        UI_SkillUpButton_R,
    }
    enum Texts
    {
        Q_LvText,
        W_LvText,
        E_LvText,
        R_LvText,
        LevelText,
    }

    enum GameObjects
    {
        NotificationLevel,
        LevelUpEffect,
    }

    private void Start()
    {
        Init();
    }

    Coroutine coroutine;
    bool isPopup = false;           // 전체 체크 
    bool[] isPopups = new bool[4];  // 올라왔는지 따로 체크

    private void Update()
    {
        // Canvas Render Camera Setting (어디서 바뀌어서 오는거지..?)
        if (this.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
        {
            this.GetComponent<Canvas>().worldCamera = Camera.main;
            this.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        }

        UI_Update();

        _ped.position = Input.mousePosition;
        OnPointerEnter(_ped.position);
        OnPointerExit(_ped.position);

        if(ps.Skill_Point > 0 && isPopup == false)
        {
            if(coroutine == null)
                coroutine = StartCoroutine(CoSkillPointUpUIPopup());
        }
    }

    List<GameObject> UI_SkillUpButtonList = new List<GameObject>();
    public override void Init()
    {
        base.Init();


        pm = GameObject.Find("@Scene").GetComponent<PlayerMgr>();
        // 연결
        Managers.Input.KeyAction -= ControlPopUpUI;
        Managers.Input.KeyAction += ControlPopUpUI;

        //player = GameObject.Find("@Scene").GetComponent<Player_Controller>();
        //player = GameObject.Find("Player").GetComponent<Player_State>();
        player = GameObject.Find("@Scene").GetComponent<Player>();
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

        emoticon = GameObject.Find("@UI_Root").GetComponentInChildren<UI_Emoticon>();
        

        Bind<Button>(typeof(Buttons));
        //UI_SkillUpButtonList.Add(GetButton((int)Buttons.UI_SkillUpButton_P).gameObject);
        UI_SkillUpButtonList.Add(GetButton((int)Buttons.UI_SkillUpButton_Q).gameObject);
        UI_SkillUpButtonList.Add(GetButton((int)Buttons.UI_SkillUpButton_W).gameObject);
        UI_SkillUpButtonList.Add(GetButton((int)Buttons.UI_SkillUpButton_E).gameObject);
        UI_SkillUpButtonList.Add(GetButton((int)Buttons.UI_SkillUpButton_R).gameObject);
        foreach(GameObject go in UI_SkillUpButtonList)
        {
            go.BindEvent(skillUpButtonClicked);
        }

        //
        _gr = Util.GetOrAddComponent<GraphicRaycaster>(this.gameObject);
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

        // skill level texts
        Bind<Text>(typeof(Texts));
        // 초기화
        for(int i = 0; i < 4;i++)
        {
            GetText(i).text = $"{player.skill_level[i]}";
        }


        // Canvas Render Camera Setting
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        this.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;

        //
        Bind<GameObject>(typeof(GameObjects));

        UI_Init();

        // Level Up Popup Set Active False
        GetObject((int)GameObjects.NotificationLevel).SetActive(false);
    }

    public IEnumerator CoSkillPointUpUIPopup(bool goDown = false)
    {
        float percent = 0;
        float start= 0;
        float dest = 0;
        if (goDown == true)
        {
            start = 232;
            dest = start - 128;
        }
        else
        {
            start = 104;
            dest = start + 128;
        }

        while (percent < 1)
        {
            percent += Time.deltaTime;
            float value = Mathf.Lerp(start, dest, percent);

            // Q, W, E
            for (int i = 0; i < 3; i++)
            {
                // 만렙이면 안 올라옴(내려가는 경우는 무조건 내려가야하므로 조건으로 추가)
                if (isPopups[i] == false && player.skill_level[i] == 4)
                {
                    continue;
                }

                UI_SkillUpButtonList[i].transform.position = new Vector3(UI_SkillUpButtonList[i].transform.position.x, value, UI_SkillUpButtonList[i].transform.position.z);
            }

            // R(궁극기)            
            // 특정 레벨 이후 혹은 내려가는 경우에만 움직임
            if (isPopups[3] == true || (player.skill_level[3] != 4 && player.skill_level[3] < ps.Level / 8))
            {
                UI_SkillUpButtonList[3].transform.position = new Vector3(UI_SkillUpButtonList[3].transform.position.x, value, UI_SkillUpButtonList[3].transform.position.z);
                //isPopup_R = true;
            }
            yield return null;
        }

        // 이동 완료 후
        // 내려간 경우는 isPopup : false , RaycastTarget : Off
        // 올라간 경우는 isPopup : true , RaycastTarget : On
        if (goDown) isPopup = false;
        else        isPopup = true;
        for (int i = 0; i < 4; i++)
        {
            if (UI_SkillUpButtonList[i].transform.position.y > 150)
                isPopups[i] = true;
            else
                isPopups[i] = false;

            //if (i == 3 && player.skill_level[i] >= ps.Level / 8) continue;

            UI_SkillUpButtonList[i].GetComponent<Image>().raycastTarget = isPopups[i];
        }

        StopCoroutine(coroutine);
        coroutine = null;
    }

    private void skillUpButtonClicked(PointerEventData obj)
    {
        if (isPopup == false) return;
        //if (obj.pointerEnter.GetComponent<Button>().interactable == false) return;

        coroutine = StartCoroutine(CoSkillPointUpUIPopup(true));

        int idx = 0;
        foreach(GameObject go in UI_SkillUpButtonList)
        {
            if(go == obj.pointerPress.gameObject)
                break;
            idx++;
        }
        if (player.skill_level[idx] < 4)
        {
            player.skill_level[idx]++;
            GetText(idx).text = $"{player.skill_level[idx]}";
        }
        else
            Debug.Log("MAX SKILL LEVEL");

        isPopup = false;

        ps.Skill_Point--;
    }

    private void skillUpButtonClicked(GameObject clickedGO)
    {
        if (isPopup == false) return;

        coroutine = StartCoroutine(CoSkillPointUpUIPopup(true));

        int idx = 0;
        foreach (GameObject go in UI_SkillUpButtonList)
        {
            if (go == clickedGO)
                break;
            idx++;
        }
        if (player.skill_level[idx] < 4)
        {
            player.skill_level[idx]++;
            GetText(idx).text = $"{player.skill_level[idx]}";
        }
        else
            Debug.Log("MAX SKILL LEVEL");

        isPopup = false;

        ps.Skill_Point--;
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

        // LEVEL UP Effect
        if(ps.level_up == true)
        {
            StartCoroutine(CoLevelUpPopup());
        }
    }

    IEnumerator CoLevelUpPopup()
    {
        GetObject((int)GameObjects.NotificationLevel).SetActive(true);
        GetText((int)Texts.LevelText).text = $"LEVEL {Level.text}";
        GetObject((int)GameObjects.LevelUpEffect).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(10.0f);
        
        GetObject((int)GameObjects.NotificationLevel).SetActive(false);
        StopCoroutine(CoLevelUpPopup());
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
        if (player.IsInteractWithNPC)
            return;

        // 키보드 입력 -> 팝업UI On/Off
        if (Input.GetKeyDown(KeyCode.I))
        {
            // 인벤토리 UI

            if(!uI_Inventory)
            {
                ui_Equipment = Managers.UI.ShowPopupUI<UI_Equipment>();
                //ui_Inven = Managers.UI.ShowPopupUI<UI_Inven>();
                uI_Inventory = Managers.UI.ShowPopupUI<UI_Inventory>();
            }
            else
            {
                //if (!ui_Inven.IsPeek())
                //    return;
                if (!uI_Inventory.IsPeek())
                    return;
                
                // 인벤토리 내용(변경사항) json 저장
                uI_Inventory.Save();

                //saveInven();
                //Managers.UI.ClosePopupUI(ui_Inven);
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
                // 인벤토리 내용(변경사항) json 저장(?)
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

        // 스킬 레벨업 단축키 (좌컨트롤 + QWER)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                skillUpButtonClicked(UI_SkillUpButtonList[0]);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
            {
                skillUpButtonClicked(UI_SkillUpButtonList[1]);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
            {
                skillUpButtonClicked(UI_SkillUpButtonList[2]);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                skillUpButtonClicked(UI_SkillUpButtonList[3]);
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

    public IEnumerator coKeyUpCheck()
    {
        while (true)
        {
            if (Input.GetKeyUp(key_action))
            {
                int selected = radialMenu.Hide();
                //Debug.Log($"Selected : {selected}");

                // 감정표현
                player.emotion(selected);
                StopCoroutine(co);
            }

            if (Input.GetKeyUp(key_emoticon))
            {
                int selected = radialMenu.Hide();
                //Debug.Log($"Selected : {selected}");

                // 이모티콘
                emoticon.transform.GetChild(0).gameObject.SetActive(true);
                emoticon.setEmoticon(selected);
                StopCoroutine(co);
            }

            yield return null;
        }
    }


    // Skill Slot Tooltip

    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);
        if (_rrList.Count == 0) return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }

    UI_SkillDescription ui_tooltip;
    UI_ItemSlot itemSlot_tooltip;
    private void OnPointerEnter(Vector2 pointer)
    {
        // 중복 수행 방지
        pm.ToolTip();
        if (itemSlot_tooltip == RaycastAndGetFirstComponent<UI_ItemSlot>())
            return;

        itemSlot_tooltip = RaycastAndGetFirstComponent<UI_ItemSlot>();
        if (itemSlot_tooltip == null) return;
        Transform skillTr = itemSlot_tooltip.transform;
        if (itemSlot_tooltip != null)
        {
            if (!ui_tooltip)
            {
                ui_tooltip = Managers.UI.ShowPopupUI<UI_SkillDescription>();
            }

            int idx = skillTr.GetSiblingIndex();
            ui_tooltip.setSkillTooltip(player.skillDatas[idx], pointer);
        }
    }

    private void OnPointerExit(Vector2 pointer)
    {
        UI_ItemSlot itemSlot = RaycastAndGetFirstComponent<UI_ItemSlot>();
        if (itemSlot == null && ui_tooltip)
            ui_tooltip.ClosePopupUI();
    }

}
