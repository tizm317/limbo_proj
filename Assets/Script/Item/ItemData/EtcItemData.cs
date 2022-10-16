using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Etc_", menuName = "Inventory System/Item Data/Etc", order = 5)]
public class EtcItemData : CountableItemData
{
    /* 기타 아이템의 공통 데이터 */

    //
    public float Value => _value;
    [SerializeField] private float _value;


    public override Item CreatItem()
    {
        return new EtcItem(this);
    }
}
