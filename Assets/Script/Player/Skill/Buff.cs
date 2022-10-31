using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public void Show_buff(float _time, GameObject _target, Sprite _img)
    {
        StartCoroutine(_buff(_time,_target,_img));
    }

    IEnumerator _buff(float _time, GameObject _target, Sprite img)
    {
        float time = _time;
        GameObject debuff;
        GameObject temp;

        debuff = new GameObject("Debuff");
        debuff.transform.SetParent(_target.transform);
        debuff.transform.localPosition = new Vector3(0,_target.GetComponent<CapsuleCollider>().height + 0.2f,0);
        List<GameObject> v = new List<GameObject>();
        for(int i = 0; i < _target.transform.childCount; i++)
        {
            if(_target.transform.GetChild(i).name == "Debuff")
                v.Add(_target.transform.GetChild(i).gameObject);
        }
        if(v.Count > 1)
        {
            Destroy(debuff);
            debuff = _target.transform.Find("Debuff").gameObject;
        }
            
        int sibligs = debuff.transform.childCount;
        temp = new GameObject("Debuff" + (sibligs + 1).ToString());
        temp.transform.SetParent(debuff.transform);
        temp.transform.localPosition = Vector3.zero;
        temp.AddComponent<SpriteRenderer>().sprite = img;
        while(time > 0)
        {
            time -= Time.deltaTime;
            if(debuff.transform.childCount > 1)
            {
                float my_index = debuff.transform.Find(temp.name).GetSiblingIndex();
                temp.transform.localPosition = new Vector3(1.28f * (my_index - debuff.transform.childCount / 2f),0,0);
            }
            else
                temp.transform.localPosition = Vector3.zero;
            debuff.transform.forward = new Vector3(0,0,-1);
            yield return new WaitForEndOfFrame();
        }
        Destroy(temp);
    }
}
