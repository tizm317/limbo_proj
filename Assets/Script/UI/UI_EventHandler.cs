using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // EventSystem에서 던져주는 이벤트 받아옴
    // UI 에서 캐치해서 콜백 날려줌

    // 필요한 인터페이스 정의 및 구현해주면 됨.
    // 포인터클릭핸들러, 드래그핸들러 상속받음 (클릭 이벤트 , 드래그 이벤트 받아오기 위해)

    // 함수랑 연결 짓는 것도 밑에 콜백함수 연결시켜서 함수가 실행되도록 하면됨.

    // Action 이용해서 추가하고 싶은 함수 연동
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 누가 클릭하면 , 구독한 곳으로 event 전파

        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData); // 구독 신청한 애들한테 전파
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position;
        //Debug.Log("OnDrag");

        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

 
}
