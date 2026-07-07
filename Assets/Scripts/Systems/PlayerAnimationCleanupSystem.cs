using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerAnimationCleanupSystem : ISystem
{
    private EntityQuery playerQuery;

    public void OnCreate(ref SystemState state)
    {
        //playerQuery = SystemAPI.QueryBuilder()
        //    .WithAll<PlayerAnimationComponentData>()
        //    .WithNone<PlayerComponentData, LocalTransform>()
        //    .Build();

        //state.RequireForUpdate(playerQuery);
    }

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

            state.EntityManager.RemoveComponent<PlayerManagedComponentData>(entity);
        }
    }
}