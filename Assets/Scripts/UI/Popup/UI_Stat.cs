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
    int SP;
    float STR;
    float DEX;
    float INT;
    float LUC;


    private void Start()
    {
        Init();
    }

    PlayerStat statInfo;
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        // TODO : 수정해야함
        statInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();

        // Stat Button, Text Dictionary
        for (int i = 0; i < (int)StatType.COUNT; i++)
        {
            string statTypeName = ((StatType)i).ToString();
            buttonDic.Add(statTypeName, GetButton(i));
            textDic.Add(statTypeName, GetText(i));
        }

        // SP Text Dictinary 추가
        textDic.Add("SP", GetText((int)Texts.SP_Value));

        GetButton((int)Buttons.ApplyButton).gameObject.BindEvent(ApplyButtonClicked);
        GetButton((int)Buttons.ResetButton).gameObject.BindEvent(ResetButtonClicked);


        // Stat 읽어오기
        UpdateStat(statInfo);
    }

    private void ApplyButtonClicked(PointerEventData data)
    {
        Debug.Log("Apply");
        //statInfo.Stat_Change();
        //UpdateStat(statInfo);
    }

    private void ResetButtonClicked(PointerEventData data)
    {
        Debug.Log("Reset");
    }

    private void UpdateStat(int sp, float s, float d, float i, float l)
    {
        SP  = sp;
        STR = s;
        DEX = d;
        INT = i;
        LUC = l;

        UpdateUI(SP, STR, DEX, INT, LUC);
    }

    private void UpdateStat(PlayerStat statInfo)
    {
        UpdateStat(statInfo.Stat_Point, statInfo.STR, statInfo.DEX, statInfo.INT, statInfo.LUC);
    }

    private void UpdateUI(int sp, float s, float d, float i, float l)
    {
        // Update Text
        textDic["SP"].text  = $"{sp}";
        textDic["STR"].text = $"{s}";
        textDic["DEX"].text = $"{d}";
        textDic["INT"].text = $"{i}";
        textDic["LUC"].text = $"{l}";
    }

    private void OnEnable()
    {
        if(statInfo)
            UpdateStat(statInfo);
    }
}

