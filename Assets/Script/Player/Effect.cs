using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Effect : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject grand_parent,parent;
    public GameObject Indicator;//인디케이터 오브젝트
    public GameObject[] obj = new GameObject[12];

    public IEnumerator Show_Indicator(bool body, bool pos_selected, float rad, float range)
    {
        Indicator.SetActive(true);
        Indicator.GetComponent<MeshRenderer>().material.SetFloat("Angle",rad);
        float _range = range * 2;
        Indicator.transform.localScale = new Vector3(_range,_range,_range);
        if(body)
        {
            while(!pos_selected)
            { 
                Indicator.transform.position = new Vector3(this.transform.position.x,0,this.transform.position.z);
                Indicator.transform.LookAt(Input.mousePosition);
                yield return new WaitForEndOfFrame();
            }   
        }
        else
        {
            while(!pos_selected)
            {
                Indicator.transform.position = Input.mousePosition;
                yield return new WaitForEndOfFrame();
            }  
        }
    }

    public void Play_Effect()
    {
        obj = Resources.LoadAll<GameObject>("Prefabs/Effect/Crust");
        grand_parent = GameObject.Find("Effect_Manager");
        if(grand_parent==null)
        {
            grand_parent = new GameObject("Effect_Manager");
            //grand_parent.transform.SetParent(GameObject.Find("Player").gameObject.GetComponent<Transform>());
            //grand_parent.transform.position = GameObject.Find("Player").gameObject.GetComponent<Transform>().position;
        }
        parent = new GameObject("Effect" + grand_parent.transform.childCount.ToString());
        parent.transform.SetParent(grand_parent.transform);
        parent.transform.position = GameObject.Find("Player").GetComponent<Transform>().position;
        int count = Random.Range(15, 25);
        for(int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(obj[(int)Random.Range(0,12)]);
            temp.transform.SetParent(parent.transform);
            temp.transform.position = parent.transform.position;
            temp.name = i.ToString();
            temp.transform.localScale = new Vector3(Random.Range(0,0.05f),Random.Range(0,0.05f),Random.Range(0,0.05f));
            Rigidbody rigid = temp.GetComponent<Rigidbody>();
            rigid.AddForce(new Vector3(Random.Range(-1f,1f),0.5f,Random.Range(-1f,1f)) * Random.Range(0f,1f),ForceMode.Impulse);
        }
        StartCoroutine(CameraShake(1f,parent));
    }

    IEnumerator CameraShake(float duration,GameObject p)
    {
        Vector3 original_position = Camera.main.transform.position;
        float time = 0f;
        float range = 0.2f;
        while(time < duration)
        {
            Vector3 new_pos = original_position;
            if(Camera.main.transform.position != original_position)
                new_pos = original_position;
            else
                new_pos = original_position +new Vector3((Random.Range(-1f,1)>0)?range:-range,(Random.Range(-1f,1)>0)?range:-range,0);
            Camera.main.transform.position = new_pos;
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;           
        }
        
        if(Camera.main.transform.position != original_position)
        {
            Camera.main.transform.position = original_position;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(p);
    }

    
    
}
