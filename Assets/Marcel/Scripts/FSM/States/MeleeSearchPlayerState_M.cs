using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSearchPlayerState_M : BaseState_M
{
    // SEARCH TARGET STATE
    //
    // Intented behaviour: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    // Intended condition: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor
    //                     invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.
    //

    private string animName;
    private float rotationSpeed = 100f;
    private float rotationThreshold = 0.2f;
    private float timeBetweenRotations = 3;
    private float elapsedTime;
    private float watchoutRotationDegree = 30;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    public int rotationCounter;
    public MeleeSearchPlayerState_M(BaseStateMachine_M _sm, string _animName) : base(_sm)
    {
        animName = _animName;
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entering Search Player State");
        sm.animator.CrossFade(animName, 0.1f);
        originalRotation = sm.transform.rotation;
        rotationCounter = 0;
        elapsedTime = 0;
    }

    public override void OnStateExit()
    {
        Debug.Log("Left Search Player State");
    }
    public override void OnStateUpdate()
    {
        Debug.Log("Updating Search Player State");
        targetRotation = originalRotation;
        if (rotationCounter == 0)
        {
            targetRotation = Quaternion.Euler(0f, originalRotation.eulerAngles.y + watchoutRotationDegree, 0f);
        }
        else if (rotationCounter == 1)
        {
            targetRotation = Quaternion.Euler(0f, originalRotation.eulerAngles.y - watchoutRotationDegree, 0f);
        }
        else
        {
            targetRotation = originalRotation;
        }
        sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (Quaternion.Angle(sm.transform.rotation, targetRotation) < rotationThreshold)
        {
            elapsedTime += Time.deltaTime;
        }
        if (elapsedTime > timeBetweenRotations)
        {
            rotationCounter++;
            elapsedTime = 0;
        }
    }
}
