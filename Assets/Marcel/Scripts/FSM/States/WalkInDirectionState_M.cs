using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkInDirectionState_M : BaseState_M
{
    private string animName;
    private Ray mouseRay;
    private float movementSpeed = 5;
    private float rotationSpeed = 1000;
    private float targetThreshold = 0.1f;
    private Vector3 targetPosition;
    private Vector3 direction;
    private Quaternion targetRotation;
    
    public WalkInDirectionState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered WalkInDirection State");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left WalkInDirection State");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating WalkInDirection State");
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            targetPosition = hitInfo.point;
            targetPosition.y = sm.transform.position.y;
        }

        direction = (targetPosition - sm.transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Vector3.Distance(sm.transform.position, targetPosition) > targetThreshold)
        {
            sm.transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }
}
