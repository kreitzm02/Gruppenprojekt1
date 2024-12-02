using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/Chase")]
public class ChaseCedrik : BaseStateCedrik
{
    private bool initialized = false;
    private Rigidbody thisRigidbody;
    private GameObject thisGameObject;
    private Animator thisAnimator;
    private GameObject detectedPlayer;

    private float detectionRadius;

    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    [Tooltip("Objects with the Layer that can be detected")]
    [SerializeField]
    LayerMask layerToDetect;

    [SerializeField]
    private InRangeConditionCedrik rangeCondition;



    public override void Initialize(GameObject _thisObject)
    {
        if (!initialized)
        {
            thisGameObject = _thisObject;
            thisRigidbody = thisGameObject.GetComponent<Rigidbody>();
            thisAnimator = thisGameObject.GetComponent<Animator>();
            initialized = true;
        }
        detectionRadius = rangeCondition.GetDetectionRadius();
    }

    public override void OnStateEnter()
    {
        Debug.Log("ChaseEnter");
        foreach (Collider _coll in Physics.OverlapSphere(thisGameObject.transform.position, detectionRadius, layerToDetect))
        {
            if (_coll.CompareTag("Player"))
            {
                detectedPlayer = _coll.gameObject;
                break;
            }
        }
    }

    public override void OnStateUpdate()
    {
        Debug.Log("ChaseUpdate");
        Vector3 direction = (detectedPlayer.transform.position - thisGameObject.transform.position).normalized;
        thisRigidbody.MovePosition(thisGameObject.transform.position + direction * speed * Time.deltaTime);
        thisAnimator.Play("RunningInCombat");
        thisGameObject.transform.rotation = Quaternion.Slerp(thisGameObject.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }

    public override void OnStateExit()
    {
        Debug.Log("ChaseExit");
        detectedPlayer = null;
    }
}
