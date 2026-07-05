using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
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
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    public void Execute(in Entity player, ref PhysicsVelocity playerVelocity, in PhysicsMass playerMass, in LocalTransform playerTransform, ref PlayerMovementComponentData playerMovementData, in PlayerComponentData playerData)
    {
        if (playerMovementData.IsGrounded == false)
            return;
        if (playerMovementData.IsJump)
        {
            HandleJump(ref playerVelocity, playerMass, playerData, ref playerMovementData);
            return;
        }

        if (playerMovementData.MoveDirection == 0f)
            return;

        SetPlayerRotation(player, playerTransform, playerMovementData);
        HandlePlayerMovement(player, playerVelocity, playerMovementData, playerData);
    }

    [BurstCompile]
    private void HandleJump(ref PhysicsVelocity playerVelocity, PhysicsMass playerMass, PlayerComponentData playerData, ref PlayerMovementComponentData playerMovementData)
    {
        playerVelocity.ApplyLinearImpulse(playerMass, new float3(0f, playerData.JumpForce, 0f));
        playerMovementData.IsJump = false;
    }

    [BurstCompile]
    private void SetPlayerRotation(Entity player, LocalTransform playerTransform, PlayerMovementComponentData data)
    {
        quaternion newPlayerRotation = data.MoveDirection > 0f ? new quaternion(0, 0, 0, 1) : new quaternion(0, 1, 0, 0);
        Ecb.SetComponent(player, new LocalTransform { Position = playerTransform.Position, Rotation = newPlayerRotation, Scale = playerTransform.Scale });
    }

    [BurstCompile]
    private void HandlePlayerMovement(Entity player, PhysicsVelocity playerVelocity, PlayerMovementComponentData movementData, PlayerComponentData playerData)
    {
        float3 linearVelocity = new float3(movementData.MoveDirection * playerData.Speed, playerVelocity.Linear.y, playerVelocity.Linear.z);
        Ecb.SetComponent(player, new PhysicsVelocity { Linear = linearVelocity });
    }

}
