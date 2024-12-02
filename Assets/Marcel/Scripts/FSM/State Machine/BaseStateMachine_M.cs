using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public abstract class BaseStateMachine_M : MonoBehaviour
{
    public Animator animator;
<<<<<<< Updated upstream:Assets/Marcel/Scripts/FSM/State Machine/BaseStateMachine_M.cs
    protected BaseState_M currentState;
    protected Dictionary<BaseState_M, List<Transition_M>> statesDict = new();
    protected List<Transition_M> anyStateTransitions = new();
=======
    protected BaseState currentState;
    protected Dictionary<BaseState, List<M_Transition>> statesDict = new();
    protected List<M_Transition> anyStateTransitions = new();
>>>>>>> Stashed changes:Assets/Marcel/Scripts/FSM/State Machine/BaseStateMachine.cs

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        SetupStates();
    }
    protected virtual void Update()
    {
        currentState.OnStateUpdate();
        CheckTransitions();
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        currentState?.OnStateCollisionEnter(collision);
    }
    protected virtual void OnDrawGizmos()
    {
        currentState?.OnStateGizmos();
    }


    public virtual void SetState(BaseState_M state)
    {
        currentState?.OnStateExit();
        currentState = state;
        currentState.OnStateEnter();
    }
    public virtual void CheckTransitions()
    {
        foreach (var transition in anyStateTransitions)
        {
            if (transition.condition.Invoke())
            {
                SetState(transition.toState);
                break;
            }
        }
        foreach (var transition in statesDict[currentState])
        {
            if (transition.condition.Invoke())
            {
                SetState(transition.toState);
                break;
            }
        }
    }

    public abstract void SetupStates();
}
