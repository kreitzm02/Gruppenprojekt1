using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToTargetState : BaseState // TODO
{
    string animName;
    private Transform targetedEnemy;
    public bool hasTargetEnemy;
    private float movementSpeed = 5;
    private float rotationSpeed = 360;
    private Quaternion targetRotation;
    public Vector3 targetPoint;
    private Vector3 direction;
    private float targetThreshold = 0.2f;
    public WalkToTargetState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered WalkToTarget State");
        sm.animator.CrossFade(animName, 0.1f);
        if (targetedEnemy == null)
        {
            hasTargetEnemy = false;
            Debug.Log("WalkToTarget State was called without a target");
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Left WalkToTarget State");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating WalkToTarget State");
        if (targetedEnemy == null)
        {
            targetPoint = sm.transform.position;
            return;
        }
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
