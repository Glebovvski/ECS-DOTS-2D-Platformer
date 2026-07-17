using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PlatformMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        new PlatformMovementJob()
        {
            Ecb = GetEntityCommandBuffer(ref state),
            DeltaTime = deltaTime
        }.ScheduleParallel();

        //state.Dependency.Complete();

        //jobHandle.Complete();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
    }
}

[BurstCompile]
public partial struct PlatformMovementJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float DeltaTime;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery]int sortKey, in Entity entity, ref LocalTransform localTransform, ref PlatformMovementComponentData platformData)
    {
        float3 initialPosition = platformData.InitialPosition;
        float3 movementVector = platformData.MovementVector;
        float speed = platformData.MovementSpeed;

        int movementDirection = platformData.IsReverse ? -1 : 1;
        float3 targetPosition = initialPosition + movementVector * movementDirection;
        float3 movementStep = movementDirection * speed * math.normalize(movementVector) * DeltaTime;
        float3 newPos = localTransform.Position + movementStep;

        bool shouldReverse = platformData.IsReverse ? math.all(newPos <= targetPosition) : math.all(newPos >= targetPosition);
        if (shouldReverse)
            ReverseMovement(sortKey, entity, ref platformData);

        localTransform.Position = newPos;
        Ecb.SetComponent(sortKey, entity, localTransform);
    }

    [BurstCompile]
    private void ReverseMovement(int sortKey, Entity entity, ref PlatformMovementComponentData platformData)
    {
        platformData.IsReverse = !platformData.IsReverse;
        Ecb.SetComponent(sortKey, entity, platformData);
    }
}