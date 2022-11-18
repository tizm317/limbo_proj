using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Login : UI_Scene
{
    // 로그인 씬 용 UI

    enum Buttons
    {
        ButtonLogin,
        ButtonCreate,
        ButtonCredit,
        ButtonExit,
    }

    enum GameObjects
    {
        AccountName,
        Password,
    }

    enum Images
    {
        //BackGroundImage
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (inputFieldHelper.CheckFocus() == true)
            OnClickLoginButton(null);
    }

    InputFieldHelper inputFieldHelper;
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ButtonCreate).gameObject.BindEvent(OnClickCreateButton);
        GetButton((int)Buttons.ButtonLogin).gameObject.BindEvent(OnClickLoginButton);
        GetButton((int)Buttons.ButtonExit).gameObject.BindEvent(Util.Quit);
        GetButton((int)Buttons.ButtonCredit).gameObject.BindEvent(OnClickCreditButton);
        

        inputFieldHelper = new InputFieldHelper();
        inputFieldHelper.Add(GetObject((int)GameObjects.AccountName).GetComponent<InputField>());
        inputFieldHelper.Add(GetObject((int)GameObjects.Password).GetComponent<InputField>());
        inputFieldHelper.SetFocus(0);
    }

    private void OnClickCreditButton(PointerEventData obj)
    {
        Managers.UI.ShowPopupUI<UI_Credit>();
    }

    public void OnClickCreateButton(PointerEventData data)
    {
        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            Debug.Log(res.CreateOk);

            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

            if(res.CreateOk)
            {
                UI_NoticeBox uI_NoticeBox = Managers.UI.ShowPopupUI<UI_NoticeBox>(); // 계정 생성 성공 알림 UI
                uI_NoticeBox.Notice(1, res.CreateOk);
            }
            else
            {
                UI_NoticeBox uI_NoticeBox = Managers.UI.ShowPopupUI<UI_NoticeBox>(); // 계정 생성 실패 알림 UI
                uI_NoticeBox.Notice(1, res.CreateOk);
            }
        });  
    }
    public void OnClickLoginButton(PointerEventData data)
    {
        string account = Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text;
        string password = Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text;

        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = password
        };

        Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/login", packet, (res) =>
        {
            Get<GameObject>((int)GameObjects.AccountName).GetComponent<InputField>().text = "";
            Get<GameObject>((int)GameObjects.Password).GetComponent<InputField>().text = "";

            if (res.LoginOk)
            {
                Managers.Network.AccountId = res.AccountId;
                Managers.Network.Token = res.Token;

                UI_SelectServer popup = Managers.UI.ShowPopupUI<UI_SelectServer>();
                popup.SetServers(res.ServerList);
            }
            else
            {
                UI_NoticeBox uI_NoticeBox = Managers.UI.ShowPopupUI<UI_NoticeBox>(); // 계정 생성 실패 알림 UI
                uI_NoticeBox.Notice(2, res.LoginOk);
            }
        });
    }



    // Not Use
    //private void StartButtonClicked(PointerEventData data)
    //{
    //    string inputID = GetObject((int)GameObjects.InputFieldUsername).GetComponent<InputField>().text;
    //    string inputPW = GetObject((int)GameObjects.InputFieldPassword).GetComponent<InputField>().text;
    //    Debug.Log($"Input ID : {inputID} | PW : {inputPW}");
    //    try
    //    {
    //        if (Managers.Data.PlayerTable[inputID].PW == inputPW)
    //        {
    //            Debug.Log($"Login ID : {Managers.Data.PlayerTable[inputID].ID} | PW : {Managers.Data.PlayerTable[inputID].PW}");
    //            Managers.Scene.LoadScene(Define.Scene.InGame);
    //        }
    //        else
    //        {
    //            //Debug.Log($"ID : {Managers.Data.PlayerTable[inputID].ID} | PW : {Managers.Data.PlayerTable[inputID].PW}");
    //            Debug.Log("Wrong ID or PW");
    //        }
    //    }
    //    catch(KeyNotFoundException)
    //    {
    //        Debug.Log("Wrong ID or PW");
    //    }
    //}
    //private void EndButtonClicked(PointerEventData data)
    //{
    //    Util.Quit();
    //}
}
