using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    private EntityQuery bulletEntityQuery;

    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        bulletEntityQuery = SystemAPI.QueryBuilder().WithAll<BulletComponentData>().WithAll<LocalTransform>().Build();
    }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        new BulletMovementJob()
        {
            Ecb = GetEntityCommandBuffer(ref state),
            DeltaTime = deltaTime
        }.ScheduleParallel(bulletEntityQuery);
    }

    [BurstCompile]
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
    }
}

[BurstCompile]
public partial struct BulletMovementJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float DeltaTime;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int entityIndex, in Entity bullet, in LocalTransform localTransform, BulletComponentData bulletData)
    {
        float3 bulletCurrentPos = localTransform.Position;
        float3 newBulletPosition = bulletCurrentPos + bulletData.Direction * bulletData.Speed * DeltaTime;
        LocalTransform newLocalTransform = LocalTransform.FromPositionRotation(newBulletPosition, quaternion.Euler(bulletData.Rotation));
        Ecb.SetComponent(entityIndex, bullet, newLocalTransform);
    }
}
