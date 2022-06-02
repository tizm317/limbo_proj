using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTestScene : BaseScene
{
    void Awake()
    {
        // cf) Awake : Start 보다 먼저, 컴포넌트 꺼져있어도 오브젝트가 들고 있으면 가능
        // 주의 : 오브젝트가 꺼져있으면 안 됨.
        Init();
    }

    protected override void Init()
    {
        // 초기화

        base.Init();

        // 씬타입 설정
        SceneType = Define.Scene.MapTest;

        // 씬 UI
        Managers.UI.ShowSceneUI<UI_InGame>("UI_InGame");

        // 커서컨트롤러 @Scene
        gameObject.GetOrAddComponent<CursorController>();
        // @Scene으로 옮김
        gameObject.GetOrAddComponent<Setting>();
        gameObject.GetOrAddComponent<Player_Controller>();

    }
    public override void Clear()
    {
        // 이 씬이 종료될 때 날려줘야 하는 부분 넣어줘야 함
    }
}
