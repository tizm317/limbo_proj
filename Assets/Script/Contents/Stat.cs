using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    // player, enemy 공통 필요 변수
    [SerializeField] int level;  //레벨
    [SerializeField] int hp;  //hp
    [SerializeField] int maxHp;  //최대 hp
    [SerializeField] int attack;  //공격력
    [SerializeField] int defense;  //방어력
    [SerializeField] float movespeed;  //이동하는 속도

    //tool에서 확인하기 위함
    public int Level { get { return level; } set { level = value; } }
    public int Hp { get { return hp; } set { hp = value; } }
    public int MaxHp { get { return maxHp; } set { maxHp = value; } }
    public int Attack { get { return attack; } set { attack = value; } }
    public int Defense { get { return defense; } set { defense = value; } }
    public float MoveSpeed { get { return movespeed; } set { movespeed = value; } }

}
