using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /* 인벤토리 클래스
     * 아이템 배열로 관리
     * 인벤토리 내부 동작
     * InventoryUI와 상호작용
     */

    // 아이템 수용 한도
    public int Capacity { get; private set; }

    // 초기 수용 한도
    [SerializeField, Range(6, 42)]
    private int _initialCapacity = 42;

    // 최대 수용 한도 (아이템 배열 크기)
    [SerializeField, Range(6, 42)]
    private int _maxCapacity = 42;

    // 연결된 인벤토리 UI
    [SerializeField]
    private UI_Inventory _UI_inventory;

    public UI_Inventory UI_Inventory => _UI_inventory;

    // 아이템 목록
    [SerializeField]
    private Item[] _items;

    // 소유 골드
    [SerializeField]
    private uint _MyGolds; // unsigned int

    // 최대 소유 골드
    [SerializeField, Range(0, uint.MaxValue)]
    private uint _maxGolds = uint.MaxValue;

    public uint Golds { get { return _MyGolds; }  set { _MyGolds = value; } }

    public bool Empty()
    {
        foreach(Item it in _items)
        {
            if (it != null) return false;
        }
        return true;
    }



    /* Methods */
    public void Init()
    {
        _items = new Item[_maxCapacity];
        Capacity = _initialCapacity;
    }

    private void Awake()
    {
        _items = new Item[_maxCapacity];
        Capacity = _initialCapacity;

        //_UI_inventory = Managers.UI.ShowPopupUI<UI_Inventory>();

        // 인벤토리 UI랑 연결
        //_UI_inventory.SetInventory(this);
    }

    // 인벤토리UI에서 호출
    public void SetInventoryUI(UI_Inventory inventoryUI)
    {
        _UI_inventory = inventoryUI;
    }

    public void test()
    {
        // test
        //WeaponItem w1 = new WeaponItem(axeItemData, 10);
        //Add(w1.Data, 1);
        //ArmorItem a1 = new ArmorItem(armorItemData, 10);
        //Add(a1.Data, 1);
        //PotionItem p1 = new PotionItem(potionItemData, 1);
        //Add(p1.Data, 99);
        //PotionItem p2 = new PotionItem(potionItemData, 1);
        //Add(p2.Data, 30);

        // 아이템
        foreach(ItemData data in itemDatas)
        {
            CountableItemData cid = data as CountableItemData;
            if (cid != null)
                Add(cid, cid.MaxAmount);
            else
                Add(data, 1);
        }

        // 골드
        _MyGolds = int.MaxValue;
        _UI_inventory.SetMyGolds(_MyGolds);
    }

    //[SerializeField]
    //private WeaponItemData axeItemData;
    //[SerializeField]
    //private ArmorItemData armorItemData;
    //[SerializeField]
    //private PotionItemData potionItemData;

    public ItemData[] itemDatas = new ItemData[16];


    private void Start()
    {
        UpdateAccessibleStatesAll();
        //Debug.Log(Capacity);
    }

    public void Buy(ItemData item)
    {
        // 소유 금액 -= 아이템 가격
        if(Golds - item.Price <0)
        {
            Debug.Log($"{Golds} < {item.Price}");
            return;
        }

        Golds -= item.Price;

        // Golds 갱신
        UpdateCurrency();
    }


    // 해당 슬롯의 아이템 판매
    public void Sell(int idx)
    {
        if (_items[idx] == null) return;

        // 판매 가능한 아이템일 경우
        if (_items[idx] is ISellableItem sellableItem)
        {
            bool success = sellableItem.Sell();
            if (success)
            {
                // 소유 금액 += 아이템 가격
                if(Golds + (ulong)_items[idx].Data.Price > uint.MaxValue)
                {
                    // 오버 플로우 임시 방편
                    Debug.Log("Int Overflow");
                    Debug.Log("You Get Over Max Golds");
                    Debug.Log("You Can't Get More Golds. Plz Use Golds.");
                    Golds = uint.MaxValue;
                }
                else
                    Golds += _items[idx].Data.Price;

                // Golds 갱신
                UpdateCurrency();

                // Equipment Item -> Remove
                if(sellableItem is EquipmentItem)
                {
                    Remove(idx);
                }

                UpdateSlot(idx);
            }
        }

    }

    internal void trimItems()
    {
        // 인벤토리 내 아이템 사이 빈칸 없이 앞에서부터 채우기
        for(int targetItemIdx = 0; targetItemIdx < Capacity; targetItemIdx++)
        {
            // 아이템 있는 슬롯 찾기
            if (_items[targetItemIdx] == null) continue; 

            // 내 앞 슬롯 중에서 가장 빠른 인덱스 찾기
            Swap(targetItemIdx, FindEmptySlot(endIdx:targetItemIdx));
        }
    }

    // 인덱스가 수용 범위 내인지 검사
    private bool IsValidIndex(int idx)
    {
        return idx >= 0 && idx < Capacity;
    }

    // 빈 슬롯 중 가장 빠른 인덱스 찾기 (없으면 -1 리턴)
    private int FindEmptySlot(int startIdx = 0, int endIdx = -1)
    {
        if(endIdx == -1) endIdx = Capacity; // 입력 없을 시 Capcity로 설정하기 위함

        for(int i = startIdx; i < endIdx; i++)
        {
            if (_items[i] == null)
                return i;
        }
        return -1;
    }

    // 모든 슬롯 UI에 접근 가능 여부 업데이트
    public void UpdateAccessibleStatesAll()
    {
        _UI_inventory.SetAccessibleSlotRange(Capacity); 
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
    // 유효하지 않은 인덱스     : -1
    // 빈 슬롯                 : 0
    // 셀 수 없는 아이템        : 1
    // 셀 수 있는 아이템        : Amount
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
        if (!IsValidIndex(idx)) return "";
        if (_items[idx] == null) return "";

        return _items[idx].Data.Name;
    }

    public void UpdateCurrency()
    {
        if(Golds >= _maxGolds)
        {
            // Max Golds
            Golds = _maxGolds;
            Debug.Log("You Have Max Golds");
            Debug.Log("You Can't Get More Golds. Plz Use Golds.");
        }
        else if(Golds < 0)
        {
            Golds = 0;
        }

        _UI_inventory.SetMyGolds(Golds);
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

    // 아이템 위치 교환
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

    // 셀 수 있는 아이템 수량 나누기
    internal void SplitItems(int indexFrom, int indexTo, int amount, bool tryRemove =false)
    {
        if(tryRemove == false)
        {
            if (!IsValidIndex(indexFrom)) return;
            if (!IsValidIndex(indexTo)) return;

            Item _itemFrom = _items[indexFrom];
            Item _itemTo = _items[indexTo];

            CountableItem _ciFrom = _itemFrom as CountableItem;
            // CountableItem To Empty Slot
            if (_ciFrom != null && _itemTo == null)
            {
                _items[indexTo] = _ciFrom.SeperateAndClone(amount);

                UpdateSlot(indexFrom);
                UpdateSlot(indexTo);
            }
        }
        else // 분할 버리기
        {
            if (!IsValidIndex(indexFrom)) return;
            Item _itemFrom = _items[indexFrom];
            CountableItem _ciFrom = _itemFrom as CountableItem;
            // CountableItem To Empty Slot
            if (_ciFrom != null)
            {
                _ciFrom.SeperateAndClone(amount);

                UpdateSlot(indexFrom);
            }
        }

    }

    // 앞에서부터 비어있는 슬롯 인덱스 탐색
    private int FindEmptySlotIndex(int start_idx = 0)
    {
        for (int i = start_idx; i < Capacity; i++)
            if (_items[i] == null)
                return i;

        return -1;
    }

    // 앞에서부터 개수 여유 있는 셀 수 있는 아이템의 슬롯 인덱스 탐색
    private int FIndCountableItemSlotIndex(CountableItemData target, int start_idx = 0)
    {
        for(int i = start_idx; i < Capacity; i++)
        {
            var current = _items[i];
            if (current == null) continue;

            // 아이템 종류 일치, 개수 여유 확인
            if(current.Data == target && current is CountableItem ci)
            {
                if (!ci.IsMax) return i; 
            }
        }
        
        return -1;
    }

    // 해당 슬롯의 아이템 사용
    public void Use(int idx)
    {
        if (_items[idx] == null) return;

        // 사용 가능한 아이템일 경우
        if(_items[idx] is IUsableItem usableItem)
        {
            // 소모템의 경우 여기서 수량 감소
            bool success = usableItem.Use();

            // 장비 아이템은 착용해야 함
            if(usableItem is EquipmentItem equipmentItem)
            {
                // 1. 장비창 UI로 이동
                // TODO : 
                Debug.Log($"{equipmentItem.Data.Name} 착용");

                // 2. 인벤토리에서 제거
                Remove(idx);
            }
            else // 소모 아이템
            {
                // 1. 효과 적용
                // TODO :
                Debug.Log($"{_items[idx]} 사용");

                // 수량은 위에서 감소 usableItem.Use();
            }

            if (success)
                UpdateSlot(idx);
        }
    }

    public void Remove(int idx)
    {
        if (!IsValidIndex(idx)) return;

        _items[idx] = null;
        UpdateSlot(idx);
    }


    #region ItemSort
    // 아이템 가중치 딕셔너리
    // 아이템 타입에 따라 가중치
    private readonly static Dictionary<Type, int> _sortWeightDict = new Dictionary<Type, int>
    {
        {typeof(PotionItemData), 10000 },
        {typeof(WeaponItemData), 20000 },
        {typeof(ArmorItemData), 30000 },
        {typeof(EtcItemData), 40000},
        {typeof(QuestItemData), 50000},
    };

    private class ItemComparer : IComparer<Item>
    {
        // 아이템 우선순위 : 음,0,양수 리턴
        // 음수 : x 가 y 보다 앞
        public int Compare(Item x, Item y)
        {
            return (x.Data.ID + _sortWeightDict[x.Data.GetType()]) - (y.Data.ID + _sortWeightDict[y.Data.GetType()]);
        }
    }
    private static readonly ItemComparer _itemComparer = new ItemComparer();

    // 아이템 정렬
    public void SortAll()
    {
        // 1. Trim
        trimItems();

        // 2. Sort
        Array.Sort(_items, 0, FindEmptySlotIndex(), _itemComparer); // FindEmptySlotIndex 로 아이템 있는 슬롯 개수 구하기

        // 3. Update
        UpdateAllSlot();
    }

    private void UpdateAllSlot()
    {
        for(int i = 0; i < 42; i++)
            UpdateSlot(i);
    }
    #endregion
}
