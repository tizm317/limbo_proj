using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    // player, enemy 공통 필요 변수
    //tool에서 확인하기 위함
    // HY : 내부에서 쓰이는 변수 이름 앞에 _ 붙여둠
    [SerializeField] protected int _level;  //레벨
    [SerializeField] protected float _hp;  //hp
    [SerializeField] protected float _maxHp;  //최대 hp
    [SerializeField] protected float _attack;  //공격력
    [SerializeField] protected float _defense;  //방어력
    [SerializeField] protected float _moveSpeed;  //이동하는 속도
    [SerializeField] protected float _turnSpeed; // 턴하는 속도
    [SerializeField] protected float _attackSpeed; //공격속도
    [SerializeField] protected ItemData _itemdata; //인벤토리에 넣을 아이템 

    Enemy enemy;
    Inventory inventory;

    // 외부에서 사용할 때
    public int Level { get { return _level; } set { _level = value; } }
    public float Hp { get { return _hp; } set { _hp = value; } }
    public float MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public float Attack { get { return _attack; } set { _attack = value; } }
    public float Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float TurnSpeed { get { return _turnSpeed; } set { _turnSpeed = value; } }
    public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
    public ItemData ItemData { get { return _itemdata; } set { _itemdata = value; } }

    private void Awake()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 6.0f;
        _defense = 0;
        _moveSpeed = 0.5f;
        _turnSpeed = 5.0f;
    }
    void Init()
    {
        enemy = gameObject.GetComponent<Enemy>();
        inventory = GameObject.Find("@Scene").GetComponent<Inventory>();
    }
    public void Start()
    {
        Init();
    }
    public virtual void OnAttacked(Stat attacker)
    {
       
        float damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage; //나의 hp에서 demage 만큼 깎는다
        if (Hp <= 0)  //음수 경우 hp = 0;
        {
            Hp = 0;  //내가 죽었을 경우
            //enemy.State = Define.State.Hit;
            OnDead(attacker);
        }
    }

    public virtual void OnAttacked(float Damage, Stat attacker)
    {
        float damage = Mathf.Max(0, Damage - Defense);
        Hp -= damage; //나의 hp에서 demage 만큼 깎는다
        if (Hp <= 0)  //음수 경우 hp = 0;
        {
            Hp = 0;  //내가 죽었을 경우
            enemy.State = Define.State.Hit;
            if(enemy.State != Define.State.Die)
                OnDead(attacker);
        }
    }
    protected virtual void OnDead(Stat attacker)
    {
        PlayerStat playerStat = attacker as PlayerStat;
        /*
        if (playerStat != null) //경험치
        {
            playerStat.Exp += 10;
        }
        */
        //int tempIdx;
        //inventory.Add(_itemdata, idx: out tempIdx, 1);
        StartCoroutine(Die());

        //아이템 리스트로 해서 아이템 넣어두고 랜덤하게 나올 수 있도록 만들어야 함
        int tempIdx;
        inventory.Add_Without_UI_Update(ItemData, idx: out tempIdx, 1);

    }
    IEnumerator Die()
    {
        enemy.State = Define.State.Die;

        yield return new WaitForSeconds(7.0f);

        Managers.Game.Despawn(gameObject);
    }
}
