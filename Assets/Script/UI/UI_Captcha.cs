using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Captcha : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] private Image[] uiCodeImage;
    [SerializeField] private InputField uiCodeInput;
    [SerializeField] private Text uiErrorsText;
    [SerializeField] private Button uiRefreshButton;
    [SerializeField] private Button uiSubmitButton;
    [SerializeField] private Text countText;

    [Header("Captcha Generator :")]
    [SerializeField] private CaptchaGenerator captchaGenerator;


    [SerializeField] private int Count;

    private CaptchaAlphabets[] currentCaptcha;

    private Coroutine co;
    private int tryCount = 0;
    private const int MaxTryCount = 3;
    private float timer = 60f; // 20s * 3

    void Start()
    {
        //GenerateCaptcha();

        //Buttons:
        uiRefreshButton.onClick.AddListener(GenerateCaptcha);
        uiSubmitButton.onClick.AddListener(Submit);
    }

    public void GenerateCaptcha()
    {
        uiCodeInput.text = "";
        currentCaptcha = captchaGenerator.Generate();
        string answer = "";

        //Change UI:
        for (int i = 0; i < Count; i++)
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

    private void Submit()
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
        this.gameObject.SetActive(false);
        tryCount = 0;
        countText.text = $"Try Count : {tryCount}/3";
        StopCoroutine(co);
    }

    private void countUp()
    {
        tryCount++;
        countText.text = $"Try Count : {tryCount}/3";
        Debug.Log($"<color=red>Try Count : {tryCount}</color>");
    }
}
