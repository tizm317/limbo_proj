using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDescription : UI_Popup
{
    enum GameObjects
    {
        DescriptionPanel,
    }
    
    enum Texts
    {
        NameText,
        TypeText,
        GradeText,
        CountText,
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

        GetText((int)Texts.NameText).text = "아이템 : ";
        GetText((int)Texts.TypeText).text = "종류 : ";
        GetText((int)Texts.GradeText).text = "등급 : ";
        GetText((int)Texts.CountText).text = "개수 : ";
    }

    
    public void setDescription(string name, string type, string grade, int count, Vector3 mousePointerPos)
    {
        GetText((int)Texts.NameText).text += name;
        GetText((int)Texts.TypeText).text += type;
        GetText((int)Texts.GradeText).text += grade;
        GetText((int)Texts.CountText).text += $"{count}";

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
