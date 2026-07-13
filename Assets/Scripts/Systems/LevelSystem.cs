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

    private Entity currentScene;
    private int currentLevelIndex;
    private Scene persistentScene;
    protected override void OnCreate()
    {
        RequireForUpdate<NextLevelComponentData>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        persistentScene = scene;
        if (scene.buildIndex != (int)SceneType.Game)
            return;

        currentLevelIndex = 0;
        currentScene = Entity.Null;
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
        if (currentLevelIndex > LevelReferences.Instance.SceneReferences.Count - 1)
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
        currentScene = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, LevelReferences.Instance.SceneReferences[currentLevelIndex++]);
        LevelLoaded?.Invoke();
    }

    public void UnloadPreviousLevel()
    {
        Debug.LogError("UNLOAD CALL");
        if (Entity.Null.Equals(currentScene))
            return;

        Debug.LogError("UNLOAD SUB SCENE");
        //World.EntityManager.CompleteAllTrackedJobs();
        //SceneManager.SetActiveScene(persistentScene);
        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, currentScene);
        
    }

    public bool IsCurrentSceneUnloaded()
    {
        if (Entity.Null.Equals(currentScene))
            return true;
        return !SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, currentScene);
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
