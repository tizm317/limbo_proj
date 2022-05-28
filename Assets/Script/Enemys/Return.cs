using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : MonoBehaviour
{
    private ObjectPool objectPool;

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    private void OnDisable()
    {
        if (objectPool != null)
            objectPool.ReturnEnemy(this.gameObject);
    }
}
