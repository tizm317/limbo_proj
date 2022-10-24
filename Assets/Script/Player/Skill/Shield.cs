using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
            gameObject.GetComponent<MeshCollider>().isTrigger = true; 
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Enemy"))
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
    }
}
