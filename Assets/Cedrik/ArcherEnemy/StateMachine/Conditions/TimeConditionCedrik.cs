using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Conditions/TimeCondition")]
public class TimeConditionCedrik : BaseConditionCedrik
{
    [SerializeField]
    private float minTime = 5.0f;
    [SerializeField]
    private float maxTime = 5.0f;

    private float timeUntilTrue;
    private float ellapsedTime;

    public override void Initialize(GameObject _thisGameObject)
    {
        timeUntilTrue = Random.Range(minTime,maxTime);
        ellapsedTime = 0;
    }

    public override bool IsConditionMet()
    {
        ellapsedTime += Time.deltaTime;
        return ellapsedTime >= timeUntilTrue;
    }
}
