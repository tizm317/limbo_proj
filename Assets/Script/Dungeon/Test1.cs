using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject m_PrntGo;
    public Transform m_PrntTrans;
    public int m_count;

    [SerializeField] private GameObject[] m_ChildrenGos;
    [SerializeField] private Transform[] m_ChildrenTranses;
    [SerializeField] private int count1, count2, result = 0;

    void Start()
    {
        m_ChildrenGos = GetChildren(m_PrntGo);
        m_ChildrenTranses = GetChildren(m_PrntTrans);
        m_count = GetChildrenCount(m_PrntGo);
    }
    void update()
    {
        Debug.Log("update 중");
        m_count = GetChildrenCount(m_PrntGo);

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
    public int GetChildrenCount(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
            count1++;
        }
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if(children[i] == null && children[i].activeSelf == false)
            {
                count2++;
            }
        }
        result = count1 - count2;
        return result;
    }
}
