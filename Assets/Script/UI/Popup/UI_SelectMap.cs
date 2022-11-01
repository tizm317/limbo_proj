using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectMap : UI_Popup
{
    public List<UI_SelectMapItem> Items { get; } = new List<UI_SelectMapItem>();

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

            GameObject go = Managers.UI.MakeSubItem<UI_SelectMapItem>(parent: layout.transform).gameObject;
            UI_SelectMapItem mapItem = go.GetOrAddComponent<UI_SelectMapItem>();
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
