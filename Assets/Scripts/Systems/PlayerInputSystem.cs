using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
public partial class PlayerInputSystem : SystemBase
{
    private InputControls.PlayerMapActions playerMapActions;
    private bool hasPlayerMapActions;

    private float moveDirection;

    public void SetPlayerMapActions(InputControls.PlayerMapActions _playerMapActions)
    {
        if (hasPlayerMapActions)
        {
            playerMapActions.Move.performed -= OnMovePerformed;
            playerMapActions.Move.canceled -= OnMoveCanceled;
            playerMapActions.Jump.performed -= OnJumpPerformed;
        }

        playerMapActions = _playerMapActions;
        hasPlayerMapActions = true;

        playerMapActions.Move.performed += OnMovePerformed;
        playerMapActions.Move.canceled += OnMoveCanceled;

        playerMapActions.Jump.performed += OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        var playerMovementData = SystemAPI.GetComponentRW<PlayerMovementComponentData>(player);
        playerMovementData.ValueRW.IsJump = true;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<float>();
        Debug.LogError("Move performed: " + moveDirection);
        Entity player = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        RefRW<PlayerMovementComponentData> playerData = SystemAPI.GetComponentRW<PlayerMovementComponentData>(player);

        playerData.ValueRW.MoveDirection = moveDirection;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveDirection = 0f;
        Debug.LogError("Move canceled");
        Entity player = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        RefRW<PlayerMovementComponentData> playerData = SystemAPI.GetComponentRW<PlayerMovementComponentData>(player);

        playerData.ValueRW.MoveDirection = moveDirection;
    }

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerComponentData>();
        RequireForUpdate<PlayerMovementComponentData>();
    }

    protected override void OnUpdate()
    {
        // Entity player = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        // RefRW<PlayerMovementComponentData> playerData = SystemAPI.GetComponentRW<PlayerMovementComponentData>(player);

        // playerData.ValueRW.MoveDirection = moveDirection;
    }

    protected override void OnDestroy()
    {
        if (!hasPlayerMapActions)
            return;

        playerMapActions.Move.performed -= OnMovePerformed;
        playerMapActions.Move.canceled -= OnMoveCanceled;
        playerMapActions.Jump.performed -= OnJumpPerformed;
    }
}