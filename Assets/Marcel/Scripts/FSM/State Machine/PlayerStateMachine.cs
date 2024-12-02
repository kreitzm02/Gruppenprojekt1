using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateMachine : BaseStateMachine
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
        PlayerIdleState idleState = new(this, "Idle");
        WalkInDirectionState walkDirectionState = new(this, "Walking_A");
        WalkToPointState walkPointState = new(this, "Walking_A");
        WalkToTargetState walkTargetState = new(this, "Walking_A");
        MeleeDeathState deathState = new(this, "Death_A");
        DodgeState dodgeState = new(this, "Dodge_Forward", "Dodge_Backward", "Dodge_Left", "Dodge_Right");

        statesDict.Add(idleState, new List<Transition>
        {
            new Transition(walkPointState, () => inputHandler.IsClick()),
            new Transition(walkDirectionState, () => inputHandler.IsHold()),
            new Transition(walkTargetState, () => false),
            new Transition(dodgeState, () => Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.LeftControl)),
        });
        statesDict.Add(walkDirectionState, new List<Transition>
        {
            new Transition(idleState, () => !Input.GetMouseButton(0)),
            new Transition(dodgeState, () => Input.GetKeyDown(KeyCode.LeftControl)),
        });
        statesDict.Add(walkPointState, new List<Transition>
        {
            new Transition(idleState, () => Vector3.Distance(transform.position, walkPointState.targetPosition) < walkPointState.targetThreshold),
            new Transition(idleState, () => Input.GetMouseButton(1)),
            new Transition(walkPointState, () => inputHandler.IsClick()),
            new Transition(walkDirectionState, () => inputHandler.IsHold()),
            new Transition(dodgeState, () => Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.LeftControl)),
            new Transition(walkTargetState, () => false),
        });
        statesDict.Add(walkTargetState, new List<Transition> // TODO
        {
            new Transition(idleState, () => Vector3.Distance(transform.position, walkTargetState.targetPoint) < walkPointState.targetThreshold),
            new Transition(idleState, () => Input.GetMouseButton(1)),
            new Transition(walkPointState, () => inputHandler.IsClick()),
            new Transition(walkDirectionState, () => inputHandler.IsHold()),
            new Transition(dodgeState, () => Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.LeftControl)),
        });
        statesDict.Add(dodgeState, new List<Transition>
        {
            new Transition(walkDirectionState, () => Vector3.Distance(transform.position, dodgeState.originalPosition) > dodgeState.targetDistance),
        });
        statesDict.Add(deathState, new List<Transition>
        {
        });

        anyStateTransitions.Add(new Transition(deathState, () => killable.CheckDeathCondition() == true && currentState != deathState));

        SetState(idleState);
    }
}
