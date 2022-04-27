using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    // 팝업 사용할 때 상속 받게 될 베이스 클래스

    // 참고 : 팝업 뜰 때, 뒤에 뜬 팝업 UI 클릭 못하도록 UI 프리팹에 Blocker 이미지(패널) 만들어서 막음

    public override void Init()
    {
        // 팝업 UI : Canvas 내 order 변경 요청
        Managers.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        // UI_Popup 상속받은 애들 쉽게 Close popup 쓰도록 만든 인터페이스
        Managers.UI.ClosePopupUI(this);
    }
}
