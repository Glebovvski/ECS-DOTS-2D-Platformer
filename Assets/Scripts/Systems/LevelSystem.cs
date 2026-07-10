using System;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class LevelSystem : SystemBase
{
    public event Action NextLevel;

    protected override void OnCreate()
    {
        RequireForUpdate<NextLevelComponentData>();
    }

    protected override void OnUpdate()
    {
        var nextLevelData = SystemAPI.GetSingletonRW<NextLevelComponentData>();

        if (nextLevelData.ValueRO.IsInvoked)
            return;

        NextLevel?.Invoke();
        nextLevelData.ValueRW.IsInvoked = true;
    }
}
