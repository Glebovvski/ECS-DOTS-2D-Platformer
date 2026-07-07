using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class PlayerGroundAuthoring : MonoBehaviour
{
    [SerializeField] private float3 overlapDetectionOffset = new float3(0f, -0.5f, 0f);
    [SerializeField] private PhysicsCategoryTags deadZoneBelongsTo;
    [SerializeField] private PhysicsCategoryTags deadZoneCollidesWith;
    [SerializeField] private PhysicsCategoryTags groundBelongsTo;
    [SerializeField] private PhysicsCategoryTags groundCollidesWith;

    public float3 OverlapDetectionOffset => overlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter => new()
    {
        BelongsTo = deadZoneBelongsTo.Value,
        CollidesWith = deadZoneCollidesWith.Value,
    };

    public CollisionFilter GroundCollisionFilter => new()
    {
        BelongsTo = groundBelongsTo.Value,
        CollidesWith = groundCollidesWith.Value,
    };
}

public class PlayerGroundBaker : Baker<PlayerGroundAuthoring>
{
    public override void Bake(PlayerGroundAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new PlayerGroundComponentData()
        {
            OverlapDetectionOffset = authoring.OverlapDetectionOffset,
            DeadZoneCollisionFilter = authoring.DeadZoneCollisionFilter,
            GroundCollisionFilter = authoring.GroundCollisionFilter
        });
    }
}
