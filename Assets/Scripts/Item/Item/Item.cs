using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    /* 실제 아이템 클래스 */
    // 각각의 아이템이 가질 데이터
    // 아이템의 동작 메써드

    public ItemData Data {get; private set;}
    public Item(ItemData data) => Data = data;
}
