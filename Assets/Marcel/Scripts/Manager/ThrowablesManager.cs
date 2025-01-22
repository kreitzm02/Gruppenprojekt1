using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowablesManager : MonoBehaviour
{
    public static ThrowablesManager Instance { get; private set; }
    public Vector3 targetPosition;
    public Vector3 originPosition;
    public GameObject throwablePrefab;
    public float throwForce = 1;
    public float upwardForce = 1;
    public bool throwingActive = false;
    public float throwSpeed = 15f;
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
    
    void Update()
    {
        if (throwingActive)
        {
            ThrowObject();
            throwingActive = false;
        }
    }

    private void ThrowObject()
    {
        // 1) Instantiate your grenade/bomb
        GameObject obj = Instantiate(throwablePrefab, originPosition, Quaternion.identity);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (!rb) return;

        // 2) Compute ballistic velocity so it lands EXACTLY on target, ignoring drag
        Vector3 velocity = ComputeBallisticVelocity(originPosition, targetPosition, throwSpeed);
        if (velocity == Vector3.zero)
        {
            Debug.LogWarning("No ballistic solution found: target too far or too high for throwSpeed=" + throwSpeed);
            // We might do something else, like fallback to max range or a higher arc
            return;
        }

        // 3) Apply that velocity
        rb.velocity = velocity;
    }

    /// <summary>
    /// Low-arc ballistic formula: 
    /// returns the velocity that hits `end` from `start` with speed `throwSpeed`.
    /// If no solution, returns Vector3.zero.
    /// </summary>
    private Vector3 ComputeBallisticVelocity(Vector3 start, Vector3 end, float speed, float gravity = 9.81f)
    {
        Vector3 diffXZ = new Vector3(end.x - start.x, 0f, end.z - start.z);
        float dXZ = diffXZ.magnitude;
        if (dXZ < 0.01f)
            return Vector3.zero;

        float dy = end.y - start.y;
        float vSqr = speed * speed;
        float v4 = vSqr * vSqr;

        float discriminant = v4 - gravity * (gravity * dXZ * dXZ + 2f * dy * vSqr);
        if (discriminant < 0f)
            return Vector3.zero;

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float angle = Mathf.Atan((vSqr - sqrtDisc) / (gravity * dXZ)); // low arc

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        // Horizontal direction
        Vector3 dirXZ = diffXZ.normalized;
        // Build final velocity
        Vector3 result = dirXZ * (speed * cos);
        result.y = speed * sin;
        return result;
    }
}
