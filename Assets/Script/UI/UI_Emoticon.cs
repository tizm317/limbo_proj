using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Emoticon : UI_Base
{
    private Sprite[] emoticons;
    private int counts = 8;

    enum Images
    {
        Emoticon,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        // 이모티콘 이미지 비활성화
        transform.GetChild(0).gameObject.SetActive(false);

        emoticons = new Sprite[counts];
        for(int i = 0; i < counts; i++)
        {
            string num = i.ToString("000");
            emoticons[i] = Managers.Resource.Load<Sprite>($"Sprites/Emoticon/Emoticon{num}");
        }
    }

    public void setEmoticon(int num)
    {
        if(num < 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            Debug.Log("이모티콘 없음");
        }
        else
            GetImage((int)Images.Emoticon).sprite = emoticons[num];

        StartCoroutine(setDisableAfterOneSec());
    }

    IEnumerator setDisableAfterOneSec()
    {
        yield return new WaitForSeconds(1f);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
