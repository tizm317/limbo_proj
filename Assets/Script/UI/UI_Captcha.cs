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

    [Header("Captcha Generator :")]
    [SerializeField] private CaptchaGenerator captchaGenerator;


    [SerializeField] private int Count;

    private CaptchaAlphabets[] currentCaptcha;


    void Start()
    {
        GenerateCaptcha();

        //Buttons:
        uiRefreshButton.onClick.AddListener(GenerateCaptcha);
        uiSubmitButton.onClick.AddListener(Submit);
    }

    private void GenerateCaptcha()
    {
        currentCaptcha = captchaGenerator.Generate();

        //Change UI:
        for(int i = 0; i < Count; i++)
            uiCodeImage[i].sprite = currentCaptcha[i].Image;
        uiErrorsText.gameObject.SetActive(false);

    }

    private void Submit()
    {
        string enteredCode = uiCodeInput.text;

        if(captchaGenerator.IsCodeValid(enteredCode, currentCaptcha))
        {
            //valid
            uiErrorsText.gameObject.SetActive(false);
            Debug.Log("<color=green>Valid Code</color>");

        }
        else
        {
            //invalid
            uiErrorsText.gameObject.SetActive(true);
            Debug.Log("<color=red>Invalid Code</color>");
        }
    }

}
