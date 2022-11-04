using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISellableItem
{
    /* 판매 가능한 아이템 인터페이스 */
    // 퀘스트 아이템 : 판매 불가

    // 아이템 판매 : 성공 여부 리턴
    bool Sell();
}
