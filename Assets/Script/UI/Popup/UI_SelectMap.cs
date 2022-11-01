using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectMap : UI_Popup
{
    public List<UI_SelectMapItem> Items { get; } = new List<UI_SelectMapItem>();

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
    }


    public void SetMaps(List<MapInfo> maps)
    {
        Items.Clear();

        GameObject layout = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        foreach (Transform child in layout.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < maps.Count; i++)
        {
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
