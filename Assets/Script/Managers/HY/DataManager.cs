using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init()
    {
        // json 파일 읽어옴
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        // (추가 하는 부분)
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
}
