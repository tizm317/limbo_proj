using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    // player, enemy 공통 필요 변수
    //tool에서 확인하기 위함
    // HY : 내부에서 쓰이는 변수 이름 앞에 _ 붙여둠
    [SerializeField] protected int _level;  //레벨
    [SerializeField] protected int _hp;  //hp
    [SerializeField] protected int _maxHp;  //최대 hp
    [SerializeField] protected int _attack;  //공격력
    [SerializeField] protected int _defense;  //방어력
    [SerializeField] protected float _moveSpeed;  //이동하는 속도
    [SerializeField] protected float _turnSpeed; // 턴하는 속도

    // 외부에서 사용할 때
    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float TurnSpeed { get { return _turnSpeed; } set { _turnSpeed = value; } }

    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 0;
        _moveSpeed = 4.0f;
        _turnSpeed = 20.0f;
    }
}
