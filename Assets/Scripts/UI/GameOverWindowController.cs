using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Localization;

public class GameOverWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Localization")]
    [SerializeField] private LocalizedString winGameLocalizationKey;
    [SerializeField] private LocalizedString loseGameLocalizationKey;

    private GameOverSystem gameOverSystem;

    private void Awake()
    {
        gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
        container.SetActive(false);
    }

    private void OnEnable()
    {
        if (gameOverSystem == null)
            return;

        gameOverSystem.GameOver += OnGameOver;
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
    }
}
