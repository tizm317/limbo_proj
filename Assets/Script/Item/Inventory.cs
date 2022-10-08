using System;
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

    // 해당하는 인덱스의 슬롯 상태 및 UI 업데이트
    public void UpdateSlot(int idx)
    {
        if (!IsValidIndex(idx)) return;

        Item item = _items[idx];

        // 1. 아이템이 슬롯에 존재하는 경우
        if(item != null)
        {
            // 아이콘 등록
            _UI_inventory.SetItemIcon(idx, item.Data.IconSprite);

            // 1.1 셀 수 있는 아이템
            if(item is CountableItem ci)
            {
                // 1.1.1 수량이 0인 경우, 아이템 제거
                if(ci.IsEmpty)
                {
                    _items[idx] = null;
                    RemoveIcon();
                    return;
                }
                else // 1.1.2 수량 텍스트 표시
                {
                    _UI_inventory.SetItemAmountText(idx, ci.Amount);
                }
            }
            else // 1.2 셀 수 없는 아이템, 수량 텍스트 제거
            {
                _UI_inventory.HideItemAmountText(idx);
            }
        }
        else // 2. 빈 슬롯, 아이템 제거
        {
            RemoveIcon();
        }

        // local : 아이템 제거
        void RemoveIcon()
        {
            _UI_inventory.RemoveItem(idx);
            _UI_inventory.HideItemAmountText(idx);
        }
    }

    public void Swap(int fromIdx, int toIdx)
    {
        if (!IsValidIndex(fromIdx)) return;
        if (!IsValidIndex(toIdx)) return;

        Item itemFrom = _items[fromIdx];
        Item itemTo = _items[toIdx];

        // 1. 셀 수 있는 아이템 && 동일한 아이템
        // fromIdx -> toIdx 로 개수 합치기
        if(itemFrom != null && itemTo != null && itemFrom.Data == itemTo.Data && itemFrom is CountableItem ciFrom && itemTo is CountableItem ciTo)
        {
            int maxAmount = ciTo.MaxAmount;
            int sum = ciFrom.Amount + ciTo.Amount;
            if(sum <= maxAmount)
            {
                ciFrom.SetAmount(0);
                ciTo.SetAmount(sum);
            }
            else
            {
                ciFrom.SetAmount(sum - maxAmount);
                ciTo.SetAmount(maxAmount);
            }
        }
        else // 2. 일반적인 경우 : 슬롯 교체
        {
            _items[fromIdx] = itemTo;
            _items[toIdx] = itemFrom;
        }

        // 두 슬롯 갱신
        UpdateSlot(fromIdx);
        UpdateSlot(toIdx);
    }


    // 인벤토리에 아이템 추가
    // 넣는 데 실패한 잉여 아이템 개수 리턴
    // 리턴 값 0이면 모두 성공
    public int Add(ItemData itemData, int amount = 1)
    {
        int idx;

        // 1. 수량이 있는 아이템
        if(itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;
            idx = -1;

            while(amount > 0)
            {
                // 1.1 이미 해당 아이템이 인벤토리에 존재하고, 개수 여유 있는지 검사
                if(findNextCountable)
                {
                    idx = FIndCountableItemSlotIndex(ciData, idx + 1);
                
                    // 개수 여유 있는 기존 슬롯이 더 이상 없다고 판단될 경우, 빈 슬롯부터 탐색 시작
                    if(idx == -1)
                    {
                        findNextCountable = false;
                    }
                    else // 기존 슬롯을 찾은 경우, 양 증가시키고, 초과량 존재 시 amount에 초기화
                    {
                        CountableItem ci = _items[idx] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(idx);
                    }
                }
                else // 1.2 빈 슬롯 탐색
                {
                    idx = FindEmptySlot(idx + 1);

                    // 빈 슬롯 조차 없는 경우 종료
                    if (idx == -1) break;
                    else // 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 초과량 계산
                    {
                        // 새로운 아이템 생성
                        CountableItem ci = ciData.CreatItem() as CountableItem;
                        ci.SetAmount(amount);

                        // 슬롯에 추가
                        _items[idx] = ci;

                        // 남은 개수 계산
                        amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;

                        UpdateSlot(idx);
                    }

                }
            }
        }
        else // 2. 수량이 없는 아이템
        {
            // 2.1 1개만 넣는 경우
            if(amount == 1)
            {
                idx = FindEmptySlotIndex();
                if(idx != -1)
                {
                    // 아이템 생성하고 슬롯에 추가
                    _items[idx] = itemData.CreatItem();
                    amount = 0;

                    UpdateSlot(idx);
                }
            }

            // 2.2 2개 이상의 수량 없는 아이템을 동시 추가하는 경우
            for(idx = -1; amount >0; amount--)
            {
                // 아이템에 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                idx = FindEmptySlotIndex(idx + 1);

                // 다 넣지 못한 경우 루프 종료
                if (idx == -1) break;

                // 아이템 생성하여 슬롯 추가
                _items[idx] = itemData.CreatItem();

                UpdateSlot(idx);
            }
        }

        // 넣는 데 실패한 잉여 아이템 개수 리턴
        return amount;
    }

    private int FindEmptySlotIndex(int idx_in = 0)
    {
        int idx = idx_in;

        //TODO

        return idx;
    }

    private int FIndCountableItemSlotIndex(CountableItemData ciData, int v)
    {
        int idx = -1;

        // TODO

        return idx;
    }
}
