﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
public class ArmorItemData : EquipmentItemData
{
    /* 방어구 아이템의 공통 데이터 */

    // 방어량?
    public float Value => _value;
    [SerializeField] private float _value;

    // 방어 부위
    public string Part => _part;
    [SerializeField] private string _part;

    public override Item CreatItem()
    {
        return new ArmorItem(this);
    }
}
