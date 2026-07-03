using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private List<GameObject> bulletPrefabs;
    [SerializeField] private int bulletCount;
    public uint InitialIndex => (uint)new System.Random().Next(0, Int32.MaxValue);

    public int BulletCount => bulletCount;
    public List<GameObject> BulletPrefabs => bulletPrefabs;
}

public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
{
    public override void Bake(BulletSpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        var bulletBufferElementData = AddBuffer<BulletBufferElementData>(entity);
        foreach (var bulletPrefab in authoring.BulletPrefabs)
        {
            bulletBufferElementData.Add(new BulletBufferElementData()
            {
                BulletPrefab = GetEntity(bulletPrefab, TransformUsageFlags.Dynamic)
            });
        }
        AddComponent(entity, new BulletSpawnerComponentData()
        {
            Random = new Unity.Mathematics.Random(authoring.InitialIndex),
            BulletCount = authoring.BulletCount,
        });
    }
}
