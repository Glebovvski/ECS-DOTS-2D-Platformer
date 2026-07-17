using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct PlayerDetectionSystem : ISystem, ISystemStartStop
{
    private float3 overlapDetectionOffset;
    private CollisionFilter deadZoneCollisionFilter;
    private CollisionFilter endFlagCollisionFilter;
    private CollisionFilter collectiblesCollisionFilter;
    private CollisionFilter groundCollisionFilter;

    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerDetectionComponentData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<PlayerComponentData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        var playerComponentData = SystemAPI.GetSingleton<PlayerComponentData>();
        if (playerComponentData.IsDead)
            return;

        if (SystemAPI.HasSingleton<NextLevelComponentData>())
            return;

        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        new PlayerDetectionJob()
        {
            CollisionWorld = collisionWorld,
            Ecb = GetEntityCommandBuffer(ref state),
            OverlapDetectionOffset = overlapDetectionOffset,
            DeadZoneCollisionFilter = deadZoneCollisionFilter,
            GroundCollisionFilter = groundCollisionFilter,
            EndFlagCollisionFilter = endFlagCollisionFilter,
            CollectibleCollisionFilter = collectiblesCollisionFilter,
        }.Schedule();
    }

    [BurstCompile]
    private void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        Entity entity = SystemAPI.GetSingletonEntity<PlayerDetectionComponentData>();
        var playerGroundData = SystemAPI.GetComponentRO<PlayerDetectionComponentData>(entity).ValueRO;
        overlapDetectionOffset = playerGroundData.OverlapDetectionOffset;
        deadZoneCollisionFilter = playerGroundData.DeadZoneCollisionFilter;
        groundCollisionFilter = playerGroundData.GroundCollisionFilter;
        endFlagCollisionFilter = playerGroundData.EndFlagCollisionFilter;
        collectiblesCollisionFilter = playerGroundData.CollectibleCollisionFilter;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
}

[BurstCompile]
public partial struct PlayerDetectionJob : IJobEntity
{
    [ReadOnly] public CollisionWorld CollisionWorld;
    [ReadOnly] public float3 OverlapDetectionOffset;

    public EntityCommandBuffer Ecb;
    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter GroundCollisionFilter;
    public CollisionFilter EndFlagCollisionFilter;
    public CollisionFilter CollectibleCollisionFilter;

    [BurstCompile]
    public unsafe void Execute(ref PlayerMovementComponentData movementData, ref PlayerComponentData playerComponentData, in PhysicsCollider physicsCollider, in LocalTransform playerTransform)
    {

        var boxCollider = (Unity.Physics.BoxCollider*)physicsCollider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;

        bool isDead = CheckBox(playerTransform, boxGeometry, DeadZoneCollisionFilter);
        playerComponentData.IsDead = isDead;
        if (isDead)
            return;

        HandleCollectibleCollisionDetection(playerTransform, boxGeometry);
        HandleGroundCollisionDetection(ref movementData, playerTransform, boxGeometry);
        HandleEndFlagCollisionDetection(playerTransform, boxGeometry);
    }

    [BurstCompile]
    private void HandleCollectibleCollisionDetection(LocalTransform playerTransform, BoxGeometry boxGeometry)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);
        bool isCollectible = OverlapBox(playerTransform, boxGeometry, ref hits, CollectibleCollisionFilter);
        if (isCollectible)
        {
            foreach (var hit in hits)
            {
                Ecb.AddComponent(hit.Entity, new CollectedCollectibleComponentData());
            }
        }

        hits.Dispose();
    }

    [BurstCompile]
    private unsafe void HandleEndFlagCollisionDetection(LocalTransform playerTransform, BoxGeometry boxGeometry)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);

        bool isEndFlag = OverlapBox(playerTransform, boxGeometry, ref hits, EndFlagCollisionFilter);
        if (isEndFlag)
            Ecb.AddComponent(hits[0].Entity, new NextLevelComponentData());

        hits.Dispose();
    }

    [BurstCompile]
    private void HandleGroundCollisionDetection(ref PlayerMovementComponentData movementData, LocalTransform playerTransform, BoxGeometry boxGeometry)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);
        bool isGrounded = OverlapBox(playerTransform, boxGeometry, ref hits, GroundCollisionFilter);

        if (isGrounded)
            movementData.GroundHitEntity = hits[0].Entity;

        hits.Dispose();
        movementData.IsGrounded = isGrounded;
    }

    [BurstCompile]
    public bool CheckBox(LocalTransform playerTransform, BoxGeometry boxGeometry, CollisionFilter collisionFilter)
    {
        return CollisionWorld.CheckBox(playerTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, collisionFilter);
    }

    [BurstCompile]
    public bool OverlapBox(LocalTransform playerTransform, BoxGeometry boxGeometry, ref NativeList<DistanceHit> hits, CollisionFilter collisionFilter)
    {
        return CollisionWorld.OverlapBox(playerTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, collisionFilter);
    }
}
