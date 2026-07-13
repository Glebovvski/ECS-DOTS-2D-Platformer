using System;
using Unity.Entities;

public partial class InputSystem : SystemBase
{
    private InputControls inputControls;
    private GameOverSystem gameOverSystem;
    private LevelSystem levelSystem;

    protected override void OnCreate()
    {
        inputControls = new InputControls();
        var playerInputSystem = EntityManager.World.CreateSystemManaged<PlayerInputSystem>();
        playerInputSystem.SetPlayerMapActions(inputControls.PlayerMap);
    }

    protected override void OnStartRunning()
    {
        gameOverSystem = EntityManager.World.GetExistingSystemManaged<GameOverSystem>();
        levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        gameOverSystem.GameOver += OnGameOver;
        levelSystem.NextLevel += OnNextLevel;
        levelSystem.LevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        inputControls.Enable();
    }

    private void OnNextLevel()
    {
        inputControls.Disable();
    }

    private void OnGameOver(bool state)
    {
        inputControls.Disable();
    }

    protected override void OnStopRunning()
    {
        gameOverSystem.GameOver -= OnGameOver;
        levelSystem.NextLevel -= OnNextLevel;
        levelSystem.LevelLoaded -= OnLevelLoaded;
    }

    protected override void OnUpdate()
    {
    }
}
