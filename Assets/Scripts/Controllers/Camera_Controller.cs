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

    
}
