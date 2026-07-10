using System;
using Unity.Entities;
using UnityEngine;

public class NextLevelWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject container;

    private LevelSystem levelSystem;

    private void Awake()
    {
        levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        container.SetActive(false);
    }

    private void OnEnable()
    {
        levelSystem.NextLevel += OnNextLevel;
    }

    private void OnNextLevel()
    {
        container.SetActive(true);
    }

    private void OnDisable()
    {
        levelSystem.NextLevel -= OnNextLevel;
    }
}
