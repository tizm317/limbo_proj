using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : Item
{
    /* 실제 장비 아이템 클래스 */

    #region Attributes
    // - 공통 데이터 -
    public EquipmentItemData EquipmentData { get; private set; }
    public int MaxDurability => EquipmentData.MaxDurability;

    // 아이템 내구도가 Max인지 여부 -> 더이상 수리할 수 없음
    public bool IsMaxDurability => Durability >= MaxDurability;

    // - 개별 데이터 -
    // 각 아이템의 내구도
    public int Durability { get; protected set; }

    // 아이템 내구도가 다 닳아서 부서졌는지 여부 -> 파괴 or 사용 불가
    public bool IsBroken => Durability <= 0;
    #endregion

    #region Methods
    public EquipmentItem(EquipmentItemData data, int durability = 10) 
        : base(data)
    {
        EquipmentData = data;
        SetDerability(durability);
    }

    // 내구도 지정(범위 제한)
    public void SetDerability(int durability)
    {
        // 0~MaxDurability
        Durability = Mathf.Clamp(durability, 0, MaxDurability); 
    }

    // 내구도 추가 및 초과량 반환(?)(수리할 때??)
    public int AddDurabilityAndGetExcess(int added_durability)
    {
        int nextAmount = Durability + added_durability;
        SetDerability(nextAmount);

        return (nextAmount > MaxDurability) ? nextAmount - MaxDurability : 0;
    }
    #endregion
}
