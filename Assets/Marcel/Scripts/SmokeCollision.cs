using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MeleeSkeletonBehaviour_M enemy = other.GetComponent<MeleeSkeletonBehaviour_M>();
            if (enemy != null)
            {
                enemy.IsInsideSmoke = true;
                enemy.smokeRegistrated = false;
            }
        }
    }
}
