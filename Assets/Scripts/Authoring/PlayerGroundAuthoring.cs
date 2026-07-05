using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerGroundAuthoring : MonoBehaviour
{
    [SerializeField] private float3 overlapDetectionOffset = new float3(0f, -0.5f, 0f);

    public float3 OverlapDetectionOffset => overlapDetectionOffset;
}

public class PlayerGroundBaker : Baker<PlayerGroundAuthoring>
{
    public override void Bake(PlayerGroundAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new PlayerGroundComponentData()
        {
            OverlapDetectionOffset = authoring.OverlapDetectionOffset
        });
    }
}
