using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // FILE

// ���� ���� �о���̴� �κ�

public interface ILoader<Key, Value>
{
    // ����ϰ� �����ϱ� ���� �������̽� ����
    // �� �������̽� ������ �ִ� Ŭ������ Dictionary<K, V> return �ϴ� MakeDict() �����ؾ� ��
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    // ���ӿ� �����ϴ� ��� ��ġ �����ϴ� �Ŵ���

    // ex : ������ ����, ������ ����ġ, ���� ai �� ������ etc
    // �ϵ� �ڵ��ϸ�, ��ġ ��ĥ �� ã�Ƽ� �����ؾ���
    // �������Ͽ� ������, ������Ʈ�Ϸ��� ������ �ٽ� �ؾ���

    // ��� ��ġ ���� -> json ���Ϸ� ��� ����
    // �� ������� ���ϸ� �ٲٸ� �ٷ� �����.
    // ����, Ŭ�� ����ȭ�� ����

    // DataManager�� ��ųʸ��� �������
    // ��ųʸ��� ��� �ִ°� ȿ���� - (����Ʈ ã�� ���� �� ��ȸ�ؾ���.)
    // �ܺο��� ����� �� : Dictionary �̾Ƽ� ��� (���� : Dictionary<int, Stat> dict = Managers.Data.StatDict;)
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>(); // Ű ���� ������
    //(�߰� �ϴ� �κ�)
    //public Dictionary<int, Data.Pos> PosDict { get; private set; } = new Dictionary<int, Data.Pos>();
    public Dictionary<int, Data.Map> MapDict { get; private set; } = new Dictionary<int, Data.Map>();
    public Dictionary<int, Data.Item> InvenDict { get; private set; } = new Dictionary<int, Data.Item>();

    public void Init()
    {
        

        // json ���� �о��
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        InvenDict = LoadJson<Data.ItemData, int, Data.Item>("InvenData").MakeDict();
        // (�߰� �ϴ� �κ�)

        // path ���� MapData ���� ���ϸ� json ���Ϸ� ���� ��,
        //string path = "D:/Unity/limbo_proj/Assets/Resources/Data/MapData.json"; // ��� ���� �ʿ�

        // �ؿ��� �ΰ��Ӿ� �ʵ�����
        if (Managers.Scene.CurrentScene.SceneType != Define.Scene.InGame)
            return;

        string path = "Data/MapData.json"; // InGame �� �� ������ ����

        if (!File.Exists(path))
        {
            // �� �ȿ� �ִ� ������Ʈ ���� ī��Ʈ
            int objCount = 0;

            string json = "";

            // �� ������Ʈ ��� �θ� ������Ʈ
            GameObject prop = GameObject.Find("prop");


            // �� ������Ʈ(child) ��ȸ
            foreach (Transform child in prop.transform)
            {
                // Layer �� Building �ƴϸ� continue (���߿� ���� �ٲ� �� ���� - �߿� �ǹ��� �Ѵٴ���)
                if (child.gameObject.layer != (int)Define.Layer.Building)
                    continue;

                // dictionary�� �ٷ� �ֱ�
                {
                    Data.Map obj = new Data.Map();

                    obj.code = objCount;
                    obj.name = child.name;
                    obj.x = child.position.x;
                    obj.y = child.position.y;
                    obj.z = child.position.z;

                    MapDict.Add(objCount, obj);
                }

                // MakeList() ���� List ���� ��ȯ
                json = MakeList(child.gameObject, objCount);
                objCount++;
            }
            // List �������� json�� ����� ä�� ����

            // json���� ����
            SaveJson(json);

            // �̴ϸ� ų ��, Map Dictionary �������
            // UI_InGame���� MakeMapDict() ȣ��
        }
        else
        {
            // Map Dictionary ����
            MapDict = LoadJson<Data.MapData, int, Data.Map>("MapData").MakeDict();
        }

    }

    public void MakeMapDict()
    {
        // ó���� json ������ ��� save �ϰ� �ٷ� load ���ؼ� �̴ϸ� ų �� load�ϵ���
        MapDict = LoadJson<Data.MapData, int, Data.Map>("MapData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        // json ���� �о���� �Լ�
        // �߰��� �� ���� �ǵ� �ʿ� ����

        // ���� ���� ����� �� : json vs xml
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");


        // ����Ƽ json �Ľ� ����
        // FromJson : json -> class�� <-> ToJson
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    // ���� ��� (MakeList , Savejson)

    [System.Serializable]
    public class ObjContainer
    {
        public int code = 0;
        public string name;
        public float x;
        public float y;
        public float z;
    }

    public class SaveData
    {
        // List ����� ���ؼ�
        public List<ObjContainer> map = new List<ObjContainer>();
    }
    SaveData saveData = new SaveData();

    public class SaveData2
    {
        // List ����� ���ؼ�
        public List<itemContainer> items = new List<itemContainer>();
    }

    SaveData2 saveData2 = new SaveData2();
    public void resetSaveData2()
    {
        // ���� ��������ؼ�?
        saveData2.items.Clear();
    }

    // ����Ʈ ����� �κ�(MakeList)�̶� �����ϴ� �κ�(SaveJson) ����
    public string MakeList(GameObject obj, int count)
    {
        // ����Ʈ ����� �Լ�

        // ������Ʈ ���� ���� �����̳�
        ObjContainer MyObjContainer = new ObjContainer();

        // ���� �־��ְ�
        MyObjContainer.code = count;
        MyObjContainer.name = obj.name;
        MyObjContainer.x = obj.transform.position.x;
        MyObjContainer.y = obj.transform.position.y;
        MyObjContainer.z = obj.transform.position.z;

        // List �� add
        saveData.map.Add(MyObjContainer);

        // List�� json����
        // List ��� ���� ���̴µ�
        // �ᱹ ���������� ������ ����� (����������..)
        string json = JsonUtility.ToJson(saveData, true);

        return json;
    }

    // �ӽ÷�.. �����ۿ�
    [System.Serializable]
    public class itemContainer
    {
        public int id = 0;
        public string name;
        public string type;
        public string grade;
        public int count;
    }

    public string MakeListInDict(Data.Item item)
    {
        // ����Ʈ ����� �Լ�

        // ������Ʈ ���� ���� �����̳�
        itemContainer MyItemContainer = new itemContainer();

        // ���� �־��ְ�
        MyItemContainer.id = item.id;
        MyItemContainer.name = item.name;
        MyItemContainer.type = item.type;
        MyItemContainer.grade = item.grade;
        MyItemContainer.count = item.count;

        // List �� add
        saveData2.items.Add(MyItemContainer); 

        // List�� json����
        // List ��� ���� ���̴µ�
        // �ᱹ ���������� ������ ����� (����������..)
        string json = JsonUtility.ToJson(saveData2, true);
        
        return json;
    }


    public void SaveJson(string json)
    {
        // json ���Ϸ� �����ϴ� �Լ�
        string path = "Assets/Resources/Data/MapData.json"; // ��� ���� �ʿ�

        // ����
        File.WriteAllText(path, json);
    }

    public void SaveJson(string json, string path)
    {
        // json ���Ϸ� �����ϴ� �Լ�
        string path1 = $"Assets/Resources/Data/{path}"; // ��� ���� �ʿ�

        // ����
        File.WriteAllText(path1, json);
    }


}
