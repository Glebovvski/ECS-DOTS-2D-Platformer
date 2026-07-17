using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
partial struct SnapPlayerToPlatformSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerComponentData>();
        state.RequireForUpdate<PlayerMovementComponentData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerComponentData = SystemAPI.GetSingleton<PlayerComponentData>();
        if (playerComponentData.IsDead)
            return;

        var playerMovementData = SystemAPI.GetSingleton<PlayerMovementComponentData>();
        if (!playerMovementData.IsGrounded)
            return;

        var groundHitEntity = playerMovementData.GroundHitEntity;
        bool isMovingPlatform = SystemAPI.HasComponent<PlatformMovementComponentData>(groundHitEntity);
        if (!isMovingPlatform)
            return;

        var platformData = SystemAPI.GetComponentRO<PlatformMovementComponentData>(groundHitEntity);
        state.Dependency = new SnapPlayerToPlatformJob()
        {
            Ecb = GetEntityCommandBuffer(ref state),
            PlatformData = platformData.ValueRO
        }.Schedule(state.Dependency);

        //jobHandle.Complete();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
}

[BurstCompile]
public partial struct SnapPlayerToPlatformJob : IJobEntity
{
    public EntityCommandBuffer Ecb;
    public PlatformMovementComponentData PlatformData;

    [BurstCompile]
    public void Execute(in Entity player, ref PhysicsVelocity playerVelocity, in PlayerMovementComponentData playerMovement)
    {
        if (playerMovement.MoveDirection != 0)
            return;

        float3 direction = math.normalize(PlatformData.MovementVector);
        if (PlatformData.IsReverse)
            direction = -direction;

        float platformSpeed = PlatformData.MovementSpeed;
        playerVelocity.Linear = platformSpeed * direction;
        Ecb.SetComponent(player, playerVelocity);
    }
}