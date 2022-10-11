using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Potion_", menuName = "Inventory System/Item Data/Potion", order = 3)]
public class PotionItemData : CountableItemData
{
    /* 포션 아이템의 공통 데이터 */

    // 포션의 효과량(회복량 등)
    public float Value => _value;
    [SerializeField] private float _value;


    public override Item CreatItem()
    {
        return new PotionItem(this);
    }

}
