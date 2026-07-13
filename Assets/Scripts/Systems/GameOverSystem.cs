using System;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial class GameOverSystem : SystemBase
{
    public event Action<bool> GameOver;
    private LevelSystem levelSystem;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerComponentData>();
    }

    protected override void OnStartRunning()
    {
        levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        levelSystem.LastLevelCompleted += WinGame;
    }

    private void WinGame()
    {
        GameOver?.Invoke(false);
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        EntityManager.DestroyEntity(playerEntity);
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

    protected override void OnStopRunning()
    {
        levelSystem.LastLevelCompleted -= WinGame;
    }
}
