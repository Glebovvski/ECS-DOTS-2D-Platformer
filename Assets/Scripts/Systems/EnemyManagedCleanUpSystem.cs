using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct EnemyManagedCleanUpSystem : ISystem
{
    private EntityQuery enemyManagedQuery;

    public void OnCreate(ref SystemState state)
    {
        enemyManagedQuery = SystemAPI.QueryBuilder().WithAll<EnemyManagedComponentData>().WithNone<LocalTransform>().Build();

        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(enemyManagedQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = enemyManagedQuery.ToEntityArray(Allocator.Temp);
        EntityCommandBuffer Ecb = GetEntityCommandBuffer(ref state);

        foreach (var enemy in enemies)
        {
            var enemyManagedData = state.EntityManager.GetComponentObject<EnemyManagedComponentData>(enemy);
            Object.Destroy(enemyManagedData.GameObject);
            Ecb.RemoveComponent<EnemyManagedComponentData>(enemy);
        }
    }

    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }
}
