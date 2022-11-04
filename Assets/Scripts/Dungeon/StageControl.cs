using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageControl : MonoBehaviour
{
    public GameObject m_PrntGo;
    public Transform m_PrntTrans;

    public int curNumber = 0;
    public int curStage = 0;

    [SerializeField] private GameObject[] m_ChildrenGos;
    [SerializeField] private Transform[] m_ChildrenTranses;
    public StageManager m_StageManager;

    void Start()
    {
        m_ChildrenGos = GetChildren(m_PrntGo);
        m_ChildrenTranses = GetChildren(m_PrntTrans);

        curNumber = m_ChildrenGos.Length;
    }
    void Update()
    {
        m_ChildrenGos = GetChildren(m_PrntGo);
        if (m_ChildrenGos.Length != curNumber)
        {
            curNumber = m_ChildrenGos.Length;
        }
        if (m_ChildrenGos.Length == 0)
        {
            Debug.Log("clear");
            m_StageManager.NextsStage();
        }

    }

    public Transform[] GetChildren(Transform parent)
    {
        Transform[] children = new Transform[parent.childCount];

        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        return children;
    }

    public GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }

        return children;
    }
}
