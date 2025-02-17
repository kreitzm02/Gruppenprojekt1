using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Physics;

public class EnemyDetectionManager : MonoBehaviour
{
    public static EnemyDetectionManager Instance;
    public List<GameObject> Enemies = new List<GameObject>();
    public GameObject[] colList;

    [SerializeField] int maxUnits = 250; // Maximale Anzahl an Gegnern 
    public static int MaxHits = 2;       // Maximale Treffer pro Gegner
    
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
        colList = new GameObject[MaxHits * maxUnits];
    }
    private void Start()
    {
        InvokeRepeating("CalculateOverlaps", 0.1f, 0.2f);
    }
    private void CalculateOverlaps()
    {
        int count = Enemies.Count;
        if (count == 0)
            return;

        NativeArray<Vector3> positions = new NativeArray<Vector3>(count, Allocator.TempJob);
        NativeArray<float> distance = new NativeArray<float>(count, Allocator.TempJob);
        NativeArray<OverlapSphereCommand> commands = new NativeArray<OverlapSphereCommand>(count, Allocator.TempJob);
        NativeArray<ColliderHit> hitColliders = new NativeArray<ColliderHit>(MaxHits * count, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            if (Enemies[i] == null) continue;
            positions[i] = Enemies[i].transform.position;
            MeleeSkeletonBehaviour_M behaviour = Enemies[i].GetComponent<MeleeSkeletonBehaviour_M>();
            if (behaviour != null)
            {
                distance[i] = behaviour.viewDistance;
            }
            else
            {
                distance[i] = 10f;
            }
        }

        QueryParameters queryParams = new QueryParameters
        {
            layerMask = 1 << 9
        };
        OverlapSphereJob job = new OverlapSphereJob
        {
            command = commands,
            position = positions,
            distance = distance,
            queryParameters = queryParams
        };

        JobHandle handle = job.Schedule(count, 1);
        handle = OverlapSphereCommand.ScheduleBatch(commands, hitColliders, 1, MaxHits, handle);
        handle.Complete();

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < MaxHits; j++)
            {
                int index = i * MaxHits + j;
                if (hitColliders[index].collider != null)
                    colList[index] = hitColliders[index].collider.gameObject;
                else
                    colList[index] = null;
            }
        }

        positions.Dispose();
        distance.Dispose();
        commands.Dispose();
        hitColliders.Dispose();
    }
    public void RegisterEnemy(GameObject enemy)
    {
        if (!Enemies.Contains(enemy))
        {
            Enemies.Add(enemy);
        }
    }
    public void UnregisterEnemy(GameObject enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
        }
    }
    public GameObject[] GetResultsForEnemy(int index)
    {
        GameObject[] results = new GameObject[MaxHits];
        int start = index * MaxHits;
        for (int i = 0; i < MaxHits; i++)
        {
            int arrIndex = start + i;
            if (arrIndex < colList.Length)
                results[i] = colList[arrIndex];
        }
        return results;
    }
}


