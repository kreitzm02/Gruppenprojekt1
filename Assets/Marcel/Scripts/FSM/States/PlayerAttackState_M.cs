using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using TMPro;
using UnityEngine;

public class PlayerAttackState_M : BaseState_M
{
    string animName;
    private Vector3 weaponHitBox;
    private Vector3 targetPosition;
    private Ray mouseRay;
    private Vector3 direction;
    private Quaternion targetRotation;
    private float rotationSpeed = 1000f;
    private Transform targetedEnemy;
    private IDamageable targetDamageable;
    private IKillable targetKillable;
    private AnimatorStateInfo animStateInfo;
    private PlayerAttackStateMachine_M playerSM;
    public bool attackFinished = false;
    private bool damageGiven = false;
    private BaseState_M currentMovementState;
    public PlayerAttackState_M(BaseStateMachine_M _sm) : base(_sm)
    {
    }

    public override void OnStateEnter()
    {
        Debug.Log("Entered PlayerAttackState_M");
        playerSM = sm as PlayerAttackStateMachine_M;
        animName = playerSM.playerBehaviour.GetAttackAnimation();
        sm.animator.CrossFade(animName, 0f);
        attackFinished = false;
        damageGiven = false;
    }

    public override void OnStateExit()
    {
        Debug.Log("Left PlayerAttackState_M");
        currentMovementState = playerSM.playerStateMachine.GetCurrentState();
        playerSM.playerStateMachine.OverrideCurrentState(currentMovementState);
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Updating PlayerAttackState_M");
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
        weaponHitBox = new Vector3(sm.transform.position.x, sm.transform.position.y + 1f, sm.transform.position.z) + sm.transform.forward * 1.1f;
        animStateInfo = sm.animator.GetCurrentAnimatorStateInfo(0);
        float normalizedAnimTime = animStateInfo.normalizedTime % 1.0f;
        playerSM.testing_NormalizedAttackAnimTime = normalizedAnimTime;
        if (normalizedAnimTime >= 0.7f)
        {
            attackFinished = true;
            damageGiven = false;
        }
        else
        {
            attackFinished = false;
        }
        if (normalizedAnimTime >= 0.38f && normalizedAnimTime <= 0.46f && !attackFinished && !damageGiven)
        {
            Collider[] collider = Physics.OverlapSphere(weaponHitBox, 0.4f);
            foreach (var col in collider)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    targetDamageable = col.gameObject.GetComponent<IDamageable>();
                    if (targetDamageable == null)
                        continue;
                    targetDamageable.GainDamage(playerSM.attackable.GetAttackDamage());
                    Rigidbody targetRb = col.gameObject.GetComponent<Rigidbody>();
                    targetRb.AddForce(direction * 7.0f, ForceMode.Impulse);
                    damageGiven = true;
                }
            }
        }
    }

    public override void OnStateGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(weaponHitBox, 0.5f);
    }
}
