using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Authoring;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PhysicsShapeAuthoring))]
public class PlatformMovementAuthoring : MonoBehaviour
{
    [SerializeField] private float3 movementVector;
    [SerializeField] private float movementSpeed;

    public float3 MovementVector => movementVector;
    public float MovementSpeed => movementSpeed;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color visualisationColor = new Color(0.4f, 0.4f, 0.9f);
        float3 initialPosition = transform.position;
        float3 startPos = initialPosition - movementVector;
        float3 endPos = initialPosition + movementVector;
        Handles.DrawBezier(startPos, endPos, startPos, endPos, visualisationColor, null, 6f);

        var shape = GetComponent<PhysicsShapeAuthoring>();
        float3 platformSize = shape.GetBoxProperties().Size * transform.localScale;
        Color cachedColor = Handles.color;
        Handles.color = visualisationColor;
        Handles.DrawWireCube(startPos, platformSize);
        Handles.DrawWireCube(endPos, platformSize);
        Handles.color = cachedColor;
    }
#endif
}

public class PlatformMovementBaker : Baker<PlatformMovementAuthoring>
{
    public override void Bake(PlatformMovementAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new PlatformMovementComponentData()
        {
            InitialPosition = authoring.transform.position,
            MovementVector = authoring.MovementVector,
            MovementSpeed = authoring.MovementSpeed,
            IsReverse = false
        });
    }
}