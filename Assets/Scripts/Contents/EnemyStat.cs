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
    public ItemData[] _itemData; //인벤토리에 넣을 아이템 
    public ItemData _questItemData; //인벤토리에 넣을 아이템 

    //[SerializeField] protected uint _gold; //골드

    Enemy enemy;
    Inventory inventory;

    public float aRate; //a비율
    public float bRate; //b비율
    private float rate;
    private int length;

    // 외부에서 사용할 때
    public int EnemyExp { get { return _enemyExp; } set { _enemyExp = value; } }
    public ItemData[] ItemData { get { return _itemData; } set { _itemData = value; } }
    public ItemData QuestItemData { get { return _questItemData; } set { _questItemData = value; } }
    //public uint Gold { get { return _gold; } set { _gold = value; } }

    void Init()
    {
        enemy = gameObject.GetComponent<Enemy>();
        inventory = GameObject.Find("@Scene").GetComponent<Inventory>();

        float a = bRate / (bRate + aRate);
        length = _itemData.Length;
        rate = length - (length * a);
    }
    void Start()
    {
        Init();
        _moveSpeed = 1.0f;
        _turnSpeed = 3.0f;
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
        PlayerStat playerStat = attacker as PlayerStat;
        
        if (playerStat != null) //경험치
            playerStat.Exp += EnemyExp;

        int tempIdx;

        inventory.Add_Without_UI_Update(_itemData[GetRandomRate()], out tempIdx);  //랜덤 아이템
        inventory.Add_Without_UI_Update(_questItemData, out tempIdx);  //퀘스트 아이템
        inventory.AddGold((uint)(_enemyExp*8));   //gold는 경험치의 8배

        StartCoroutine(Die());

        ////아이템 리스트로 해서 아이템 넣어두고 랜덤하게 나올 수 있도록 만들어야 함
        //int tempIdx;
        //inventory.Add_Without_UI_Update(ItemData[_itemIndex], idx: out tempIdx, 1);
    }

    private int GetRandomRate()
    {
        int tmp = Random.Range(0, length);

        if (tmp <= rate - 1) // length는 0을 포함하지 않기 때문에 0부터 세기 위해 정답 개수 - 1
        {
            return 0; // 프리팹 배열의 인덱스로 반환
        }
        else return 1;

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
