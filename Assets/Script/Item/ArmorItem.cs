using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : EquipmentItem, IUsableItem, ISellableItem
{
    /* 실제 방어구 아이템 클래스 */

    public ArmorItemData ArmorData { get; private set; }

    public float Value => ArmorData.Value;
    public string Part { get; protected set; }

    public ArmorItem(ArmorItemData data, int durability = 10) : base(data, durability) 
    {
        ArmorData = data;
        Part = data.Part;
    }

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
