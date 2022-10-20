using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcItem : CountableItem, IUsableItem, ISellableItem
{
    /* 기타 아이템 */
    // 화살, 입장권, 강화석

    public EtcItemData EtcData { get; private set; }
    public float Value => EtcData.Value;


    public EtcItem(EtcItemData data, int amount = 1) : base(data, amount) 
    {
        EtcData = data;
        if (data.IsCoin)
            data.Tooltip = "<color=Red>Saved Golds :  G</color>";
    }

    public bool Sell()
    {
        Amount--;

        return true;
    }

    public bool Use()
    {
        Amount--;

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new EtcItem(CountableData as EtcItemData, amount);

    }

    public void SaveGoldsToString(ulong AddingGolds)
    {
        ulong currentSavedGolds = 0;
        currentSavedGolds = SavedGoldsToUlong();
        currentSavedGolds += AddingGolds;

        EtcData.Tooltip = currentSavedGolds.ToString();
        
        // 단위 추가
        EtcData.Tooltip += " G";

        // 컬러 및 "Saved Golds : " 추가
        EtcData.Tooltip = $"<color=Red>Saved Golds : {EtcData.Tooltip}</color>";
    }

    public ulong SavedGoldsToUlong()
    {
        ulong currentSavedGolds = 0;

        if (EtcData.Tooltip.Length >= 2)
        {
            // 컬러 제거
            EtcData.Tooltip = EtcData.Tooltip.Remove(0, 11);
            EtcData.Tooltip = EtcData.Tooltip.Remove(EtcData.Tooltip.Length - 8, 8);

            // Saved Golds : 제거
            EtcData.Tooltip = EtcData.Tooltip.Remove(0, 14);

            // G 단위 제거
            EtcData.Tooltip = EtcData.Tooltip.Remove(EtcData.Tooltip.Length - 2, 2);
        }

        ulong.TryParse(EtcData.Tooltip, out currentSavedGolds);

        return currentSavedGolds;
    }
}
