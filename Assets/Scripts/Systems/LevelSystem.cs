using Cysharp.Threading.Tasks;
using System;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class LevelSystem : SystemBase
{
    public const int AnimationTime = 1000;

    public event Action LastLevelCompleted;
    public event Action LevelLoaded;
    public event Action NextLevel;

    private SceneType currentScene;
    private Entity currentEntityScene;
    private int currentLevelIndex;
    //private Scene persistentScene;

    public async void LoadScene(SceneType sceneType, LoadSceneMode mode)
    {
        Debug.LogError($"Scene Type {sceneType}");
        await LoadingScreen(true);
        if (currentScene != SceneType.Main)
            await SceneManager.UnloadSceneAsync((int)currentScene);

        await SceneManager.LoadSceneAsync((int) sceneType, mode);
        currentScene = sceneType;
        await LoadingScreen(false);
    }

    protected override void OnCreate()
    {
        RequireForUpdate<NextLevelComponentData>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadScene(SceneType.Menu, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //persistentScene = scene;
        if (scene.buildIndex != (int)SceneType.Game)
            return;

        currentLevelIndex = 0;
        currentEntityScene = Entity.Null;
        LoadNextLevel(false);
    }

    protected override void OnUpdate()
    {
        var nextLevelData = SystemAPI.GetSingletonRW<NextLevelComponentData>();

        if (nextLevelData.ValueRO.IsInvoked)
            return;

        NextLevel?.Invoke();
        nextLevelData.ValueRW.IsInvoked = true;
    }

    public async void LoadNextLevel(bool isLoadingScreen = true)
    {
        var entitySceneReferenceDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        if (currentLevelIndex > entitySceneReferenceDynamicBuffer.Length - 1)
        {
            LastLevelCompleted?.Invoke();
            return;
        }

        if (isLoadingScreen)
            await LoadingScreen(true);

        await UnloadPreviousLevel();
        await LoadNextSubScene();

        if (isLoadingScreen)
            await LoadingScreen(false);
    }

    private async UniTask LoadingScreen(bool isEnabled)
    {
        if (isEnabled)
            await SceneManager.LoadSceneAsync((int)SceneType.LoadingScreen, LoadSceneMode.Additive);

        await UniTask.Delay(AnimationTime);

        if (!isEnabled)
            await SceneManager.UnloadSceneAsync((int)SceneType.LoadingScreen);
    }

    private async UniTask LoadNextSubScene()
    {
        Debug.LogError("LOAD SUB SCENE");
        var entitySceneReferenceDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        currentEntityScene = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entitySceneReferenceDynamicBuffer[currentLevelIndex++].EntitySceneReference);

        while (!SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentEntityScene))
            await UniTask.Yield();

        LevelLoaded?.Invoke();
    }

    public async UniTask UnloadPreviousLevel()
    {
        if (Entity.Null.Equals(currentEntityScene))
            return;

        Debug.LogError("UNLOAD SUB SCENE");
        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentEntityScene);

        while (SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentEntityScene))
            await UniTask.Yield();

        await UniTask.WaitForEndOfFrame();
    }

    public bool IsCurrentSceneUnloaded()
    {
        if (Entity.Null.Equals(currentEntityScene))
            return true;
        return !SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentEntityScene);
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
