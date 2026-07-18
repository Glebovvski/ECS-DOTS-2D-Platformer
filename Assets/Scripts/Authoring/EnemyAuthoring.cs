using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject enemyVisualization;

    public GameObject EnemyVisualization => enemyVisualization;
}

public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponentObject(entity, new EnemyVisualizationComponentData()
        {
            EnemyVisualization = authoring.EnemyVisualization
        });
    }
}
