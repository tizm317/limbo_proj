using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weapon", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    /* 무기 아이템의 공통 데이터 */

    public string Class => _class;
    public float Damage => _damage;

    [Tooltip("착용 가능 직업")]
    [SerializeField] private string _class;
    [Tooltip("무기 아이템의 데미지 수치")]
    [SerializeField] private float _damage;


    public override Item CreatItem()
    {
        return new WeaponItem(this);
    }
}
