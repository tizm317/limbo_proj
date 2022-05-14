using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class Setting : MonoBehaviour
{
    public Slider[] AudioSlide;
    public Toggle[] Toggle;
    private float[] temp = new float[10];
    public AudioMixer master;
    public Slider BrightSlide;
    // Start is called before the first frame update
    public void SetVolume(int idx)
    {
        string name;
        switch(idx)
        {
            case 0:
                name = "BGM";
                break;
            case 1:
                name = "SFX";
                break;
            default:
                name = "BGM";
                break;
        }
        float volume = AudioSlide[idx].value;
        if(volume == -40f)
        {
            master.SetFloat(name,-80);//음소거
            Toggle[idx].isOn = true;
        }
           
        else
        {
            master.SetFloat(name,volume);
            temp[idx] = volume;
            Toggle[idx].isOn = false;
        }
    }

    public void Volume_Toggle(int idx)
    {
        if(Toggle[idx])
        {
            Debug.Log(temp[idx]);
            AudioSlide[idx].value = -80f;
        }
        else
        {
            Debug.Log(temp[idx]);
            AudioSlide[idx].value = temp[idx];
        }
    }
}
