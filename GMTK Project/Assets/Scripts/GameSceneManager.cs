using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void GoToGameplay()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}

