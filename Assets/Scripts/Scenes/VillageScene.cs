using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageScene : GameScene
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        // 씬타입 설정
        SceneType = Define.Scene.InGameVillage;
        
        // 초기화
        base.Init();
    }
    public override void Clear()
    {
        // 이 씬이 종료될 때 날려줘야 하는 부분 넣어줘야 함
        Debug.Log("Village Scene Clear");
    }
}
