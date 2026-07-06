using Unity.Entities;
using UnityEngine;

public struct PlayerAnimationComponentData : ICleanupComponentData
{
    public UnityObjectRef<Animator> Animator;
}