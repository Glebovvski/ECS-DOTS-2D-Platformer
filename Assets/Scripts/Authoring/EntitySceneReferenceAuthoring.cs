using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
#if UNITY_EDITOR
public class EntitySceneReferenceAuthoring : MonoBehaviour
{
    [SerializeField] private List<UnityEditor.SceneAsset> levels;

    public List<UnityEditor.SceneAsset> Levels => levels;
}

public class EntitySceneReferenceBaker : Baker<EntitySceneReferenceAuthoring>
{
    public override void Bake(EntitySceneReferenceAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var buffer = AddBuffer<EntitySceneReferenceBufferElementData>(entity);
        foreach(var sceneAsset in authoring.Levels)
        {
            buffer.Add(new EntitySceneReferenceBufferElementData()
            {
                EntitySceneReference = new EntitySceneReference(sceneAsset)
            });
        }
    }
}
#endif