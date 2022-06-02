using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Controller : MonoBehaviour
{
    //public int enemyHP = 300;

    public Animator anim;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void TakeDamage(int damageAmount)
    {
        //재작성 필요
        /*
        enemyHP -= damageAmount;
        if (enemyHP <= 0)
        {
            anim.SetTrigger("death");
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            anim.SetTrigger("damage");

        }
        */
    }
    
}
