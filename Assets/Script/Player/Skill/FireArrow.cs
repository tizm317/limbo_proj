using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour
{
    // Start is called before the first frame update
    float damage;
    PlayerStat attacker;
    [SerializeField]
    float speed;
    public void Run(PlayerStat stat, float Damage, Vector3 dir)
    {
        damage = Damage;
        attacker = stat;
        StartCoroutine(_Run(dir));
    }

    IEnumerator _Run(Vector3 dir)
    {
        float time = 0;
        while(time < 5f)
        {
            gameObject.transform.forward = dir;
            gameObject.transform.position += dir * speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.transform.tag == "Enemy")
        {
            col.gameObject.GetComponent<Stat>().OnAttacked(damage,attacker);
        }
    }
}
