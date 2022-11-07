using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    // �α��� ��
    // @Scene

    UI_Login _sceneUI;

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        SceneType = Define.Scene.Login;
        base.Init();

        Managers.Web.BaseUrl = "https://localhost:5001/api";

        Screen.SetResolution(640, 480, false);

        // �� UI
        _sceneUI = Managers.UI.ShowSceneUI<UI_Login>("UI_Login");

        // Login Scene BGM
        Managers.Sound.Play("Sound/BGM/BGM_Ambient_Version", Define.Sound.Bgm);

        //List<GameObject> list = new List<GameObject>();

        // Ǯ�Ŵ��� �׽�Ʈ
        //for (int i = 0; i < 5; i++)
            //Managers.Resource.Instantiate("Enemy_Skeleton");
            //list.Add(Managers.Resource.Instantiate("Enemy_Skeleton"));

        // �ٽ� �ֱ�
        //foreach (GameObject obj in list)
        //    Managers.Resource.Destroy(obj);
    }

    private void Update()
    {
        // Ư�� Ű ������(�ϵ��ڵ�) �ٸ� ������ �̵� test
        //if(Input.GetKeyDown(KeyCode.Q))
        //{
        //    // ����Ƽ���� �� �Ŵ��� ����
        //    // ��� ����
        //    //SceneManager.LoadScene("Game");
        //    // Async �迭 �Լ� : �񵿱�� �ϴ� �۾�
        //    // MMORPG ���� �� ���� �� ��, �� �ε��Ϸ��� �� �����ɸ�
        //    // �̷��� �α��� �ϴ� �����̳�, �κ� ������ �߿�, ���� �� �ε� �������ݾ� �ε��ϴ� ���
        //    // async �̿��ϸ� ��׶��忡�� ������ �ε� ����
        //    // �ε� ȭ�� ���� �� ����


        //    // -> �ٵ� BaseScene, GameScene, LoginScene ��� Init, Clear �� �� �߿� �۾� �ϴϱ�
        //    // �׷� �۾� �ѹ��� �����ϱ� ���ؼ� Scene Manager �ϳ� ���� ���� / ���û���
        //    // -> ���� ���� �Ŵ�����
        //    Managers.Scene.LoadScene(Define.Scene.InGame);
        //}
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear");
    }

}
