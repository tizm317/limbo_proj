using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    // 씬 UI 사용할 때 상속 받게 될 Scene UI 베이스 클래스

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        // 팝업 UI 아니니까,  Canvas 내 order 변경 X
        Managers.UI.SetCanvas(gameObject, false);
    }
}
