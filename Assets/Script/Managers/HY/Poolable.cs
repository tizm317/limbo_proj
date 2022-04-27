using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    // 아무 기능 없이, 오브젝트가 이 컴포넌트 들고 있으면 풀링할 대상임을 알려줌
    // 풀링 필요한 프리팹에 넣어두자!

    public bool IsUsing; // 현재 풀링된 상태인지
}
