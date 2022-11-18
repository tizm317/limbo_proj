using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction
{
    SKILL1,
    SKILL2,
    SKILL3,
    SKILL4,
    OPTION,
    EMOJI,
    EMOTEACTION,
    UITOGGLE,
    INVENTORY,
    MINIMAP,
    ZOOM,
    STAT,
    KEYCOUNT//마지막 친구임, Count대신에 쓸거
}

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
    public static bool first = true;
}
public class KeyManager : MonoBehaviour
{
    
    KeyCode[] default_key = new KeyCode[]{KeyCode.Q,KeyCode.W,KeyCode.E,KeyCode.R,KeyCode.Escape,KeyCode.T,KeyCode.G, KeyCode.Tab, KeyCode.I, KeyCode.M, KeyCode.Z, KeyCode.S};

    void Awake()
    {
        if(KeySetting.first)
            Init();
    }

    void Init()
    {
        for(int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
        {
            KeySetting.keys.Add((KeyAction)i,default_key[i]);
        }
        KeySetting.first = false;
    }

    int key = -1;

    private void OnGUI()
    {
        Event KeyEvent = Event.current;
        if(KeyEvent.isKey)
        {
            KeyCode input = KeyEvent.keyCode;
            bool already_using = false;
            for(int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
                if(KeySetting.keys[(KeyAction)i] == input)
                    already_using = true;
            if(!already_using)
                KeySetting.keys[(KeyAction)key] = input;
            //default_key[key] = KeyEvent.keyCode;
            key = -1;
        }
    }

    public void ChangeKey(int num)
    {
        key = num;
    }
}
//https://www.youtube.com/watch?v=ODB3IOFqmrE
