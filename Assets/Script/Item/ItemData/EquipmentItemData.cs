using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    public int MaxDurability => _maxDurability;
    [SerializeField] private int _maxDurability = 100;
}
