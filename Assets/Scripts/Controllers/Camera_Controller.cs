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

    [SerializeField]
    public bool camera_control = true;

    //�̴ϸʿ� ī�޶�
    //Camera minimapCam;


    void Start()
    {
        SetQuarterView(_delta);
        Set_Resolution();
        //minimapCam = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
    }
    public void SetTarget(GameObject target)
    {
        _player = target;
    }

    void LateUpdate()
    {
        
        if(camera_control)
        {
            
            if(_player == null || !_player.activeSelf)
            {
                return;
            }
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
        }
        // �̴ϸ� ī�޶� �÷��̾� ����ٴϰ�
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

    public void FOV_Control(int value)
    {
        Camera cam = this.gameObject.GetComponent<Camera>();
        if (Mathf.Abs(cam.fieldOfView - value) > 1.0f)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, value, Time.deltaTime * 2);
    }

    public void Set_Resolution()
    {
        int set_Width = 1920;
        int set_Height = 1080;
        int device_Width = Screen.width;
        int device_Height = Screen.height;

        Screen.SetResolution(set_Width,(int)((float)device_Height/device_Width) * set_Width, true);
        if((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        {
            float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
            Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
}
