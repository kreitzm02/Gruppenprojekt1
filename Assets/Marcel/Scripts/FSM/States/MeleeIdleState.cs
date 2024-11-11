using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeIdleState : BaseState
{
    // IDLE STATE
    //
    // Intented behaviour: The object plays an idle animation and stays on its current position.
    //                     The duration of the idle state depends on a random value.
    // Intended condition: The state has a timer running, and if the timer hits a certain
    //                     time stamp, the state should change.
    //

    private string animName;
    public float randomTime = 0f;
    public float pastTime = 0f;
    public MeleeIdleState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.1f);
        randomTime = Random.Range(3, 5);
        Debug.Log("Entered Idle State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Idle State");
        pastTime = 0f;
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating Idle State");
        pastTime += Time.deltaTime;
    }
}
