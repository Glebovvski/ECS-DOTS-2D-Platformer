using System;
using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button gameOverBtn;

    [Header("Localization")]
    [SerializeField] private LocalizedString winGameLocalizationKey;
    [SerializeField] private LocalizedString loseGameLocalizationKey;

    private GameOverSystem gameOverSystem;
    private LevelSystem levelSystem;

    private void Awake()
    {
        gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
        levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        container.SetActive(false);
    }

    private void OnEnable()
    {
        if (gameOverSystem == null)
            return;

        gameOverSystem.GameOver += OnGameOver;
        gameOverBtn.onClick.AddListener(OnGameOverBtnClicked);
    }

    private void OnGameOverBtnClicked()
    {
        StartCoroutine(UnloadSceneRoutine());
    }

    private IEnumerator UnloadSceneRoutine()
    {
        levelSystem.UnloadPreviousLevel();
        while(levelSystem.IsCurrentSceneUnloaded() == false)
        {
            yield return null;
        }
        yield return null;
        container.SetActive(false);
        SceneManager.LoadScene((int)SceneType.Menu);
    }

    private void OnGameOver(bool isPlayerDead)
    {
        container.SetActive(true);
        text.text = isPlayerDead ? loseGameLocalizationKey.GetLocalizedString() : winGameLocalizationKey.GetLocalizedString();
    }

    private void OnDisable()
    {
        if (gameOverSystem == null)
            return;

        gameOverSystem.GameOver -= OnGameOver;
        gameOverBtn.onClick.RemoveListener(OnGameOverBtnClicked);
    }
}
