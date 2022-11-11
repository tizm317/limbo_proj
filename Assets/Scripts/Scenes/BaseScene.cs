using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    // 모든 씬 클래스 부모 역할
    // 누군가는 선봉대 역할을 해야함
    // 그 역할을 누가? 특정 오브젝트가 하면 안되겠지
    // 이걸 위한 씬 이라는 새로운 놈 만들고 씬 관련 모든 초기화 담당하도록 만듦

    // abstract class

    // 씬 타입으로 Define에서 관리
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    { 
        // 씬 시작할 때 eventSystem 확인 과정
        // eventsystem 있어야 UI 가능
        // 프리팹 만들어서 초기화
        // 있는지 체크 후 없으면 만들어줌.
        Object eventSystem = GameObject.FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        //Set_Resolution();
    }

    public void Set_Resolution()
    {
        int set_Width = 2560;
        int set_Height = 1440;
        int device_Width = Screen.width;
        int device_Height = Screen.height;

        // Screen.SetResolution(set_Width,(int)((float)device_Height/device_Width) * set_Width, false);
        // if((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        // {
        //     float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
        //     Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, 1f); // 새로운 Rect 적용
        // }
        // else // 게임의 해상도 비가 더 큰 경우
        // {
        //     float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
        //     Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        // }
        if (set_Width > device_Width)
        {
            set_Height = (int)(set_Height * ((float)device_Width / set_Width));
            set_Width = device_Width;
        }
        if (set_Height > device_Height)
        {
            set_Width = (int)(set_Width * ((float)device_Height / set_Height));
            set_Height = device_Height;
        }
        Debug.LogFormat("set_Width = {0}, set_Height = {1}", set_Width, set_Height);
        Screen.SetResolution(set_Width, set_Height, false);
        if ((float)set_Width / set_Height < (float)device_Width / device_Height) // 기기의 해상도비가 더 큰 경우!
        {
            float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // 새로운 너비
            Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, set_Height); // 새로운 Rect 적용
            //Debug.LogFormat("Current Resolution = {0} * {1}",new_Width,set_Height);
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, set_Width, newHeight); // 새로운 Rect 적용
            //Debug.LogFormat("Current Resolution = {0} * {1}",set_Width,newHeight);
        }
    }

    // base에서 구현 안 할 거니까 abstaract
    // 씬 날리기 전에 해야하는 작업들 수행하는 함수
    public abstract void Clear();

}
