﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStat : Stat
{

    // player 필요 변수
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;
    [SerializeField] protected float _next_level_up;
    [SerializeField] protected float _regeneration;
    [SerializeField] protected float _mana;
    [SerializeField] protected float _max_mana;
    [SerializeField] protected float _mana_regeneration;
    public int Exp { get { return _exp; } set { _exp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }
    private bool level_up = true;
    public float next_level_up {get {return _next_level_up; } set { _next_level_up = value; }}
    public float current_exp;
    public float Regeneration { get { return _regeneration; } set { _regeneration = value; }}
    public float Mana { get { return _mana; } set { _mana = value; }}
    public float MaxMana{ get { return _max_mana; } set { _max_mana = value; } }
    public float Mana_Regeneration { get { return _mana_regeneration; } set { _mana_regeneration = value; }}
    public int STR,DEX,INT,LUC;
    public int Item_Hp, Item_Regeneration, Item_Attack, Item_MoveSpeed, Item_AttackSpeed, Item_Mana, Item_Mana_Regeneration;
    public float Item_Hp_percent, Item_Attack_percent, Item_Mana_percent, Item_Mana_Regeneration_percent;
    float time;

    void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 20.0f;
        _defense = 5;
        _moveSpeed = 6.0f;
        _turnSpeed = 20.0f;
        _exp = 0;
        _gold = 0;
<<<<<<< HEAD
=======
        HP_bar = GameObject.Find("Fill").GetComponent<Image>();
>>>>>>> 7aaf810b61289cdbc68ec6f4f6c223690e4fd660
        Level_Update();
        time = 0;
        STR = 5;
    }

    void Update()
    {
        HP_Update();
        Level_Update();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Potal")
            Managers.Scene.LoadScene(Define.Scene.Village);
    }

    void HP_Update()
    {
        time += Time.deltaTime;
        if(_hp < 0)
        {
            Player ps = GameObject.FindObjectOfType<Player>();
            ps.curState = Player.State.STATE_DIE;
            ps.Ani_State_Change();
        }
        else if(_hp > MaxHp)
        {
            _hp = MaxHp;
        }
        else if(_hp < MaxHp && time >= 1)
        {
            _hp += Regeneration;
            time = 0;
        }
    }

    void Level_Update()
    {
        if(level_up)
        {
            level_up = false;
            current_exp = next_level_up;
            if(Level < 17)
                next_level_up = (Level) * (Level) + 6 *(Level);
            else
                next_level_up = (2.5f * (Level+1) * (Level+1)) - (40.5f * (Level+1)) + 360f;
        }
        if(Exp >= next_level_up)
        {
            Level++;
            level_up = true;
            Stat_Update();
        }
    }

    void Stat_Update()
    {
        MaxHp  = (Level * STR + Item_Hp + 100) * (1 + Item_Hp_percent);
        Hp = MaxHp;
        Regeneration = (Level * STR + Item_Regeneration + 1) * 0.01f;
        Attack = ((0.5f * Level * STR) + Item_Attack) * (1 + Item_Attack_percent)+20;
        MoveSpeed = 4 + (1 / 40) * (DEX -20) + Item_MoveSpeed;
        AttackSpeed = 2.6f - (1 / 50) * (DEX - 20) - Item_AttackSpeed;
        MaxMana = 0.5f * (Level * INT + Item_Mana + 100) * (1 + Item_Mana_percent);
        Mana = MaxMana;
        Mana_Regeneration = 0.5f * ((Level * INT) * 0.1f + Item_Regeneration + 1) * (1 + Item_Mana_Regeneration_percent);
    }

    void Stat_Change(string name, int num)
    {
        switch(name)
        {
            case "STR" :
                STR += num;
                break;
            case "DEX" :
                DEX += num;
                break;
            case "INT" :
                INT += num;
                break;
            case "LUC" :
                LUC += num;
                break;
            default:
                break;
        }
        Stat_Update();
    }

    protected override void OnDead(Stat attacker)
    {
        //동작 없음
    }
}
