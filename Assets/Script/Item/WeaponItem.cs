using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : EquipmentItem, IUsableItem
{
    /* 실제 무기 아이템 클래스 */

    public WeaponItem(WeaponItemData data, int durability = 10) : base(data, durability) { }
    
    // 내구도 감소..?
    public bool Use()
    {
        //Durability--;

        return true;
    }
}
