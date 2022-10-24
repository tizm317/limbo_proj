using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    float time;
    public GameObject start, end, obj;
    public float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > 1)
        {
            StartCoroutine(_Attack(obj));
            time = 0f;
        }
    }
    //원거리 공격 테스트용 코드, 코루틴을 써야할까, 아니면 오브젝트에 스크립트를 집어넣고 업데이트로 돌려야할까..?

    void Attack()
    {
        GameObject temp = Instantiate(obj);
    }

    IEnumerator _Attack(GameObject arrow)
    {
        GameObject temp = Instantiate(arrow);
        temp.transform.position = start.transform.position;
         
        float time = 0;
        bool touched = true;
        while(touched)
        {
            yield return new WaitForEndOfFrame();
            Vector3 dir = (end.transform.position - temp.transform.position).normalized;
            temp.transform.position += dir*Time.deltaTime*speed;
            temp.transform.up = dir.normalized;
            time += Time.deltaTime;
            if(Vector3.Distance(temp.transform.position, end.transform.position) < 0.5 || time > 15f)
            {
                Destroy(temp);
                Debug.Log("hit");
                touched = false;
            }
        }
    }

}
