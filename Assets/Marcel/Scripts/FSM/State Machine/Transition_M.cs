using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition_M
{
    public BaseState_M toState;
    public Func<bool> condition;
    public Transition_M(BaseState_M _toState, Func<bool> _condition)
    {
        toState = _toState;
        condition = _condition;
    }
}
