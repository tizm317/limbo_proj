using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    /* 장비 아이템의 공통 데이터 */

    public int MaxDurability => _maxDurability;
    
    [Tooltip("장비 최대 내구도")]
    [SerializeField] private int _maxDurability = 100;
}
