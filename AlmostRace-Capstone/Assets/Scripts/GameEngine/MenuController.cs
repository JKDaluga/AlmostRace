using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    Creator and developer of script: Leonardo Caballero
*/
public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public Text winText;
    public GameObject Countdown;
    private VehicleInput[] arrV;
    private EngineAudio[] engineSoundsToControl;
    private VehicleCollisionEffects[] sparkSoundsToControl;

    private void Start()
    {
        arrV = FindObjectsOfType<VehicleInput>();
        engineSoundsToControl = FindObjectsOfType<EngineAudio>();
        sparkSoundsToControl = FindObjectsOfType<VehicleCollisionEffects>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if (pauseMenu.activeSelf == true)
            {
                if (Countdown.activeSelf == true)
                {
                    //doNothing
                }
                else
                {
                    turnOff(true);
                }
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                foreach (EngineAudio engineSound in engineSoundsToControl)
				{
					engineSound.toggleEngine(true);
				}
                foreach (VehicleCollisionEffects sparkSound in sparkSoundsToControl)
				{
					sparkSound.toggleSparksSound(true);
				}
            }
            else
            {
                turnOff(false);
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                foreach (EngineAudio engineSound in engineSoundsToControl)
				{
					engineSound.toggleEngine(false);
				}
                foreach (VehicleCollisionEffects sparkSound in sparkSoundsToControl)
				{
					sparkSound.toggleSparksSound(false);
				}
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


    public void close()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        if (Countdown.activeSelf == false)
        {
            turnOff(true);
        }
        foreach (EngineAudio engineSound in engineSoundsToControl)
        {
            engineSound.toggleEngine(true);
        }
        foreach (VehicleCollisionEffects sparkSound in sparkSoundsToControl)
        {
            sparkSound.toggleSparksSound(true);
        }
    }

    private void turnOff(bool stat)
    {
        foreach (VehicleInput t in arrV)
        {
            t.setStatus(stat);
        }
    }
}
