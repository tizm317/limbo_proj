using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 아이템 수용 한도
    public int Capacity { get; private set; }

    // 초기 수용 한도
    [SerializeField, Range(8, 64)]
    private int _initialCapacity = 32;

    // 최대 수용 한도 (아이템 배열 크기)
    [SerializeField, Range(8, 64)]
    private int _maxCapacity = 64;

    // 연결된 인벤토리 UI
    [SerializeField]
    private UI_Inventory _UI_inventory;

    // 아이템 목록
    [SerializeField]
    private Item[] _items;

    /* Methods */
    private void Awake()
    {
        _items = new Item[_maxCapacity];
        Capacity = _initialCapacity;
    }

    private void Start()
    {
        
    }

    // 인덱스가 수용 범위 내인지 검사
    private bool IsValidIndex(int idx)
    {
        return idx >= 0 && idx < Capacity;
    }

    // 빈 슬롯 인덱스 찾기 (없으면 -1)
    private int FindEmptySlot(int startIdx = 0)
    {
        for(int i = startIdx; i < Capacity; i++)
        {
            if (_items[i] == null)
                return i;
        }
        return -1;
    }

    // 모든 슬롯 UI에 접근 가능 여부 업데이트
    public void UpdateAccessibleStatesAll()
    {
        _UI_inventory.SetAccessibleSlotRange(Capacity); // -> TODO
    }

    // 해당 슬롯이 아이템을 갖고있는지 검사
    public bool HasItem(int idx)
    {
        return IsValidIndex(idx) && (_items[idx] != null);
    }

    // 해당 슬롯의 아이템이 셀수 있는 아이템인지
    public bool IsCountableItem(int idx)
    {
        return HasItem(idx) && (_items[idx] is CountableItem);
    }

    // 해당 슬롯의 아이템 개수 리턴
    public int GetCurrentAmount(int idx)
    {
        if (!IsValidIndex(idx)) return -1;      // 유효하지 않은 Index : -1
        if (_items[idx] == null) return 0;      // 빈 슬롯 : 0

        CountableItem ci = _items[idx] as CountableItem;
        if (ci == null) return 1;               // 셀 수 없는 아이템 : 1
        return ci.Amount;                       // 셀 수 있는 아이템 : Amount
    }

    // 해당 슬롯의 아이템 정보 리턴
    public ItemData GetItemData(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_items[idx] == null) return null;

        return _items[idx].Data;
    }

    // 해당 슬롯의 아이템 이름 리턴
    public string GetItemName(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_items[idx] == null) return null;

        return _items[idx].Data.Name;
    }
}
