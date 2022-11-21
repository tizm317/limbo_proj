using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Quest : UI_Popup
{
    ItemData[] ItemDatas;
    QuestNpc npc;
    Data.Quest quest;

    enum Texts
    {
        TitleText,
        DescriptionText,
    }

    enum GameObjects
    {
        ObjectivesGroup,
        ItemSlot1,
        ItemSlot2,
    }

    enum Buttons
    {
        CloseButton,
        AcceptButton,
    }

    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(endButtonClicked);
        GetButton((int)Buttons.AcceptButton).gameObject.BindEvent(acceptButtonClicked);


        ItemDatas = GameObject.Find("@Scene").GetComponent<Inventory>().itemDatas;
    }

    public void endButtonClicked(PointerEventData data)
    {
        npc.stateMachine(Define.Event.EVENT_REJECT_QUEST);
        quest.get = 0;
        //Managers.Data.QuestDict[quest.id] = quest;
    }

    public void acceptButtonClicked(PointerEventData data)
    {
        npc.stateMachine(Define.Event.EVENT_ACCEPT_QUEST);
        quest.get = 1;
        //Managers.Data.QuestDict[quest.id] = quest;
    }

    public void getNpcInfo(Npc clickedNpc)
    {
        // 대화창 UI 랑 대화하는 NPC 연결해주기 위함
        npc = clickedNpc as QuestNpc;
    }

    public void getQuest(int questId)
    {
        // 퀘스트 정보
        quest = null;
        Managers.Data.QuestDict.TryGetValue(questId, out quest);
        //Debug.Log(quest.get);

        if (quest.get == 1 || quest.clear == 1) return; // 이미 받거나 깬 경우
        
        GetText((int)Texts.TitleText).text = quest.name;
        GetText((int)Texts.DescriptionText).text = quest.description;


        // 수집해야하는 아이템 이름
        Data.ItemData targetitemData = null;
        Managers.Data.ItemDict.TryGetValue(quest.targetItemId, out targetitemData);

        GameObject objGroup =  GetObject((int)GameObjects.ObjectivesGroup);
        
        Text objText = objGroup.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        objText.text = targetitemData.name.ToString();

        // 수집해야하는 아이템 갯수
        Text currentCount =  objGroup.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        Text requiredCount =  objGroup.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Text>();
        currentCount.text = "0";
        requiredCount.text = quest.targetItemCount.ToString();

        // 보상
        foreach(ItemData data in ItemDatas)
        {
            if(quest.rewardItemId == data.ID)
            {
                GetObject((int)GameObjects.ItemSlot1).transform.GetChild(0).GetComponent<Image>().sprite = data.IconSprite;
                break;
            }
        }
    }
}
