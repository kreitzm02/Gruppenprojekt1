using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThrowingState_M : BaseState_M
{
    string animName;
    GameObject fakeThrowObj;
    GameObject realThrowObj;
    Transform slot;
    Transform slotTransform;
    GameObject newObject;
    GameObject crosshairObject;
    PlayerAttackStateMachine_M playerSM;
    private AnimatorStateInfo animStateInfo;
    public bool canBeCancelled;
    private bool aimingIsComplete;
    private bool activateThrowingIsComplete;
    private bool removingCrosshairIsComplete;
    private Vector3 direction;
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    private float rotationSpeed = 1000;
    private Vector3 targetThrowPosition;
    public bool stateCompleted;
    private BaseState_M currentMovementState;
    public ThrowingState_M(BaseStateMachine_M _sm, string _animName, GameObject _fakethrowObj, GameObject _realthrowobj, Transform _slot) : base(_sm)
    {
        animName = _animName;
        fakeThrowObj = _fakethrowObj;
        realThrowObj = _realthrowobj;
        slot = _slot;
    }

    public override void OnStateEnter()
    {
        sm.animator.CrossFade(animName, 0.0f);
        playerSM = sm as PlayerAttackStateMachine_M;
        try
        {
            slotTransform = sm.transform.Find("Rig/root/hips/spine/chest/upperarm.r/lowerarm.r/wrist.r/hand.r/handslot.r/").GetChild(0);
            slotTransform.gameObject.SetActive(false);
        }
        catch
        {

        }
        newObject = Object.Instantiate(fakeThrowObj, new Vector3(slot.position.x, slot.position.y - 0.1f, slot.position.z), slot.rotation, slot);
        aimingIsComplete = false;
        activateThrowingIsComplete = false;
        removingCrosshairIsComplete = false;
        stateCompleted = false;
        canBeCancelled = true;
    }

    public override void OnStateExit()
    {

        Object.Destroy(newObject);
        if (slotTransform != null)
            slotTransform.gameObject.SetActive(true);
        currentMovementState = playerSM.playerStateMachine.GetCurrentState();
        playerSM.playerStateMachine.OverrideCurrentState(currentMovementState);
        sm.animator.speed = 1;
        CrosshairManager.Instance.crosshairIsActive = false;
    }

    public override void OnStateUpdate()
    {
        animStateInfo = sm.animator.GetCurrentAnimatorStateInfo(0);
        //float normalizedAnimTime = animStateInfo.normalizedTime % 1.0f;
        if (!aimingIsComplete)
        {
            CrosshairManager.Instance.crosshairIsActive = true;
            targetPoint = CrosshairManager.Instance.crosshairTransform.position;
            direction = (targetPoint - sm.transform.position).normalized;
            direction.y = 0;
            targetRotation = Quaternion.LookRotation(direction);
            sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        if (animStateInfo.normalizedTime > 0.4f && aimingIsComplete == false)
        {
            sm.animator.speed = 0;
           //CrosshairManager.Instance.crosshairIsActive = true;
           //targetPoint = CrosshairManager.Instance.crosshairTransform.position;
           //direction = (targetPoint - sm.transform.position).normalized;
           //direction.y = 0;
           //targetRotation = Quaternion.LookRotation(direction);
           //sm.transform.rotation = Quaternion.RotateTowards(sm.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            canBeCancelled = false;
            if (Input.GetKeyUp(KeyCode.Q))
            {
                aimingIsComplete = true;
            }
        }
        if (aimingIsComplete && !removingCrosshairIsComplete)
        {
            sm.animator.speed = 1;
            targetThrowPosition = CrosshairManager.Instance.crosshairTransform.position;
            CrosshairManager.Instance.crosshairIsActive = false;
            removingCrosshairIsComplete = true;
        }
        if (animStateInfo.normalizedTime > 0.53f && aimingIsComplete && !activateThrowingIsComplete)
        {
            Vector3 originPosition = newObject.transform.position;
            Object.Destroy(newObject);
            ThrowablesManager.Instance.targetPosition = targetThrowPosition;
            ThrowablesManager.Instance.originPosition = originPosition;
            ThrowablesManager.Instance.throwablePrefab = realThrowObj;
            ThrowablesManager.Instance.throwingActive = true;
            activateThrowingIsComplete = true;
        }
        if (animStateInfo.normalizedTime > 0.9f)
        {
            stateCompleted = true;
        }
    }
}
