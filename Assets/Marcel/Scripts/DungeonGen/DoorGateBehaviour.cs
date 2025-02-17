using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorGateBehaviour : MonoBehaviour
{
    public bool gateIsActivated = false;
    [SerializeField] private float gateSpeed = 1f;
    private Vector3 targetPosition;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = transform.position + Vector3.up * 3.75f;
        RegisterToManager();
    }

    private void Update()
    {
        if (gateIsActivated && transform.position.y < targetPosition.y)
        {
            CloseGate();
        }
        else if (!gateIsActivated && transform.position.y > initialPosition.y)
        {
            OpenGate();
        }
    }

    private void RegisterToManager()
    {
        DungeonLevelManager.Instance.doorGateBehaviours.Add(this);
    }

    private void CloseGate()
    {
        transform.position += Vector3.up * gateSpeed * Time.deltaTime;
    }

    private void OpenGate()
    {
        transform.position -= Vector3.up * gateSpeed * Time.deltaTime;
    }
}
