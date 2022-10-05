using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : CountableItem
{
    public PotionItem(PotionItemData data, int amount = 1) : base(data, amount) { }
    public bool Use()
    {
        Amount--; // 임시 : 개수 하나 감소

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new PotionItem(CountableData as PotionItemData, amount);
    }
}
