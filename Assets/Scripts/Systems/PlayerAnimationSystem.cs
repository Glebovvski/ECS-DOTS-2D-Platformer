using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct PlayerAnimationSystem : ISystem
{
    private EntityQuery playerQuery;

    public void OnCreate(ref SystemState state)
    {
        playerQuery = SystemAPI.QueryBuilder()
            .WithAll<LocalTransform>()
            .WithAll<PlayerMovementComponentData>()
            .WithAll<PlayerManagedComponentData>()
            .Build();

        state.RequireForUpdate(playerQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();

        if (playerQuery.CalculateEntityCount() != 1)
            return;

        Entity playerEntity = playerQuery.GetSingletonEntity();

        PlayerMovementComponentData playerMovementData =
            state.EntityManager.GetComponentData<PlayerMovementComponentData>(playerEntity);

        LocalTransform localTransform =
            state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        PlayerManagedComponentData playerAnimationData =
            state.EntityManager.GetComponentData<PlayerManagedComponentData>(playerEntity);

        Animator animator = playerAnimationData.Animator.Value;

        if (animator == null)
            return;

        Transform animatorTransform = animator.transform;

        animatorTransform.position = localTransform.Position;
        animatorTransform.rotation = localTransform.Rotation;

        float scale = localTransform.Scale;
        animatorTransform.localScale = new Vector3(scale, scale, scale);

        if (playerMovementData is { IsGrounded: true, IsJump: true })
        {
            animator.SetTrigger("Jump");
        }

        animator.SetBool(
            "Run",
            math.abs(playerMovementData.MoveDirection) > 0f
        );
    }
}