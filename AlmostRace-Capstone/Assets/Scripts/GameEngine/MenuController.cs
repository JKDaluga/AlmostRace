using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
    Creator and developer of script: Leonardo Caballero
*/
public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public GameObject settingsMenu;
    public GameObject buttonPanel;
    public Text winText;
    public GameObject Countdown;
    private VehicleInput[] arrV;
    private EventSystem _eventSystem;
    private EngineAudio[] _engineSoundsToControl;
    private VehicleCollisionEffects[] _sparkSoundsToControl;
    private SphereCarController[] _sphereCarController;
    private VehicleInput[] _vehicleInput;
    private static bool isGamePaused = true;
    private static bool isGameEnded = false;

    private void Start()
    {
        isGamePaused = false;
        isGameEnded = false;
        arrV = FindObjectsOfType<VehicleInput>();
        _eventSystem = FindObjectOfType<EventSystem>();
        _engineSoundsToControl = FindObjectsOfType<EngineAudio>();
        _sparkSoundsToControl = FindObjectsOfType<VehicleCollisionEffects>();
        _sphereCarController = FindObjectsOfType<SphereCarController>();
        _vehicleInput = FindObjectsOfType<VehicleInput>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Pause") && !Countdown.activeSelf)
        {
            if (pauseMenu.activeSelf == true)
            {
                if (!Countdown.activeSelf)
                {
                    turnOnOff(true);
                }
                foreach(VehicleInput car in _vehicleInput)
                {
                    car.setStatus(true);
                }
                Time.timeScale = 1f;
                settingsMenu.SetActive(false);
                buttonPanel.SetActive(true);
                pauseMenu.SetActive(false);
                isGamePaused = false;
                UnpauseSoundHandle();
            }
            else
            {
                turnOnOff(false);
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                isGamePaused = true;
                _engineSoundsToControl = FindObjectsOfType<EngineAudio>();
                foreach (EngineAudio engineSound in _engineSoundsToControl)
                {
                    engineSound.toggleEngine(false);
                }
                _sparkSoundsToControl = FindObjectsOfType<VehicleCollisionEffects>();
                foreach (VehicleCollisionEffects sparkSound in _sparkSoundsToControl)
                {
                    sparkSound.toggleSparksSound(false);
                }
                _vehicleInput = FindObjectsOfType<VehicleInput>();
                foreach (VehicleInput car in _vehicleInput)
                {
                    car.setStatus(false);
                }
                _eventSystem.SetSelectedGameObject(null);
                _eventSystem.SetSelectedGameObject(resumeButton);
            }
        }
    }

    public void restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }


    public void close()
    {
        if (!Countdown.activeSelf)
        {
            turnOnOff(true);
        }
        foreach (VehicleInput car in _vehicleInput)
        {
            car.setStatus(true);
        }
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isGamePaused = false;
        UnpauseSoundHandle();
    }

    private void turnOnOff(bool stat)
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

    public static bool isPaused()
    {
        return isGamePaused;
    }

    public static void setIsGamePaused(bool isPaused)
    {
        isGamePaused = isPaused;
    }

    public static void GameEnds(bool isEnded)
    {
        isGameEnded = isEnded;
    }
}
