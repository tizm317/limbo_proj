using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : CountableItem
{
    /* 실제 퀘스트 아이템 클래스 */
    // Countable

    #region Attrubutes
    // - 공통 데이터 -
    public QuestItemData QuestData { get; private set; }
    public float Value => QuestData.Value;
    #endregion

    #region Methods
    public QuestItem(QuestItemData data, int amount = 1) 
        : base(data, amount) 
    {
        QuestData = data;
    }

    protected override CountableItem Clone(int amount)
    {
        return new QuestItem(CountableData as QuestItemData, amount);
    }
    #endregion
}
