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

        // �ѹ��� (���� �Ⱦ��� �ӽ�)
        if(Managers.Object.MyPlayer == null && SceneType != Define.Scene.Login && SceneType != Define.Scene.Unknown)
        {
            PlayerInfo info = new PlayerInfo() { Name = "MyPlayer", PlayerId = 0, PosInfo = new PositionInfo() };
            Managers.Object.Add(info, myPlayer: true);
        }
    }

    // base���� ���� �� �� �Ŵϱ� abstaract
    // �� ������ ���� �ؾ��ϴ� �۾��� �����ϴ� �Լ�
    public abstract void Clear();

}
