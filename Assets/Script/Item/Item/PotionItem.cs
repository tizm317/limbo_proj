using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : CountableItem, IUsableItem, ISellableItem
{
    /* 실제 포션 아이템 클래스 */
    // Countable , Usable, Sellable

    #region Attributes
    // - 공통 데이터 -
    public PotionItemData potionData { get; private set; }
    public float Value => potionData.Value;
    #endregion

    #region Methods

    // 생성자
    public PotionItem(PotionItemData data, int amount = 1)
    : base(data, amount) 
    {
        potionData = data;
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
        Amount--; // 임시 : 개수 하나 감소

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new PotionItem(CountableData as PotionItemData, amount);
    }
#endregion
}
