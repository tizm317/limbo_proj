using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Quest_", menuName = "Inventory System/Item Data/Quest", order = 4)]
public class QuestItemData : CountableItemData
{
    /* 퀘스트 아이템의 공통 데이터 */

    public float Value => _value;
    [SerializeField] private float _value;


    public override Item CreatItem()
    {
        return new QuestItem(this);
    }
}
