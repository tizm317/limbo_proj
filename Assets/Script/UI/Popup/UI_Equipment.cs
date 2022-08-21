using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Equipment : UI_Popup
{
    Transform player;
    Camera selfCam;
    float x_cam;
    float z_cam;
    float r;

    enum GameObjects
    {
        Slider,
    }

    private void Start()
    {
        Init();
    }


    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.Slider).BindEvent(onSliderDrag, Define.UIEvent.Drag);


        player = GameObject.Find("Player").transform;
        selfCam = player.Find("SelfCam").GetComponent<Camera>();
        selfCam.gameObject.SetActive(true);

        // 캠 위치,방향 초기화
        selfCam.transform.localPosition = new Vector3(0f, 1.2f, 3f);
        selfCam.transform.LookAt(player.GetChild(0)); // Hips 기준

        // r 계산
        x_cam = selfCam.transform.position.x;
        z_cam = selfCam.transform.position.z;
        r = Vector2.Distance(new Vector2(x_cam, z_cam), new Vector2(player.position.x, player.position.z));
    }

    public void onSliderDrag(PointerEventData data)
    {
        // 카메라 회전
        x_cam = selfCam.transform.position.x;
        z_cam = selfCam.transform.position.z;
        float sValue = GetObject((int)GameObjects.Slider).GetComponent<Slider>().value; // 0 ~ 1

        x_cam = player.position.x + r * Mathf.Sin(sValue * 2 * Mathf.PI);
        if (sValue > 0.25f && sValue <= 0.75f)
            z_cam = player.position.z + Mathf.Sqrt(r * r - (x_cam - player.position.x) * (x_cam - player.position.x));
        else
            z_cam = player.position.z - Mathf.Sqrt(r * r - (x_cam - player.position.x) * (x_cam - player.position.x));

        selfCam.transform.position = new Vector3(x_cam, selfCam.transform.position.y, z_cam);
        selfCam.transform.LookAt(player.GetChild(0)); // Hips 기준
    }
    private void OnDisable()
    {
        if(selfCam)
            selfCam.gameObject.SetActive(false);
    }
}
