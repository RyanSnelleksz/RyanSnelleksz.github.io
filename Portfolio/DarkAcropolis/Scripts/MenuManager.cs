using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public PauseManager pause = null;
    public GameObject winCanvas = null;
    public GameObject loseCanvas = null;

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Win()
    {
        winCanvas.SetActive(true);
        pause.Paused();
    }

    public void Lose()
    {
        loseCanvas.SetActive(true);
        pause.Paused();
    }
}