using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerManagedCleanupSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<PlayerManagedComponentData> managedData, Entity entity)
                 in SystemAPI.Query<RefRO<PlayerManagedComponentData>>()
                     .WithNone<PlayerComponentData, LocalTransform>()
                     .WithEntityAccess())
        {
            var go = managedData.ValueRO.GameObject;

            if (go != null)
            {
                Object.Destroy(go);
            }
            var ecb = GetEntityCommandBuffer(ref state);
            ecb.RemoveComponent<PlayerManagedComponentData>(entity);
            //state.EntityManager.RemoveComponent<PlayerManagedComponentData>(entity);
        }
    }


    [BurstCompile]
    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
}