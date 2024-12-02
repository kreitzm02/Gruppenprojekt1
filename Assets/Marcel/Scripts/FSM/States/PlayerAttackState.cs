using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : BaseState
{
    string animName;
    public PlayerAttackState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerAttackState");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerAttackState");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerAttackState");
    }
}
