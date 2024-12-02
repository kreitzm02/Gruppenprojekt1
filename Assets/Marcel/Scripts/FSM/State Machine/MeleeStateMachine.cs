using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.UIElements;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.Rendering;

public class MeleeStateMachine : BaseStateMachine
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
        MeleeIdleState idleState = new(this, "2H_Melee_Idle");
        MeleeWalkState walkState = new(this, "Walking_B");
        MeleeAttackState attackState = new(this, "2H_Melee_Attack_Chop");
        MeleeAttackState heavyAttackState = new(this, "1H_Melee_Attack_Jump_Chop");
        MeleeChasePlayerState chasePlayerState = new(this, "Running_A");
        MeleeLastSeenPosState lastSeenPosState = new(this, "Running_A");
        MeleeSearchPlayerState searchPlayerState = new(this, "Idle_Combat");
        MeleeRunTowardsPlayerState runTowardsPlayerState = new(this, "Running_C");
        MeleeRetreatState retreatState = new(this, "Walking_Backwards");
        MeleeDeathState deathState = new(this, "Death_C_Skeletons");

        statesDict.Add(idleState, new List<M_Transition>
        {
            new M_Transition(walkState, () => idleState.pastTime > idleState.randomTime), // enemy is not visible
            new M_Transition(chasePlayerState, () => targetedEnemy != null && attackedEnemy == null), // enemy is visible
        });
        statesDict.Add(walkState, new List<M_Transition>
        {
            new M_Transition(idleState, () => Vector3.Distance(transform.position, walkState.targetPoint) < walkState.targetThreshold), // target point reached and enemy not visible
            new M_Transition(chasePlayerState, () => targetedEnemy != null && attackedEnemy == null), // enemy is visible
        });
        statesDict.Add(chasePlayerState, new List<M_Transition>
        {
            new M_Transition(attackState, () => attackedEnemy != null), // enemy is in attack range
            new M_Transition(lastSeenPosState, () => attackedEnemy == null && targetedEnemy == null), // enemy is not visible
        });
        statesDict.Add(attackState, new List<M_Transition>
        {
            new M_Transition (idleState, () => attackState.attackFinished && attackState.enemyKilled), // enemy is dead
            new M_Transition(retreatState, () => UnityEngine.Random.Range(1, 1000) == 2), // randomly starting heavy attack
            new M_Transition(chasePlayerState, () => attackedEnemy == null && targetedEnemy != null && attackState.attackFinished), // enemy is visible but not in attack range
            new M_Transition(idleState, () => attackedEnemy == null && targetedEnemy == null && attackState.attackFinished), // enemy is not visible
        });
        statesDict.Add(lastSeenPosState, new List<M_Transition>
        {
            new M_Transition(searchPlayerState, () => Vector3.Distance(transform.position, lastSeenPosState.targetPoint) < lastSeenPosState.targetThreshold), // target point reached
            new M_Transition(chasePlayerState, () => targetedEnemy != null), // enemy is visible
        });
        statesDict.Add(searchPlayerState, new List<M_Transition>
        {
            new M_Transition(chasePlayerState, () => targetedEnemy != null), // enemy is visible after scouting
            new M_Transition(idleState, () => targetedEnemy == null && searchPlayerState.rotationCounter == 2), // enemy is not visible after scouting
        });
        statesDict.Add(runTowardsPlayerState, new List<M_Transition>
        {
            new M_Transition(heavyAttackState, () => Vector3.Distance(transform.position, runTowardsPlayerState.targetPoint) < runTowardsPlayerState.targetThreshold), // reached enemy position
            new M_Transition(lastSeenPosState, () => targetedEnemy == null) // enemy is not visible
        });
        statesDict.Add(retreatState, new List<M_Transition>
        {
            new M_Transition(lastSeenPosState, () => targetedEnemy == null), // enemy is not visible
            new M_Transition(runTowardsPlayerState, () => Vector3.Distance(transform.position, retreatState.targetedEnemy.position) >= retreatState.moveDistance), // enemy is visible
        });
        statesDict.Add(deathState, new List<M_Transition>
        {
            // object will be destroyed after death state is called, so no transitions here.
        });
        statesDict.Add(heavyAttackState, new List<M_Transition>
        {
            // this state is just the attack state but with other transition conditions and another animations.
            new M_Transition (idleState, () => heavyAttackState.attackFinished && heavyAttackState.enemyKilled), // enemy is dead
            new M_Transition(idleState, () => targetedEnemy == null && attackedEnemy == null && heavyAttackState.attackFinished), // enemy is not visible
            new M_Transition(chasePlayerState, () => heavyAttackState.attackFinished), // enemy is visible - but not in attack range
        });

        anyStateTransitions.Add(new M_Transition(deathState, () => killable.CheckDeathCondition() == true && currentState != deathState));


        SetState(idleState);
    }
    protected override void Update()
    {
        targetedEnemy = targetDetectable.DetectTargetVisibleRange();
        attackedEnemy = targetDetectable.DetectTargetAttackRange();
        base.Update();
    }
}

