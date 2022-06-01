using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStat : Stat
{
    private void Start()
    {
        _level = 1;
        _hp = 300;
        _maxHp = 300;
        _attack = 20;
        _defense = 5;

    }
}
