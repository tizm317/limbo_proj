using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int ID => _id;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite IconSprite => _iconSprite;

    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [Multiline]
    [SerializeField] private string _tooltip;               // 아이템 설명
    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private GameObject _dropItemPrefab;    // 바닥에 떨어질 때 생성할 아이템 프리팹

    // 타입에 맞는 새로운 아이템 생성
    public abstract Item CreatItem();
}
