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
    private CollisionFilter deadZoneCollisionFilter;
    private CollisionFilter groundCollisionFilter;

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
            OverlapDetectionOffset = overlapDetectionOffset,
            DeadZoneCollisionFilter = deadZoneCollisionFilter,
            GroundCollisionFilter = groundCollisionFilter
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
        var playerGroundData = SystemAPI.GetComponentRO<PlayerGroundComponentData>(entity).ValueRO;
        overlapDetectionOffset = playerGroundData.OverlapDetectionOffset;
        deadZoneCollisionFilter = playerGroundData.DeadZoneCollisionFilter;
        groundCollisionFilter = playerGroundData.GroundCollisionFilter;
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

    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter GroundCollisionFilter;

    [BurstCompile]
    public unsafe void Execute(ref PlayerMovementComponentData movementData, ref PlayerComponentData playerComponentData, in PhysicsCollider physicsCollider, in LocalTransform playerTransform)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);

        var boxCollider = (Unity.Physics.BoxCollider*)physicsCollider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;

        bool isDead = OverlapBox(playerTransform, boxGeometry, ref hits, DeadZoneCollisionFilter);
        playerComponentData.IsDead = isDead;
        if(isDead)
        {
            Debug.LogError("DEAD");
            hits.Dispose();
            return;
        }
        bool isGrounded = OverlapBox(playerTransform, boxGeometry, ref hits, GroundCollisionFilter);
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

    [BurstCompile]
    public bool OverlapBox(LocalTransform playerTransform, BoxGeometry boxGeometry, ref NativeList<DistanceHit> hits, CollisionFilter collisionFilter)
    {
        return CollisionWorld.OverlapBox(playerTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, collisionFilter);
    }
}
