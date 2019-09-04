using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{
	// private void Start()
	// {
	// 	DontDestroyOnLoad(gameObject);
	// }

    public void LoadSpecificScene(int i)
    {
    	SceneManager.LoadScene(i);

    	Time.timeScale = 1;
    }

    public void Quit()
    {
    	Application.Quit();
    }
}
