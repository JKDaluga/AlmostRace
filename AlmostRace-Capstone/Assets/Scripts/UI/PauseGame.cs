using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Leonardo Caballero
    Purpose: A Pause Screen
    *LEGACY CODE*
*/
public class PauseGame : MonoBehaviour
{
	public GameObject pausePanel;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)
		||	Input.GetButtonDown("Pause")
		||	Input.GetButtonDown("PauseMac"))
        {
        	pausePanel.SetActive(!pausePanel.activeSelf);
            AudioManager.instance.Play("Pause Sound", transform);
            if (pausePanel.activeSelf == false)
        	{
        		Time.timeScale = 1;
        	}
        	else
        	{
        		Time.timeScale = 0;
        	}
        }
    }
}
