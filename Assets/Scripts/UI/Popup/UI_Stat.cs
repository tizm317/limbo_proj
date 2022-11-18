using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Stat : UI_Base
{
    enum Texts
    {
        STR_Value,
        DEX_Value,
        INT_Value,
        LUC_Value,
     
        SP_Value,
    }

    enum Buttons
    {
        STR_UpButton,
        DEX_UpButton,
        INT_UpButton,
        LUC_UpButton,
        ApplyButton,
        ResetButton,
    }

    enum StatType
    {
        STR,
        DEX,
        INT,
        LUC,
        COUNT,

        SP,
    }

    Dictionary<string, Button> buttonDic = new Dictionary<string, Button>();
    Dictionary<string, Text> textDic = new Dictionary<string, Text>();

    // 최종 스텟이 아니고, 잠시 담아두기 위한 변수
    int SP;
    int STR;
    int DEX;
    int INT;
    int LUC;


    private void Start()
    {
        Init();
    }

    PlayerStat statInfo;
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        // TODO : 수정해야함 (서버에서 내 캐릭터로 가져와야 함)
        statInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();
        statInfo.ConnectStatUI(this);

        // Stat Button, Text Dictionary
        for (int i = 0; i < (int)StatType.COUNT; i++)
        {
            string statTypeName = ((StatType)i).ToString();
            buttonDic.Add(statTypeName, GetButton(i));
            textDic.Add(statTypeName, GetText(i));

            GetButton(i).gameObject.BindEvent(StatUpButtonClicked);
        }

        // SP Text Dictinary 추가
        textDic.Add("SP", GetText((int)Texts.SP_Value));

        GetButton((int)Buttons.ApplyButton).gameObject.BindEvent(ApplyButtonClicked);
        GetButton((int)Buttons.ResetButton).gameObject.BindEvent(ResetButtonClicked);


        // Stat 읽어오기
        UpdateStat(statInfo);
    }

    private void StatUpButtonClicked(PointerEventData data)
    {
        // 스킬 포인트가 없으면 아무일도 일어나지 않는다
        if (SP <= 0) return;

        // 스킬 포인트 사용
        SP--;

        // 선택된 스텟 증가
        string statName = data.selectedObject.name.Substring(0, 3);
        switch(statName)
        {
            case "STR":
                STR++;
                break;
            case "DEX":
                DEX++;
                break;
            case "INT":
                INT++;
                break;
            case "LUC":
                LUC++;
                break;
            default:
                Debug.LogError("Something Wrong");
                break;
        }

        // UI 업데이트
        UpdateUI(SP, STR, DEX, INT, LUC);

        //Debug.Log(data.selectedObject.name);
    }

    private void ApplyButtonClicked(PointerEventData data)
    {
        Debug.Log("Apply");
        
        // 최종 Apply 버튼이 눌려야 실제로 반영이 된다.
        statInfo.Stat_Change(SP, STR, DEX, INT, LUC);
        
        // 최종 반영된 스텟을 가져와서 다시 업데이트 시켜준다
        UpdateStat(statInfo);
    }

    private void ResetButtonClicked(PointerEventData data)
    {
        Debug.Log("Reset");

        // 최종 반영되지 않은 데이터로 덮어서 전으로 돌린다
        UpdateStat(statInfo);
    }

    private void UpdateStat(int sp, int s, int d, int i, int l)
    {
        // 값 업데이트
        SP  = sp;
        STR = s;
        DEX = d;
        INT = i;
        LUC = l;

        // UI 업데이트
        UpdateUI(SP, STR, DEX, INT, LUC);
    }

    public void UpdateStat(PlayerStat statInfo)
    {
        // PlayerStat 바로 사용하기 위해서 만든 Wrapper 함수
        UpdateStat(statInfo.Stat_Point, statInfo.STR, statInfo.DEX, statInfo.INT, statInfo.LUC);
    }

    private void UpdateUI(int sp, int s, int d, int i, int l)
    {
        // Update Text UI
        textDic["SP"].text  = $"{sp}";
        textDic["STR"].text = $"{s}";
        textDic["DEX"].text = $"{d}";
        textDic["INT"].text = $"{i}";
        textDic["LUC"].text = $"{l}";
    }

    private void OnEnable()
    {
        // 스텟창 켜질 때 갱신
        if(statInfo)
            UpdateStat(statInfo);
    }
}

