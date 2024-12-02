using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/Idle")]
public class IdleCedrik : BaseStateCedrik
{
    bool initialized = false;
    //GameObject thisObject;
    Animator thisAnimator;

    public override void Initialize(GameObject _thisObject)
    {
        if (!initialized)
        {
            thisAnimator = _thisObject.GetComponent<Animator>();
            initialized = true;
        }
    }

    public override void OnStateEnter()
    {
        Debug.Log("idle enter");
    }

    public override void OnStateUpdate()
    {
        thisAnimator.Play("Idle");
    }

    public override void OnStateExit()
    {
    }
}
