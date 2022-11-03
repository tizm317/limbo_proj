using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillDescription : UI_Popup
{

    enum GameObjects
    {
        DescriptionPanel,
    }
    
    enum Texts
    {
        NameText,
        TooltipText,
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

        Reset();
    }

    private void Reset()
    {
        GetText((int)Texts.NameText).text = "";
        GetText((int)Texts.TooltipText).text = "";
    }


    // Skill Tooltip
    public void setSkillTooltip(SkillData data, Vector3 mousePointerPos)
    {
        // TODO : 설명 넣어야함
        Reset();
        GetText((int)Texts.NameText).text += ("<color=#ff8000ff>" + data.Name + "</color>");
        GetText((int)Texts.TooltipText).text += data.Tooltip;

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
