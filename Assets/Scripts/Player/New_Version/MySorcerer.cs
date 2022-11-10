using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySorcerer : Sorcerer
{
    // 내가 조종하는 소서러 클래스
    protected override void Init()
    {
        base.Init();

        GetIndicator();

        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        Managers.Input.KeyAction -= OnKeyClicked;
        Managers.Input.KeyAction += OnKeyClicked;

        cam = Camera.main;
        cam.GetComponent<Camera_Controller>().SetTarget(this.gameObject);

        // 미니맵
        //ui_MiniMap = GameObject.Find("@UI_Root").GetComponentInChildren<UI_MiniMap>();
    }

    protected override void Move()
    {
        // 서버에 패킷 보냄
        // 이전 상태, 이전 목적지 위치
        State prevState = curState;
        Vector3 prevDest = Dest;

        base.Move(); // 이동하는 부분

        // State가 변하거나, 목적지 위치가 변하면 서버에 패킷 보냄
        if (prevState != curState || Dest != prevDest)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;

            Managers.Network.Send(movePacket);
        }
    }
}
