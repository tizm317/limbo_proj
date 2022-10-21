using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUsableItem
{
    /* 사용 가능한 아이템 인터페이스 */
    // 무기, 소모템 등

    // 아이템 사용 : 성공 여부 리턴
    bool Use();
}
