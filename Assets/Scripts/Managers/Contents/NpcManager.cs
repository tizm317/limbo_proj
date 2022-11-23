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
            for(int i = 0; i < Npcs.Length; i++)
            {
                switch(i)
                {
                    case 0:
                        Util.GetOrAddComponent<MerchantNpc>(Npcs[i]);
                        break;
                    case 1:
                        Util.GetOrAddComponent<Npc>(Npcs[i]);
                        break;
                    case 2:
                        Util.GetOrAddComponent<Npc>(Npcs[i]);
                        break;
                    case 3:
                        Util.GetOrAddComponent<MapNpc>(Npcs[i]);
                        break;
                    case 4:
                        Util.GetOrAddComponent<QuestNpc>(Npcs[i]);
                        break;
                    case 5:
                        Util.GetOrAddComponent<QuestNpc>(Npcs[i]);
                        break;
                    case 6:
                        Util.GetOrAddComponent<QuestNpc>(Npcs[i]);
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
                
                id = i;
                Npcs[i].GetComponent<Npc>().Init(id);
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
