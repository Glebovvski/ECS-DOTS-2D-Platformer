using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField] private float3 direction;
    [SerializeField] private float3 rotation;
    [SerializeField] private float speed;

    public float3 Direction => direction;
    public float3 Rotation => rotation;
    public float Speed => speed;
}

public class BulletBaker : Baker<BulletAuthoring>
{
    public override void Bake(BulletAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new BulletComponentData()
        {
            Direction = authoring.Direction,
            Rotation = authoring.Rotation,
            Speed = authoring.Speed
        });
    }
}
