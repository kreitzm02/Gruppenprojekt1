using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDetectRadiusCedrik : MonoBehaviour
{
    [SerializeField]
    private float detectionRadius = 12.0f;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
