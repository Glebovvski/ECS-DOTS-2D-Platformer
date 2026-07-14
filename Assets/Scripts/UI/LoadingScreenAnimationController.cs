using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenAnimationController : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private LevelSystem levelSystem;

    private void OnEnable()
    {
        levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();

        levelSystem.LevelLoaded += OnLevelLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        text.canvasRenderer.SetAlpha(0f);
        image.canvasRenderer.SetAlpha(0f);
        Fade(1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == (int)SceneType.LoadingScreen)
            return;

        Fade(0f);
    }

    private void OnLevelLoaded()
    {
        Fade(0f);
    }

    private void Fade(float fade)
    {
        text.CrossFadeAlpha(fade, LevelSystem.AnimationTime / 1000f, false);
        image.CrossFadeAlpha(fade, LevelSystem.AnimationTime / 1000f, false);
    }

    private void OnDisable()
    {
        levelSystem.LevelLoaded -= OnLevelLoaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
