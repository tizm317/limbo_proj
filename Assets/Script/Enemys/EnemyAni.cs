using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAni : MonoBehaviour
{
    public const string WALK = "walk";
    public const string ATTACK = "attack1";
    public const string DIE = "death1";

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
