using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    // 여러가지 정보들 enum type 으로 관리하는 class

    public enum Layer
    {
        Monster = 8,
        Ground,
        Block,
    }

    public enum Scene
    {
        // 씬 타입 Define 에서 관리
        Unknown, // 디폴트
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm, // loop 연관
        Effect,
        MaxCount  // MaxCount는 갯수 세기 위함
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown, // 처음 누르는 행위
        PointerUp,   // 떼는 행위
        Click, // 잠깐 눌렀다 떼는 행위
    }

    public enum CameraMode
    {
        QuarterView,
    }
}
