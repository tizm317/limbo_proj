using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Emoticon : UI_Base
{
    public Sprite[] emoticons;
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

        //GetImage((int)Images.Emoticon).sprite = emoticon0;

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


        //switch (num)
        //{
        //    case 0:
        //        GetImage((int)Images.Emoticon).sprite = emoticon0;
        //        break;
        //    case 1:
        //        GetImage((int)Images.Emoticon).sprite = emoticon1;
        //        break;
        //    case 2:
        //        GetImage((int)Images.Emoticon).sprite = emoticon2;
        //        break;
        //    case 3:
        //        GetImage((int)Images.Emoticon).sprite = emoticon3;
        //        break;
        //    case 4:
        //        GetImage((int)Images.Emoticon).sprite = emoticon4;
        //        break;
        //    case 5:
        //        GetImage((int)Images.Emoticon).sprite = emoticon5;
        //        break;
        //    case 6:
        //        GetImage((int)Images.Emoticon).sprite = emoticon6;
        //        break;
        //    case 7:
        //        GetImage((int)Images.Emoticon).sprite = emoticon7;
        //        break;
        //    default:
        //        transform.GetChild(0).gameObject.SetActive(false);
        //        Debug.Log("이모티콘 없음");
        //        break;
        //}
    }

    IEnumerator setDisableAfterOneSec()
    {
        yield return new WaitForSeconds(1f);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
