using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateMachine_M : BaseStateMachine_M
{
    private IKillable killable;
    private IInputHandler inputHandler;

    protected override void Start()
    {
        killable = GetComponent<IKillable>();
        inputHandler = GetComponent<IInputHandler>();
        base.Start();
    }

    protected override void Update()
    {
        inputHandler.ButtonDownCheck();
        base.Update();
    }
    public override void SetupStates()
    {
        PlayerIdleState_M idleState = new(this, "Idle");
        WalkInDirectionState_M walkDirectionState = new(this, "Walking_A");
        WalkToPointState_M walkPointState = new(this, "Walking_A");
        WalkToTargetState_M walkTargetState = new(this, "Walking_A");
        MeleeDeathState_M deathState = new(this, "Death_A");
        DodgeState_M dodgeState = new(this, "Dodge_Forward", "Dodge_Backward", "Dodge_Left", "Dodge_Right");

        statesDict.Add(idleState, new List<Transition_M>
        {
            new Transition_M(walkPointState, () => inputHandler.IsClick()),
            new Transition_M(walkDirectionState, () => inputHandler.IsHold()),
            new Transition_M(walkTargetState, () => false),
            new Transition_M(dodgeState, () => Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.LeftControl)),
        });
        statesDict.Add(walkDirectionState, new List<Transition_M>
        {
            new Transition_M(idleState, () => !Input.GetMouseButton(0)),
            new Transition_M(dodgeState, () => Input.GetKeyDown(KeyCode.LeftControl)),
        });
        statesDict.Add(walkPointState, new List<Transition_M>
        {
            new Transition_M(idleState, () => Vector3.Distance(transform.position, walkPointState.targetPosition) < walkPointState.targetThreshold),
            new Transition_M(idleState, () => Input.GetMouseButton(1)),
            new Transition_M(walkPointState, () => inputHandler.IsClick()),
            new Transition_M(walkDirectionState, () => inputHandler.IsHold()),
            new Transition_M(dodgeState, () => Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.LeftControl)),
            new Transition_M(walkTargetState, () => false),
        });
        statesDict.Add(walkTargetState, new List<Transition_M> // TODO
        {
            new Transition_M(idleState, () => Vector3.Distance(transform.position, walkTargetState.targetPoint) < walkPointState.targetThreshold),
            new Transition_M(idleState, () => Input.GetMouseButton(1)),
            new Transition_M(walkPointState, () => inputHandler.IsClick()),
            new Transition_M(walkDirectionState, () => inputHandler.IsHold()),
            new Transition_M(dodgeState, () => Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.LeftControl)),
        });
        statesDict.Add(dodgeState, new List<Transition_M>
        {
            new Transition_M(walkDirectionState, () => Vector3.Distance(transform.position, dodgeState.originalPosition) > dodgeState.targetDistance),
        });
        statesDict.Add(deathState, new List<Transition_M>
        {
        });

        anyStateTransitions.Add(new Transition_M(deathState, () => killable.CheckDeathCondition() == true && currentState != deathState));

        SetState(idleState);
    }
}
