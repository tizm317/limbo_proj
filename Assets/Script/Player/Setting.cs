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
    private Light light;
    public AudioMixer master;
    public Slider BrightSlide;
    // Start is called before the first frame update
    void Start()
    {
        light = GameObject.Find("Directional Light").GetComponent<Light>();
    }
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
        if(volume != -40f)
        {
            master.SetFloat(name,volume);  
            Toggle[idx].isOn = false;
        }         
        else
        {
            master.SetFloat(name,-80);//음소거
            Toggle[idx].isOn = true;
        }
    }

    public void Volume_Toggle(int idx)
    {
        if(Toggle[idx].isOn)
        {
            float volume = AudioSlide[idx].value;
            temp[idx] = volume;
            Debug.Log(temp[idx]);
            AudioSlide[idx].value = -80f;
        }
        else
        {
            Debug.Log(temp[idx]);
            AudioSlide[idx].value = temp[idx];
        }
    }

    public void Set_BrightNess()
    {
        light.intensity = BrightSlide.value;
    }
}
