using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_NewMiniMap : UI_Popup
{
    // MiniMap New Version
    
    enum Texts
    {
        MapNameText,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));

        // 맵 이름 초기화
        Text _MapNameText = GetText((int)Texts.MapNameText);
        _MapNameText.text = SceneManager.GetActiveScene().name;
        if (_MapNameText.text.Contains("InGame"))
            _MapNameText.text = _MapNameText.text.Substring(6);
    }
}
