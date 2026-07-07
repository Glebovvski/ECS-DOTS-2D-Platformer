using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public struct PlayerGroundComponentData : IComponentData
{
    public float3 OverlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter; 
    public CollisionFilter GroundCollisionFilter;
}
