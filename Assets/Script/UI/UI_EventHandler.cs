using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // EventSystem���� �����ִ� �̺�Ʈ �޾ƿ�
    // UI ���� ĳġ�ؼ� �ݹ� ������

    // �ʿ��� �������̽� ���� �� �������ָ� ��.
    // ������Ŭ���ڵ鷯, �巡���ڵ鷯 ��ӹ��� (Ŭ�� �̺�Ʈ , �巡�� �̺�Ʈ �޾ƿ��� ����)

    // �Լ��� ���� ���� �͵� �ؿ� �ݹ��Լ� ������Ѽ� �Լ��� ����ǵ��� �ϸ��.

    // Action �̿��ؼ� �߰��ϰ� ���� �Լ� ����
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���� Ŭ���ϸ� , ������ ������ event ����

        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData); // ���� ��û�� �ֵ����� ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position;
        //Debug.Log("OnDrag");

        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

 
}
