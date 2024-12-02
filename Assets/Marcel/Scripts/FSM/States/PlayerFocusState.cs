using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFocusState : BaseState
{
    string animName;
    public PlayerFocusState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;   
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerFocusState");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerFocusState");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerFocusState");
    }
}
