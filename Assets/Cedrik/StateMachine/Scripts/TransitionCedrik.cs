using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "StateMachine/Transition")]
public class TransitionCedrik : ScriptableObject
{
    public BaseStateCedrik originState;

    [SerializeField]
    public List<TargetStateWithCond> targetStateWithCond;
    
}

[System.Serializable]
public struct TargetStateWithCond
{
    public BaseStateCedrik targetState;
    public BaseConditionCedrik targetCondition;
    

}