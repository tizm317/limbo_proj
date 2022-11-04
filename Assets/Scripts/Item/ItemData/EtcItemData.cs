using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Etc_", menuName = "Inventory System/Item Data/Etc", order = 5)]
public class EtcItemData : CountableItemData
{
    /* 기타 아이템의 공통 데이터 */

    #region Attributes
    public float Value => _value;

    [Tooltip("기타 아이템의 효과량?")]
    [SerializeField] private float _value;
    #endregion

    #region Methods
    public override Item CreatItem()
    {
        return new EtcItem(this);
    }
    #endregion
}
