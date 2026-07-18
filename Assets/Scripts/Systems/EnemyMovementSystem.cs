using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct EnemyMovementSystem : ISystem
{
    private EntityQuery enemyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyManagedComponentData>().WithAll<EnemyMovementComponentData>().Build();
        state.RequireForUpdate(enemyQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Debug.Log("MOVING ENEMY SYSTEM");
        NativeArray<Entity> enemies = enemyQuery.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in enemies)
        {
            Debug.Log($"Entity: {entity.Index}");
            var enemyManagedData = state.EntityManager.GetComponentObject<EnemyManagedComponentData>(entity);

        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
