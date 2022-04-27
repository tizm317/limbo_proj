using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel,
    }


    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        // 실제 인벤토리 정보 참고해서
        for (int i = 0; i < 8; i++)
        {
            //GameObject item = Managers.Resource.Instantiate("UI/Scene/UI_Inven_Item");
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform).gameObject;
            //item.transform.SetParent(gridPanel.transform);


            // 1번째 방법
            //UI_Inven_Item invenItem = Util.GetOrAddComponent<UI_Inven_Item>(item);
            // item.GetOrAddComponent 로  수정 - Extension 이용
            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            // 2번째 방법 : 프리팹 저장할 때 add component
            invenItem.SetInfo($"집행검{i}번");
        }
    }



}
