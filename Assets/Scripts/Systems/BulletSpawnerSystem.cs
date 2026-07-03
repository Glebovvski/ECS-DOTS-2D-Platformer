using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletSpawnerComponentData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity spawnerEntity = SystemAPI.GetSingletonEntity<BulletSpawnerComponentData>();
        BulletSpawnerComponentData spawnerData = SystemAPI.GetSingleton<BulletSpawnerComponentData>();
        var buffer = SystemAPI.GetBuffer<BulletBufferElementData>(spawnerEntity);

        for (int index = 0; index < spawnerData.BulletCount; index++)
        {
            float3 randomPosition = GetRandomSpawnPosition(ref spawnerData);
            Entity bulletPrefab = GetRandomPrefab(ref spawnerData, ref buffer);
            Entity bulletEntity = state.EntityManager.Instantiate(bulletPrefab);
            var bulletData = SystemAPI.GetComponentRO<BulletComponentData>(bulletEntity);

            LocalTransform newLocalTransform = LocalTransform.FromPositionRotation(randomPosition, quaternion.Euler(bulletData.ValueRO.Rotation, math.RotationOrder.XYZ));
            state.EntityManager.SetComponentData(bulletEntity, newLocalTransform);
        }

        SystemAPI.SetSingleton(spawnerData);

        state.Enabled = false;
    }

    private static Entity GetRandomPrefab(ref BulletSpawnerComponentData spawnerData, ref DynamicBuffer<BulletBufferElementData> buffer)
    {
        RandomizeInstance(ref spawnerData);
        int randomIndex = spawnerData.Random.NextInt(0, buffer.Length);
        return buffer[randomIndex].BulletPrefab;

    }

    private static float3 GetRandomSpawnPosition(ref BulletSpawnerComponentData spawnerData)
    {
        RandomizeInstance(ref spawnerData);
        float randomPositionY = spawnerData.Random.NextFloat(-20f, 20f);
        float randomPositionX = spawnerData.Random.NextFloat(-30f, 30f);
        return new float3(randomPositionX, randomPositionY, 0f);
    }

    private static void RandomizeInstance(ref BulletSpawnerComponentData spawnerData)
    {
        uint index = spawnerData.Random.NextUInt(UInt32.MaxValue);
        spawnerData.Random = Unity.Mathematics.Random.CreateFromIndex(index);
    }
}