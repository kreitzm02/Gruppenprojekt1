using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    public BaseState toState;
    public Func<bool> condition;
    public Transition(BaseState _toState, Func<bool> _condition)
    {
        toState = _toState;
        condition = _condition;
    }
}
