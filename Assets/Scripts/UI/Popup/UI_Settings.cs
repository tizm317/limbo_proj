﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : UI_Popup
{
    // TODO : 기능 채우기
    [SerializeField]
    private GameObject[] taps;
    public Slider[] AudioSlide = new Slider[2];
    public Toggle[] Toggle = new Toggle[2];
    private Slider BrightSlide;
    private new Light light;
    private new KeyManager KM;
    public Button[] buttons = new Button[5];
    public Text[] button_texts = new Text[5];
    enum GameObjects
    {
        BGMSwitch,
        BGMSlider,
        SFXSwitch,
        SFXSlider,
    }
    private bool FullScreen = false;
    enum Buttons
    {
        CloseButton,
        ApplyButton,
        SKILL1,
        SKILL2,
        SKILL3,
        SKILL4,
        OPTION,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        KM = gameObject.GetComponent<KeyManager>();
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
        taps[2].SetActive(true);
        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(CloseButtonClicked);
        GetButton((int)Buttons.ApplyButton).gameObject.BindEvent(ApplyButtonClicked);
        GetButton((int)Buttons.SKILL1).gameObject.BindEvent(SKILL1Change);
        buttons[0] = GetButton((int)Buttons.SKILL1).gameObject.GetComponent<Button>();
        GetButton((int)Buttons.SKILL2).gameObject.BindEvent(SKILL2Change);
        buttons[1] = GetButton((int)Buttons.SKILL2).gameObject.GetComponent<Button>();
        GetButton((int)Buttons.SKILL3).gameObject.BindEvent(SKILL3Change);
        buttons[2] = GetButton((int)Buttons.SKILL3).gameObject.GetComponent<Button>();
        GetButton((int)Buttons.SKILL4).gameObject.BindEvent(SKILL4Change);
        buttons[3] = GetButton((int)Buttons.SKILL4).gameObject.GetComponent<Button>();
        GetButton((int)Buttons.OPTION).gameObject.BindEvent(OPTIONChange);
        buttons[4] = GetButton((int)Buttons.OPTION).gameObject.GetComponent<Button>();
        taps[2].SetActive(false);
        for(int i = 0; i < buttons.Length; i++)
        {
            button_texts[i] = buttons[i].transform.Find("Text").GetComponent<Text>();
        }
        StartCoroutine(Button_Text_Update());
    }
    public void Set_Resolution(int idx, string resolution)
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


        if(set_Width > device_Width)
        {
            set_Height = (int)(set_Height * ((float)device_Width/set_Width));
            set_Width = device_Width;
        }
        if(set_Height > device_Height)
        {
            set_Width = (int)(set_Width * ((float)device_Height/set_Height));
            set_Height = device_Height;
        }
        Debug.LogFormat("set_Width = {0}, set_Height = {1}",set_Width, set_Height);
        Screen.SetResolution(set_Width,set_Height, false);
        if((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        {
            float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
            Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, 1f); // 새로운 Rect 적용
            //Debug.LogFormat("Current Resolution = {0} * {1}",new_Width,set_Height);
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            //Debug.LogFormat("Current Resolution = {0} * {1}",set_Width,newHeight);
        }
    }

    public void Full_Screen(bool input)
    {
        FullScreen = input;
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

    private void SKILL1Change(PointerEventData data)
    {
        KM.ChangeKey(0);
        button_texts[0].text = KeySetting.keys[KeyAction.SKILL1].ToString();
    }
    private void SKILL2Change(PointerEventData data)
    {
        KM.ChangeKey(1);
        button_texts[1].text = KeySetting.keys[KeyAction.SKILL2].ToString();
    }
    private void SKILL3Change(PointerEventData data)
    {
        KM.ChangeKey(2);
        button_texts[2].text = KeySetting.keys[KeyAction.SKILL3].ToString();
    }
    private void SKILL4Change(PointerEventData data)
    {
        KM.ChangeKey(3);
        button_texts[3].text = KeySetting.keys[KeyAction.SKILL4].ToString();
    }
    private void OPTIONChange(PointerEventData data)
    {
        KM.ChangeKey(4);
        button_texts[4].text = KeySetting.keys[KeyAction.OPTION].ToString();
    }
   
    private void _Button_Text_Update()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            button_texts[i].text = KeySetting.keys[(KeyAction)i].ToString();
        }
    }

    IEnumerator Button_Text_Update()
    {
        while(true)
        {
            if(taps[2].activeSelf)
                _Button_Text_Update();
            yield return new WaitForEndOfFrame();
        }
    }
    
}
