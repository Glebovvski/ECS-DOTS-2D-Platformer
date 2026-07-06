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
            .WithAll<PlayerAnimationComponentData>()
            .Build();

        state.RequireForUpdate(playerQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        // Required because this system reads LocalTransform on main thread
        // while physics may have scheduled transform-writing jobs.
        state.Dependency.Complete();

        if (playerQuery.CalculateEntityCount() != 1)
            return;

        Entity playerEntity = playerQuery.GetSingletonEntity();

        PlayerMovementComponentData playerMovementData =
            state.EntityManager.GetComponentData<PlayerMovementComponentData>(playerEntity);

        LocalTransform localTransform =
            state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

        PlayerAnimationComponentData playerAnimationData =
            state.EntityManager.GetComponentData<PlayerAnimationComponentData>(playerEntity);

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