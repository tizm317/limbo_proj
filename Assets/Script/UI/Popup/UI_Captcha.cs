using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Captcha : UI_Popup
{
    [Header("UI References :")]
    private Image[] uiCodeImage;
    private int CodeLength = 6;
    private InputField uiCodeInput;
    private Text uiErrorsText;
    private Text countText;

    [Header("Captcha Generator :")]
    [SerializeField] private CaptchaGenerator captchaGenerator;
    private CaptchaAlphabets[] currentCaptcha;

    private Coroutine co;
    private int tryCount = 0;
    private const int MaxTryCount = 3;
    private float timer = 60f; // 20s * 3

    enum GameObjects
    {
        InputField,
    }
    enum Texts
    {
        ErrorText,
        CountText,
    }
    enum Images
    {
        CaptchaImg1,
        CaptchaImg2,
        CaptchaImg3,
        CaptchaImg4,
        CaptchaImg5,
        CaptchaImg6,
    }
    enum Buttons
    {
        RefreshButton,
        SubmitButton,
    }


    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        uiErrorsText = GetText((int)Texts.ErrorText).GetComponent<Text>();
        countText = GetText((int)Texts.CountText).GetComponent<Text>();

        uiCodeInput = GetObject((int)GameObjects.InputField).GetComponent<InputField>();

        uiCodeImage = new Image[CodeLength];
        for(int i = 0; i < CodeLength; i++)
            uiCodeImage[i] = GetImage(i).GetComponent<Image>();

        GetButton((int)Buttons.RefreshButton).gameObject.BindEvent(GenerateCaptcha);
        GetButton((int)Buttons.SubmitButton).gameObject.BindEvent(Submit);

        captchaGenerator = Managers.Resource.Load<CaptchaGenerator>("CaptchaGen/CaptchaGenerator");
    }

    public void GenerateCaptcha(PointerEventData data = null)
    {
        uiCodeInput.text = "";
        currentCaptcha = captchaGenerator.Generate();
        string answer = "";

        //Change UI:
        for (int i = 0; i < CodeLength; i++)
        {
            answer += currentCaptcha[i].Value.ToString();

            uiCodeImage[i].sprite = currentCaptcha[i].Image;
            uiCodeImage[i].transform.localEulerAngles = new Vector3(0, 0, 0);
            uiCodeImage[i].transform.Rotate(Vector3.forward, Random.Range(-60, 61));
        }
        uiErrorsText.gameObject.SetActive(false);

        Debug.Log($"Captcha : {answer}");

        co = StartCoroutine("CoTimeCheck", timer/MaxTryCount); // 20s * 3
    }

    private void Submit(PointerEventData data)
    {
        string enteredCode = uiCodeInput.text;

        if(captchaGenerator.IsCodeValid(enteredCode, currentCaptcha))
        {
            //valid
            uiErrorsText.gameObject.SetActive(false);
            Debug.Log("<color=green>Valid Code</color>");
            endCaptcha();
        }
        else
        {
            //invalid
            uiErrorsText.gameObject.SetActive(true);
            Debug.Log("<color=red>Invalid Code</color>");
        }

        countUp();
        if (tryCount >= MaxTryCount)
            endCaptcha();
    }

    IEnumerator CoTimeCheck(float seconds = 60f)
    {
        yield return new WaitForSeconds(seconds);

        // time out
        Debug.Log("<color=red>Time Out !</color>");
        
        countUp();
        if (tryCount < MaxTryCount)
            GenerateCaptcha();
        else
            endCaptcha();
    }

    private void endCaptcha()
    {
        // end
        if(tryCount > MaxTryCount)
            Debug.Log("<color=red>Failed !</color>");

        uiErrorsText.gameObject.SetActive(false);
        //this.gameObject.SetActive(false);
        tryCount = 0;
        countText.text = $"Try Count : {tryCount}/3";
        StopCoroutine(co);

        ClosePopupUI();
    }

    private void countUp()
    {
        tryCount++;
        countText.text = $"Try Count : {tryCount}/3";
        Debug.Log($"<color=red>Try Count : {tryCount}</color>");
    }
}
