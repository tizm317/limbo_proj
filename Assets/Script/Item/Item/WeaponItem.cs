using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : EquipmentItem, IUsableItem, ISellableItem
{
    /* 실제 무기 아이템 클래스 */
    // Usable, Sellable

    #region Attributes
    // - 공통 데이터 -
    public WeaponItemData weaponData { get; private set; }
    public string Class => weaponData.Class;
    public float Damage => weaponData.Damage;
    #endregion

    #region Methods
    
    // 생성자
    public WeaponItem(WeaponItemData data, int durability = 10)
        : base(data, durability)
    {
        weaponData = data;
    }

    // 아이템 판매
    public bool Sell()
    {
        return true;
    }

    // 아이템 사용
    public bool Use()
    {
        return true;
    }
    #endregion
}
