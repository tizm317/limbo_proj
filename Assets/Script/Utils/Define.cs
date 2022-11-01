using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    // 여러가지 정보들 enum type 으로 관리하는 class

    public enum WorldObject
    {
        Unknown,
        Monster,
        Player,
    }
    public enum State
    {
        Idle,
        Moving,
        Patroll,
        Skill,
        JumpSkill,
        Hit,
        Die,
    }
    public enum Layer
    {
        Wall = 8,
        Monster = 9,
        Ground,
        Block,
        Building,
        NPC,
    }

    public enum Scene
    {
        // 씬 타입 Define 에서 관리
        // 순서 말고 type으로 받아서 상관x
        Unknown, // 디폴트
        Login,
        Lobby,
        //InGame,
        Dungeon,
        InGameBoss,
        InGameNatureBoss,
        InGameDesertBoss,

        InGameVillage,
        InGameNature,
        InGameDesert,
        InGameCemetery, // 공동묘지
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
        PointerUp,
        PointerDown,
        PointerEnter,
        PointerExit,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown, // 처음 누르는 행위
        PointerUp,   // 떼는 행위
        Click, // 잠깐 눌렀다 떼는 행위
        Right_Press,//우클 좌클 구분용
        Right_PointerDown,//우클 좌클 구분용
    }

    public enum CameraMode
    {
        QuarterView,
    }

    // For NPC State Machine
    public enum NpcState
    {
        // 기본
        STATE_IDLE,
        STATE_NPC_UI_POPUP,
        STATE_DIALOGUE,
        // 상점
        STATE_SHOP_UI_POPUP,
        //STATE_BUY_UI_POPUP,
        //STATE_SELL_UI_POPUP,
        //// 창고
        //STATE_STORAGE_UI_POPUP,
        //// 퀘스트
        STATE_QUEST_UI_POPUP,
        STATE_MAP_UI_POPUP,
    }

    // Event
    public enum Event
    {
        // 기본 NPC EVENT
        EVENT_NPC_CLICKED_IN_DISTANCE,
        //EVENT_QUIT_UI_POPUP,
        EVENT_PUSH_DIALOGUE,
        //EVENT_OTHER_DIALOGUE,
        EVENT_QUIT_DIALOGUE,
        // 상점
        EVENT_PUSH_SHOP,
        //EVENT_PUSH_BUY,
        //EVENT_QUIT_BUY,
        //EVENT_PUSH_SELL,
        //EVENT_QUIT_SELL,
        EVENT_QUIT_SHOP,
        // 창고
        // ...
        // 퀘스트
        EVENT_PUSH_QUEST,
        EVENT_ACCEPT_QUEST,
        EVENT_REJECT_QUEST,
        //EVENT_QUIT_QUEST,
        EVENT_PUSH_MAP,
        EVENT_QUIT_MAP,
    }

    public enum Job
    {
        WARRIOR,
        ARCHER,
        SORCERER,
    }
}
