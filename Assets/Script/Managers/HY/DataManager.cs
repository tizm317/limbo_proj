using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // FILE

// 실제 파일 읽어들이는 부분

public interface ILoader<Key, Value>
{
    // 깔끔하게 관리하기 위해 인터페이스 만듦
    // 이 인터페이스 가지고 있는 클래스는 Dictionary<K, V> return 하는 MakeDict() 구현해야 함
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    // 게임에 등장하는 모든 수치 관리하는 매니저

    // ex : 레벨별 스텟, 레벨별 경험치, 몬스터 ai 등 데이터 etc
    // 하드 코딩하면, 수치 고칠 때 찾아서 수정해야함
    // 실행파일에 묶여서, 업데이트하려면 배포를 다시 해야함

    // 모든 수치 관련 -> json 파일로 들고 있음
    // 웹 통신으로 파일만 바꾸면 바로 적용됨.
    // 서버, 클라 동기화할 때도

    // DataManager가 딕셔너리로 들고있음
    // 딕셔너리로 들고 있는게 효율적 - (리스트 찾기 위해 다 순회해야함.)
    // 외부에서 사용할 때 : Dictionary 뽑아서 사용 (예시 : Dictionary<int, Stat> dict = Managers.Data.StatDict;)
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>(); // 키 값을 레벨로
    //(추가 하는 부분)
    //public Dictionary<int, Data.Pos> PosDict { get; private set; } = new Dictionary<int, Data.Pos>();
    public Dictionary<int, Data.Map> MapDict { get; private set; } = new Dictionary<int, Data.Map>();

    public void Init()
    {
        // json 파일 읽어옴
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        //PosDict = LoadJson<Data.PosData, int, Data.Pos>("PosData").MakeDict();
        // (추가 하는 부분)

        // path 내에 MapData 존재 안하면 json 파일로 저장 후,
        string path = "D:/Unity/limbo_proj/Assets/Resources/Data/MapData.json"; // 경로 수정 필요
        if (!File.Exists(path))
        {
            // 맵 안에 있는 오브젝트 갯수 카운트
            int objCount = 0;

            string json = "";

            // 맵 오브젝트 담긴 부모 오브젝트
            GameObject prop = GameObject.Find("prop");

            // 맵 오브젝트(child) 순회
            foreach (Transform child in prop.transform)
            {
                // Layer 가 Building 아니면 continue (나중에 조건 바뀔 수 있음 - 중요 건물만 한다던가)
                if (child.gameObject.layer != (int)Define.Layer.Building)
                    continue;

                // MakeList() 에서 List 만들어서 반환
                json = MakeList(child.gameObject, objCount);
                objCount++;
            }
            // List 최종본이 json에 저장된 채로 나옴

            // json파일 저장
            SaveJson(json);
        }
        
        // Map Dictionary 만듦
        MapDict = LoadJson<Data.MapData, int, Data.Map>("MapData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        // json 파일 읽어오는 함수
        // 추가할 때 따로 건들 필요 없음
        
        // 파일 포맷 맞춰야 함 : json vs xml
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");

        //Debug.Log("dd");
        
        // 유니티 json 파싱 제공
        // FromJson : json -> class로 <-> ToJson
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    // 저장 기능 (MakeList , Savejson)

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
        // List 만들기 위해서
        public List<ObjContainer> map = new List<ObjContainer>();
    }
    SaveData saveData = new SaveData();

    // 리스트 만드는 부분(MakeList)이랑 저장하는 부분(SaveJson) 나눔
    string MakeList(GameObject obj, int count)
    {
        // 리스트 만드는 함수

        // 오브젝트 정보 담을 컨테이너
        ObjContainer MyObjContainer = new ObjContainer();

        // 정보 넣어주고
        MyObjContainer.code = count;
        MyObjContainer.name = obj.name;
        MyObjContainer.x = obj.transform.position.x;
        MyObjContainer.y = obj.transform.position.y;
        MyObjContainer.z = obj.transform.position.z;

        // List 에 add
        saveData.map.Add(MyObjContainer);

        // List를 json으로
        // List 계속 새로 덮이는데
        // 결국 마지막꺼로 덮여서 저장됨 (최종본으로..)
        string json = JsonUtility.ToJson(saveData, true);

        return json;
    }

    void SaveJson(string json)
    {
        // json 파일로 저장하는 함수
        string path = "D:/Unity/limbo_proj/Assets/Resources/Data/MapData.json"; // 경로 수정 필요

        // 저장
        File.WriteAllText(path, json);
    }

    
}
