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
        inputControls.Enable();
        var playerInputSystem = EntityManager.World.CreateSystemManaged<PlayerInputSystem>();
        playerInputSystem.SetPlayerMapActions(inputControls.PlayerMap);
    }

    protected override void OnStartRunning()
    {
        gameOverSystem = EntityManager.World.GetExistingSystemManaged<GameOverSystem>();
        levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        gameOverSystem.GameOver += OnGameOver;
        levelSystem.NextLevel += OnNextLevel;
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
    }

    protected override void OnUpdate()
    {
    }
}
