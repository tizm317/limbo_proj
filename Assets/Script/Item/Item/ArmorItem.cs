using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : EquipmentItem, IUsableItem, ISellableItem
{
    /* 실제 방어구 아이템 클래스 */
    // Usable, Sellable

    #region Attrubutes
    // - 공통 데이터 -
    public ArmorItemData ArmorData { get; private set; }
    public float Value => ArmorData.Value;
    public string Part => ArmorData.Part;
    #endregion

    #region Methods

    // 생성자
    public ArmorItem(ArmorItemData data, int durability = 10) 
        : base(data, durability) 
    {
        ArmorData = data;
    }

    // 아이템 판매
    public bool Sell()
    {
        return true;
    }

    // 아이템 사용 -> 장착
    public bool Use()
    {
        return true;
    }

    // 내구도 감소..? -> 이거는 공격 당할 때
    //Durability--;
    #endregion
}
