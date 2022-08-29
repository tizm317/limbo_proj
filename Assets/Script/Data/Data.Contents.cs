using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ( ��� �ҷ��о������) ��Ÿ���� �κ� 
// Contents�� Data (AI �� ����)

namespace Data
{
    #region Stat
    [Serializable] // �޸𸮿��� ��� �ִ°� ���Ϸ� ��ȯ�Ҽ��ִٴ� �ǹ�
    public class Stat
    {
        // json ���Ͽ����� �̸��ϰ� �� ���Ͽ����� �̸� ������� (�ٸ��� �� ã��)
        // Ÿ�Ե� �������
        // public �����̿��� �о����, �ƴϸ� [SerializedField]
        public int level;
        public int hp;
        public int attack;

        // ���˸� ����ΰ� �ѹ��� �ҷ���
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        // ILoader �������̽� ����

        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            //data.stats.ToDictionary(); // Linq ��� ��� ���µ� IOS�ʿ��� ���� ���Ƽ� �� �Ⱦ�
            // �׳� �������� foreach ������ �־���
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
        // ILoader �������̽� ����

        // �̸� json ���� �ȿ� �ִ� �̸��ϰ� �������
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
    // (�߰�)

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
    [Serializable] // �޸𸮿��� ��� �ִ°� ���Ϸ� ��ȯ�Ҽ��ִٴ� �ǹ�
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

    // ������� �ٽ�
    #region Inventory
    [Serializable] // �޸𸮿��� ��� �ִ°� ���Ϸ� ��ȯ�Ҽ��ִٴ� �ǹ�
    public class Item
    {
        public int id;
        public string name;
        public string type;     // ������ ���� ����
        public string grade;    // ��ͼ�
        public int count;       // � ������ �ִ���
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
    //    Tuple<string, int> compositeKey; // playerId, itemID ����Ű

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
        // ���� player ID �� �κ��丮�� list�� ����� ����

        public List<Inventory> inventories = new List<Inventory>();
        public Dictionary<string, List<Inventory>> MakeDict()
        {
            Dictionary<string, List<Inventory>> dict2 = new Dictionary<string, List<Inventory>>();

            string playerId = null;
            List<Inventory> items = null;
            foreach (Inventory inven in inventories)
            {
                // ���� ID �� ������ ����Ʈ ����� ����
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
        // �ϴ��� ������ Ŭ������ ���ļ� Item2��� ���̹��ص� (�������� �κ��丮���̿��� ��ü������ �����ʿ�)
        // item ȿ�� �� �ϴ� ����
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
