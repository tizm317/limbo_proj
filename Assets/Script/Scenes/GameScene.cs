using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    // ���Ӿ�
    // ���Ӿ��� ������ ����
    // @Scene ������Ʈ : ������

    // �ڷ�ƾ ����
    /*
    //class Test
    //{
    //    public int Id = 0;
    //}

    //class CoroutineTest : IEnumerable
    //{
    //    public IEnumerator GetEnumerator()
    //    {
    //        //// return Ÿ�� : System.Object �̶� ���� ���� ����
    //        //// yield return : �Ͻ�����
    //        //yield return new Test() { Id = 1 }; 
    //        ////yield return null; // null �̿��ؼ� ������ �ؿ��� null üũ������ϱ���
    //        //yield break; // ��¥ �����ϴ¹�, �Ϲ� �Լ������� return;

    //        //// Unreachable
    //        //yield return new Test() { Id = 2 };
    //        //yield return new Test() { Id = 3 };
    //        //yield return new Test() { Id = 4 };

    //        for(int i = 0; i < 1000000; i++)
    //        {
    //            if (i % 10000 == 0)
    //                yield return null; // ����° ������ ��� ����
    //        }


    //    }

    //    void GenerateItem()
    //    {
    //        // �������� ������ش�
    //        // DB ����

    //        // ����
    //        // ����
    //    }
    //}

    Coroutine co; // handle ����
    */

    void Awake()
    {
        // cf) Awake : Start ���� ����, ������Ʈ �����־ ������Ʈ�� ��� ������ ����
        // ���� : ������Ʈ�� ���������� �� ��.
        Init();
    }

    protected override void Init()
    {
        // �ʱ�ȭ
        
        base.Init();

        // ��Ÿ�� ����
        SceneType = Define.Scene.InGame;

        // �� UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");
        Managers.UI.ShowSceneUI<MiniMap>("MiniMap");
        //Managers.UI.ShowSceneUI<UI_Inven>();


        // Ǯ�Ŵ��� �׽�Ʈ
        //for (int i = 0; i < 5; i++)
        //    Managers.Resource.Instantiate("UnityChan");


        /*
         * �ڷ�ƾ ����
           �ڷ�ƾ �׽�Ʈ
        CoroutineTest test = new CoroutineTest();
        foreach(System.Object t in test)
        {
            // ���⼭ �Ǵ�����
            Test value = (Test)t;
            Debug.Log(value.Id);
        }

        co = StartCoroutine(CoExplodeAfterSeconds(4.0f));
        StartCoroutine("CoStopExplode", 2.0f);
        */


        // DataManager test - �ܺο��� ����� ��
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Dictionary<int, Data.Pos> dict_pos = Managers.Data.PosDict;


        // Ŀ����Ʈ�ѷ� @Scene
        gameObject.GetOrAddComponent<CursorController>();
    }

    // �ڷ�ƾ ����
    /*
    IEnumerator CoStopExplode(float seconds)
    {
        Debug.Log("Stop Enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Stop Execute!!!");
        if(co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    IEnumerator CoExplodeAfterSeconds(float seconds)
    {
        //yield return null; // �ѹ��� ���� ���� ƽ���� �̾

        Debug.Log("Explode Enter");

        // IEnumerator �� ���ӿ�����Ʈ ��ȯ�ؼ� ���Ƿ� ������ Ŭ���� ����������
        // waitforseconds �� ����Ƽ�� ������ Ŭ����
        yield return new WaitForSeconds(seconds); // �����κ��� seconds �Ŀ� ����

        Debug.Log("Explode Execute!!");

        co = null;

    }
    */

    public override void Clear()
    {
        // �� ���� ����� �� ������� �ϴ� �κ� �־���� ��
    }



}
