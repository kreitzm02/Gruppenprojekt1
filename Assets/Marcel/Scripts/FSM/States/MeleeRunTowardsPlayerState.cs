using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRunTowardsPlayerState : BaseState
{
    // RUN TOWARDS TARGET STATE
    //
    // Intented behaviour: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    // Intended condition: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    //

    string animName;
    private Transform targetedEnemy;
    public bool hasTargetEnemy;
    private float movementSpeed = 8;
    private float rotationSpeed = 360;
    private Quaternion targetRotation;
    public Vector3 targetPoint;
    public Vector3 originalTargetPoint;
    private Vector3 direction;
    public float targetThreshold = 1.5f;
    private MeleeStateMachine meleeSM;
    public MeleeRunTowardsPlayerState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        meleeSM = sm as MeleeStateMachine;
        Debug.Log("Entering Run Towards Player State");
        targetedEnemy = meleeSM.targetedEnemy;
        sm.animator.CrossFade(animName, 0.1f);
        targetPoint = targetedEnemy.position;
        direction = (targetPoint - sm.transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
        originalTargetPoint = targetPoint;
    }

    public override void OnStateExit()
    {
        Debug.Log("Leaving Run Towards Player State");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating Run Towards Player State");
        if (targetedEnemy == null)
        {
            targetPoint = sm.transform.position;
            return;
        }
        if (Vector3.Distance(originalTargetPoint, targetedEnemy.position) < 5)
        {
            targetPoint = targetedEnemy.position;
            direction = (targetPoint - sm.transform.position).normalized;
            direction.y = 0;
            targetRotation = Quaternion.LookRotation(direction);
        }
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Quaternion.Angle(targetRotation, sm.transform.rotation) < targetThreshold)
        {
            sm.transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }

    public override void OnStateGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(targetPoint, Vector3.one * 0.2f);
    }
}
