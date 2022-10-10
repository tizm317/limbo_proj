using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float check;
    bool op;

    public float speed = 5;
    // Update is called once per frame
    void Update()
    {
        check += Time.deltaTime;
        if(check > 5)
        {
            op = !op;
            check = 0;
        }
        if(op)
        {
            gameObject.transform.position += (new Vector3(Time.deltaTime, 0 ,0) * speed);
        }
        else
        {
            gameObject.transform.position -= (new Vector3(Time.deltaTime, 0 ,0) * speed);
        }
    }
}
