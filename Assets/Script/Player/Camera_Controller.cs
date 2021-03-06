using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f,6.0f,-5.0f);

    [SerializeField]
    GameObject _player = null;

    [SerializeField]
    Vector3 _yPos = new Vector3(0.0f, 1.0f, 0.0f);

    //미니맵용 카메라
    //Camera minimapCam;


    void Start()
    {
        _player = GameObject.Find("Player");
        SetQuarterView(_delta);
        //minimapCam = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
    }

    void LateUpdate()
    {
        wheel_Control(5);
        if(_mode == Define.CameraMode.QuarterView)
        {
            RaycastHit hit;
            if(Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + _delta.normalized * dist + _yPos;
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
        }

        // 미니맵 카메라 플레이어 따라다니게
       //minimapCam.transform.position = new Vector3(_player.transform.position.x, minimapCam.transform.position.y, _player.transform.position.z);
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }

    void wheel_Control(float speed)
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if((_delta.z < -5 && wheel > 0) || (_delta.z > -15 && wheel < 0))
            _delta += new Vector3(0, -1.2f * wheel * speed ,wheel * speed);
    }
}
