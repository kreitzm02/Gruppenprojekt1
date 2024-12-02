using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateCedrik : ScriptableObject
{
    public virtual void Initialize(GameObject _thisObject)
    {
        
    }

    public abstract void OnStateEnter();

    public abstract void OnStateUpdate();

    public abstract void OnStateExit();
}
