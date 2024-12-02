using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/WaitAndScout")]
public class WaitAndScoutCedrik : BaseStateCedrik
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
    }

    public override void OnStateUpdate()
    {
        thisAnimator.Play("IdleCombat");
    }

    public override void OnStateExit()
    {
    }
}
