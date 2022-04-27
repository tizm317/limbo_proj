using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f,6.0f,-5.0f);

    [SerializeField]
    GameObject _player = null;

    [SerializeField]
    Vector3 _yPos = new Vector3(0.0f, 1.0f, 0.0f); // Player Position 발바닥이어서 조정값

    void Start()
    {
        
    }

    void LateUpdate()
    {
        // 덜덜 떨리는 문제?
        // why? 카메라컨트롤러의 좌표세팅을 update에 했기 때문
        // playerController 랑 카메라컨트롤러 둘 다 update문에 둠
        // 누가 먼저 실행될지 알수없음
        // 이게 나중 -> LateUpdate() 로 바꿔

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

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
