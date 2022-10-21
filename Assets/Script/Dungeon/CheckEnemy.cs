using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemy : MonoBehaviour
{
    public GameObject m_PrntGo;
    //public Transform m_PrntTrans;

    [SerializeField] private GameObject[] m_ChildrenGos;
    [SerializeField] private int count =0;
    //[SerializeField] private Transform[] m_ChildrenTranses;


    void Start()
    {
        m_ChildrenGos = GetChildren(m_PrntGo);
        //m_ChildrenTranses = GetChildren(m_PrntTrans);
    }


    //public Transform[] GetChildren(Transform parent)
    //{
    //    Transform[] children = new Transform[parent.childCount];

    //    for (int i = 0; i < parent.childCount; i++)
    //    {
    //        children[i] = parent.GetChild(i);
    //    }

    //    return children;
    //}

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
            count++;
        }

        return count;
    }
}
