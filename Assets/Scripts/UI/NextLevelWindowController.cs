using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject container;

    [SerializeField] private Button nextLevelBtn;

    private LevelSystem levelSystem;

    private void Awake()
    {
        levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        container.SetActive(false);
    }

    private void OnEnable()
    {
        levelSystem.NextLevel += OnNextLevel;
        nextLevelBtn.onClick.AddListener(OnNextLevelBtnClicked);
    }

    private void OnNextLevel()
    {
        container.SetActive(true);
    }

    private void OnDisable()
    {
        levelSystem.NextLevel -= OnNextLevel;
        nextLevelBtn.onClick.RemoveListener(OnNextLevelBtnClicked);
    }

    private void OnNextLevelBtnClicked()
    {
        levelSystem.LoadNextLevel();
        container.SetActive(false);
    }
}
