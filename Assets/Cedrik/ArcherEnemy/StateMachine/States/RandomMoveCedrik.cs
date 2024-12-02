using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/ArcherStates/RandomMove")]
public class RandomMoveCedrik : BaseStateCedrik
{
    [SerializeField]
    private bool initialized = false;
    private Rigidbody thisRigidbody;
    private GameObject thisGameObject;
    private Animator thisAnimator;
    private Vector3 spawnPoint;
    private Vector3 randomDirection;
    public Vector3 targetWaypoint { get; private set; }

    [SerializeField]
    private float minMoveRange = 2.0f;
    [SerializeField]
    private float maxMoveRange = 5.0f;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    [SerializeField]
    private float maxDistanceToSpawn = 15f;

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
        randomDirection = GenerateRandomDirection();
        targetWaypoint = thisGameObject.transform.position + (randomDirection * Random.Range(minMoveRange, maxMoveRange));
        onWaypointCondition.waypointLocation = targetWaypoint;
        Debug.Log("random move enter");
    }

    public override void OnStateUpdate()
    {
        if (InAllowedRange())
        {
            thisRigidbody.MovePosition(thisGameObject.transform.position + randomDirection * speed * Time.deltaTime);
            thisAnimator.Play("OutOfCombWalk");
            thisGameObject.transform.rotation = Quaternion.Slerp(thisGameObject.transform.rotation, Quaternion.LookRotation(randomDirection), rotationSpeed * Time.deltaTime);
        }
        else
        {
            randomDirection = GenerateRandomDirection();
            targetWaypoint = thisGameObject.transform.position + (randomDirection * Random.Range(minMoveRange, maxMoveRange));
            Debug.Log(targetWaypoint);
            onWaypointCondition.waypointLocation = targetWaypoint;
        }
    }

    public override void OnStateExit()
    {
    }

    
    private bool InAllowedRange()
    {
        return Vector3.Distance(targetWaypoint,spawnPoint) < maxDistanceToSpawn;
    }


    private Vector3 GenerateRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
}
