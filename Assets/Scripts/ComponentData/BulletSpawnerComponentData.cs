using Unity.Entities;
using Unity.Mathematics;

public struct BulletSpawnerComponentData : IComponentData
{
    public Random Random;
    public int BulletCount;
}

public struct BulletBufferElementData : IBufferElementData
{
    public Entity BulletPrefab;
}