using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct PlayerGroundSystem : ISystem, ISystemStartStop
{
    private float3 overlapDetectionOffset;

    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerGroundComponentData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        new PlayerGroundJob()
        {
            CollisionWorld = collisionWorld,
            OverlapDetectionOffset = overlapDetectionOffset
        }.Schedule();
    }

    [BurstCompile]
    private void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        Entity entity = SystemAPI.GetSingletonEntity<PlayerGroundComponentData>();
        overlapDetectionOffset = SystemAPI.GetComponentRO<PlayerGroundComponentData>(entity).ValueRO.OverlapDetectionOffset;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct PlayerGroundJob : IJobEntity
{
    [ReadOnly] public CollisionWorld CollisionWorld;
    [ReadOnly] public float3 OverlapDetectionOffset;

    [BurstCompile]
    public unsafe void Execute(ref PlayerMovementComponentData movementData, in PhysicsCollider physicsCollider, in LocalTransform playerTransform)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);

        var boxCollider = (Unity.Physics.BoxCollider*)physicsCollider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;

        bool isGrounded = CollisionWorld.OverlapBox(playerTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, boxCollider->GetCollisionFilter());
        movementData.IsGrounded = isGrounded;

        foreach (var hit in hits)
        {
            if (hit.Entity != Entity.Null)
            {
                Debug.DrawLine(playerTransform.Position, hit.Position, Color.red, 10f);
            }
        }

        hits.Dispose();
    }
}
