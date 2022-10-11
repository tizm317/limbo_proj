using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItem : Item
{
    /* 실제 셀 수 있는 아이템 클래스 */

    public CountableItemData CountableData { get; private set; }

    public int Amount { get; protected set; }
    public int MaxAmount => CountableData.MaxAmount;
    public bool IsMax => Amount >= MaxAmount;
    public bool IsEmpty => Amount <= 0;

    public CountableItem(CountableItemData data, int amount = 1) : base(data)
    {
        CountableData = data;
        SetAmount(amount);
    }

    // 개수 지정(범위 제한)
    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount); // 0~MaxAmount
    }

    // 개수 추가 및 초과량 반환
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount > MaxAmount) ? nextAmount - MaxAmount : 0;
    }

    // 개수 나누어 복제
    public CountableItem SeperateAndClone(int amount)
    {
        if (Amount <= 1) return null;   // 수량 1개 이하, 복제 불가
        if (amount > Amount - 1)        // 복제 가능 최대치(현재 수량 - 1)
            amount = Amount - 1;

        Amount -= amount;               // 현재 수량에서 빼주고
        return Clone(amount);           // 복사본 생성
    }

    protected abstract CountableItem Clone(int amount);
}
