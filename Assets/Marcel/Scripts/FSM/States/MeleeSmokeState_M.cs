using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSmokeState_M : BaseState_M
{
    private string animName;
    public MeleeSmokeState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        
    }

    public override void OnStateUpdate()
    {
        
    }
}
