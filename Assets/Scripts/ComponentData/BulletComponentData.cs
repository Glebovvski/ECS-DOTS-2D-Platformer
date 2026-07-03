using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BulletComponentData : IComponentData
{
    public float3 Direction;
    public float3 Rotation;
    public float Speed;
}
