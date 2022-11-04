using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : CountableItem, IUsableItem
{
    /* 넘치는 골드 저장하기 위한 코인아이템 실제 클래스 */
    // Countable, Usable 이긴한데 실제로는 Countable은 아님(따로 예외처리)
    // MaxCount = 1

    #region Attrubutes
    
    // - 공통 데이터 -
    public CoinItemData CoinData { get; private set; }
    
    // - 개별 데이터 -
    
    // 저장된 골드 수량 (오버플로우 방지 위해 string 저장)
    public string SavedGolds { get; private set; }
    #endregion

    #region Methods

    // 생성자
    public CoinItem(CoinItemData data, int amount = 1)
    : base(data, amount)
    {
        CoinData = data;
        SavedGolds = "0";
    }

    // 코인 아이템 사용 => 저장된 골드가 인벤토리 내 골드로 합쳐짐
    public bool Use()
    {
        Amount--;
        return true;
    }

    // ulong 골드 -> string 저장
    public void SaveGoldsToString(ulong AddingGolds)
    {
        // 현재 저장 골드 + 추가 저장 골드
        ulong currentSavedGolds = 0;
        currentSavedGolds = SavedGoldsToUlong();
        currentSavedGolds += AddingGolds;

        // to string
        SavedGolds = currentSavedGolds.ToString();

        // Tooltip에 표시 + 단위 등 추가
        CoinData.Tooltip = SavedGolds;
        // 단위 추가
        CoinData.Tooltip += " G";
        // 컬러 및 "Saved Golds : " 추가
        CoinData.Tooltip = $"<color=Red>Saved Golds : {CoinData.Tooltip}</color>";
    }

    // string으로 저장된 골드 -> ulong 골드
    public ulong SavedGoldsToUlong()
    {
        ulong currentSavedGolds = 0;
        ulong.TryParse(SavedGolds, out currentSavedGolds);

        return currentSavedGolds;
    }

    protected override CountableItem Clone(int amount)
    {
        return new CoinItem(CountableData as CoinItemData, amount);
    }
    #endregion
}
