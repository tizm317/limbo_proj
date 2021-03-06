using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStat : Stat
{

    // player 필요 변수
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;

    public int Exp { get { return _exp; } set { _exp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }
    private Image HP_bar;
    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 20;
        _defense = 5;
        _moveSpeed = 6.0f;
        _turnSpeed = 20.0f;
        _exp = 0;
        _gold = 0;
        HP_bar = GameObject.Find("Filler").GetComponent<Image>();
    }

    void Update()
    {
        HP_bar.fillAmount = _hp/100.0f;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Potal")
            Managers.Scene.LoadScene(Define.Scene.Village);
    }
}
