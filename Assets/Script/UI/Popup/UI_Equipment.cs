using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Equipment : UI_Popup
{
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
    }

    public void onSliderDrag(PointerEventData data)
    {
        Transform player = GameObject.Find("Player").transform;
        Camera selfCam = player.gameObject.GetComponentInChildren<Camera>();
        float x_cam = selfCam.transform.position.x;
        float z_cam = selfCam.transform.position.z;

        float r = Vector2.Distance(new Vector2(x_cam, z_cam), new Vector2(player.position.x, player.position.z));

        float sValue = GetObject((int)GameObjects.Slider).GetComponent<Slider>().value; // 0 ~ 1
        // -r r -r
        if (sValue < 0.5f)
        {
            x_cam = player.position.x + sValue * 4 * r - r;
            z_cam = player.position.z + Mathf.Sqrt(r * r - (x_cam - player.position.x) * (x_cam - player.position.x));
        }
        else
        {
            x_cam = player.position.x + 3 * r - sValue * 4 * r;
            z_cam = player.position.z - Mathf.Sqrt(r * r - (x_cam - player.position.x) * (x_cam - player.position.x));
        }

        selfCam.transform.position = new Vector3(x_cam, selfCam.transform.position.y, z_cam);
        selfCam.transform.LookAt(player.GetChild(0)); // Hips 기준
    }
}
