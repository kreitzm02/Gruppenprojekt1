using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MeleeSkeletonBehaviour_M : MonoBehaviour, IDamageable, IKillable, ITargetDetectable, IAttackable
{
    public int healthPoints = 100;
    public int maxHealthPoints = 100;
    public int attackDamage = 15;
    [HideInInspector] public Transform targetedEnemy;
    [HideInInspector] public Transform attackedEnemy;
    public float viewDistance = 15;
    public float attackRange = 1;
    public float nearRange = 8;
    public float viewConeAngle = 180;
    public string targetName = "Player";
    private HealthBar healthBar;
    private int layerMaskExcludeOwn = ~(1 << 7);
    public bool IsInsideSmoke = false;
    private int smokeDuration = 7;
    public bool smokeRegistrated = false;
    public float deathPosY = -10;
    public bool registeredDeath = false;

    private OperationType operationType = OperationType.Single;

    EnemyDetectionManager enemyDetectionManager;

    private int myIndex = -1;

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        enemyDetectionManager = EnemyDetectionManager.Instance;
        if (enemyDetectionManager != null)
        {
            enemyDetectionManager.RegisterEnemy(gameObject);
            myIndex = enemyDetectionManager.Enemies.IndexOf(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (enemyDetectionManager != null && operationType == OperationType.Multi)
            enemyDetectionManager.UnregisterEnemy(gameObject);
    }

    private void Update()
    {
        if (healthPoints == 0 && registeredDeath == false)
        {
            DungeonLevelManager.Instance.registeredKilledEnemy = true;
            registeredDeath = true;
        }
        operationType = EnemyDetectionToggleForMT.Instance.operationType;
        if (healthBar != null)
            healthBar.UpdateHealthBar(healthPoints, maxHealthPoints);
        if (IsInsideSmoke && smokeRegistrated == false)
        {
            StartCoroutine("Smoke");
            smokeRegistrated=true;
        }
    }
    private System.Collections.IEnumerator Smoke()
    {
        yield return new WaitForSeconds(smokeDuration);
        IsInsideSmoke = false;
    }
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
        if (operationType == OperationType.Multi)
        {
            if (enemyDetectionManager == null || myIndex < 0)
                return null;

            GameObject[] detectedObjects = enemyDetectionManager.GetResultsForEnemy(myIndex);
            foreach (GameObject obj in detectedObjects)
            {
                if (obj == null) continue;
                Collider col = obj.GetComponent<Collider>();
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
                RaycastHit[] rayCastHits = Physics.RaycastAll(transform.position, directionToTarget, Vector3.Distance(transform.position, target.position), layerMaskExcludeOwn);
                if (rayCastHits.Length > 1)
                {
                    continue;
                }
                Debug.Log("Target is in line of sight!");
                return target;
            }
            return null;
        }
        else if (operationType == OperationType.Single)
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
                RaycastHit[] rayCastHits = Physics.RaycastAll(transform.position, directionToTarget, Vector3.Distance(transform.position, target.position), layerMaskExcludeOwn);
                if (rayCastHits.Length > 1)
                {
                    continue;
                }
                Debug.Log("Target is in line of sight!");
                return target;
            }
            return null;
        }
        else return null;
    }
    public Transform DetectTargetNearRange()
    {
        
        Collider[] nearCollider = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in nearCollider)
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
