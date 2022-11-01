using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    [SerializeField] protected int _skill_point;
    public int Exp { get { return _exp; } set { _exp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }
    public bool level_up = true;
    public float next_level_up {get {return _next_level_up; } set { _next_level_up = value; }}
    public float current_exp;
    public float Regeneration { get { return _regeneration; } set { _regeneration = value; }}
    public float Mana { get { return _mana; } set { _mana = value; }}
    public float MaxMana{ get { return _max_mana; } set { _max_mana = value; } }
    public float Mana_Regeneration { get { return _mana_regeneration; } set { _mana_regeneration = value; }}
    public int Skill_Point { get { return _skill_point; }  set { _skill_point = value; } }
    private int max_skill_point = 16; // 4(qwer) * 4렙
    public float STR,DEX,INT,LUC;
    public float Item_Hp, Item_Regeneration, Item_Attack, Item_MoveSpeed, Item_AttackSpeed, Item_Mana, Item_Mana_Regeneration;
    public float Item_Hp_percent, Item_Attack_percent, Item_Mana_percent;
    float time;
    public bool isDead = false;
    private Player ps;
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
        _gold = 0;
        _skill_point = 0;
        Level_Update();
        Stat_Update();
        time = 0;
        STR = 5;
        ps = GameObject.Find("@Scene").gameObject.GetComponent<Player>();
    }

    void Update()
    {
        HPMP_Update();
        Level_Update();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "VillagePotal")
            Managers.Scene.LoadScene(Define.Scene.InGameVillage);
        else if (collision.gameObject.name == "BossPotal")
        {
            if (SceneManager.GetActiveScene().name == "InGameCemetery")
            {
                    Managers.Scene.LoadScene(Define.Scene.InGameBoss);

            }
            else if (SceneManager.GetActiveScene().name == "InGameNature")
            {
                    Managers.Scene.LoadScene(Define.Scene.InGameNatureBoss);


            }
            else if (SceneManager.GetActiveScene().name == "InGameDesert")
            {
                    Managers.Scene.LoadScene(Define.Scene.InGameDesertBoss);
            }
        }
            
    }

    void HPMP_Update()
    {
        time += Time.deltaTime;
        if(_hp < 0)
        {
            isDead = true;
            
            ps.curState = Player.State.STATE_DIE;
            ps.Ani_State_Change();
        }
        if(time >= 1)
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
            if(Skill_Point < max_skill_point)
                Skill_Point++;
            level_up = true;
            Stat_Update();

            // TODO : Skill Point UI
            SkillUpUIPopUp();
        }
    }

    void SkillUpUIPopUp()
    {
        Debug.Log("Lv Up => SP Up");
        UI_InGame _UI_InGame = GameObject.Find("@Scene").GetComponent<GameScene>().UI_InGame;
        //TODO
    }


    void Stat_Update()
    {
        MaxHp  = (Level * STR + Item_Hp + 100) * (1 + Item_Hp_percent);
        Hp = MaxHp;
        Regeneration = (Level * STR + Item_Regeneration + 1) * 0.01f;
        Attack = ((0.5f * Level * STR) + Item_Attack) * (1 + Item_Attack_percent)+20;
        MoveSpeed = 4 + (1 / 40) * (DEX -20) + Item_MoveSpeed;
        AttackSpeed = 1 + (1 / 50) * (DEX - 20) + Item_AttackSpeed;
        MaxMana = 0.5f * (Level * INT + Item_Mana + 100) * (1 + Item_Mana_percent);
        Mana = MaxMana;
        Mana_Regeneration = 0.5f * (Level * INT + Item_Mana + 100) * (1 + Item_Mana_percent) * (0.1f + Item_Mana_Regeneration);
    }

    public override void OnAttacked(Stat attacker)
    {
       
        float damage = Mathf.Max(0, attacker.Attack - Defense);
        if(ps.attackable == true)
            Hp -= damage; //나의 hp에서 demage 만큼 깎는다
        if (Hp <= 0)  //음수 경우 hp = 0;
        {
            Hp = 0;  //내가 죽었을 경우
            //enemy.State = Define.State.Hit;
            OnDead(attacker);
        }
    }

    public void Stat_Change(Define.Stat stat, int num)
    {
        switch(stat)
        {
            case Define.Stat.STR :
                STR += num;
                break;
            case Define.Stat.DEX :
                DEX += num;
                break;
            case Define.Stat.INT :
                INT += num;
                break;
            case Define.Stat.LUK :
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
