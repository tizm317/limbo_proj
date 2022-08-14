using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NpcName : UI_Base
{
    // over NPC head Name UI tag
    
    enum Texts
    {
        NameText,
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        // NPC 이름 초기화
        GetText((int)Texts.NameText).text = transform.parent.GetComponentInChildren<Npc>()._name;

        // rotation 초기화 : 카메라랑 맞춰줌
        transform.rotation = Camera.main.transform.rotation; 
    }


}
