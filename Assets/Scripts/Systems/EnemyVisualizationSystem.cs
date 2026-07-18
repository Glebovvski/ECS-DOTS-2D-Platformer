using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct EnemyVisualizationSystem : ISystem
{
    private EntityQuery enemyQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyVisualizationComponentData>().WithNone<EnemyManagedComponentData>().Build();
        state.RequireForUpdate(enemyQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = enemyQuery.ToEntityArray(Allocator.Temp);
        foreach (var enemy in enemies)
        {
            var enemyVisualization = state.EntityManager.GetComponentObject<EnemyVisualizationComponentData>(enemy);
            var enemyVisualizationGO = Object.Instantiate(enemyVisualization.EnemyVisualization);
            enemyVisualizationGO.transform.position = SystemAPI.GetComponent<LocalTransform>(enemy).Position;

            state.EntityManager.AddComponentObject(enemy, new EnemyManagedComponentData()
            {
                GameObject = enemyVisualizationGO,
                Transform = enemyVisualizationGO.transform,
                Animator = enemyVisualizationGO.GetComponent<Animator>()
            });
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
