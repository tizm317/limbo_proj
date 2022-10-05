using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : EquipmentItem
{
    public WeaponItem(WeaponItemData data, int durability = 10) : base(data, durability) { }
    
    // 내구도 감소..?
    public bool Use()
    {
        Durability--;

        return true;
    }
}
