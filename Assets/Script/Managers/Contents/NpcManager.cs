using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager
{
    public GameObject[] Npcs;
    int id = 0;


    public void Init()
    {
        Npcs = GameObject.FindGameObjectsWithTag("NPC");

        Dictionary<int, Data.Npc> dict = Managers.Data.NpcDict;

        foreach (GameObject npc in Npcs)
        {
            switch (dict[id++].job)
            {
                case "Npc":
                    Util.GetOrAddComponent<Npc>(npc);
                    break;
                case "MerchantNpc":
                    Util.GetOrAddComponent<MerchantNpc>(npc);
                    break;
                case "QuestNpc":
                    Util.GetOrAddComponent<QuestNpc>(npc);
                    break;
                case "EnchantNpc":
                    Util.GetOrAddComponent<EnchantNpc>(npc);
                    break;
                case "MapNpc":
                    Util.GetOrAddComponent<MapNpc>(npc);
                    break;
            }
        }
    }

    public void Clear()
    {
        Npcs = null;
    }
}
