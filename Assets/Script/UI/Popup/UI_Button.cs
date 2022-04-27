using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UI_Button : UI_Popup
{
    // 버튼용

    // enum 이용해서
    // button, text 목록 정리해두고
    // 연결해줌
    // 4가지 : 버튼. 텍스트, 게임오브젝트, 이미지


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

        Bind<Button>(typeof(Buttons)); // Buttons 의 enum 타입을 넘기겠다 는 의미
        Bind<Text>(typeof(Texts)); // Relection 이용해서 enum 넘겨줌. enum 이름에 typeof 사용해서 넘겨줌 => 정확히 Texts를 넘긴거는 아니고, 이런 형식(이 enum 타입)을 넘기겠다고 호출한 것.
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        // 클릭 event 추가 과정 (한줄로 처리하는 ExtensionMethod 활용)
        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);

        // 드래그 event 추가 과정 (기존 방법)
        // 1. go 찾기 
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        // 2. 포인터이벤트데이터 받으면 {...} 반환하는 람다함수 를 go 와 연결 (Drag type)
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag); // {...} 내용 : 누른 go를 드래그하기 위함
    }

    //int _score = 0;

    public void OnButtonClicked(PointerEventData data)
    {

    }

    //public void OnButtonClicked(PointerEventData data)
    //{
    //    //_score++;

    //    //GetText((int)Texts.ScoreText).text = $"점수 : {_score}";

    //    Debug.Log("dd");
    //}

}

