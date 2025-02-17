using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEditor.Rendering;
using UnityEngine;

[BurstCompile]
public struct OverlapSphereJob : IJobParallelFor
{
    private static readonly ProfilerMarker marker = new ProfilerMarker(ProfilerCategory.Physics, "OverlapSphereJob.Execute");
    public NativeArray<OverlapSphereCommand> command;
    public NativeArray<Vector3> position;
    public NativeArray<float> distance;
    public QueryParameters queryParameters;
    public void Execute(int index)
    {
        command[index] = new OverlapSphereCommand(position[index], distance[index], queryParameters);
    }
}
