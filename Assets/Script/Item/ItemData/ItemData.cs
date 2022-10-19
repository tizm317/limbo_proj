using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    /* 아이템의 공통 데이터 */

    public int ID => _id;
    public string Name => _name;

    public string Grade => _grade;
    public string Tooltip => _tooltip;
    public Sprite IconSprite => _iconSprite;

    public uint Price => _price;


    [SerializeField] private int        _id;
    [SerializeField] private string     _name;
    [SerializeField] private string     _grade;
    [Multiline]
    [SerializeField] private string     _tooltip;           // 아이템 설명
    [SerializeField] private Sprite     _iconSprite;
    [SerializeField] private GameObject _dropItemPrefab;    // 바닥에 떨어질 때 생성할 아이템 프리팹
    [SerializeField] private uint        _price;            // unsigned int

    // 타입에 맞는 새로운 아이템 생성
    public abstract Item CreatItem();
}
