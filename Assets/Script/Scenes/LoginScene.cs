using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    // �α��� ��
    // @Scene

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        //List<GameObject> list = new List<GameObject>();

        //// Ǯ�Ŵ��� �׽�Ʈ
        //for (int i = 0; i < 5; i++)
        //    list.Add(Managers.Resource.Instantiate("UnityChan"));

        // �ٽ� �ֱ�
        //foreach (GameObject obj in list)
        //    Managers.Resource.Destroy(obj);
    }

    private void Update()
    {
        // Ư�� Ű ������(�ϵ��ڵ�) �ٸ� ������ �̵� test
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // ����Ƽ���� �� �Ŵ��� ����
            // ��� ����
            //SceneManager.LoadScene("Game");
            // Async �迭 �Լ� : �񵿱�� �ϴ� �۾�
            // MMORPG ���� �� ���� �� ��, �� �ε��Ϸ��� �� �����ɸ�
            // �̷��� �α��� �ϴ� �����̳�, �κ� ������ �߿�, ���� �� �ε� �������ݾ� �ε��ϴ� ���
            // async �̿��ϸ� ��׶��忡�� ������ �ε� ����
            // �ε� ȭ�� ���� �� ����


            // -> �ٵ� BaseScene, GameScene, LoginScene ��� Init, Clear �� �� �߿� �۾� �ϴϱ�
            // �׷� �۾� �ѹ��� �����ϱ� ���ؼ� Scene Manager �ϳ� ���� ���� / ���û���
            // -> ���� ���� �Ŵ�����
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear");
    }

}
