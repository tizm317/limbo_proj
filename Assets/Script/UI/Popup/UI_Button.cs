using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UI_Button : UI_Popup
{
    // ��ư��

    // enum �̿��ؼ�
    // button, text ��� �����صΰ�
    // ��������
    // 4���� : ��ư. �ؽ�Ʈ, ���ӿ�����Ʈ, �̹���


    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
        //ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons)); // Buttons �� enum Ÿ���� �ѱ�ڴ� �� �ǹ�
        Bind<Text>(typeof(Texts)); // Relection �̿��ؼ� enum �Ѱ���. enum �̸��� typeof ����ؼ� �Ѱ��� => ��Ȯ�� Texts�� �ѱ�Ŵ� �ƴϰ�, �̷� ����(�� enum Ÿ��)�� �ѱ�ڴٰ� ȣ���� ��.
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        // Ŭ�� event �߰� ���� (���ٷ� ó���ϴ� ExtensionMethod Ȱ��)
        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);

        // �巡�� event �߰� ���� (���� ���)
        // 1. go ã�� 
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        // 2. �������̺�Ʈ������ ������ {...} ��ȯ�ϴ� �����Լ� �� go �� ���� (Drag type)
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag); // {...} ���� : ���� go�� �巡���ϱ� ����
    }

    //int _score = 0;

    public void OnButtonClicked(PointerEventData data)
    {

    }

    //public void OnButtonClicked(PointerEventData data)
    //{
    //    //_score++;

    //    //GetText((int)Texts.ScoreText).text = $"���� : {_score}";

    //    Debug.Log("dd");
    //}

}

