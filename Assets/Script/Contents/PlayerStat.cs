using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{

    // player 필요 변수
    [SerializeField] int exp;

    public int Exp { get { return exp; } set { exp = value; } }
}
