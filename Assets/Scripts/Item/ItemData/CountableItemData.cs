using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    /* 셀 수 있는 아이템의 공통 데이터 */

    public int MaxAmount => _maxAmount;
    
    [Tooltip("아이템 최대 수량")]
    [SerializeField] private int _maxAmount = 99;
}
