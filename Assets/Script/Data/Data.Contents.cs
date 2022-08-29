using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파일 포맷 ( 어떻게 불러읽어들일지) 나타내는 부분 
// Contents용 Data (AI 용 따로)

namespace Data
{
    #region Stat
    [Serializable] // 메모리에서 들고 있는걸 파일로 변환할수있다는 의미
    public class Stat
    {
        // json 파일에서의 이름하고 이 파일에서의 이름 맞춰야함 (다르면 못 찾음)
        // 타입도 맞춰야함
        // public 변수이여야 읽어들임, 아니면 [SerializedField]
        public int level;
        public int hp;
        public int attack;

        // 포맷만 맞춰두고 한번에 불러옴
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        // ILoader 인터페이스 포함

        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            //data.stats.ToDictionary(); // Linq 라는 기능 쓰는데 IOS쪽에서 버그 많아서 잘 안씀
            // 그냥 수동으로 foreach 문으로 넣어줌
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);

            return dict;
        }
    }
    #endregion
    #region Map
    [Serializable]
    public class Map
    {
        public int code;
        public string name;
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class MapData : ILoader<int, Map>
    {
        // ILoader 인터페이스 포함

        // 이름 json 파일 안에 있는 이름하고 맞춰야함
        public List<Map> map = new List<Map>();

        public Dictionary<int, Map> MakeDict()
        {
            Dictionary<int, Map> dict = new Dictionary<int, Map>();

            foreach (Map ele in map)
                dict.Add(ele.code, ele);

            return dict;
        }
    }
    #endregion
    // (추가)

    #region NPC
    [Serializable]
    public class Npc
    {
        public int id;
        public string name;
        public string job;
    }

    [Serializable]
    public class NpcData : ILoader<int, Npc>
    {
        public List<Npc> npcs = new List<Npc>();

        public Dictionary<int, Npc> MakeDict()
        {
            Dictionary<int, Npc> dict = new Dictionary<int, Npc>();

            foreach (Npc ele in npcs)
                dict.Add(ele.id, ele);

            return dict;
        }
    }
    #endregion

    #region Dialog
    [Serializable] // 메모리에서 들고 있는걸 파일로 변환할수있다는 의미
    public class Dialog
    {
        public int lineNum;
        public string name;
        public string script;
    }

    [Serializable]
    public class DialogData : ILoader<int, Dialog>
    {

        public List<Dialog> dialogs = new List<Dialog>();

        public Dictionary<int, Dialog> MakeDict()
        {
            Dictionary<int, Dialog> dict = new Dictionary<int, Dialog>();
            foreach (Dialog dialog in dialogs)
                dict.Add(dialog.lineNum, dialog);

            return dict;
        }
    }
    #endregion

    // 여기부터 다시
    #region Inventory
    [Serializable] // 메모리에서 들고 있는걸 파일로 변환할수있다는 의미
    public class Item
    {
        public int id;
        public string name;
        public string type;     // 아이템 종류 구분
        public string grade;    // 희귀성
        public int count;       // 몇개 가지고 있는지
    }

    [Serializable]
    public class ItemData : ILoader<int, Item>
    {
        public List<Item> items = new List<Item>();

        public Dictionary<int, Item> MakeDict()
        {
            Dictionary<int, Item> dict = new Dictionary<int, Item>();

            foreach (Item item in items)
                dict.Add(item.id, item);

            return dict;
        }
    }
    #endregion

    #region Inventory2
    [Serializable] 
    public class Inventory
    {
        public string playerId;
        public int itemID;
        public int itemCount;
    }

    //[Serializable]
    //public class InventoryData : ILoader<Tuple<string, int>, Inventory>
    //{
    //    public List<Inventory> inventories = new List<Inventory>();
    //    Tuple<string, int> compositeKey; // playerId, itemID 복합키

    //    public Dictionary<Tuple<string, int>, Inventory> MakeDict()
    //    {
    //        Dictionary<Tuple<string, int>, Inventory> dict = new Dictionary<Tuple<string, int>, Inventory>();

    //        foreach (Inventory inven in inventories)
    //        {
    //            compositeKey = Tuple.Create(inven.playerId, inven.itemID);
    //            dict.Add(compositeKey, inven);
    //        }

    //        return dict;
    //    }
    //}

    [Serializable]
    public class InventoryData : ILoader<string, List<Inventory>>
    {
        // 같은 player ID 의 인벤토리를 list로 만들기 위함

        public List<Inventory> inventories = new List<Inventory>();
        public Dictionary<string, List<Inventory>> MakeDict()
        {
            Dictionary<string, List<Inventory>> dict2 = new Dictionary<string, List<Inventory>>();

            string playerId = null;
            List<Inventory> items = null;
            foreach (Inventory inven in inventories)
            {
                // 같은 ID 의 아이템 리스트 만들기 위함
                if (playerId != inven.playerId)
                    items = new List<Inventory>();
                items.Add(inven);
                //dict2.Add(inven.playerId, items);
                dict2[inven.playerId] = items;
                
                playerId = inven.playerId;
            }

            return dict2;
        }
    }
    #endregion


    #region ItemTable
    [Serializable]
    public class Item2
    {
        // 일단은 아이템 클래스가 겹쳐서 Item2라고 네이밍해둠 (위에꺼는 인벤토리용이여서 전체적으로 수정필요)
        // item 효과 는 일단 빼둠
        public int itemId;
        public string itemName;
        public string itemType;
        public string itemGrade;
    }

    [Serializable]
    public class ItemTable : ILoader<int, Item2>
    {
        public List<Item2> itemTable = new List<Item2>();

        public Dictionary<int, Item2> MakeDict()
        {
            Dictionary<int, Item2> dict = new Dictionary<int, Item2>();

            foreach (Item2 it in itemTable)
                dict.Add(it.itemId, it);

            return dict;
        }
    }
    #endregion
}
