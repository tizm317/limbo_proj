using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : EquipmentItem
{
    public ArmorItem(ArmorItemData data, int durability = 10) : base(data, durability) { }
    
    // 내구도 감소..?
    public bool Use()
    {
        Durability--;

        return true;
    }
}
