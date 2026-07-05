using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    public float Speed => speed;
    public float JumpForce => jumpForce;
}

public class Baker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerComponentData()
        {
            Speed = authoring.Speed,
            JumpForce = authoring.JumpForce
        });
        AddComponent(entity, new PlayerMovementComponentData()
        {
            MoveDirection = 0,
            IsJump = false,
            IsGrounded = true
        });
    }
}
