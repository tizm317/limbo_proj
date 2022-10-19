using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : EquipmentItem, IUsableItem, ISellableItem
{
    /* 실제 무기 아이템 클래스 */

    public WeaponItemData weaponData { get; private set; }

    public WeaponItem(WeaponItemData data, int durability = 10) : base(data, durability) 
    {
        weaponData = data;
    }

    public string Class => weaponData.Class;
    public float Damage => weaponData.Damage;

    public bool Sell()
    {
        return true;
    }

    // 내구도 감소..?
    public bool Use()
    {
        //Durability--;

        return true;
    }
}
