using Unity.Entities;
using UnityEngine;

public class SceneCollectibleAuthoring : MonoBehaviour
{
    [SerializeField] private CollectibleType type;

    public CollectibleType Type => type;
}

public class SceneCollectibleBaker : Baker<SceneCollectibleAuthoring>
{
    public override void Bake(SceneCollectibleAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new SceneCollectibleComponentData()
        {
            Type = authoring.Type,
        });
    }
}