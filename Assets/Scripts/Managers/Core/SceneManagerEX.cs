using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    // Unity ���� SceneManager �ֱ� ������ Extended ����
    // �� ����, �� �̵�

    // ������ BaseScene ���
    // BaseScene ������Ʈ ����ִ� ������Ʈ�� BaseScene���� ���(���ʸ�����)
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        // �� �̵�
        // ����Ƽ ���� ���������� string �޾Ҵµ�
        // �츮�� Define.Scene ���� enum �� Ȱ���ϴϱ� �ٲ�

        Managers.Clear(); // ���ʿ��� �޸� �� ����
        //LoadingScene.LoadScene(GetSceneName(type));
        // ����Ƽ ���� LoadScene �Լ� Ȱ�� - string �� �ֱ� ���� GetSceneName �Լ� ���
        SceneManager.LoadScene(GetSceneName(type)); // ���� �� �̵�  
    }

    string GetSceneName(Define.Scene type)
    {
        // Define.Scene ��(enum)�� string���� �����ϴ� �Լ�
        // ����Ƽ ���� SceneManager.LoadScene �� string �� �Է� �ޱ� ����.

        string name = System.Enum.GetName(typeof(Define.Scene), type); // ���� �̸� ���� (C# ���÷��� ��� Ȱ��)
        return name;
    }

    public void Clear()
    {
        // ���� �� Ŭ���� �۾�
        // ���� ����ϴ� �� ã�Ƽ� Ŭ����
        CurrentScene.Clear(); 
        
    }
}
