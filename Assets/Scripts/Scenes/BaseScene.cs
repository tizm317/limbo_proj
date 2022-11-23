using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    // ��� �� Ŭ���� �θ� ����
    // �������� ������ ������ �ؾ���
    // �� ������ ����? Ư�� ������Ʈ�� �ϸ� �ȵǰ���
    // �̰� ���� �� �̶�� ���ο� �� ����� �� ���� ��� �ʱ�ȭ ����ϵ��� ����

    // abstract class

    // �� Ÿ������ Define���� ����
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    { 
        // �� ������ �� eventSystem Ȯ�� ����
        // eventsystem �־�� UI ����
        // ������ ���� �ʱ�ȭ
        // �ִ��� üũ �� ������ �������.
        Object eventSystem = GameObject.FindObjectOfType(typeof(EventSystem));
        if (eventSystem == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        Screen.SetResolution(1920,1080,false);
        //Set_Resolution();
    }

    public void Set_Resolution()
    {
        int set_Width = 2560;
        int set_Height = 1440;
        int device_Width = Screen.width;
        int device_Height = Screen.height;

        // Screen.SetResolution(set_Width,(int)((float)device_Height/device_Width) * set_Width, false);
        // if((float)set_Width / set_Height < (float)device_Width / device_Height) // ����� �ػ󵵺� �� ū ���!
        // {
        //     float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // ���ο� �ʺ�
        //     Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, 1f); // ���ο� Rect ����
        // }
        // else // ������ �ػ� �� �� ū ���
        // {
        //     float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // ���ο� ����
        //     Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        // }
        if (set_Width > device_Width)
        {
            set_Height = (int)((float)set_Height * ((float)device_Width / (float)set_Width));
            set_Width = device_Width;
        }
        if (set_Height > device_Height)
        {
            set_Width = (int)((float)set_Width * ((float)device_Height / (float)set_Height));
            set_Height = device_Height;
        }
        //Debug.LogFormat("set_Width = {0}, set_Height = {1}", set_Width, set_Height)
        Screen.SetResolution(set_Width, set_Height, false);
        if ((float)set_Width / set_Height < (float)device_Width / device_Height) // ����� �ػ󵵺� �� ū ���!
        {
            float new_Width = ((float)set_Width / set_Height) / ((float)device_Width / device_Height); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - new_Width) / 2f, 0f, new_Width, set_Height); // ���ο� Rect ����
            //Debug.LogFormat("Current Resolution = {0} * {1}",new_Width,set_Height);
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)device_Width / device_Height) / ((float)set_Width / set_Height); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, set_Width, newHeight); // ���ο� Rect ����
            //Debug.LogFormat("Current Resolution = {0} * {1}",set_Width,newHeight);
        }
    }

    // base���� ���� �� �� �Ŵϱ� abstaract
    // �� ������ ���� �ؾ��ϴ� �۾��� �����ϴ� �Լ�
    public abstract void Clear();

}
