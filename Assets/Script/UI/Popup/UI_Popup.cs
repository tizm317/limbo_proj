using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    // �˾� ����� �� ��� �ް� �� ���̽� Ŭ����

    // ���� : �˾� �� ��, �ڿ� �� �˾� UI Ŭ�� ���ϵ��� UI �����տ� Blocker �̹���(�г�) ���� ����

    public override void Init()
    {
        // �˾� UI : Canvas �� order ���� ��û
        Managers.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        // UI_Popup ��ӹ��� �ֵ� ���� Close popup ������ ���� �������̽�
        Managers.UI.ClosePopupUI(this);
    }
}
