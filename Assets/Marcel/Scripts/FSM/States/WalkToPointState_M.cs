using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToPointState_M : BaseState_M
{
    private string animName;
    private Ray mouseRay;
    private float movementSpeed = 5;
    private float rotationSpeed = 1000;
    public float targetThreshold = 0.1f;
    public Vector3 targetPosition;
    private Vector3 direction;
    private Quaternion targetRotation;

    public WalkToPointState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered WalkToPoint State");
        sm.animator.CrossFade(animName, 0.1f);
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            targetPosition = hitInfo.point;
            targetPosition.y = sm.transform.position.y;
        }
    }

    public override void OnStateExit()
    {
        Debug.Log("Left WalkToPoint State");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating WalkToPoint State");
        direction = (targetPosition - sm.transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Vector3.Distance(sm.transform.position, targetPosition) > targetThreshold)
        {
            sm.transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }

    public override void OnStateGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(targetPosition, Vector3.one * 0.2f);
    }
}
