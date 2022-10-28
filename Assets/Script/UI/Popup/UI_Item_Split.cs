using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item_Split : UI_Popup
{
    enum GameObjects
    {
        Panel,
        InputField,
    }
    enum Texts
    {
        CautionText,
        YesText,
        NoText,
        AmountText,
    }
    enum Buttons
    {
        YesButton,
        NoButton,
    }

    // event
    public delegate void OnClicked();
    public event OnClicked buttonClicked;
    bool split; // Y/N button
    int splitAmount = 0;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (inputFieldHelper.CheckFocus() == true)
            YesButtonClick(null);
    }

    InputFieldHelper inputFieldHelper;
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.YesButton).gameObject.BindEvent(YesButtonClick, Define.UIEvent.Click);
        GetButton((int)Buttons.NoButton).gameObject.BindEvent(NoButtonClick, Define.UIEvent.Click);

        //split = false;
        splitAmount = 0;

        inputFieldHelper = new InputFieldHelper();
        inputFieldHelper.Add(GetObject((int)GameObjects.InputField).GetComponent<InputField>());
        inputFieldHelper.SetFocus(0);
    }

    private void YesButtonClick(PointerEventData data)
    {
        //split = true;
        splitAmount = 0;
        try
        {
            splitAmount = int.Parse(GetText((int)Texts.AmountText).text);
        }
        catch(Exception e)
        {
            // 숫자 외 입력 무시
            Debug.Log(e);
        }
        buttonClicked();
    }
    private void NoButtonClick(PointerEventData data)
    {
        //split = false;
        splitAmount = 0;
        buttonClicked();
    }

    public int SplitItems()
    {
        // UI_Inven_Item 에서 어떤 버튼(Y/N) 눌렸는지 확인용
        //return split;
        return splitAmount;
    }
}
