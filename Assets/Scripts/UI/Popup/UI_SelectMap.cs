using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectMap : UI_Popup
{
    public List<MapButton> Items { get; } = new List<MapButton>();

    enum Buttons
    {
        ButtonClose
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.ButtonClose).gameObject.BindEvent(closeButtonClicked);
    }

    Npc npc;

    public void getNpcInfo(Npc clickedNpc)
    {
        // 대화창 UI 랑 대화하는 NPC 연결해주기 위함
        npc = clickedNpc;
    }

    public void closeButtonClicked(PointerEventData data)
    {
        npc.stateMachine(Define.Event.EVENT_QUIT_MAP);
    }

    public void SetMaps(List<MapInfo> maps)
    {
        Items.Clear();

        GameObject layout = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        foreach (Transform child in layout.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].scene == Managers.Scene.CurrentScene.SceneType)
                continue;

            GameObject go = Managers.UI.MakeSubItem<MapButton>(parent: layout.transform).gameObject;
            MapButton mapItem = go.GetOrAddComponent<MapButton>();


            /////////////////////////////////////////////////////////////////////////////////////////////
            // 해상도 조절
            //Button mapBtn = mapItem.transform.GetChild(0).GetChild(0).GetComponent<Button>();
            Button mapBtn = mapItem.transform.GetComponent<Button>();

            float w = mapBtn.GetComponent<RectTransform>().rect.width;
            float h = mapBtn.GetComponent<RectTransform>().rect.height;
            // 기준 해상도 2560x1440
            w /= 2560; 
            w *= Screen.width; // 설정된 해상도
            //Screen.currentResolution.width; // 내 기기 해상도
            h /= 1440;
            h *= Screen.height;
            //Screen.currentResolution.height;
            mapBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
            /////////////////////////////////////////////////////////////////////////////////////////////

            Items.Add(mapItem);

            mapItem.Info = maps[i];
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (Items.Count == 0) return;

        foreach (var item in Items)
        {
            item.RefreshUI();
        }
    }
}
