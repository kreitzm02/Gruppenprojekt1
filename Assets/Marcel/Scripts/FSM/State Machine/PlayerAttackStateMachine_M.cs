using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerAttackStateMachine_M : BaseStateMachine_M
{
    private IKillable killable;
    private IInputHandler inputHandler;
    public IAttackable attackable;
    public PlayerBehaviour_M playerBehaviour;
    public PlayerStateMachine_M playerStateMachine;
    private int previousHealthPoints;
    [SerializeField] private GameObject fakeThrowObject;
    [SerializeField] private GameObject realThrowObject;
    [SerializeField] private Transform handSlot;
    [SerializeField] public float testing_NormalizedAttackAnimTime;
    protected override void Start()
    {
        killable = GetComponent<IKillable>();
        inputHandler = GetComponent<IInputHandler>();
        attackable = GetComponent<IAttackable>();
        playerBehaviour = GetComponent<PlayerBehaviour_M>();
        playerStateMachine = GetComponent<PlayerStateMachine_M>();
        previousHealthPoints = playerBehaviour.healthPoints;
        base.Start();
    }
    protected override void Update()
    {
        inputHandler.ButtonDownCheck();
        base.Update();
        previousHealthPoints = playerBehaviour.healthPoints;
    }
    public override void SetupStates()
    {
        PlayerAttackState_M attackState = new(this);
        ThrowingState_M throwingState = new(this, "Throw", fakeThrowObject, realThrowObject, handSlot);
        NullState_M nullState = new(this);

        statesDict.Add(attackState, new List<Transition_M>
        {
            new Transition_M(nullState, () => attackState.attackFinished),
            //new Transition_M(nullState, () => playerStateMachine.GetCurrentState() is HitState_M),
        });
        statesDict.Add(throwingState, new List<Transition_M>
        {
            new Transition_M(nullState, () => throwingState.stateCompleted), //test
            new Transition_M(nullState, () => Input.GetKeyUp(KeyCode.Q) && throwingState.canBeCancelled),
            new Transition_M(nullState, () => playerStateMachine.GetCurrentState() is HitState_M),
        });
        statesDict.Add(nullState, new List<Transition_M>
        {
            new Transition_M(attackState, () => Input.GetKeyDown(KeyCode.E) && currentState != attackState && playerStateMachine.GetCurrentState() is not HitState_M && playerStateMachine.GetCurrentState() is not MeleeDeathState_M),
            new Transition_M(throwingState, () => Input.GetKeyDown(KeyCode.Q) && playerStateMachine.GetCurrentState() is not MeleeDeathState_M && playerStateMachine.GetCurrentState() is not HitState_M),
        });

        SetState(nullState);
    }
}
