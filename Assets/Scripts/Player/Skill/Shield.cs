using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
            col.transform.gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 2,ForceMode.Impulse);
            Debug.LogFormat("OnCollisionEnter && tag == {0}",col.gameObject.tag);
        }
        else
            gameObject.GetComponent<MeshCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Enemy"))
        {
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
        }
        else
            gameObject.GetComponent<MeshCollider>().isTrigger = true;
       
    }
}
