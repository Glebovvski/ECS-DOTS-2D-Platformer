using Unity.Entities;
using UnityEngine;

public struct PlayerManagedComponentData : ICleanupComponentData
{
    public UnityObjectRef<Animator> Animator;
    public UnityObjectRef<GameObject> GameObject;
    public UnityObjectRef<Transform> Transform;
}