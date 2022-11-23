using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArcher : Archer
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
        ui_MiniMap = GameObject.Find("@UI_Root").GetComponentInChildren<UI_NewMiniMap>();
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
            movePacket.DestInfo = DestInfo;
            movePacket.PosInfo = PosInfo;

            Managers.Network.Send(movePacket);
        }
    }

    protected override IEnumerator Archer_Q()
    {
        pos_selected = false;
        canceled = false;
        Vector3 pos;
        StartCoroutine(Show_ArrowIndicator(true,5));//range값은 5넘어가면 안됨
        while(!canceled)
        {
            while(!pos_selected)
            {
                if(Input.GetMouseButton(0))
                {
                    destination.Clear();
                    RaycastHit hit;
                    bool raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit);
                    if (!raycastHit)
                        canceled = true;
                    else
                    {              
                        pos = hit.point;
                        player.transform.forward = new Vector3(pos.x - player.transform.position.x, 0, pos.z - player.transform.position.z).normalized;
                        pos_selected = true;
                    }
                }
                else if(Input.GetMouseButton(1)||Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.E)||Input.GetKey(KeyCode.R))
                {
                    canceled = true;
                    on_skill = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
            if(pos_selected)
            {
                curState = State.Skill;
                skill = HotKey.Q;
                Ani_State_Change();
                Managers.Sound.Stop();
                Managers.Sound.Play("Sound/Skill_Sound/Archer_Q",Define.Sound.Effect, 1.0f, true);
                attackable = false;
                yield return new WaitForSeconds(1.08f);
                attackable = true;
                
                cool[0] = cool_max[0];
                on_skill = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        pos_selected = false;
        canceled = false;
    }

    
}
