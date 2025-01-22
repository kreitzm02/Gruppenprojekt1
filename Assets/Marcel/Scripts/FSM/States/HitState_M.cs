using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState_M : BaseState_M
{
    string animName;
    private AnimatorStateInfo animStateInfo;
    public bool animationComplete;
    private Vector3 enemyPosition;
    private float knockbackForce;
    public HitState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.1f);
        Debug.Log("Entering Hit State");
        animationComplete = false;
    }

    public override void OnStateExit()
    {
        Debug.Log("Leaving Hit State");
    }

    public override void OnStateUpdate()
    {
        animStateInfo = sm.animator.GetCurrentAnimatorStateInfo(0);
        float normalizedAnimTime = animStateInfo.normalizedTime % 1.0f;
        if (normalizedAnimTime >= 0.95f)
        {
            animationComplete = true;
        }
    }
}
