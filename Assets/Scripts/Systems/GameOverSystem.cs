using System;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial class GameOverSystem : SystemBase
{
    public event Action<bool> GameOver;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerComponentData>();
    }

    protected override void OnUpdate()
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        var playerComponentData = SystemAPI.GetComponent<PlayerComponentData>(playerEntity);
        
        if (!playerComponentData.IsDead)
            return;

        GameOver?.Invoke(true);
        EntityManager.DestroyEntity(playerEntity);
    }
}
