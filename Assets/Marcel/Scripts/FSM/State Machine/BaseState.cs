using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public BaseStateMachine sm;
    public BaseState(BaseStateMachine _sm)
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
