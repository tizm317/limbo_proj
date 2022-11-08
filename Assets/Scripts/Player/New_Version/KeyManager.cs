using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction
{
    Q,
    W,
    E,
    R,
    ESC,
    KEYCOUNT//마지막 친구임, Count대신에 쓸거
}

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
    public static bool first = true;
}
public class KeyManager : MonoBehaviour
{
    
    KeyCode[] default_key = new KeyCode[]{KeyCode.Q,KeyCode.W,KeyCode.E,KeyCode.R,KeyCode.Escape};

    void Start()
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
            KeySetting.keys[(KeyAction)key] = KeyEvent.keyCode;
            default_key[key] = KeyEvent.keyCode;
            key = -1;
        }
    }

    public void ChangeKey(int num)
    {
        key = num;
    }
}
//https://www.youtube.com/watch?v=ODB3IOFqmrE
