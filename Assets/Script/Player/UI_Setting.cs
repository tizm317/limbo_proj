using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class UI_Setting : UI_Popup
{
    private Slider[] AudioSlide = new Slider[2];
    private Toggle[] Toggle = new Toggle[2];
    private float[] temp = new float[10];
    private new Light light;
    public AudioMixer master;
    private Slider BrightSlide;

    float brightness;
    float volume;
    float BGM_volume;
    float SFX_volume;


    // 하위 오브젝트들
    enum GameObjects
    {
        Setting,
        Text_Setting,
        // BGM
        Slider_BGM,
        Background_BGM,
        Fill_Area_BGM,
        Fill_BGM,
        Handle_Slide_Area_BGM,
        Handle_BGM,
        Toggle_BGM,
        Background_Toggle_BGM,
        Checkmark_BGM,
        Text_BGM,
        // SFX
        Slider_SFX,
        Background_SFX,
        Fill_Area_SFX,
        Fill_SFX,
        Handle_Slide_Area_SFX,
        Handle_SFX,
        Toggle_SFX,
        Background_Toggle_SFX,
        Checkmark_SFX,
        Text_SFX,
        // Brightness
        Slider_Brightness,
        Background_Bright,
        Fill_Area_Bright,
        Fill_Bright,
        Handle_Slide_Area_Bright,
        Handle_Bright,
        Text_Brightness,
    }
    
    
    void Start()
    {
        light = GameObject.Find("Directional Light").GetComponent<Light>();
       
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 바인딩

        Bind<GameObject>(typeof(GameObjects));
        AudioSlide[0] = GetObject((int)GameObjects.Slider_BGM).GetComponent<Slider>();
        AudioSlide[1] = GetObject((int)GameObjects.Slider_SFX).GetComponent<Slider>();

        // BGM, SFX 초기화
        master.GetFloat("BGM", out BGM_volume);
        AudioSlide[0].value = BGM_volume;

        master.GetFloat("SFX", out SFX_volume);
        AudioSlide[1].value = SFX_volume;

        BrightSlide = GetObject((int)GameObjects.Slider_Brightness).GetComponent<Slider>();

        Toggle[0] = GetObject((int)GameObjects.Toggle_BGM).GetComponent<Toggle>();
        Toggle[1] = GetObject((int)GameObjects.Toggle_SFX).GetComponent<Toggle>();

        // 토글 초기화
        if (BGM_volume == -80f)
            Toggle[0].isOn = true;
        if (SFX_volume == -80f)
            Toggle[1].isOn = true;

        // 슬라이더 Event 바인딩
        GetObject((int)GameObjects.Slider_BGM).gameObject.BindEvent(OnBGMSliderDrag, Define.UIEvent.Drag);
        GetObject((int)GameObjects.Slider_SFX).gameObject.BindEvent(OnSFXSliderDrag, Define.UIEvent.Drag);
        GetObject((int)GameObjects.Slider_Brightness).gameObject.BindEvent(Set_BrightNess, Define.UIEvent.Drag);

        // 토글 Event 바인딩
        GetObject((int)GameObjects.Toggle_BGM).gameObject.BindEvent(OnBGMToggle, Define.UIEvent.Click);
        GetObject((int)GameObjects.Toggle_SFX).gameObject.BindEvent(OnSFXToggle, Define.UIEvent.Click);
    }

    public void OnBGMSliderDrag(PointerEventData data)
    {
        SetVolume(0);
    }
    public void OnSFXSliderDrag(PointerEventData data)
    {
        SetVolume(1);
    }
    public void OnBGMToggle(PointerEventData data)
    {
        Volume_Toggle(0);
    }
    public void OnSFXToggle(PointerEventData data)
    {
        Volume_Toggle(1);
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
        volume = AudioSlide[idx].value;
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
        string name;
        switch (idx)
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

        volume = AudioSlide[idx].value;
        if (Toggle[idx].isOn)
        {
            temp[idx] = volume;
            //Debug.Log(temp[idx]);
            volume = -80f;
        }
        else
        {
            //Debug.Log(temp[idx]);
            volume = temp[idx];
        }
        AudioSlide[idx].value = volume;
        master.SetFloat(name, volume);
    }

    public void Set_BrightNess(PointerEventData data)
    {
        light.intensity = BrightSlide.value;
    }
}
