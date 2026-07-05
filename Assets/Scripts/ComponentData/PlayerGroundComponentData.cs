using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerGroundComponentData : IComponentData
{
    public float3 OverlapDetectionOffset;
}
