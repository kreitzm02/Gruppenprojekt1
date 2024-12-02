using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/GainDistance")]
public class GainDistanceCedrik : BaseStateCedrik
{
    private bool initialized = false;
    private Rigidbody thisRigidbody;
    private GameObject thisGameObject;
    private Animator thisAnimator;
    private GameObject detectedPlayer;

    private float inMeleeRange;

    [SerializeField]
    private float speed = 6.0f;
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
        inMeleeRange = rangeCondition.GetDetectionRadius();
    }

    public override void OnStateEnter()
    {
        Debug.Log("GainDistanceEnter");
        foreach (Collider _coll in Physics.OverlapSphere(thisGameObject.transform.position, inMeleeRange, layerToDetect))
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
        Debug.Log("GainDistanceUpdate");
        Vector3 direction = (detectedPlayer.transform.position - thisGameObject.transform.position).normalized * -1;
        thisRigidbody.MovePosition(thisGameObject.transform.position + direction * speed * Time.deltaTime);
        thisAnimator.Play("RunningInCombat");
        thisGameObject.transform.rotation = Quaternion.Slerp(thisGameObject.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }

    public override void OnStateExit()
    {
        Debug.Log("GainDistanceExit");
        detectedPlayer = null;
    }
}
