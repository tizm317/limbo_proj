using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWarrior : Warrior
{
    #region 코루틴 Wrapper 메소드
    // predicate 조건 불충족하면, 대기함
    private void ProcessLater(Func<bool> predicate, Action job)
    {
        StartCoroutine(PorcessLaterRoutine());

        // Local
        IEnumerator PorcessLaterRoutine()
        {
            yield return new WaitUntil(predicate);
            job?.Invoke();
        }
    }
    #endregion

    protected override void Init()
    {
        base.Init();

        GetIndicator();

        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

        Managers.Input.KeyAction -= OnKeyClicked;
        Managers.Input.KeyAction += OnKeyClicked;

        // 인게임 씬 전에 Init한번하는데? -> null 이면 나중에 다시 Init 실행
        cam = Camera.main;
        Camera_Controller controller = cam.GetComponent<Camera_Controller>();
        if (cam == null || controller == null)
        {
            // player 가 아직 생성 전이면, 생긴 이후에 다시 Init하도록 코루팀으로 대기함
            ProcessLater(() => Camera.main != null, () => Init());
            return;
        }
        controller.SetTarget(this.gameObject);


        // 미니맵
        //ui_MiniMap = GameObject.Find("@UI_Root").GetComponentInChildren<UI_MiniMap>();
    }

    protected override void Move()
    {
        State prevState = curState;
        Vector3 prevDest = Dest;
        //List<Vector3> prevDest = Destination;

        base.Move();

        // State가 변하거나, 위치가 변하면 패킷 보냄
        if(prevState != curState || Dest != prevDest)
        //if(Dest != prevDest)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;

            //foreach (var v in dest)
            //{
            //    movePacket.Destinations.Add(v);
            //}

            Managers.Network.Send(movePacket);
        }
    }
}
