using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weapon", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    /* 무기 아이템의 공통 데이터 */

    // 데미지
    public float Damage => _damage;
    [SerializeField] private float _damage;

    public override Item CreatItem()
    {
        return new WeaponItem(this);
    }
}
