using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDescription : UI_Popup
{

    enum GameObjects
    {
        DescriptionPanel,
       // RemoveButton,
       // UseButton,
    }
    
    enum Texts
    {
        NameText,
        TypeText,
        GradeText,
        PriceText,
        TooltipText,
        //RemoveText,
        //UseText,
    }

    private void Awake()
    {
        Init();   
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

        //GetObject((int)GameObjects.UseButton).BindEvent((PointerEventData) =>
        //{
        //    //TODO:
        //    ClosePopupUI();
        //});

        //GetObject((int)GameObjects.RemoveButton).BindEvent((PointerEventData) =>
        //{
        //    //TODO:
        //    ClosePopupUI();
        //});

        Reset();
    }

    private void Reset()
    {
        GetText((int)Texts.NameText).text = "";
        GetText((int)Texts.TypeText).text = "";
        GetText((int)Texts.GradeText).text = "";
        GetText((int)Texts.TooltipText).text = "";
        GetText((int)Texts.PriceText).text = "";

        //GetText((int)Texts.NameText).text = "아이템 : ";
        //GetText((int)Texts.TypeText).text = "종류 : ";
        //GetText((int)Texts.GradeText).text = "등급 : ";
        //GetText((int)Texts.TooltipText).text = "설명 : ";
    }


    public void setTooltip(ItemData data, Vector3 mousePointerPos)
    {

        Reset();
        GetText((int)Texts.NameText).text += ("<color=#ff8000ff>" + data.Name + "</color>");

        string itemType = data.GetType().ToString().Replace("ItemData", string.Empty);
        GetText((int)Texts.TypeText).text += itemType;

        GetText((int)Texts.GradeText).text += ("<color=#539047FF>" + data.Grade + "</color>");
        GetText((int)Texts.TooltipText).text += data.Tooltip;

        GetText((int)Texts.PriceText).text = ("<color=Red>" + data.Price + " G"+ "</color>");



        float panelWidth = ((RectTransform)(GetObject((int)GameObjects.DescriptionPanel).transform)).rect.width;
        float panelHeight = ((RectTransform)(GetObject((int)GameObjects.DescriptionPanel).transform)).rect.height;
        float xPosValue = panelWidth / 1.75f;
        float yPosValue = panelHeight / 1.75f;

        // 화면 범위 안 넘어가게 위치 조정
        if (mousePointerPos.x + panelWidth > Screen.width)
            mousePointerPos.x -= xPosValue;
        else
            mousePointerPos.x += xPosValue;

        if (mousePointerPos.y - panelHeight < 0)
            mousePointerPos.y += yPosValue;
        else
            mousePointerPos.y -= yPosValue;

        GetObject((int)GameObjects.DescriptionPanel).transform.position = mousePointerPos;
    }

    string _useText = "사용";
    public void setDescription(string name, string type, string grade, int count, Vector3 mousePointerPos, bool tooltip = true)
    {
        Reset();
        GetText((int)Texts.NameText).text += ("<color=#ff8000ff>" + name + "</color>");
        GetText((int)Texts.TypeText).text += type;
        GetText((int)Texts.GradeText).text += grade;
        //GetText((int)Texts.CountText).text += $"{count}";

        //GetObject((int)GameObjects.RemoveButton).SetActive(false);
        //GetObject((int)GameObjects.UseButton).SetActive(false);
        if (!tooltip)
        {
            switch (type)
            {
                case "equipment":
                    _useText = "장착";
                    break;
                case "consumable":
                case "etc":
                default:
                    _useText = "사용";
                    break;
            }
            //GetText((int)Texts.UseText).text = _useText;

            //GetObject((int)GameObjects.RemoveButton).SetActive(true);
            //GetObject((int)GameObjects.UseButton).SetActive(true);
        }

        // Panel's width , weight 이용해서 위치 조정하기 위한 값
        float panelWidth = ((RectTransform)(GetObject((int)GameObjects.DescriptionPanel).transform)).rect.width;
        float panelHeight = ((RectTransform)(GetObject((int)GameObjects.DescriptionPanel).transform)).rect.height;
        float xPosValue = panelWidth / 1.75f;
        float yPosValue = panelHeight / 1.75f;

        // 화면 범위 안 넘어가게 위치 조정
        if (mousePointerPos.x + panelWidth > Screen.width)
            mousePointerPos.x -= xPosValue;
        else
            mousePointerPos.x += xPosValue;

        if (mousePointerPos.y - panelHeight < 0)
            mousePointerPos.y += yPosValue;
        else
            mousePointerPos.y -= yPosValue;
        
        GetObject((int)GameObjects.DescriptionPanel).transform.position = mousePointerPos;
        
    }
}
