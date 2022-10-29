using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWarrior : Warrior
{
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
        State prevState = curState;
        Vector3 prevPos = Pos;

        base.Move();

        // State가 변하거나, 위치가 변하면 패킷 보냄
        if(prevState != curState || Pos != prevPos)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
        }
    }
}
