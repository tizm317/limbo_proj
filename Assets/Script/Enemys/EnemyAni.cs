using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAni : MonoBehaviour
{
    public const string MOVING = "moving";
    public const string ATTACK = "Attack";
    public const string DIE = "Die";

    Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animation>();

        
    }
    public void ChangeAni(string aniName)
    {
        anim.CrossFade(aniName);
    }
}
