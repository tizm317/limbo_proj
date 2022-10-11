using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr//Managers가 만약 Ingame에서 생성되는 거라면? Instance로 추가, 아니라면 Singleton을 Awake에 추가해주어야함
{
    // Start is called before the first frame update
    void Awake()
    {
        GetInfo();
    }
    void Start()
    {
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void GetInfo()
    {
        //캐릭터 종류와 데이터를 받아오는 내용이 필요함
    }

    void Init()
    {
        //캐릭터 종류와 데이터에 따라서 새로운 Instantiate해주는 내용이 필요함
    }
}
