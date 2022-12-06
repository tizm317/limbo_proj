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

            /////////////////////////////////////////////////////////////////////////////////////////////
            // 해상도 조절
            Button serverBtn = serverItem.transform.GetComponentInChildren<Button>();
            float w = serverBtn.GetComponent<RectTransform>().rect.width;
            float h = serverBtn.GetComponent<RectTransform>().rect.height;
            // 기준 해상도 2560x1440
            w /= 2560;
            w *= Screen.width; // 설정된 해상도
            //Screen.currentResolution.width; // 내 기기 해상도
            h /= 1440;
            h *= Screen.height;
            //Screen.currentResolution.height;
            serverBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
            /////////////////////////////////////////////////////////////////////////////////////////////

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
