using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRetreatState_M : BaseState_M
{
    // RETREAT STATE
    //
    // Intented behaviour: Ensures that the object moves backwards until a certain distance ( moveDistance ) to the  
    //                     target has been reached. The object is continuously rotated towards the target object.
    // Intended condition: The intended condition for exiting this state is when the position of the object is
    //                     approx the same as the position of the targetPoint, with targetThreshold as a buffer.
    //

    public Transform targetedEnemy;
    private float movementSpeed = 5;
    private float rotationSpeed = 360;
    public float moveDistance = 10;
    private float targetThreshold = 0.75f;
    private Vector3 direction;
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    private string animName;
    private MeleeStateMachine_M meleeSM;
    public MeleeRetreatState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        meleeSM = sm as MeleeStateMachine_M;  
        targetedEnemy = meleeSM.targetedEnemy;
        sm.animator.CrossFade(animName, 0.1f);
        Debug.Log("Entering Retreat State");
    }

    public override void OnStateExit()
    {
        Debug.Log("Leaving Retreat State");
    }

    public override void OnStateUpdate()
    {
        if (targetedEnemy == null)
        {
            targetPoint = sm.transform.position;
            return;
        }
        meleeSM.lastSeenTargetPos = targetPoint;
        targetPoint = targetedEnemy.position;
        direction = (targetPoint - sm.transform.position).normalized;
        Debug.Log("Updating Retreat State");
        direction.y = 0f;
        targetRotation = Quaternion.LookRotation(direction);
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Quaternion.Angle(targetRotation, sm.transform.rotation) < targetThreshold)
        {
            sm.transform.position -= direction * movementSpeed * Time.deltaTime;
        }
    }
}
