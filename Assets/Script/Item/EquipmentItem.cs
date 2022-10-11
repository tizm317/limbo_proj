using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : Item
{
    /* 실제 장비 아이템 클래스 */

    public EquipmentItemData EquipmentData { get; private set; }

    // 내구도
    public int Durability { get; protected set; }
    public int MaxDurability => EquipmentData.MaxDurability;
    public bool IsMax => Durability >= MaxDurability;
    public bool IsEmpty => Durability <= 0;

    public EquipmentItem(EquipmentItemData data, int durability = 10) : base(data)
    {
        EquipmentData = data;
        SetDerability(durability);
    }

    // 내구도 지정(범위 제한)
    public void SetDerability(int durability)
    {
        Durability = Mathf.Clamp(durability, 0, MaxDurability); // 0~MaxDurability
    }

    // 내구도 추가 및 초과량 반환(?)(수리할 때??)
    public int AddDurabilityAndGetExcess(int added_durability)
    {
        int nextAmount = Durability + added_durability;
        SetDerability(nextAmount);

        return (nextAmount > MaxDurability) ? nextAmount - MaxDurability : 0;
    }

    // 셀 수 있는 아이템 아님
    //// 개수 나누어 복제
    //public CountableItem SeperateAndClone(int amount)
    //{
    //    if (Amount <= 1) return null;   // 수량 1개 이하, 복제 불가
    //    if (amount > Amount - 1)        // 복제 가능 최대치(Amount - 1)
    //        amount = Amount - 1;

    //    Amount -= amount;
    //    return Clone(amount);
    //}

    //protected abstract CountableItem Clone(int amount);
}
