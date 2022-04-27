using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init()
    {
        // json ���� �о��
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        // (�߰� �ϴ� �κ�)
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        // json ���� �о���� �Լ�
        // �߰��� �� ���� �ǵ� �ʿ� ����
        
        // ���� ���� ����� �� : json vs xml
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");

        //Debug.Log("dd");
        
        // ����Ƽ json �Ľ� ����
        // FromJson : json -> class�� <-> ToJson
        return JsonUtility.FromJson<Loader>(textAsset.text); 
    }
}
