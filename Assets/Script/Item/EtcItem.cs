using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcItem : CountableItem, IUsableItem
{
    /* 기타 아이템 */
    // 화살, 입장권, 강화석

    public EtcItem(EtcItemData data, int amount = 1) : base(data, amount) { }

    public bool Use()
    {
        Amount--;

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new EtcItem(CountableData as EtcItemData, amount);

    }
}
