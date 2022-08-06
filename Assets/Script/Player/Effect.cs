using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Effect : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject obj, grand_parent,parent;
    void Awake()
    {
        
    }
    void Start()
    {
        //StartCoroutine(Play_Effect());
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Play_Effect();
    }

    void Play_Effect()
    {
        obj = Resources.Load<GameObject>("Prefabs/Shard");
        grand_parent = GameObject.Find("Effect");
        if(grand_parent==null)
            grand_parent = new GameObject("Effect");
        parent = new GameObject("Effect" + grand_parent.transform.childCount.ToString());
        parent.transform.SetParent(grand_parent.transform);
        int count = Random.Range(15, 25);
        for(int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(obj);
            temp.transform.SetParent(parent.transform);
            temp.name = i.ToString();
            temp.transform.localScale += new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f));
            Rigidbody rigid = temp.GetComponent<Rigidbody>();
            rigid.AddForce(new Vector3(Random.Range(-0.5f,0.5f),1,Random.Range(-0.5f,0.5f)) * Random.Range(0f,1f),ForceMode.Impulse);
        }
        StartCoroutine(CameraShake(0.5f));
        StartCoroutine(Destroy_Effect(parent));
    }

    IEnumerator Destroy_Effect(GameObject p)
    {
        yield return new WaitForSeconds(2f);
        Destroy(p);
    }

    IEnumerator CameraShake(float duration)
    {
        Vector3 original_position = Camera.main.transform.position;
        float time = 0f;
        while(time < duration)
        {
            if(Camera.main.transform.position != original_position)
                Camera.main.transform.position = original_position;
            else
                Camera.main.transform.position += new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),0);
            time += 0.05f;
            Debug.Log(time);
            yield return new WaitForSeconds(0.1f);
        }
        Camera.main.transform.position = original_position;
    }
}
