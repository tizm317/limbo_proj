using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    /* 실제 아이템 클래스 */

    public ItemData Data {get; private set;}
    public Item(ItemData data) => Data = data;

    //

}
