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

    [Space]
    public Sprite[] sprites_action;

    [Space]
    public Sprite[] sprites_emoticon;

    Coroutine co;
    
    private void Start()
    {
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