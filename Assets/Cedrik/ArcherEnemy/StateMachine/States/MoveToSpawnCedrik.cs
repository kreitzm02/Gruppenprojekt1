using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/MoveToSpawn")]
public class MoveToSpawnCedrik : BaseStateCedrik
{
    private bool initialized = false;
    private Rigidbody thisRigidbody;
    private GameObject thisGameObject;
    private Animator thisAnimator;
    private Vector3 spawnPoint;
    private Vector3 direction;
    public Vector3 targetWaypoint { get; private set; }

    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float rotationSpeed = 5.0f;

    [SerializeField]
    OnWaypointConditionCedrik onWaypointCondition;

    private void OnEnable()
    {
        ResetInitialize();
    }

    private void ResetInitialize()
    {
        initialized = false;
    }

    public override void Initialize(GameObject _thisObject)
    {
        if (!initialized)
        {
            thisGameObject = _thisObject;
            thisRigidbody = thisGameObject.GetComponent<Rigidbody>();
            thisAnimator = thisGameObject.GetComponent<Animator>();
            spawnPoint = thisGameObject.GetComponent<StateMachineRefsCedrik>().spawnPointRef;
            initialized = true;
        }
    }

    public override void OnStateEnter()
    {
        targetWaypoint = spawnPoint;
        onWaypointCondition.waypointLocation = targetWaypoint;
        direction = (targetWaypoint - thisGameObject.transform.position).normalized;
    }

    public override void OnStateUpdate()
    {
            thisRigidbody.MovePosition(thisGameObject.transform.position + direction * speed * Time.deltaTime);
            thisAnimator.Play("OutOfCombWalk");
            thisGameObject.transform.rotation = Quaternion.Slerp(thisGameObject.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }

    public override void OnStateExit()
    {
    }
}
