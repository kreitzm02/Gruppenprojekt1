using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/Attack")]
public class RangedAttackCedrik : BaseStateCedrik
{
    private bool initialized = false;
    private GameObject thisGameObject;
    private Rigidbody thisRigidbody;
    private Animator thisAnimator;
    private GameObject detectedPlayer;

    private float attackRange;

    [SerializeField]
    private float rotationSpeed = 5.0f;
    [Tooltip("Objects with the Layer that can be detected")]
    [SerializeField]
    LayerMask layerToDetect;

    [SerializeField]
    private InRangeConditionCedrik rangeCondition;

    private float crossFadeDuration = 0.2f;
    private float ellapsedTime = 0.0f;

    public override void Initialize(GameObject _thisObject)
    {
        if (!initialized)
        {
            thisGameObject = _thisObject;
            thisAnimator = thisGameObject.GetComponent<Animator>();
            initialized = true;
        }
        attackRange = rangeCondition.GetDetectionRadius();
    }

    public override void OnStateEnter()
    {
        foreach (Collider _coll in Physics.OverlapSphere(thisGameObject.transform.position, attackRange, layerToDetect))
        {
            if (_coll.CompareTag("Player"))
            {
                detectedPlayer = _coll.gameObject;
                break;
            }
        }
        //thisAnimator.Play("RangedAiming");
        Debug.Log("AttackEnter");
        ellapsedTime = 0.0f;
    }

    public override void OnStateUpdate()
    {
        Debug.Log("AttackUpdate");
        thisAnimator.CrossFade("RangedShoot", crossFadeDuration);
        if(ellapsedTime >= crossFadeDuration)
        {
            thisAnimator.Play("RangedShoot");
        }
        //if (IsAnimationFinished("RangedAiming"))
        //{
        //    Debug.Log("AttackUpdateInIf");
        //    thisAnimator.Play("RangedShoot");
        //}
        ellapsedTime += Time.deltaTime;
        Vector3 direction = (detectedPlayer.transform.position - thisGameObject.transform.position).normalized;
        thisGameObject.transform.rotation = Quaternion.Slerp(thisGameObject.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }

    public override void OnStateExit()
    {
        Debug.Log("AttackExit");
        detectedPlayer = null;
    }

    private bool IsAnimationFinished(string _animationName)
    {
        AnimatorStateInfo info = thisAnimator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(_animationName) && info.normalizedTime >= 1.0f;
    }
}
