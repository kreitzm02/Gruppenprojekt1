using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerIdleState_M : BaseState_M
{
    string animName;
    private Ray mouseRay;
    private Vector3 direction;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private float rotationSpeed = 1000f;
    public PlayerIdleState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerIdleState_M");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerIdleState_M");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerIdleState_M");
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            targetPosition = hitInfo.point;
            targetPosition.y = sm.transform.position.y;
        }
        direction = (targetPosition - sm.transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
