using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState_M
{
    public BaseStateMachine_M sm;
    public BaseState_M(BaseStateMachine_M _sm)
    {
        sm = _sm;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public virtual void OnStateFixedUpdate()
    { }
    public abstract void OnStateExit();
    public virtual void OnStateGizmos()
    { }
    public virtual void OnStateCollisionEnter(Collision _col)
    { }
}
