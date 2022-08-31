using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRadialMenu : MonoBehaviour
{
    public RadialMenu radialMenu;
    public KeyCode key = KeyCode.G;

    Player_Controller player;

    [Space]
    public Sprite[] sprites;

    private void Start()
    {
        radialMenu.SetPieceImageSprites(sprites);

        player = GameObject.Find("@Scene").GetComponent<Player_Controller>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(key))
        if(Input.GetMouseButtonDown(0))
        {
            radialMenu.Show();
        }
        //else if (Input.GetKeyUp(key))
        else if(Input.GetMouseButtonUp(0))
        {
            int selected = radialMenu.Hide();
            Debug.Log($"Selected : {selected}");

            player.emotion(selected);
        }
    }
}