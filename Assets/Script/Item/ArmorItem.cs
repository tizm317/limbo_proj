using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : EquipmentItem, IUsableItem, ISellableItem
{
    /* 실제 방어구 아이템 클래스 */

    public ArmorItem(ArmorItemData data, int durability = 10) : base(data, durability) { }

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
