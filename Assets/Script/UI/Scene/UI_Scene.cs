using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    // �� UI ����� �� ��� �ް� �� Scene UI ���̽� Ŭ����

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        // �˾� UI �ƴϴϱ�,  Canvas �� order ���� X
        Managers.UI.SetCanvas(gameObject, false);
    }
}
