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
    

    void Awake()
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
    }

    // base���� ���� �� �� �Ŵϱ� abstaract
    // �� ������ ���� �ؾ��ϴ� �۾��� �����ϴ� �Լ�
    public abstract void Clear();

}
