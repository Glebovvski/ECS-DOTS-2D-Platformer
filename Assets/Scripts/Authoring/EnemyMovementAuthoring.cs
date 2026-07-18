using Unity.Entities;
using UnityEngine;

public class EnemyMovementAuthoring : MonoBehaviour
{
    [SerializeField] private float speed;

    public float Speed => speed;
}

public class EnemyMovementBaker : Baker<EnemyMovementAuthoring>
{
    public override void Bake(EnemyMovementAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new EnemyMovementComponentData()
        {
            MovementSpeed = authoring.Speed
        });
    }
}
