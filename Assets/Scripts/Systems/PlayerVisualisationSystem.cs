using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PlayerVisualisationSystem : ISystem
{

    private EntityQuery playerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        playerQuery = SystemAPI.QueryBuilder().WithAll<PlayerManagedComponentData>().WithNone<PlayerAnimationComponentData>().Build();
        state.RequireForUpdate(playerQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entity = playerQuery.GetSingletonEntity();
        var playerVisualisation = playerQuery.GetSingleton<PlayerManagedComponentData>();

        var playerVisualisationGO = Object.Instantiate(playerVisualisation.PlayerVisualisation);
        // if (playerVisualisationGO.TryGetComponent<Animator>(out var animator))
        {
            var ecb = GetEntityCommandBuffer(ref state);
            ecb.AddComponent(entity, new PlayerAnimationComponentData()
            {
                Animator = playerVisualisationGO.GetComponent<Animator>()
            });
        }
    }

    [BurstCompile]
    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
