using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Button quitGameBtn;

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
        SceneManager.LoadScene((int)SceneType.Game);
    }

    private void OnDisable()
    {
        startGameBtn.onClick.RemoveListener(OnStartGameBtnClick);
        quitGameBtn.onClick.RemoveListener(OnQuitGameBtnClick);
    }
}
