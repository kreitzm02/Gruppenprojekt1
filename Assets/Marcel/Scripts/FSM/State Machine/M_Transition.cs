using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<<< Updated upstream:Assets/Marcel/Scripts/FSM/State Machine/Transition_M.cs
public class Transition_M
========
public class M_Transition
>>>>>>>> Stashed changes:Assets/Marcel/Scripts/FSM/State Machine/M_Transition.cs
{
    public BaseState_M toState;
    public Func<bool> condition;
<<<<<<<< Updated upstream:Assets/Marcel/Scripts/FSM/State Machine/Transition_M.cs
    public Transition_M(BaseState_M _toState, Func<bool> _condition)
========
    public M_Transition(BaseState _toState, Func<bool> _condition)
>>>>>>>> Stashed changes:Assets/Marcel/Scripts/FSM/State Machine/M_Transition.cs
    {
        toState = _toState;
        condition = _condition;
    }
}
