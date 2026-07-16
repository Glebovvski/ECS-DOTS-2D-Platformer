using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CollectibleAuthoring : MonoBehaviour
{
    [SerializeField] private List<CollectibleData> collectibleData;

    public List<CollectibleData> CollectibleData => collectibleData;
}

public class CollectibleBaker : Baker<CollectibleAuthoring>
{
    public override void Bake(CollectibleAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var collectibleBlobAsset = CreateCollectibleBlobAsset(authoring.CollectibleData);

        AddBlobAsset(ref collectibleBlobAsset, out _);
        AddComponent(entity, new CollectibleComponentData()
        {
            CollectiblePoolReference = collectibleBlobAsset
        });
    }

    private BlobAssetReference<CollectiblePool> CreateCollectibleBlobAsset(IReadOnlyList<CollectibleData> collectibleData)
    {
        using var builder = new BlobBuilder(Allocator.Temp);
        ref var collectiblePool = ref builder.ConstructRoot<CollectiblePool>();

        BlobBuilderArray<CollectibleContainer> arrayBuilder = builder.Allocate(ref collectiblePool.CollectibleData, collectibleData.Count);

        for(int i = 0; i < collectibleData.Count; i++)
        {
            var collectible = collectibleData[i];
            var type = collectible.CollectibleType;
            var points = collectible.Points;
            arrayBuilder[i] = new CollectibleContainer()
            {
                CollectibleType = type,
                Points = points,
            };
        }

        return builder.CreateBlobAssetReference<CollectiblePool>(Allocator.Persistent);
    }
}
