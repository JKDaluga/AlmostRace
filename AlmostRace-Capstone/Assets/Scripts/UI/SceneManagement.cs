using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void LoadSpecificScene(string sceneName)
    {
    	SceneManager.LoadScene(sceneName);

    	Time.timeScale = 1;
    }

    public void Quit()
    {
    	Application.Quit();
    }
}
