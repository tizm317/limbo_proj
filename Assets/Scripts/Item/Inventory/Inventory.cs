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

    #region 코루틴 Wrapper 메소드
    // predicate 조건 불충족하면, 대기함
    private void ProcessLater(Func<bool> predicate, Action job)
    {
        StartCoroutine(PorcessLaterRoutine());

        // Local
        IEnumerator PorcessLaterRoutine()
        {
            yield return new WaitUntil(predicate);
            job?.Invoke();
        }
    }
    #endregion

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

    UI_InGame UI_InGame;


    // PlayerStat에서
    // 아이템 목록
    //[SerializeField]
    private Item[] _items;

    // 퀘스트 목록
    [SerializeField]
    private List<Data.Quest> _myQuests;

    // 소유 골드
    [SerializeField]
    private uint _MyGolds; // unsigned int

    // 최대 소유 골드
    //[SerializeField, Range(0, uint.MaxValue)]
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

    PlayerStat myPlayerStat;

    /* Methods */
    public void Init()
    {

        // Player Stat하고 연동
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            // player 가 아직 생성 전이면, 생긴 이후에 다시 Init하도록 코루틴으로 대기함
            ProcessLater(() => GameObject.FindGameObjectWithTag("Player") != null, () => Init());
            return;
        }
        myPlayerStat = Player.GetComponent<PlayerStat>();
        if(myPlayerStat._items == null)
        {
            myPlayerStat._items = new Item[_maxCapacity];
        }
        _items = myPlayerStat._items;
        _myQuests = myPlayerStat._myQuests;
        _MyGolds = myPlayerStat.Gold;

        //_items = new Item[_maxCapacity];
        Capacity = _initialCapacity;

        // TODO 수정
        // 씬 넘어갈 때 사라지는 거 방지 임시로 -> 이거로 안됨
        //UpdateQuest();

        // Start쪽에 있던부분
        //Debug.Log(Capacity);
        UpdateAccessibleStatesAll();

        // Dictionary Setting
        foreach (ItemData data in itemDatas)
        {
            itemDict.Add(data.ID, data);
        }
    }

    private void Start()
    {
        // Init 안쪽으로
        //Debug.Log(Capacity);
        //UpdateAccessibleStatesAll();

        //// Dictionary Setting
        //foreach(ItemData data in itemDatas)
        //{
        //    itemDict.Add(data.ID, data);
        //}
    }

    private void Awake()
    {
        Init();

        //_items = new Item[_maxCapacity];
        //Capacity = _initialCapacity;

        //_UI_inventory = Managers.UI.ShowPopupUI<UI_Inventory>();

        // 인벤토리 UI랑 연결
        //_UI_inventory.SetInventory(this);
    }

    #region Quest
    // 퀘스트 추가
    internal void AddQuest(Data.Quest quest)
    {
        _myQuests.Add(quest);

        // 퀘스트 미니 UI 갱신
        if (UI_InGame)
            UI_InGame.UpdateQuestUI(quest, 0);
    }

    // 인벤토리 Update 시 같이 작동
    // UpdateSlot 안에 넣으면 될까?
    public void UpdateQuest()
    {
        foreach(Data.Quest quest in _myQuests)
        {
            // clear 시키고 다시 체크
            int myTargetItemCount = 0;

            // 인벤토리 내부 순회하면서 변화 있는지 체크
            foreach(Item it in _items)
            {
                if (it == null) continue;

                // 해당 퀘스트 아이템 가지고 있으면,
                if(it.Data.ID == quest.targetItemId)
                {
                    // 셀수 있는 아이템(중첩아이템)
                    if(it is CountableItem)
                    {
                        CountableItem ci = it as CountableItem;
                        myTargetItemCount += ci.Amount;
                    }
                    else // 중첩 안되는 아이템
                    {
                        myTargetItemCount++;
                    }
                }
            }

            // Quest Clear
            ItemData rewardItem = null;
            if (myTargetItemCount >= quest.targetItemCount)
            {
                QuestClear(quest);
                rewardItem = AddReward(quest); //
                DecreaseTargetItem(quest);
            }

            // 퀘스트 미니 UI 갱신
            if (UI_InGame)
                UI_InGame.UpdateQuestUI(quest, myTargetItemCount);

            // MyQuest에서 제거
            if (myTargetItemCount >= quest.targetItemCount)
                RemoveQuest(quest, rewardItem);
        }
    }

    // 클리어 시 보상(exp , items)
    public ItemData AddReward(Data.Quest quest)
    {
        Debug.Log($"{quest.name} Clear!!!");
        Debug.Log($"{itemDict[quest.rewardItemId]} Get!!!");
        Debug.Log($"Exp {quest.exp} Up!!!");

        // Item Reward
        //int idx;
        //Add(itemDict[quest.rewardItemId], out idx, quest.rewardItemCount);

        // Exp Reward
        myPlayerStat.Exp += quest.exp;

        // 여기서 바로 올리면 오류생김 -> quest 지워준 후 추가해주기
        return itemDict[quest.rewardItemId];
    }

    public void QuestClear(Data.Quest quest)
    {
        quest.clear = 1;
    }

    // TODO
    public void DecreaseTargetItem(Data.Quest quest)
    {
        Debug.Log($"{itemDict[quest.targetItemId].Name} : {quest.targetItemCount} Decrease!");

        //int count = 0;

        //// 인벤토리 내부 순회하면서 변화 있는지 체크
        //foreach (Item it in _items)
        //{
        //    if (it == null) continue;

        //    // 해당 퀘스트 아이템 가지고 있으면,
        //    if (it.Data.ID == quest.targetItemId)
        //    {
        //        // 셀수 있는 아이템(중첩아이템)
        //        if (it is CountableItem)
        //        {
        //            CountableItem ci = it as CountableItem;
        //            if(ci.Amount >= count)
        //            {
        //                //
        //            }
        //        }
        //        else // 중첩 안되는 아이템
        //        {
        //            count++;
        //        }
        //    }
        //}
    }

    // 퀘스트 제거
    public void RemoveQuest(Data.Quest quest, ItemData reward)
    {
        //_myQuests.Remove(quest);
        //myPlayerStat._myQuests.Remove(quest);
        if (reward == null) return;

        int rewardCount = quest.rewardItemCount;

        // 지금은 어차피 퀘스트 하나씩이라 클리어 해버림
        _myQuests.Clear();
        myPlayerStat._myQuests.Clear();

        // 퀘스트 지워준 후 보상 아이템 Add
        int idx;
        Add(reward, out idx, rewardCount);
    }
    #endregion

    // 인벤토리UI에서 호출
    public void SetInventoryUI(UI_Inventory inventoryUI)
    {
        _UI_inventory = inventoryUI;

        // 인게임UI도 연결
        UI_InGame = _UI_inventory.transform.parent.GetComponent<UI_InGame>();
    }


    public void test()
    {
        // 아이템
        foreach(ItemData data in itemDatas)
        {
            CountableItemData cid = data as CountableItemData;
            int tempIdx;
            if (cid != null)
                Add(cid, out tempIdx, cid.MaxAmount);
            else
                Add(data, out tempIdx, 1);
        }

        // 골드
        //_MyGolds = uint.MaxValue;
        _MyGolds = 0;
        _UI_inventory.SetMyGolds(_MyGolds);
    }

    public bool LoadFinish { get { return _inventory_loaded; } set { _inventory_loaded = value; } }
    private bool _inventory_loaded = false;

    //public void InvenLoad()
    //{
    //    string name = "bmc4886";
    //    List<Data.Inventory> inventory = Managers.Data.Inventories[name];

    //    foreach(Data.Inventory i in inventory)
    //    {
    //        int icount = i.itemCount;

    //        ItemData idata = null;
    //        foreach (ItemData data in itemDatas)
    //        {
    //            if(data.ID == i.itemID)
    //            {
    //                idata = data;
    //                break;
    //            }
    //        }

    //        CountableItemData cid = idata as CountableItemData;
    //        int tempIdx;
    //        if (cid != null)
    //            Add(cid, out tempIdx, icount);
    //        else
    //            Add(idata, out tempIdx, icount);
    //    }
    //    // 골드
    //    //_MyGolds = 0;

    //    //_UI_inventory.SetMyGolds(_MyGolds);

    //    // 로드 완료
    //    LoadFinish = true;
    //}

    public ItemData[] itemDatas = new ItemData[16];
    public Dictionary<int, ItemData> itemDict = new Dictionary<int, ItemData>();



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

    //[SerializeField]
    //private EtcItemData _CoinData;
    //private EtcItem coins;

    [SerializeField]
    private CoinItemData _CoinData;
    private CoinItem coins;

    // 해당 슬롯의 아이템 판매
    public void Sell(int idx)
    {
        if (_items[idx] == null) return;
        if (_items[idx].Data.Name == "Coins")
        {
            Use(idx);
            return;
        }

        // 판매 가능한 아이템일 경우
        if (_items[idx] is ISellableItem sellableItem)
        {
            bool success = sellableItem.Sell();
            if (success)
            {
                // 소유 금액 += 아이템 가격
                if(Golds + (ulong)_items[idx].Data.Price > uint.MaxValue)
                {
                    // 오버 플로우 해결
                    ulong overGolds = (Golds + (ulong)_items[idx].Data.Price) - (uint.MaxValue);

                    // 인벤토리 내 코인이 이미 존재하는지 체크
                    // 없을 시에만 새로 생성
                    bool checkCoinIcon = false;
                    foreach(Item item in _items)
                    {
                        if (item == null) continue;
                        if (item.Data.Name == "Coins")
                        {
                            checkCoinIcon = true;
                            break;
                        }
                    }

                    if(checkCoinIcon == false)
                    {
                        int index = -1;
                        Add(_CoinData, idx: out index);
                        coins = (CoinItem)_items[index];
                    }

                    coins.SaveGoldsToString(overGolds);
                    //_CoinData.SaveGoldsToString(overGolds);


                    // 오버 플로우 임시 방편
                    //Debug.Log("Int Overflow");
                    //Debug.Log("You Get Over Max Golds");
                    //Debug.Log("You Can't Get More Golds. Plz Use Golds.");
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

        // PlayerStat 쪽 Gold 동기화
        if(myPlayerStat)
            myPlayerStat.Gold = Golds; 
    }
    public void AddGold(uint gold)
    {
        Golds += gold;
        UpdateCurrency();
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

        // Quest Target Item의 개수 체크
        UpdateQuest();


        // local : 아이템 제거
        void RemoveIcon()
        {
            if(_UI_inventory != null)
            {
                _UI_inventory.RemoveItem(idx);
                _UI_inventory.HideItemAmountText(idx);
            }
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

    public bool IsUpdated = false;
    public int Add_Without_UI_Update(ItemData itemData, out int idx, int amount = 1)
    {
        // 1. 수량이 있는 아이템
        if (itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;
            idx = -1;

            while (amount > 0)
            {
                // 1.1 이미 해당 아이템이 인벤토리에 존재하고, 개수 여유 있는지 검사
                if (findNextCountable)
                {
                    idx = FIndCountableItemSlotIndex(ciData, idx + 1);

                    // 개수 여유 있는 기존 슬롯이 더 이상 없다고 판단될 경우, 빈 슬롯부터 탐색 시작
                    if (idx == -1)
                    {
                        findNextCountable = false;
                    }
                    else // 기존 슬롯을 찾은 경우, 양 증가시키고, 초과량 존재 시 amount에 초기화
                    {
                        CountableItem ci = _items[idx] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        IsUpdated = true;
                        //UpdateSlot(idx);
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

                        IsUpdated = true;
                        //UpdateSlot(idx);
                    }

                }
            }
        }
        else // 2. 수량이 없는 아이템
        {
            // 2.1 1개만 넣는 경우
            if (amount == 1)
            {
                idx = FindEmptySlotIndex();
                if (idx != -1)
                {
                    // 아이템 생성하고 슬롯에 추가
                    _items[idx] = itemData.CreatItem();
                    amount = 0;

                    IsUpdated = true;
                    //UpdateSlot(idx);
                }
            }

            // 2.2 2개 이상의 수량 없는 아이템을 동시 추가하는 경우
            for (idx = -1; amount > 0; amount--)
            {
                // 아이템에 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                idx = FindEmptySlotIndex(idx + 1);

                // 다 넣지 못한 경우 루프 종료
                if (idx == -1) break;

                // 아이템 생성하여 슬롯 추가
                _items[idx] = itemData.CreatItem();

                IsUpdated = true;
                //UpdateSlot(idx);
            }
        }

        //
        UpdateQuest();

        // 넣는 데 실패한 잉여 아이템 개수 리턴
        return amount;
    }

    // 인벤토리에 아이템 추가
    // 넣는 데 실패한 잉여 아이템 개수 리턴
    // 리턴 값 0이면 모두 성공
    public int Add(ItemData itemData, out int idx, int amount = 1)
    {
        //int idx;

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
            if(_items[idx].Data.Name == "Coins")
            {
                // Item To Gold 변환
                // 소유 금액 += 저장된 골드
                CoinItem savedGoldItem = (CoinItem)_items[idx];

                ulong savedGolds = savedGoldItem.SavedGoldsToUlong();
                if (Golds + savedGolds > uint.MaxValue)
                {
                    // 오버 플로우 해결
                    ulong overGolds = Golds + savedGolds - uint.MaxValue;

                    // 인벤토리 내 코인이 이미 존재하는지 체크
                    // 없을 시에만 새로 생성
                    //bool checkCoinIcon = false;
                    //foreach (Item item in _items)
                    //{
                    //    if (item == null) continue;
                    //    if (item.Data.Name == "Coins")
                    //    {
                    //        checkCoinIcon = true;
                    //        break;
                    //    }
                    //}

                    // 어차피 Use 하면서 코인 아이템 사라짐
                    // 0이 아닌 이상 다시 생성
                    if (overGolds != 0)
                    {
                        int index = -1;
                        Add(_CoinData, idx: out index);
                        coins = (CoinItem)_items[index];
                    }

                    coins.SaveGoldsToString(overGolds);
                    //_CoinData.SaveGoldsToString(overGolds);


                    Golds = uint.MaxValue;
                }

            }

            // 소모템의 경우 여기서 수량 감소
            bool success = usableItem.Use();

            // 장비 아이템은 착용해야 함
            if(usableItem is EquipmentItem equipmentItem)
            {
                // 1. 장비창 UI로 이동
                // TODO :
                EquipmentItem exchangedItem = null;
                bool equipSuccess = _UI_inventory.Equip(equipmentItem, out exchangedItem);

                // 2. 인벤토리에서 제거
                if(equipSuccess == true)
                {
                    //Debug.Log($"{equipmentItem.Data.Name} 착용");
                    Remove(idx);
                }

                if(exchangedItem != null)
                {
                    int tempIdx;
                    Add(exchangedItem.Data, idx: out tempIdx);
                }
            }
            else // 소모 아이템
            {
                // 1. 효과 적용
                // TODO :
                Debug.Log($"{_items[idx].Data.Name} 사용");

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
        {typeof(CoinItemData), 60000},
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

    public void UpdateAllSlot()
    {
        for(int i = 0; i < 42; i++)
            UpdateSlot(i);
    }

    public void SaveInventoryToJson()
    {
        //TODO
    }

    #endregion
}
