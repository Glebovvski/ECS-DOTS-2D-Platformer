using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Button quitGameBtn;

    private LevelSystem levelSystem;

    private void Awake()
    {
        levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
    }

    private void OnEnable()
    {   
        startGameBtn.onClick.AddListener(OnStartGameBtnClick);
        quitGameBtn.onClick.AddListener(OnQuitGameBtnClick);
    }

    private void OnQuitGameBtnClick()
    {
        Application.Quit();
    }

    private void OnStartGameBtnClick()
    {
        levelSystem.LoadScene(SceneType.Game, LoadSceneMode.Additive);
    }

    private void OnDisable()
    {
        startGameBtn.onClick.RemoveListener(OnStartGameBtnClick);
        quitGameBtn.onClick.RemoveListener(OnQuitGameBtnClick);
    }
}
