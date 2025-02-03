using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerStateMachine : BaseStateMachine_M
{
    private IKillable killable;
    private ITargetDetectable targetDetectable;
    public IAttackable attackable;

    public Transform targetedEnemy;
    public Transform attackedEnemy;
    public Vector3 lastSeenTargetPos;

    private MeleeSkeletonBehaviour_M skeletonBehaviour;
    private int previousHealthPoints;
    protected override void Start()
    {
        killable = this.GetComponent<IKillable>();
        targetDetectable = this.GetComponent<ITargetDetectable>();
        attackable = this.GetComponent<IAttackable>();
        skeletonBehaviour = GetComponent<MeleeSkeletonBehaviour_M>();
        previousHealthPoints = skeletonBehaviour.healthPoints;
        base.Start();
    }
    public override void SetupStates()
    {
        MeleeIdleState_M idleState = new(this, "idle");
        HealTargetState healState = new(this);
        MeleeWalkState_M walkState = new(this, "idle");
        DodgeState_M dodgeState = new(this, "idle", "idle", "idle", "idle");
        MeleeDeathState_M deathState = new(this, "name");
        HitState_M hitState = new(this, "nam");

        statesDict.Add(idleState, new List<Transition_M>
        {
            new Transition_M(walkState, () => idleState.pastTime > idleState.randomTime), 
        });
        statesDict.Add(healState, new List<Transition_M>
        {
            
        });
        statesDict.Add(walkState, new List<Transition_M>
        {
            new Transition_M(idleState, () => Vector3.Distance(transform.position, walkState.targetPoint) < walkState.targetThreshold),
        });
        statesDict.Add(dodgeState, new List<Transition_M>
        {
            
        });
        statesDict.Add(deathState, new List<Transition_M>
        {
            
        });
        statesDict.Add(hitState, new List<Transition_M>
        {

        });

        anyStateTransitions.Add(new Transition_M(deathState, () => killable.CheckDeathCondition() == true && currentState != deathState));
        anyStateTransitions.Add(new Transition_M(hitState, () => skeletonBehaviour.healthPoints < previousHealthPoints));
    }

    protected override void Update()
    {
        targetedEnemy = targetDetectable.DetectTargetVisibleRange();
        attackedEnemy = targetDetectable.DetectTargetAttackRange();
        base.Update();
        previousHealthPoints = skeletonBehaviour.healthPoints;
    }
}
