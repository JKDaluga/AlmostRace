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
    private EngineAudio[] _engineSoundsToControl;
    private VehicleCollisionEffects[] _sparkSoundsToControl;
    private SphereCarController[] _sphereCarController;

    private void Start()
    {
        arrV = FindObjectsOfType<VehicleInput>();
        _engineSoundsToControl = FindObjectsOfType<EngineAudio>();
        _sparkSoundsToControl = FindObjectsOfType<VehicleCollisionEffects>();
        _sphereCarController = FindObjectsOfType<SphereCarController>();
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
                UnpauseSoundHandle();
            }
            else
            {
                turnOff(false);
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                foreach (EngineAudio engineSound in _engineSoundsToControl)
				{
					engineSound.toggleEngine(false);
				}
                foreach (VehicleCollisionEffects sparkSound in _sparkSoundsToControl)
				{
					sparkSound.toggleSparksSound(false);
				}
                foreach (SphereCarController driftSound in _sphereCarController)
				{
                    driftSound.DriftAudioSource.enabled = false;
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
        UnpauseSoundHandle();
    }

    private void turnOff(bool stat)
    {
        foreach (VehicleInput t in arrV)
        {
            t.setStatus(stat);
        }
    }

    private void UnpauseSoundHandle()
    {
        foreach (EngineAudio engineSound in _engineSoundsToControl)
        {
            engineSound.toggleEngine(true);
        }
        foreach (VehicleCollisionEffects sparkSound in _sparkSoundsToControl)
        {
            sparkSound.toggleSparksSound(true);
        }
    }
}
