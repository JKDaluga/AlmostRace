using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{
    public GameObject pauseMenu;

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void LoadSpecificScene(string sceneName)
    {
    	SceneManager.LoadScene(sceneName);
    	Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
    	Application.Quit();
    }
}
