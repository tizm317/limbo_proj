using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Google.Protobuf.Protocol;

public class PlayerStat : Stat
{

    // player 필요 변수
    [SerializeField] protected int _exp;
    [SerializeField] protected uint _gold;
    [SerializeField] public float _next_level_up;
    [SerializeField] protected float _regeneration;
    [SerializeField] protected float _mana;
    [SerializeField] protected float _max_mana;
    [SerializeField] protected float _mana_regeneration;
    [SerializeField] protected int _skill_point;
    [SerializeField] protected int _stat_point;
    public int Exp { get { return _exp; } set { _exp = value; } }
    public uint Gold { get { return _gold; } set { _gold = value; } }
    public bool level_up = true;
    public float next_level_up {get {return _next_level_up; } set { _next_level_up = value; }}
    public float current_exp;
    public float Regeneration { get { return _regeneration; } set { _regeneration = value; }}
    public float Mana { get { return _mana; } set { _mana = value; }}
    public float MaxMana{ get { return _max_mana; } set { _max_mana = value; } }
    public float Mana_Regeneration { get { return _mana_regeneration; } set { _mana_regeneration = value; }}
    public int Skill_Point { get { return _skill_point; }  set { _skill_point = value; } }
    public int Stat_Point { get { return _stat_point; }  set { _stat_point = value; } }
    private int max_skill_point = 16; // 4(qwer) * 4렙
    public int MaxStatPoint { get { return max_stat_point; } }
    private int max_stat_point = 160; // 1~16 * 5 = 80
    public int STR,DEX,INT,LUC;
    public float Item_Hp, Item_Regeneration, Item_Attack, Item_MoveSpeed, Item_AttackSpeed, Item_Mana, Item_Mana_Regeneration;
    public float Item_Hp_percent, Item_Attack_percent, Item_Mana_percent;
    float time;
    public bool isDead = false;
    private Player ps;
    public Define.Job my_job;

    public Item[] _items;
    public List<Data.Quest> _myQuests;

    void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 50.0f;
        _defense = 5;
        _moveSpeed = 6.0f;
        _turnSpeed = 20.0f;
        _exp = 0;
        _gold = 100000; // 테스트용 1렙 돈 제공
        _skill_point = 0;
        _stat_point = 5; // 1렙 기본 제공
        Level_Update();
        time = 0;
        ps = gameObject.GetComponent<Player>();
        my_job = ps.my_job;
        switch(my_job)
        {
            case Define.Job.WARRIOR :
                STR = 20;
                DEX = 20;
                INT = 20;
                break;
            case Define.Job.ARCHER :
                STR = 20;
                DEX = 25;
                INT = 15;
                break;
            case Define.Job.SORCERER :
                STR = 15;
                DEX = 20;
                INT = 25;
                break;

        }
        Stat_Update();

        
    }

    void Update()
    {
        HPMP_Update();
        Level_Update();
    }


    void HPMP_Update()
    {
        time += Time.deltaTime;
        if(_hp <= 0)
        {
            isDead = true;
            
            ps.curState = State.Die;
            ps.Ani_State_Change();
        }
        if(time >= 1 && !isDead)
        {
            if(_hp < MaxHp)
            {
                _hp += Regeneration;
            }
            if(_hp > MaxHp)
            {
                _hp = MaxHp;
            }
            if(_mana < MaxMana)
            {
                _mana += Mana_Regeneration;
            }
            if(_mana > MaxMana)
            {
                _mana = MaxMana;
            }
            time = 0;
        }
    }

    public void Level_Update()
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
            if(Skill_Point < max_skill_point && Level % 2 == 0)
            {
                // 2레벨업 당 1 스킬 포인트
                Skill_Point++;
            }
            level_up = true;
            Stat_Update();
            
            // 17렙까지?
            if(Level < 32)
                _stat_point += 5; // 1레벨업 당 5 스킬 포인트
            
            if(_UI_Stat)
                _UI_Stat.UpdateStat(this); // 레벨업 할 때 갱신해줌
        }
    }



    void Stat_Update()
    {
        MaxHp  = (Level * STR + Item_Hp + 100) * (1 + Item_Hp_percent);
        Hp = MaxHp;
        Regeneration = (Level * STR + Item_Regeneration + 1) * 0.01f;
        Attack = ((0.5f * Level * STR) + Item_Attack) * (1 + Item_Attack_percent)+20;
        MoveSpeed = 4 + (1.0f / 40) * (DEX -20) + Item_MoveSpeed;
        AttackSpeed = 1 + (1.0f / 50) * (DEX - 20) + Item_AttackSpeed;
        MaxMana = 0.5f * (Level * INT + Item_Mana + 100) * (1 + Item_Mana_percent);
        Mana = MaxMana;
        Mana_Regeneration = 0.5f * (Level * INT + Item_Mana + 100) * (1 + Item_Mana_percent) * (0.1f + Item_Mana_Regeneration);
    }

    public override void OnAttacked(Stat attacker)
    {

        float damage = Mathf.Max(0, attacker.Attack - Defense);
        if (ps.attackable == true)
            Hp -= damage; //나의 hp에서 demage 만큼 깎는다
        if (Hp <= 0)  //음수 경우 hp = 0;
        {
            Hp = 0;  //내가 죽었을 경우
            OnDead(attacker);
        }
    }

    public void Stat_Change(Define.Stat stat, int num)
    {
        switch(stat)
        {
            case Define.Stat.STR :
                STR = num;
                break;
            case Define.Stat.DEX :
                DEX = num;
                break;
            case Define.Stat.INT :
                INT = num;
                break;
            case Define.Stat.LUK :
                LUC = num;
                break;
            default:
                break;
        }
        Stat_Update();
    }

    public void Stat_Change(int sp, int s, int d, int i, int l)
    {
        Stat_Point = sp;

        Stat_Change(Define.Stat.STR, s);
        Stat_Change(Define.Stat.DEX, d);
        Stat_Change(Define.Stat.INT, i);
        Stat_Change(Define.Stat.LUK, l);

        Stat_Update();
    }

    protected override void OnDead(Stat attacker)
    {
        //동작 없음
    }

    UI_Stat _UI_Stat;
    public void ConnectStatUI(UI_Stat statUI)
    {
        _UI_Stat = statUI;
    }

    public void GetStat(PlayerStat _stat)
    {
        Exp = _stat.Exp;
        Gold = _stat.Gold;
        next_level_up = _stat.next_level_up;
        current_exp = _stat.current_exp;
        Regeneration = _stat.Regeneration;
        Mana = _stat.Mana;
        MaxMana = _stat.MaxMana;
        Mana_Regeneration = _stat.Mana_Regeneration;
        Skill_Point = _stat.Skill_Point;
        Stat_Point = _stat.Stat_Point;
        STR = _stat.STR;
        DEX = _stat.DEX;
        INT = _stat.INT;
        LUC = _stat.LUC;
        Item_Hp = _stat.Item_Hp;
        Item_Regeneration = _stat.Item_Regeneration;
        Item_Attack = _stat.Item_Attack;
        Item_MoveSpeed = _stat.Item_MoveSpeed;
        Item_AttackSpeed = _stat.Item_AttackSpeed;
        Item_Mana = _stat.Item_Mana;
        Item_Mana_Regeneration = _stat.Item_Mana_Regeneration;
        Item_Hp_percent = _stat.Item_Hp_percent;
        Item_Attack_percent = _stat.Item_Attack_percent;
        Item_Mana_percent = _stat.Item_Mana_percent;
        _items = _stat._items;
        _myQuests = _stat._myQuests;
    }
}
