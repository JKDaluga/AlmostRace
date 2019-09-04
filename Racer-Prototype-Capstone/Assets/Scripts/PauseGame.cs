using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
	public GameObject pausePanel;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)
		||	Input.GetButtonDown("PauseP1")
		||	Input.GetButtonDown("PauseP2"))
        {
        	pausePanel.SetActive(!pausePanel.activeSelf);

        	if(pausePanel.activeSelf == false)
        	{
        		Time.timeScale = 1;
        	}
        	else
        	{
        		Time.timeScale = 0;
        	}
        }

        if(pausePanel.activeSelf == false)
        {
        	Time.timeScale = 1;
        }
    }
}
