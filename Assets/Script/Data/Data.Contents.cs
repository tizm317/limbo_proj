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
}
