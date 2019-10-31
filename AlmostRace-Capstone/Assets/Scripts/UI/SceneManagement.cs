using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneManagement : MonoBehaviour
{
    public GameObject pauseMenu;
    public Text winText;

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (winText != null) winText.gameObject.SetActive(false);
    }

    public void LoadSpecificScene(string sceneName)
    {
    	SceneManager.LoadScene(sceneName);
    	Time.timeScale = 1f;
        if(pauseMenu != null) pauseMenu.SetActive(false);
        if (winText != null) winText.gameObject.SetActive(false);
    }

    public void Quit()
    {
    	Application.Quit();
    }
}
