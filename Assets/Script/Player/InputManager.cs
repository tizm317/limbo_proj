using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool mouse_right_btn;
    public bool LeftShift = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouse_right_btn_change();
    }

    void mouse_right_btn_change()
    {
        if(Input.GetMouseButtonDown(1))
        {
            mouse_right_btn = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            mouse_right_btn = false;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            LeftShift = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            LeftShift = false;
        }
            

    }
}
