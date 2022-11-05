using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldHelper
{
    private List<InputField> _inputFields;
    private int curPos;

    // Tab, Enter 누르면 다음 inputfield로 이동
    // Shift + Tab 누르면 전 inputfield로 이동

    public InputFieldHelper()
    {
        _inputFields = new List<InputField>();
    }

    public void Add(InputField inputField)
    {
        _inputFields.Add(inputField);
    }

    public void SetFocus(int idx = 0)
    {
        if(idx >= 0 && idx < _inputFields.Count)
        {
            _inputFields[idx].Select();
        }
    }

    private void MoveNext()
    {
        curPos = GetCurrentPos();
        curPos++;
        curPos %= _inputFields.Count;
        _inputFields[curPos].Select();
    }

    private void MovePrev()
    {
        curPos = GetCurrentPos();
        curPos--;
        if (curPos < 0) curPos = _inputFields.Count - 1;
        _inputFields[curPos].Select();
    }

    private int GetCurrentPos()
    {
        int idx = 0;
        foreach(InputField field in _inputFields)
        {
            if(field.isFocused == true)
            {
                return idx;
            }
            idx++;
        }

        // 다 돌았는데도 focused 된 것이 없다..?
        return 0;
    }

    public bool CheckFocus()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            MoveNext();
        }
        else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) // Enter Key
        {
            // 마지막 Input Field에서 Enter 누르면 Button Click 과 같은 역할
            if (curPos == _inputFields.Count - 1)
                return ButtonClick();
            else
                MoveNext();
        }

        if (Input.GetKeyDown(KeyCode.Tab) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            MovePrev();
        }

        return false;
    }

    public bool ButtonClick()
    {
        return true;
    }
}
