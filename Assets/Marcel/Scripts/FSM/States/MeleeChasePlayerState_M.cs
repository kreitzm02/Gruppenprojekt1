using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeChasePlayerState_M : BaseState_M
{
    // CHASE TARGET STATE
    //
    // Intented behaviour: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    // Intended condition: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    //

    string animName;
    private Transform targetedEnemy;
    public bool hasTargetEnemy;
    private float movementSpeed = 5;
    private float rotationSpeed = 360;
    private Quaternion targetRotation;
    public Vector3 targetPoint;
    private Vector3 direction;
    private float targetThreshold = 0.2f;
    private MeleeStateMachine_M meleeSM;

    public MeleeChasePlayerState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        meleeSM = sm as MeleeStateMachine_M;
        targetedEnemy = meleeSM.targetedEnemy;
        sm.animator.CrossFade(animName, 0.1f);
        Debug.Log("Entered Chase Player State");
        if (targetedEnemy == null)
        {
            hasTargetEnemy = false;
            Debug.Log("Chase State was called without a target");
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Chase Player State");
    }

    public override void OnStateUpdate()
    {
        if (targetedEnemy == null)
        {
            targetPoint = sm.transform.position;
            return;
        }
        meleeSM.lastSeenTargetPos = targetPoint;
        Debug.Log("Updating Chase Player State");
        targetPoint = targetedEnemy.position;
        direction = (targetPoint - sm.transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Quaternion.Angle(targetRotation, sm.transform.rotation) < targetThreshold)
        {
            sm.transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }
}
