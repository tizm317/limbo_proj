using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    // Start is called before the first frame update
    Effect effect;
    public enum Job
    {
        Warrior,
        Archer,
        Magician,
    }
    static Job my_job;
    void Start()
    {
        effect = GameObject.Find("Player").GetComponent<Effect>();
        my_job = Job.Warrior;
    }

    public void Q()
    {
        switch(my_job)
        {
            case Job.Warrior :
                Warrior_Q();
                break;
        }
    }

    public void W()
    {

    }

    public void E()
    {

    }

    public void R()
    {

    }

    void Warrior_Q()
    {
        Debug.Log("ㅗ");
    }

}
