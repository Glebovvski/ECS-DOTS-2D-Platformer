using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerAnimationCleanupSystem : ISystem
{
    private EntityQuery playerQuery;

    public void OnCreate(ref SystemState state)
    {
        playerQuery = SystemAPI.QueryBuilder()
            .WithAll<PlayerAnimationComponentData>()
            .WithNone<PlayerComponentData, LocalTransform>()
            .Build();

        state.RequireForUpdate(playerQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<PlayerAnimationComponentData> animationData, Entity entity)
                 in SystemAPI.Query<RefRO<PlayerAnimationComponentData>>()
                     .WithNone<PlayerComponentData, LocalTransform>()
                     .WithEntityAccess())
        {
            Animator animator = animationData.ValueRO.Animator.Value;

            if (animator != null)
            {
                Object.Destroy(animator.gameObject);
            }

            state.EntityManager.RemoveComponent<PlayerAnimationComponentData>(entity);
        }
    }
}