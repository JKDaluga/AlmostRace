using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public Text winText;
    //public GameObject countDown;

    private VehicleInput[] arr;


    private void Start()
    {
        arr = FindObjectsOfType<VehicleInput>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if (pauseMenu.activeSelf == true)
            {
                turnOff(true);
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
            }
            else
            {
                turnOff(false);
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                
            }
        }
    }

    public void restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        winText.gameObject.SetActive(false);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        winText.gameObject.SetActive(false);
    }

    private void turnOff(bool stat)
    {
        foreach (VehicleInput t in arr)
        {
            t.setStatus(stat);
        }
    }
}
