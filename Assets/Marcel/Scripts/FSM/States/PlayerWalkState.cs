using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : BaseState
{
    private string animName;
    private Ray mouseRay;
    private float movementSpeed = 5;
    private float rotationSpeed = 1000;
    private float targetThreshold = 0.1f;
    private Vector3 targetPosition;
    private Vector3 direction;
    private Quaternion targetRotation;
    private bool hasTarget = false;
    public PlayerWalkState(BaseStateMachine _sm, string _animName) : base(_sm)
    {
        animName = _animName;
     }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerWalkState");
        sm.animator.CrossFade(animName, 0.1f);
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerWalkState");
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerWalkState");
        // Mausposition prüfen und Zielposition festlegen
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                targetPosition = hitInfo.point;
                targetPosition.y = sm.transform.position.y;  // Höhe des Spielers beibehalten
                hasTarget = true;  // Ziel ist nun gesetzt
            }
        }

        if (hasTarget)
        {
            Vector3 direction = (targetPosition - sm.transform.position).normalized;
            targetRotation = Quaternion.LookRotation(direction);

            // Drehe Spieler in Richtung des Ziels
            sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Bewegung zum Ziel
            if (Vector3.Distance(sm.transform.position, targetPosition) > targetThreshold)
            {
                sm.transform.position += direction * movementSpeed * Time.deltaTime;
            }
            else
            {
                hasTarget = false; // Ziel erreicht, Bewegung stoppen
            }
        }
    }
}
