using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    // Ǯ�Ŵ��� : ���ҽ� �Ŵ��� �����ϴ� ����
    // ������Ʈ Ǯ��

    // ���ҽ� ������ �޸𸮿� �ҷ�����, �� �޸𸮿� �ִ� ���� instantiate �ϴ� �۾��� �ſ� ����
    // ������ : ������Ʈ Ǯ��
    // ���� ����ϴ� ������Ʈ�� ���ΰ�, �ʿ��� �� �Ѽ� ���(���� ������, �Ⱥ��̰�)

    // Ǯ���� ��� ��� ����? ������Ʈ(Poolable) �̿��ؼ� ã��
    // ������ ������ Ǯ��

    // Ǯ�Ŵ����� ������ Ǯ ���� -> class Pool
    // Ǯ�Ŵ��� : ��ųʸ� �̿��ؼ� Ǯ ��� ������� -> Dictionary<string, Pool> _pool
    // �� ������Ʈ �������� ������ �����ϱ� ����

    #region Pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        // ����
        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            // �ʱ�ȭ

            Original = original;

            // @Pool_Root ���� �� ������Ʈ�� ��Ʈ
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root"; 

            // �ʱ�ȭ�� �� count ������ŭ ��� ����
            for(int i = 0; i < count; i++)
                Push(Create()); // create ���ڸ��� push
        }

        Poolable Create()
        {
            // ���ο� ��ü ���� �� ��ȯ 

            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name; // �̸����� (Clone) ����
            return go.GetOrAddComponent<Poolable>(); // Poolable ������Ʈ getOrAdd
        }

        public void Push(Poolable poolable)
        {
            // _poolStack �� Ǫ���ϴ� �Լ�
            // Ǫ���ϱ� ���� ������� �۾��� : parent ����, ��Ȱ��ȭ , IsUsing = false;

            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            // _poolStack ���� Pop �ϴ� �Լ�
            // �� ���Ŀ� ���ִ� �۾��� : parent ����, Ȱ��ȭ, IsUsing = true;

            Poolable poolable;

            // _poolStack�� ������ (������ ������) Pop�ؼ� ������
            // ������ �� ��ü Create��
            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            // Ȱ��ȭ
            poolable.gameObject.SetActive(true);

            // DontDestroyOnLoad ���� �뵵
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            
            // parent ����
            poolable.transform.parent = parent;
            // IsUsing
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion


    // Ǯ�Ŵ��� : ��ųʸ��� Pool ����
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();  // �������� pool �����ϴ� ��ųʸ�
    
    Transform _root; // cf) GameObject �� ��� ���� (�� Transform�� �׻� ���ԵǴ� ������Ʈ��)

    public void Init()
    {
        // �ʱ�ȭ
        // �ڽ��� ��Ʈ �����, �� ��Ʈ�� Ǯ�� �ʿ��� ������Ʈ ��� �ְ� ���� (���� ����)

        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        // Ǯ�Ŵ����� Ǯ ó�� ����� �Լ� (��ųʸ��� �ִ� �Լ�)

        // Pool ���� ����
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root; // @Pool_Root�� ���� 

        // Pool �� Ǯ�Ŵ��� ��ųʸ��� �ֱ�
        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        // Pool ���� ����ִ� �Լ�
        // ��ǲ : Poolable ������Ʈ ���� ������Ʈ
        // �� ����ϰ� Ǯ�� ��ȯ�ϴ� �Լ�

        string name = poolable.gameObject.name;

        // pool �� ���µ� Ǫ�� �ϴ� ��Ȳ? (Ư���� ��� ex : ������ �󿡼� �巡�� ������� ���� ���)
        if(_pool.ContainsKey(name) == false)
        {
            // �׳� ������
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        
        // Ǯ �Ŵ����� name���� ã�Ƽ� �ش� Ǯ�� Ǫ��(�ݳ�)
        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        // ����ϱ� ���� Ǯ�Ŵ��� ��ųʸ����� Pool ã�� �� Pop�ϴ� �Լ�
        // Ǯ���� ������Ʈ ������ �ٷ� ����ϴ� �Լ�

        // original�� �̸��� string key�� ���

        // (ó�� �õ��� ���) Ǯ�Ŵ����� orginal.name �� �ش��ϴ� Pool ������, Create �Ѵ�.
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        // Ǯ�Ŵ������� orignal.name ���� ��ųʸ����� Pool ã�Ƽ� Pop�Լ��� Poolable ����
        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        // Ǯ�Ŵ���(��ųʸ�)���� �������� ��ȯ���ִ� �Լ�

        // Ǯ�Ŵ����� ���� ��� null ��ȯ
        if (_pool.ContainsKey(name) == false)
            return null;

        // Ǯ�Ŵ����� �����ϴ� ���, ��ųʸ����� ã�Ƽ� ����
        return _pool[name].Original;
    }

    public void Clear()
    {
        // �� �̵��� ��...
        // pool ������ �ϳ�? �����ؾ� �ϳ�?
        // ��κ��� ���ӿ��� ���� ���� �ʿ�� ���µ�
        // mmoRPG ���� �������� ������Ʈ�� ���� �ٸ� ��� ������?
        // �ϴ� �������
        
        // @Pool_Root(_root) ���� child ��� �����
        foreach (Transform child in _root)
        {
            GameObject.Destroy(child.gameObject);

            // Ǯ �Ŵ����� �ʱ�ȭ
            _pool.Clear();
        }
    }
}
