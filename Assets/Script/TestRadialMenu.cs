using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRadialMenu : MonoBehaviour
{
    public RadialMenu radialMenu;
    private KeyCode key_emoticon = KeyCode.E;
    private KeyCode key_action = KeyCode.G;

    Player_Controller player;
    UI_Emoticon emoticon;

    private Sprite[] sprites_action;
    private Sprite[] sprites_emoticon;

    Coroutine co;

    enum Action
    {
        angry,
        pray,
        card_shuffle,
        twerk,
        air_guitar,
        dance,
        hi,
        surprise,
        COUNT,
    }

    int emoticonCount = 9;

    int pieceCount = 8;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        sprites_action = new Sprite[8];
        sprites_emoticon = new Sprite[8];

        string num;
        for (int i = 0; i < pieceCount; i++)
        {
            num = (i % emoticonCount).ToString("000");
            sprites_action[i] = Managers.Resource.Load<Sprite>($"Sprites/Action/{(Action)(i % (int)Action.COUNT)}");
            sprites_emoticon[i] = Managers.Resource.Load<Sprite>($"Sprites/Emoticon/Emoticon{num}");
        }

        radialMenu.SetPieceImageSprites(sprites_action);

        player = GameObject.Find("@Scene").GetComponent<Player_Controller>();
        emoticon = GameObject.Find("@UI_Root").GetComponentInChildren<UI_Emoticon>();

        Managers.Input.KeyAction -= keyDownCheck;
        Managers.Input.KeyAction += keyDownCheck;
    }


    public void keyDownCheck()
    {
        if (Input.GetKeyDown(key_action))
        //if(Input.GetMouseButtonDown(0))
        {
            radialMenu.SetPieceImageSprites(sprites_action);
            radialMenu.Show();
            co = StartCoroutine(coKeyUpCheck());
        }

        if (Input.GetKeyDown(key_emoticon))
        {
            radialMenu.SetPieceImageSprites(sprites_emoticon);
            radialMenu.Show();
            co = StartCoroutine(coKeyUpCheck());
        }

    }

    public IEnumerator coKeyUpCheck()
    {
        while(true)
        {
            if (Input.GetKeyUp(key_action))
            {
                int selected = radialMenu.Hide();
                Debug.Log($"Selected : {selected}");

                // 감정표현
                player.emotion(selected);
                StopCoroutine(co);
            }

            if (Input.GetKeyUp(key_emoticon))
            {
                int selected = radialMenu.Hide();
                Debug.Log($"Selected : {selected}");

                // 이모티콘
                emoticon.transform.GetChild(0).gameObject.SetActive(true);
                emoticon.setEmoticon(selected);
                StopCoroutine(co);
            }

            yield return null;
        }
    }
}