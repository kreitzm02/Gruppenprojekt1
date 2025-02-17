using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionToggleForMT : MonoBehaviour
{
    public static EnemyDetectionToggleForMT Instance { get; private set; }

    public OperationType operationType = OperationType.Single;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public enum OperationType
{
    Off, Single, Multi
}
