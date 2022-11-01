using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNpc : QuestNpc
{
    UI_SelectMap _UI_SelectMap;
    public override void Awake()
    {
        //Init();
    }
    public override void Init(int id)
    {
        // NPC Default Setting
        base.Init(id);

        // Plus Quest NPC Table Setting
        EventActionTable[] tempTable = new EventActionTable[10];
        for (int i = 0; i < table.Length; i++)
            tempTable[i] = table[i];

        tempTable[table.Length] = new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_MAP, null, Define.NpcState.STATE_MAP_UI_POPUP);
        tempTable[table.Length + 1] = new EventActionTable(Define.NpcState.STATE_MAP_UI_POPUP, Define.Event.EVENT_PUSH_MAP, null, Define.NpcState.STATE_IDLE); 
        tempTable[table.Length + 1] = new EventActionTable(Define.NpcState.STATE_MAP_UI_POPUP, Define.Event.EVENT_QUIT_MAP, null, Define.NpcState.STATE_NPC_UI_POPUP); 

        // 맵 관련
        tempTable[table.Length]._action -= ShowSelectMapUI;
        tempTable[table.Length]._action += ShowSelectMapUI;

        tempTable[table.Length + 1]._action -= CloseSelectMapUI;
        tempTable[table.Length + 1]._action += CloseSelectMapUI;

        table = new EventActionTable[10];
        table = (EventActionTable[])tempTable.Clone();

    }

    List<MapInfo> maps = new List<MapInfo>()
    {
        new MapInfo
        {
            Name = "Nature",
            scene = Define.Scene.InGameNature
        },
        new MapInfo
        {
            Name = "Desert",
            scene = Define.Scene.InGameDesert
        },
        new MapInfo
        {
            Name = "Cemetery",
            scene = Define.Scene.InGameCemetery
        },
        new MapInfo
        {
            Name = "Village",
            scene = Define.Scene.InGameVillage
        },
    };
    public void ShowSelectMapUI()
    {
        _UI_SelectMap = Managers.UI.ShowPopupUI<UI_SelectMap>();
        _UI_SelectMap.getNpcInfo(this);
        _UI_SelectMap.SetMaps(maps);
    }

    public void CloseSelectMapUI()
    {
        _UI_SelectMap.ClosePopupUI();
        _UI_SelectMap = null;
    }
}
