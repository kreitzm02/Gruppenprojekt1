using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachineCedrik : MonoBehaviour
{

    [SerializeField]
    private List<TransitionCedrik> transitions;

    [SerializeField]
    BaseStateCedrik startState;

    BaseStateCedrik currentState;

    GameObject thisObject;

    Dictionary<BaseStateCedrik,List<TargetStateWithCond>> Conditions = new Dictionary<BaseStateCedrik, List<TargetStateWithCond>>();

    void Start()
    {
        thisObject = this.GameObject();
        InitializeDictionary();
        currentState = startState;
        currentState.Initialize(thisObject);
        currentState.OnStateEnter();
        foreach (TargetStateWithCond _transition in Conditions[currentState])
        {
            _transition.targetCondition.Initialize(thisObject);
        }
    }

    void Update()
    {
        currentState.OnStateUpdate();
        TransitionHandler();
    }

    private void InitializeDictionary()
    {
        foreach (TransitionCedrik _transition in transitions)
        {
            Conditions[_transition.originState] = new List<TargetStateWithCond>();
            for (int i = 0; i < _transition.targetStateWithCond.Count; i++)
            {
                Conditions[_transition.originState].Add(_transition.targetStateWithCond[i]);
            }

        }
    }

    private void TransitionHandler()
    {
        foreach(TargetStateWithCond _transition in Conditions[currentState])
        {
            if(_transition.targetCondition.IsConditionMet() == true)
            {
                currentState.OnStateExit();
                currentState = _transition.targetState;
                currentState.Initialize(thisObject);
                foreach (TargetStateWithCond _transition2 in Conditions[currentState])
                {
                    _transition2.targetCondition.Initialize(thisObject);
                }
                currentState.OnStateEnter();
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (TargetStateWithCond _transition in Conditions[currentState])
        {
            if(_transition.targetCondition.IsImplemented())
            {
                _transition.targetCondition.DrawGizmos();
            }
        }
    }
}
