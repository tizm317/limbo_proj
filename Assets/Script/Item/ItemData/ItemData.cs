using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    /* 아이템의 공통 데이터 */

    #region Attributes

    public int      ID => _id;
    public string   Name => _name;
    public string   Grade => _grade;
    public uint     Price => _price;
    public Sprite   IconSprite => _iconSprite;
    public string   Tooltip { get { return _tooltip; } set { _tooltip = value; } }


    [Tooltip("아이템 고유 ID")]
    [SerializeField] private int        _id;
    [Tooltip("아이템 이름")]
    [SerializeField] private string     _name;
    [Tooltip("아이템 등급")]
    [SerializeField] private string     _grade;
    [Tooltip("아이템 가격(uint)")]
    [SerializeField] private uint       _price;
    [Tooltip("인벤토리 내 아이템 아이콘")]
    [SerializeField] private Sprite     _iconSprite;

    [Tooltip("아이템 설명")]
    [TextArea] // 여러줄 + 자동 줄바꿈 (vs Multline(자동X))
    [SerializeField] private string _tooltip;


    //[Tooltip("바닥에 떨어질 때 생성할 아이템 프리팹(사용X)")]
    //[SerializeField] private GameObject _dropItemPrefab;
    #endregion

    #region Methods
    // 타입에 맞는 새로운 아이템 생성
    public abstract Item CreatItem();
    #endregion
}
