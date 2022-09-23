using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNpc : Npc
{
    UI_Shop _UI_Shop;
    UI_Inven _UI_Inven;

    public override void Awake()
    {
        Init();
    }

    public override void Init()
    {
        curState = Define.NpcState.STATE_IDLE;

        // table init
        table = new EventActionTable[]
        {
            new EventActionTable(Define.NpcState.STATE_IDLE, Define.Event.EVENT_NPC_CLICKED_IN_DISTANCE, null, Define.NpcState.STATE_NPC_UI_POPUP),

            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_DIALOGUE, null, Define.NpcState.STATE_DIALOGUE), // 대화 시작
            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_PUSH_SHOP,  null,    Define.NpcState.STATE_SHOP_UI_POPUP),
            new EventActionTable(Define.NpcState.STATE_NPC_UI_POPUP, Define.Event.EVENT_QUIT_DIALOGUE, null, Define.NpcState.STATE_IDLE),

            new EventActionTable(Define.NpcState.STATE_DIALOGUE, Define.Event.EVENT_PUSH_DIALOGUE,  null, Define.NpcState.STATE_DIALOGUE), // 대화중
            new EventActionTable(Define.NpcState.STATE_DIALOGUE, Define.Event.EVENT_QUIT_DIALOGUE,  null, Define.NpcState.STATE_NPC_UI_POPUP), // 대화 끝 : 초기화
                                    
            new EventActionTable(Define.NpcState.STATE_SHOP_UI_POPUP, Define.Event.EVENT_QUIT_SHOP,  null, Define.NpcState.STATE_NPC_UI_POPUP),
        };

        // action 등록

        // NPC 상호작용 시작
        table[0]._action -= checkNCloseOtherPopUpUI;
        table[0]._action += checkNCloseOtherPopUpUI;
        table[0]._action -= lookAtPlayer;
        table[0]._action += lookAtPlayer;
        table[0]._action -= npcUIPopUp;
        table[0]._action += npcUIPopUp;
        table[0]._action -= showNpcInfo;
        table[0]._action += showNpcInfo;

        //NPC 거래 버튼 클릭
        table[2]._action -= ShowTradeUI;
        table[2]._action += ShowTradeUI;

        // NPC 거래 끝
        table[6]._action -= CloseTradeUI;
        table[6]._action += CloseTradeUI;


        // NPC 상호작용 끝
        table[3]._action -= npcUIClose;
        table[3]._action += npcUIClose;



        // Npc 정보 읽기
        Dictionary<int, Data.Npc> dict = Managers.Data.NpcDict;
        _id = dict[num_npc].id;
        _name = dict[_id].name;
        _job = dict[_id].job;


        num_npc++; // static 변수 이용

        
    }

    public override void npcUIPopUp()
    {
        // 클릭되어서 맨 처음 npc UI 뜨는거
        _UI_Dialogue = Managers.UI.ShowPopupUI<UI_Dialogue>();
        if (_UI_Dialogue)
            ui_dialogue_ison = true;
        else
        {
            Debug.Log("Error : No UI_Dialogue");
            return;
        }

        // UI 쪽도 NPC 정보 필요
        _UI_Dialogue.getNpcInfo(this);

        // NPC 대화
        table[1]._action -= _UI_Dialogue.dialogue;
        table[1]._action += _UI_Dialogue.dialogue;
        table[4]._action -= _UI_Dialogue.dialogue;
        table[4]._action += _UI_Dialogue.dialogue;

        // NPC 대화 끝
        table[5]._action -= _UI_Dialogue.dialogEnd;
        table[5]._action += _UI_Dialogue.dialogEnd;
    }

    public void ShowTradeUI()
    {
        // Shop UI
        _UI_Shop = Managers.UI.ShowPopupUI<UI_Shop>();
        _UI_Shop.getNpcInfo(this);

        // Inventory UI
        _UI_Inven = Managers.UI.ShowPopupUI<UI_Inven>();
    }

    public void CloseTradeUI()
    {
        // Inventory UI
        _UI_Inven.ClosePopupUI();
        _UI_Inven = null;

        // Shop UI
        _UI_Shop.ClosePopupUI();
        _UI_Shop = null;
    }
}
