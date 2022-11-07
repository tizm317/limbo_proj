using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : UI_Popup
{
    // TODO : 기능 채우기
    public Slider[] AudioSlide = new Slider[2];
    public Toggle[] Toggle = new Toggle[2];
    private Slider BrightSlide;
    private new Light light;

    enum GameObjects
    {
        BGMSwitch,
        BGMSlider,
        SFXSwitch,
        SFXSlider,
    }

    enum Buttons
    {
        CloseButton,
        ApplyButton,
    }


    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        AudioSlide[0] = GetObject((int)GameObjects.BGMSlider).GetComponent<Slider>();
        Toggle[0] = GetObject((int)GameObjects.BGMSwitch).GetComponent<Toggle>();
        if ((AudioSlide[0].value = Managers.Sound.GetVolume("BGM")) == -80f)
        {
            Toggle[0].isOn = false;
        }
        else
            Toggle[0].isOn = true;

        // SFX
        AudioSlide[1] = GetObject((int)GameObjects.SFXSlider).GetComponent<Slider>();
        Toggle[1] = GetObject((int)GameObjects.SFXSwitch).GetComponent<Toggle>();

        if ((AudioSlide[1].value = Managers.Sound.GetVolume("SFX")) == -80f)
            Toggle[1].isOn = false;
        else
            Toggle[1].isOn = true;

        // Brightness
        //BrightSlide = GetObject((int)GameObjects.Slider_Brightness).GetComponent<Slider>();
        //light = GameObject.Find("Directional Light").GetComponent<Light>();
        //BrightSlide.value = light.intensity;


        // 슬라이더 Event 바인딩
        GetObject((int)GameObjects.BGMSlider).gameObject.BindEvent(OnBGMSliderDrag, Define.UIEvent.Drag);
        GetObject((int)GameObjects.SFXSlider).gameObject.BindEvent(OnSFXSliderDrag, Define.UIEvent.Drag);
        //GetObject((int)GameObjects.Slider_Brightness).gameObject.BindEvent(Set_BrightNess, Define.UIEvent.Drag);

        // 토글 Event 바인딩
        GetObject((int)GameObjects.BGMSwitch).gameObject.BindEvent(OnBGMToggle, Define.UIEvent.Click);
        GetObject((int)GameObjects.SFXSwitch).gameObject.BindEvent(OnSFXToggle, Define.UIEvent.Click);

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseButtonClicked);
        GetButton((int)Buttons.ApplyButton).gameObject.BindEvent(ApplyButtonClicked);

    }

    private void CloseButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
    }
    private void ApplyButtonClicked(PointerEventData data)
    {
        ClosePopupUI();
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
}
