using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NotificationLevel : UI_Popup
{

    enum GameObjects
    {
        NotificationLevel,
        LevelUpEffect,
    }
    enum Texts
    {
        LevelText,
    }


    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));

    }
    private void Update()
    {
        // Canvas Render Camera Setting (어디서 바뀌어서 오는거지..?)
        if (this.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
        {
            this.GetComponent<Canvas>().worldCamera = Camera.main;
            this.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        }
    }

    Text _level;

    public void Notify(Text level)
    {
        _level = level;
        StartCoroutine(CoLevelUpPopup());
    }
    IEnumerator CoLevelUpPopup()
    {
        Init();
        //GetObject((int)GameObjects.NotificationLevel).SetActive(true);
        GetText((int)Texts.LevelText).text = $"LEVEL {_level.text}";
        GetObject((int)GameObjects.LevelUpEffect).GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(10.0f);

        //GetObject((int)GameObjects.NotificationLevel).SetActive(false);
        ClosePopupUI();
        //StopCoroutine(CoLevelUpPopup());
    }
}
