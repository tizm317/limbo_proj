using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager
{
    public GameObject[] Npcs;
    int id = 0;


    public void Init(Define.Scene scene)
    {
        Npcs = GameObject.FindGameObjectsWithTag("NPC");
        Dictionary<int, Data.Npc> dict = Managers.Data.NpcDict;
        id = 0;

        if (scene == Define.Scene.InGameVillage)
        {
            // TODO : 빌리지로 바꿔야함
            foreach (GameObject npc in Npcs)
            {
                switch(npc.transform.parent.name)
                {
                    case "Valkyrie":
                        Util.GetOrAddComponent<MerchantNpc>(npc);
                        id = 0;
                        break;
                    case "Soldier1":
                        id = 1;
                        Util.GetOrAddComponent<Npc>(npc);
                        break;
                    case "Soldier2":
                        Util.GetOrAddComponent<Npc>(npc);
                        id = 2;
                        break;
                    case "Hejmdal":
                        Util.GetOrAddComponent<MapNpc>(npc);
                        id = 3;
                        break;
                    case "Geirröth":
                        Util.GetOrAddComponent<QuestNpc>(npc);
                        id = 4;
                        break;
                    case "Askr":
                        Util.GetOrAddComponent<QuestNpc>(npc);
                        id = 5;
                        break;
                    case "Embla":
                        Util.GetOrAddComponent<QuestNpc>(npc);
                        id = 6;
                        break;
                }

                //switch (dict[id].job)
                //{
                //    case "Npc":
                //        Util.GetOrAddComponent<Npc>(npc);
                //        break;
                //    case "MerchantNpc":
                //        Util.GetOrAddComponent<MerchantNpc>(npc);
                //        break;
                //    case "QuestNpc":
                //        Util.GetOrAddComponent<QuestNpc>(npc);
                //        break;
                //    case "EnchantNpc":
                //        Util.GetOrAddComponent<EnchantNpc>(npc);
                //        break;
                //    case "MapNpc":
                //        Util.GetOrAddComponent<MapNpc>(npc);
                //        break;
                //}
                npc.GetComponent<Npc>().Init(id);
            }
        }
        else
        {
            // 필드(숲/사막/묘지) 에는 '경비 대장' 한 명
            foreach (GameObject npc in Npcs)
            {
                Util.GetOrAddComponent<MapNpc>(npc);
                npc.GetComponent<MapNpc>().Init(id: 3);
            }
        }


        
    }

    public void Clear()
    {
        Npcs = null;
    }
}
