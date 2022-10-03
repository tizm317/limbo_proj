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
        //StartButton,
        //EndButton,
    }

    enum Texts
    {
        //TitleText,
        //StartText,
        //EndText,
    }

    enum GameObjects
    {
        InputFieldUsername,
        InputFieldPassword,
    }

    enum Images
    {
        //BackGroundImage
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.ButtonLogin).gameObject.BindEvent(StartButtonClicked);
        //Bind<Text>(typeof(Texts));
        //Bind<Image>(typeof(Images));

        //GetButton((int)Buttons.StartButton).gameObject.BindEvent(StartButtonClicked);
        //GetButton((int)Buttons.EndButton).gameObject.BindEvent(EndButtonClicked);

    }


    private void StartButtonClicked(PointerEventData data)
    {
        string inputID = GetObject((int)GameObjects.InputFieldUsername).GetComponent<InputField>().text;
        string inputPW = GetObject((int)GameObjects.InputFieldPassword).GetComponent<InputField>().text;
        Debug.Log($"Input ID : {inputID} | PW : {inputPW}");
        try
        {
            if (Managers.Data.PlayerTable[inputID].PW == inputPW)
            {
                Debug.Log($"Login ID : {Managers.Data.PlayerTable[inputID].ID} | PW : {Managers.Data.PlayerTable[inputID].PW}");
                Managers.Scene.LoadScene(Define.Scene.InGame);
            }
            else
            {
                //Debug.Log($"ID : {Managers.Data.PlayerTable[inputID].ID} | PW : {Managers.Data.PlayerTable[inputID].PW}");
                Debug.Log("Wrong ID or PW");
            }
        }
        catch(KeyNotFoundException)
        {
            Debug.Log("Wrong ID or PW");
        }
    }
    private void EndButtonClicked(PointerEventData data)
    {
        Util.Quit();
    }
}
