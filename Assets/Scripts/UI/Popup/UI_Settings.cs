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
    private DuloGames.UI.UISelectField cs;
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

    public void Set_Resolution(int idx, string resolution)//바인딩하려했는데 DropDown이 아니네?ㄷㄷ
    {
        string width = resolution;
        string height = resolution;
        int mul, name_start;
        mul = resolution.IndexOf("*");
        name_start = resolution.IndexOf("(");

        width = width.Remove(mul);
        height = height.Remove(name_start).Replace(width, "").Replace("*", "").Trim();

        int set_Width = int.Parse(width);
        int set_Height = int.Parse(height);
        int device_Width = Screen.width;
        int device_Height = Screen.height;

        Screen.SetResolution(set_Width,(int)((float)device_Height/device_Width) * set_Width, false);
        if((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        {
            float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
            Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, 1f); // 새로운 Rect 적용
            Debug.LogFormat("Current Resolution = {0} * {1}",new_Width,set_Height);
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            Debug.LogFormat("Current Resolution = {0} * {1}",set_Width,newHeight);
        }
        DuloGames.UI.UISelectField cs = FindObjectOfType<DuloGames.UI.UISelectField>();
        cs.SelectOptionByIndex(idx);
        
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
