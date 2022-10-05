using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
public class ArmorItemData : EquipmentItemData
{
    // 방어량?
    public float Value => _value;
    [SerializeField] private float _value;

    public override Item CreatItem()
    {
        return new ArmorItem(this);
    }
}
