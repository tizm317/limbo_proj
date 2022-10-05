using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    public int MaxAmount => _maxAmount;
    [SerializeField] private int _maxAmount = 99;
}
