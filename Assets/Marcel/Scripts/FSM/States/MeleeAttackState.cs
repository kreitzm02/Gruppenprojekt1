using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class MeleeAttackState : BaseState
{
    // ATTACK STATE
    //
    // Intented behaviour: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    // Intended condition: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    //

    string animName;
    private float rotationSpeed = 360;
    private Quaternion targetRotation;
    public Vector3 targetPoint;
    private Vector3 direction;
    private Transform targetedEnemy;
    private AnimatorStateInfo animStateInfo;
    public bool attackFinished = false;
    public bool damageGiven = false;
    public bool enemyKilled = false;
    private IDamageable targetDamageable;
    private IKillable targetKillable;
    private Vector3 weaponHitBox;
    private Collider enemyCollider;
    private MeleeStateMachine meleeSM;
    public MeleeAttackState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.1f);
        meleeSM = sm as MeleeStateMachine;
        targetedEnemy = meleeSM.attackedEnemy;
        Debug.Log("Entered Attack State");
        if (targetedEnemy == null)
        {
            Debug.Log("Attack was called without a target");
        }
        else
        {
            targetDamageable = targetedEnemy.GetComponent<IDamageable>();
            targetKillable = targetedEnemy.GetComponent<IKillable>();
            enemyCollider = meleeSM.attackedEnemy.GetComponent<Collider>();
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Attack State");
    }

    public override void OnStateUpdate()
    {
        weaponHitBox = new Vector3(sm.transform.position.x, sm.transform.position.y + 1, sm.transform.position.z) + sm.transform.forward * 0.5f;
        animStateInfo = sm.animator.GetCurrentAnimatorStateInfo(0);
        float normalizedAnimTime = animStateInfo.normalizedTime % 1.0f;
        Debug.Log("Updating Attack State");
        if (targetedEnemy != null)
        {
            targetPoint = targetedEnemy.position;
        }
        direction = (targetPoint - sm.transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (normalizedAnimTime >= 0.99f)
        {
            attackFinished = true;
            damageGiven = false;
        }
        else
        {
            attackFinished = false;
        }
        if (normalizedAnimTime >= 0.76f && !attackFinished && !damageGiven)
        {
            Collider[] collider = Physics.OverlapSphere(weaponHitBox, 0.4f);
            foreach (var col in collider)
            {
                if (col == enemyCollider)
                {
                    targetDamageable.GainDamage(meleeSM.attackable.GetAttackDamage());
                    damageGiven = true;
                }
            }
        }
        if (targetedEnemy != null && targetKillable.CheckDeathCondition())
        {
            enemyKilled = true;
        }
        else enemyKilled = false;
    }

    public override void OnStateGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(weaponHitBox, 0.5f);
    }
}
