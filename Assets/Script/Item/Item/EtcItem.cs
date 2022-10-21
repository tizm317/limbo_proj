using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcItem : CountableItem, IUsableItem, ISellableItem
{
    /* 기타 아이템 */
    // 화살, 입장권, 강화석  등
    // Countable, Usable, Sellable

    #region Attributes
    // - 공통 데이터 -
    public EtcItemData EtcData { get; private set; }
    public float Value => EtcData.Value;
    #endregion

    #region Methods

    // 생성자
    public EtcItem(EtcItemData data, int amount = 1) 
        : base(data, amount) 
    {
        EtcData = data;
    }

    // 아이템 판매
    public bool Sell()
    {
        Amount--;

        return true;
    }

    // 아이템 사용
    public bool Use()
    {
        Amount--;

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new EtcItem(CountableData as EtcItemData, amount);
    }
    #endregion
}
