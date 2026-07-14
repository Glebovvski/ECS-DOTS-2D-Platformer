using System;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class LevelSystem : SystemBase
{
    public event Action LastLevelCompleted;
    public event Action LevelLoaded;
    public event Action NextLevel;

    private SceneType currentScene;
    private Entity currentEntityScene;
    private int currentLevelIndex;
    //private Scene persistentScene;

    public void LoadScene(SceneType sceneType, LoadSceneMode mode)
    {
        Debug.LogError($"Scene Type {sceneType}");
        if (currentScene != SceneType.Main)
            SceneManager.UnloadSceneAsync((int)currentScene);

        SceneManager.LoadSceneAsync((int) sceneType, mode);
        currentScene = sceneType;
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
        LoadNextLevel();
    }

    protected override void OnUpdate()
    {
        var nextLevelData = SystemAPI.GetSingletonRW<NextLevelComponentData>();

        if (nextLevelData.ValueRO.IsInvoked)
            return;

        NextLevel?.Invoke();
        nextLevelData.ValueRW.IsInvoked = true;
    }

    public void LoadNextLevel()
    {
        var entitySceneReferenceDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        if (currentLevelIndex > entitySceneReferenceDynamicBuffer.Length - 1)
        {
            LastLevelCompleted?.Invoke();
            return;
        }

        UnloadPreviousLevel();
        LoadNextSubScene();
    }

    private void LoadNextSubScene()
    {
        Debug.LogError("LOAD SUB SCENE");
        var entitySceneReferenceDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        currentEntityScene = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entitySceneReferenceDynamicBuffer[currentLevelIndex++].EntitySceneReference);
        LevelLoaded?.Invoke();
    }

    public void UnloadPreviousLevel()
    {
        Debug.LogError("UNLOAD CALL");
        if (Entity.Null.Equals(currentEntityScene))
            return;

        Debug.LogError("UNLOAD SUB SCENE");
        //World.EntityManager.CompleteAllTrackedJobs();
        //SceneManager.SetActiveScene(persistentScene);
        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentEntityScene);
        
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
