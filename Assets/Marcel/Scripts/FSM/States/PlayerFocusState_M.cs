using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusState_M : BaseState_M
{
    string animName;
    public PlayerFocusState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;   
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerFocusState_M");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerFocusState_M");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerFocusState_M");
    }
}
