using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    Animator DragonAni;
    public Transform target;
    public float DragonSpeed;
    bool enableAct;  //action 스위치
    int atkSetep;  // 공격단계

    private void Start()
    {
        DragonAni = GetComponent<Animator>();
        enabled = true; 
    }

    void RotateDragon()
    {
        Vector3 dir = target.position - transform.position;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);

    }

    void MoveDragon()
    {
        if ((target.position - transform.position).magnitude >= 20)
        {
            
            DragonAni.SetBool("isWalk", true);
            transform.Translate(Vector3.forward * DragonSpeed * Time.deltaTime, Space.Self);
        }
        else if ((target.position - transform.position).magnitude < 20)
        {
            DragonAni.SetBool("isWalk", false);
        }

    }

    private void Update()
    {
        if(enableAct)
        {
            RotateDragon();
            MoveDragon();
        }
    }

    void DraronAtk()
    {
        if((target.position - transform.position).magnitude < 10)
        {
            switch (atkSetep) 
            {
                case 0:
                    atkSetep += 1;
                    DragonAni.Play("AttackA");
                    break;
                case 1:
                    atkSetep += 1;
                    DragonAni.Play("AttackB");
                    break;
                case 3:
                    atkSetep += 1;
                    DragonAni.Play("AttackC");
                    break;

            }
        }
    }

    void FreezeDragon() //공격 시 이동, 회전 정지
    {
        enableAct = false;

    }
    void UnFreezeDragon()  
    {
        enableAct = true;
    }
}
