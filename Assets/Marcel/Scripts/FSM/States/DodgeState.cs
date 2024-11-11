using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DodgeState : BaseState // TODO
{
    private string animNameFwd;
    private string animNameBack;
    private string animNameLeft;
    private string animNameRight;   
    private float movementSpeed = 14f;
    private Vector3 mousePosition;
    public Vector3 originalPosition;
    private Vector3 direction;
    private Vector3 localDirection;
    private Ray mouseRay;
    public float targetDistance = 3f;

    public DodgeState(BaseStateMachine _sm, string _animFwd, string _animBack, string _animLeft, string _animRight) : base(_sm)
    {
        animNameBack = _animBack;
        animNameLeft = _animLeft;
        animNameRight = _animRight;
        animNameFwd = _animFwd;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered Dodge State");
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        sm.animator.CrossFade(animNameFwd, 0.1f);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            mousePosition = hitInfo.point;
            mousePosition.y = sm.transform.position.y;
        }
        direction = (mousePosition - sm.transform.position).normalized;
        // localDirection = sm.transform.InverseTransformDirection(direction);
        originalPosition = sm.transform.position;
        // if (localDirection.z > 0 && Mathf.Abs(localDirection.z) > Mathf.Abs(localDirection.x))
        // {
        //     Debug.Log("Fwd");
        //     sm.animator.CrossFade(animNameFwd, 0.1f);
        // }
        // else if (localDirection.z < 0 && Mathf.Abs(localDirection.z) > Mathf.Abs(localDirection.x))
        // {
        //     Debug.Log("bwd");
        //     sm.animator.CrossFade(animNameBack, 0.1f);
        // }
        // else if (localDirection.x > 0)
        // {
        //     Debug.Log("right");
        //     sm.animator.CrossFade(animNameRight, 0.1f);
        // }
        // else if (localDirection.x < 0)
        // {
        //     Debug.Log("left");
        //     sm.animator.CrossFade(animNameLeft, 0.1f);
        // }
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Dodge State");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating Dodge State");
        if (Vector3.Distance(sm.transform.position, originalPosition) < targetDistance)
        {
            sm.transform.position += direction * movementSpeed * Time.deltaTime;
        }
    }
}
