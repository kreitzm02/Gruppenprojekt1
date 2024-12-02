using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWalkState_M : BaseState_M
{
    // WALK STATE
    //
    // Intented behaviour: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    // Intended condition: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    //

    private string animName;
    private bool hasTarget = false;
    private float movementRadius = 5;
    public float targetThreshold = 0.2f;
    private float movementSpeed = 2;
    private float rotationSpeed = 1000;
    private Quaternion targetRotation;
    private Vector2 randomPoint;
    public Vector3 targetPoint;
    private Vector3 direction;
    private Vector3 startPosition;
    public MeleeWalkState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.1f);
        startPosition = sm.transform.position;
        Debug.Log("Entered Walk State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Walk State");
        hasTarget = false;
    }
    public override void OnStateUpdate()
    {
        Debug.Log("Updating Walk State");
        if (!hasTarget)
        {
            randomPoint = Random.insideUnitCircle * movementRadius;
            targetPoint = startPosition + new Vector3(randomPoint.x, 0, randomPoint.y);
            direction = (targetPoint - sm.transform.position).normalized;
            targetRotation = Quaternion.LookRotation(direction);
            hasTarget = !Physics.Raycast(sm.transform.position, direction, Vector3.Distance(sm.transform.position, targetPoint)) && Physics.Raycast(targetPoint, Vector3.down);
            Debug.Log("Target found");
        }
        else
        {
            sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(targetRotation, sm.transform.rotation) < targetThreshold)
            {
                sm.transform.position += direction * movementSpeed * Time.deltaTime;
            }
        }
    }

    public override void OnStateGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, movementRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(targetPoint, Vector3.one * 0.2f);
    }

    public override void OnStateCollisionEnter(Collision _col)
    {
        // the target position changes to the current object position, if the object is stuck because a collider is in the way.
        targetPoint = sm.transform.position;
    }
}
