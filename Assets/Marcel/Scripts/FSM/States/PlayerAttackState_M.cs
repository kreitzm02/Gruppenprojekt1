using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState_M : BaseState_M
{
    string animName;
    public PlayerAttackState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerAttackState_M");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerAttackState_M");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerAttackState_M");
    }
}
