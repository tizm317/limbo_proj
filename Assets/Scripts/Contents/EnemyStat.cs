using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStat : Stat
{
    //enemy 필요 변수
    //tool에서 확인하기 위함
    //HY : 내부에서 쓰이는 변수 이름 앞에 _ 붙여둠
    [SerializeField] protected int _enemyExp; //제공하는 exp
    //[SerializeField] protected ItemData[] _itemdata; //인벤토리에 넣을 아이템 
    //[SerializeField] private int _itemIndex;

    Enemy enemy;
    //Inventory inventory;


    // 외부에서 사용할 때
    public int EnemyExp { get { return _enemyExp; } set { _enemyExp = value; } }

    //public ItemData[] ItemData { get { return _itemdata; } set { _itemdata = value; } }

    void Init()
    {
        enemy = gameObject.GetComponent<Enemy>();
        //inventory = GameObject.Find("@Scene").GetComponent<Inventory>();
    }
    void Start()
    {
        Init();
        _moveSpeed = 1.0f;
        _turnSpeed = 5.0f;
        SetStat(_level);
    }
    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];
        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _attack = stat.attack;
        _enemyExp = stat.totalExp;

    }

    public override void OnAttacked(Stat attacker)
    {
        if (enemy.State == Define.EnemyState.Die)
            return;

        float damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage; //나의 hp에서 demage 만큼 깎는다
        enemy.State = Define.EnemyState.Hit;
        if (Hp <= 0)  //음수 경우 hp = 0;
        {
            Hp = 0;  //내가 죽었을 경우
            //enemy.State = Define.State.Die;
            if (enemy.State != Define.EnemyState.Die)
                OnDead(attacker);
        }
    }

    public override void OnAttacked(float Damage, Stat attacker)
    {
        if (enemy.State == Define.EnemyState.Die)
            return;

        float damage = Mathf.Max(0, Damage - Defense);
        Hp -= damage; //나의 hp에서 demage 만큼 깎는다
        enemy.State = Define.EnemyState.Hit;
        if (Hp <= 0)  //음수 경우 hp = 0;
        {
            Hp = 0;  //내가 죽었을 경우
            if(enemy.State != Define.EnemyState.Die)
                OnDead(attacker);
        }
    }
    protected override void OnDead(Stat attacker)
    {
        StartCoroutine(Die());

        PlayerStat playerStat = attacker as PlayerStat;
        
        if (playerStat != null) //경험치
        {
            playerStat.Exp += EnemyExp;
        }
        
        //int tempIdx;
        //inventory.Add(_itemdata, idx: out tempIdx, 1);
        

        //_itemIndex = Random.Range(1, ItemData.Length);  //확률 적용해야함

        ////아이템 리스트로 해서 아이템 넣어두고 랜덤하게 나올 수 있도록 만들어야 함
        //int tempIdx;
        //inventory.Add_Without_UI_Update(ItemData[_itemIndex], idx: out tempIdx, 1);
    }
    IEnumerator Die()
    {
        enemy.State = Define.EnemyState.Die;

        //NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        //nma.SetDestination(transform.position); //움직이지 않고 본인 위치에서 어택하도록 

        yield return new WaitForSeconds(5.0f);

        Managers.Game.Despawn(gameObject);
    }

}
