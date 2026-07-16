using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(PlayerDetectionSystem))]
partial struct CollectibleSystem : ISystem
{
    private EntityQuery collectedCollectibleQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        collectedCollectibleQuery = SystemAPI.QueryBuilder().WithAll<CollectedCollectibleComponentData>().Build();
        state.RequireForUpdate<CollectibleComponentData>();
        state.RequireForUpdate(collectedCollectibleQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> collectedCollectibles = collectedCollectibleQuery.ToEntityArray(Allocator.Temp);

        var collectibleComponentData = SystemAPI.GetSingleton<CollectibleComponentData>();
        ref var collectiblePool = ref collectibleComponentData.CollectiblePoolReference.Value;
        ref var collectibleDataArray = ref collectiblePool.CollectibleData;

        for(int i = 0; i < collectedCollectibles.Length; i++)
        {
            HandleCollectible(ref state, collectedCollectibles, i, ref collectibleDataArray);
        }

        collectedCollectibles.Dispose();
    }

    [BurstCompile]
    private void HandleCollectible(ref SystemState state, NativeArray<Entity> collectedCollectibles, int index, ref BlobArray<CollectibleContainer> collectibleDataArray)
    {
        var entity = collectedCollectibles[index];
        var sceneCollectibleComponentData = SystemAPI.GetComponent<SceneCollectibleComponentData>(entity);

        for(int i = 0; i< collectibleDataArray.Length; i++)
        {
            var collectibleContainer = collectibleDataArray[i];
            if(collectibleContainer.CollectibleType == sceneCollectibleComponentData.Type)
            {
                state.EntityManager.DestroyEntity(entity);
                var pointsEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(pointsEntity, new PointsComponentData()
                {
                    Points = collectibleContainer.Points
                });
                break;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
