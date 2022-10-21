using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventActionTable
{
    // Define.NpcState
    // Define.Event

    public Define.NpcState _curState { get; set; }
    public Define.Event _event { get; set; }
    // actions
    public Action _action { get; set; }
    public Define.NpcState _nextState { get; set; }

    public EventActionTable(Define.NpcState cs, Define.Event e, Action act, Define.NpcState ns)
    {
        _curState = cs;
        _event = e;
        //foreach(Action act in act_arr)
        //_action += act;
        _nextState = ns;
    }

}

