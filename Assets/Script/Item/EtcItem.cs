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
            data.Tooltip = " G";
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
        // G 단위 제거
        if(EtcData.Tooltip.Length >= 2)
            EtcData.Tooltip = EtcData.Tooltip.Remove(EtcData.Tooltip.Length - 2, 2);

        ulong.TryParse(EtcData.Tooltip, out currentSavedGolds);

        currentSavedGolds += AddingGolds;

        EtcData.Tooltip = currentSavedGolds.ToString();
        EtcData.Tooltip += " G";
    }
}
