using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkeletonBehaviour : MonoBehaviour, IDamageable, IKillable, ITargetDetectable, IAttackable
{
    public int healthPoints = 100;
    public int attackDamage = 15;
    public Transform targetedEnemy;
    public Transform attackedEnemy;
    public float viewDistance = 10;
    public float attackRange = 1;
    public float viewConeAngle = 80;

    public bool CheckDeathCondition()
    {
        if (healthPoints == 0)
            return true;
        else return false;
    }
    public void GainDamage(int _damage)
    {
        this.healthPoints -= _damage;
        if (this.healthPoints < 0)
            healthPoints = 0;
    }
    public Transform DetectTargetVisibleRange()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, viewDistance);
        foreach (Collider col in collider)
        {
            if (!col.CompareTag("Player") || col == this.GetComponent<Collider>())
            {
                continue;
            }
            Transform target = col.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if (angleToTarget > viewConeAngle / 2)
            {
                continue;
            }
            Debug.Log("Target is in view cone");
            RaycastHit[] rayCastHits = Physics.RaycastAll(transform.position, directionToTarget, Vector3.Distance(transform.position, target.position));
            if (rayCastHits.Length > 1)
            {
                continue;
            }
            Debug.Log("Target is in line of sight!");
            return target;
        }
        collider = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in collider)
        {
            if (!col.CompareTag("Player") || col == this.GetComponent<Collider>())
            {
                continue;
            }
            targetedEnemy = col.transform;
            return col.transform;
        }
        return null;
    }

    public Transform DetectTargetAttackRange()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in collider)
        {
            if (!col.CompareTag("Player") || col == this.GetComponent<Collider>())
            {
                continue;
            }
            return col.transform;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewConeAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewConeAngle / 2, 0) * transform.forward;
        Vector3 headPosition = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);

        //View Distance
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(headPosition, headPosition + leftBoundary * viewDistance);
        Gizmos.DrawLine(headPosition, headPosition + rightBoundary * viewDistance);

        //Attack Range 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}
