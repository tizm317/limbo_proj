using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Coin_", menuName = "Inventory System/Item Data/Coin", order = 6)]
public class CoinItemData : CountableItemData
{
    /* 코인 아이템의 공통 데이터*/

    public override Item CreatItem()
    {
        return new CoinItem(this);
    }
}
