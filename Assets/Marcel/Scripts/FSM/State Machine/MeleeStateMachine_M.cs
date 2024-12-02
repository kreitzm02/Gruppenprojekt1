using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.UIElements;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.Rendering;

public class MeleeStateMachine_M : BaseStateMachine_M
{
    private IKillable killable;
    private ITargetDetectable targetDetectable;
    public IAttackable attackable;

    public float viewDistance = 20;
    public float attackRange = 1;
    public float viewConeAngle = 80;

    public Transform targetedEnemy;
    public Transform attackedEnemy;
    public Vector3 lastSeenTargetPos;

    protected override void Start()
    {
        killable = this.GetComponent<IKillable>();
        targetDetectable = this.GetComponent<ITargetDetectable>();
        attackable = this.GetComponent<IAttackable>();
        base.Start();
    }
    public override void SetupStates()
    {
        MeleeIdleState_M idleState = new(this, "2H_Melee_Idle");
        MeleeWalkState_M walkState = new(this, "Walking_B");
        MeleeAttackState_M attackState = new(this, "2H_Melee_Attack_Chop");
        MeleeAttackState_M heavyAttackState = new(this, "1H_Melee_Attack_Jump_Chop");
        MeleeChasePlayerState_M chasePlayerState = new(this, "Running_A");
        MeleeLastSeenPosState_M lastSeenPosState = new(this, "Running_A");
        MeleeSearchPlayerState_M searchPlayerState = new(this, "Idle_Combat");
        MeleeRunTowardsPlayerState_M runTowardsPlayerState = new(this, "Running_C");
        MeleeRetreatState_M retreatState = new(this, "Walking_Backwards");
        MeleeDeathState_M deathState = new(this, "Death_C_Skeletons");

        statesDict.Add(idleState, new List<Transition_M>
        {
            new Transition_M(walkState, () => idleState.pastTime > idleState.randomTime), // enemy is not visible
            new Transition_M(chasePlayerState, () => targetedEnemy != null && attackedEnemy == null), // enemy is visible
        });
        statesDict.Add(walkState, new List<Transition_M>
        {
            new Transition_M(idleState, () => Vector3.Distance(transform.position, walkState.targetPoint) < walkState.targetThreshold), // target point reached and enemy not visible
            new Transition_M(chasePlayerState, () => targetedEnemy != null && attackedEnemy == null), // enemy is visible
        });
        statesDict.Add(chasePlayerState, new List<Transition_M>
        {
            new Transition_M(attackState, () => attackedEnemy != null), // enemy is in attack range
            new Transition_M(lastSeenPosState, () => attackedEnemy == null && targetedEnemy == null), // enemy is not visible
        });
        statesDict.Add(attackState, new List<Transition_M>
        {
            new Transition_M (idleState, () => attackState.attackFinished && attackState.enemyKilled), // enemy is dead
            new Transition_M(retreatState, () => UnityEngine.Random.Range(1, 1000) == 2), // randomly starting heavy attack
            new Transition_M(chasePlayerState, () => attackedEnemy == null && targetedEnemy != null && attackState.attackFinished), // enemy is visible but not in attack range
            new Transition_M(idleState, () => attackedEnemy == null && targetedEnemy == null && attackState.attackFinished), // enemy is not visible
        });
        statesDict.Add(lastSeenPosState, new List<Transition_M>
        {
            new Transition_M(searchPlayerState, () => Vector3.Distance(transform.position, lastSeenPosState.targetPoint) < lastSeenPosState.targetThreshold), // target point reached
            new Transition_M(chasePlayerState, () => targetedEnemy != null), // enemy is visible
        });
        statesDict.Add(searchPlayerState, new List<Transition_M>
        {
            new Transition_M(chasePlayerState, () => targetedEnemy != null), // enemy is visible after scouting
            new Transition_M(idleState, () => targetedEnemy == null && searchPlayerState.rotationCounter == 2), // enemy is not visible after scouting
        });
        statesDict.Add(runTowardsPlayerState, new List<Transition_M>
        {
            new Transition_M(heavyAttackState, () => Vector3.Distance(transform.position, runTowardsPlayerState.targetPoint) < runTowardsPlayerState.targetThreshold), // reached enemy position
            new Transition_M(lastSeenPosState, () => targetedEnemy == null) // enemy is not visible
        });
        statesDict.Add(retreatState, new List<Transition_M>
        {
            new Transition_M(lastSeenPosState, () => targetedEnemy == null), // enemy is not visible
            new Transition_M(runTowardsPlayerState, () => Vector3.Distance(transform.position, retreatState.targetedEnemy.position) >= retreatState.moveDistance), // enemy is visible
        });
        statesDict.Add(deathState, new List<Transition_M>
        {
            // object will be destroyed after death state is called, so no transitions here.
        });
        statesDict.Add(heavyAttackState, new List<Transition_M>
        {
            // this state is just the attack state but with other transition conditions and another animations.
            new Transition_M (idleState, () => heavyAttackState.attackFinished && heavyAttackState.enemyKilled), // enemy is dead
            new Transition_M(idleState, () => targetedEnemy == null && attackedEnemy == null && heavyAttackState.attackFinished), // enemy is not visible
            new Transition_M(chasePlayerState, () => heavyAttackState.attackFinished), // enemy is visible - but not in attack range
        });

        anyStateTransitions.Add(new Transition_M(deathState, () => killable.CheckDeathCondition() == true && currentState != deathState));


        SetState(idleState);
    }
    protected override void Update()
    {
        targetedEnemy = targetDetectable.DetectTargetVisibleRange();
        attackedEnemy = targetDetectable.DetectTargetAttackRange();
        base.Update();
    }
}

