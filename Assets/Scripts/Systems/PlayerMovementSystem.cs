using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // state.RequireForUpdate<PlayerMovementComponentData>();
        // state.RequireForUpdate<PlayerComponentData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new PlayerMovementJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Ecb = GetEntityCommandBuffer(ref state)
        }.Schedule();
    }

    [BurstCompile]
    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
}

[BurstCompile]
public partial struct PlayerMovementJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    public void Execute(in Entity player, in LocalTransform localTransform, in PlayerMovementComponentData playerMovementData, in PlayerComponentData playerData)
    {
        if (playerMovementData.MoveDirection == 0f)
        {
            Debug.LogError("MoveDirection is 0, skipping movement");
            return;
        }
        float3 playerCurrentPos = localTransform.Position;
        float3 newPlayerPosition = playerCurrentPos + new float3(playerMovementData.MoveDirection, 0f, 0f) * DeltaTime * playerData.Speed;
        quaternion newPlayerRotation = playerMovementData.MoveDirection > 0f ? new quaternion(0, 0, 0, 1) : new quaternion(0, 1, 0, 0);
        Ecb.SetComponent(player, new LocalTransform { Position = newPlayerPosition, Rotation = newPlayerRotation, Scale = localTransform.Scale });
    }


}
