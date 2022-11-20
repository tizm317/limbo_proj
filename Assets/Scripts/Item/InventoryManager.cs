//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class InventoryManager : MonoBehaviour
//{
//    public Dictionary<int, Item2> Items { get; } = new Dictionary<int, Item2>();

//    public void Add(Item2 item)
//    {
//        Items.Add(item.ItemDbId, item);
//    }

//    public Item2 Get(int itemDbId)
//    {
//        Item2 item = null;
//        Items.TryGetValue(itemDbId, out item);
//        return item;
//    }

//    // 조건으로 찾기
//    public Item2 Find(Func<Item2, bool> condition)
//    {
//        foreach (Item2 item in Items.Values)
//        {
//            if (condition.Invoke(item))
//                return item;
//        }
//        return null;
//    }

//    public void Clear()
//    {
//        Items.Clear();
//    }
//}
