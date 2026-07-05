using Unity.Entities;
using UnityEngine;

public struct PlayerMovementComponentData : IComponentData
{
    public float MoveDirection;
    public bool IsJump;
    public bool IsGrounded;
}
