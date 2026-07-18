using Unity.Entities;
using UnityEngine;

public class EnemyJumpAuthoring : MonoBehaviour
{
    [SerializeField] private float jumpForce;

    public float JumpForce => jumpForce;
}

public class EnemyJumpBaker : Baker<EnemyJumpAuthoring>
{
    public override void Bake(EnemyJumpAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new EnemyJumpComponentData()
        {
            JumpForce = authoring.JumpForce,
        });
    }
}
