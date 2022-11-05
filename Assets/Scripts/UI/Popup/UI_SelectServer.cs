using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectServer : UI_Popup
{
    public List<UI_SelectServerItem> Items { get; } = new List<UI_SelectServerItem>();

    private void Start()
    {
        Init();   
    }

    public override void Init()
    {
        base.Init();
    }


    public void SetServers(List<ServerInfo> servers)
    {
        Items.Clear();

        GameObject layout = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        foreach (Transform child in layout.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < servers.Count; i++)
        {
            GameObject go = Managers.UI.MakeSubItem<UI_SelectServerItem>(parent: layout.transform).gameObject;
            UI_SelectServerItem serverItem = go.GetOrAddComponent<UI_SelectServerItem>();
            Items.Add(serverItem);

            serverItem.Info = servers[i];
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (Items.Count == 0) return;

        foreach(var item in Items)
        {
            item.RefreshUI();
        }
    }
}
