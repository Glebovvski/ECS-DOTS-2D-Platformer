using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputSystem : SystemBase
{
    private InputControls inputControls;

    protected override void OnCreate()
    {
        inputControls = new InputControls();
        inputControls.Enable();
        var playerInputSystem = EntityManager.World.CreateSystemManaged<PlayerInputSystem>();
        playerInputSystem.SetPlayerMapActions(inputControls.PlayerMap);
    }

    protected override void OnUpdate()
    {
    }
}
