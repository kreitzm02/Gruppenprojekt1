using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeLastSeenPosState : BaseState
{
    // REACH LAST KNOWN POS STATE
    //
    // Intented behaviour: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    // Intended condition: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    //

    string animName;
    public float targetThreshold = 1.5f;
    private float rotationThreshold = 0.2f;
    private float movementSpeed = 5;
    private float rotationSpeed = 1000;
    private Quaternion targetRotation;
    public Vector3 targetPoint;
    private Vector3 direction;
    private MeleeStateMachine meleeSM;
    public MeleeLastSeenPosState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        meleeSM = sm as MeleeStateMachine;
        Debug.Log("Entering Last Seen Pos State");
        sm.animator.CrossFade(animName, 0.1f);
        targetPoint = meleeSM.lastSeenTargetPos;
        direction = (targetPoint - sm.transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Last Seen Pos State");
    }
    public override void OnStateUpdate()
    {
        Debug.Log("Updating Last Seen Pos State");
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Quaternion.Angle(targetRotation, sm.transform.rotation) < rotationThreshold)
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
