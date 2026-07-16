using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(CollectibleSystem))]
public partial class PointsSystem : SystemBase
{
    public event Action<float> PointsUpdated;

    private EntityQuery pointsEntityQuery;
    private LevelSystem levelSystem;
    private float points;

    protected override void OnCreate()
    {
        pointsEntityQuery = SystemAPI.QueryBuilder().WithAll<PointsComponentData>().Build();
    }

    protected override void OnStartRunning()
    {
        levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        levelSystem.LevelLoaded += OnLevelLoaded;
        new PointsController(this);
    }

    private void OnLevelLoaded()
    {
        points = 0;
        PointsUpdated?.Invoke(points);
    }

    protected override void OnUpdate()
    {
        NativeArray<Entity> pointsEntities = pointsEntityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<PointsComponentData> pointsData = pointsEntityQuery.ToComponentDataArray<PointsComponentData>(Allocator.Temp);

        for (int i = 0; i < pointsData.Length; i++)
        {
            var pointsEntity = pointsEntities[i];
            var pointData = pointsData[i];

            points += pointData.Points;
            EntityManager.DestroyEntity(pointsEntity);
            PointsUpdated?.Invoke(points);
        }

        pointsData.Dispose();
        pointsEntities.Dispose();
    }

    protected override void OnStopRunning()
    {
        levelSystem.LevelLoaded -= OnLevelLoaded;
    }
}
