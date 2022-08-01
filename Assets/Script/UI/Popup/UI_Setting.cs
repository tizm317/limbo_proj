using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class UI_Setting : UI_Popup
{
    // BGM, SFX, Brightness 조절

    private Slider[] AudioSlide = new Slider[2];
    private Toggle[] Toggle = new Toggle[2];
    private Slider BrightSlide;
    private new Light light;

    //private AudioMixer master;
    //float BGM_volume;
    //float SFX_volume;
    //private float[] temp = new float[10];


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
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 바인딩

        Bind<GameObject>(typeof(GameObjects));

        // AudioMixer
        //master = Managers.Sound.GetAudioMixer();

        // BGM
        AudioSlide[0] = GetObject((int)GameObjects.Slider_BGM).GetComponent<Slider>();
        Toggle[0] = GetObject((int)GameObjects.Toggle_BGM).GetComponent<Toggle>();

        if ((AudioSlide[0].value = Managers.Sound.GetVolume("BGM")) == -80f)
            Toggle[0].isOn = true;

        //master.GetFloat("BGM", out BGM_volume);
        //AudioSlide[0].value = BGM_volume;
        //if (BGM_volume == -80f)
        //    Toggle[0].isOn = true;

        // SFX
        AudioSlide[1] = GetObject((int)GameObjects.Slider_SFX).GetComponent<Slider>();
        Toggle[1] = GetObject((int)GameObjects.Toggle_SFX).GetComponent<Toggle>();

        if ((AudioSlide[1].value = Managers.Sound.GetVolume("SFX")) == -80f)
            Toggle[1].isOn = true;

        //master.GetFloat("SFX", out SFX_volume);
        //AudioSlide[1].value = SFX_volume;
        //if (SFX_volume == -80f)
        //    Toggle[1].isOn = true;

        // Brightness
        BrightSlide = GetObject((int)GameObjects.Slider_Brightness).GetComponent<Slider>();
        light = GameObject.Find("Directional Light").GetComponent<Light>();
        BrightSlide.value = light.intensity;

        // 슬라이더 Event 바인딩
        GetObject((int)GameObjects.Slider_BGM).gameObject.BindEvent(OnBGMSliderDrag, Define.UIEvent.Drag);
        GetObject((int)GameObjects.Slider_SFX).gameObject.BindEvent(OnSFXSliderDrag, Define.UIEvent.Drag);
        GetObject((int)GameObjects.Slider_Brightness).gameObject.BindEvent(Set_BrightNess, Define.UIEvent.Drag);

        // 토글 Event 바인딩
        GetObject((int)GameObjects.Toggle_BGM).gameObject.BindEvent(OnBGMToggle, Define.UIEvent.Click);
        GetObject((int)GameObjects.Toggle_SFX).gameObject.BindEvent(OnSFXToggle, Define.UIEvent.Click);
    }

    // 바인딩하려고 인터페이스 맞추는 용도
    public void OnBGMSliderDrag(PointerEventData data)
    {
        //SetVolume(0);
        Managers.Sound.SetVolume(0, AudioSlide, Toggle);
    }
    public void OnSFXSliderDrag(PointerEventData data)
    {
        //SetVolume(1);
        Managers.Sound.SetVolume(1, AudioSlide, Toggle);
    }
    public void OnBGMToggle(PointerEventData data)
    {
        //Volume_Toggle(0);
        Managers.Sound.Volume_Toggle(0, AudioSlide, Toggle);
    }
    public void OnSFXToggle(PointerEventData data)
    {
        //Volume_Toggle(1);
        Managers.Sound.Volume_Toggle(1, AudioSlide, Toggle);
    }
    public void Set_BrightNess(PointerEventData data)
    {
        light.intensity = BrightSlide.value;
    }


    // 사운드 매니저로 보냄
    //public void SetVolume(int idx)
    //{
    //    string name;
    //    switch (idx)
    //    {
    //        case 0:
    //            name = "BGM";
    //            break;
    //        case 1:
    //            name = "SFX";
    //            break;
    //        default:
    //            name = "BGM";
    //            break;
    //    }
    //    float volume = AudioSlide[idx].value;
    //    if (volume != -40f)
    //    {
    //        master.SetFloat(name, volume);
    //        Toggle[idx].isOn = false;
    //    }
    //    else
    //    {
    //        master.SetFloat(name, -80);//음소거
    //        Toggle[idx].isOn = true;
    //    }
    //}
    //public void Volume_Toggle(int idx)
    //{
    //    string name;
    //    switch (idx)
    //    {
    //        case 0:
    //            name = "BGM";
    //            break;
    //        case 1:
    //            name = "SFX";
    //            break;
    //        default:
    //            name = "BGM";
    //            break;
    //    }

    //    float volume = AudioSlide[idx].value;
    //    if (Toggle[idx].isOn)
    //    {
    //        temp[idx] = volume;
    //        //Debug.Log(temp[idx]);
    //        volume = -80f;
    //    }
    //    else
    //    {
    //        //Debug.Log(temp[idx]);
    //        volume = temp[idx];
    //    }
    //    AudioSlide[idx].value = volume;
    //    master.SetFloat(name, volume);
    //}
}
