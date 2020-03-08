﻿using System.Collections;
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
    public Text winText;
    public GameObject Countdown;
    private VehicleInput[] arrV;
    private EventSystem _eventSystem;
    private EngineAudio[] _engineSoundsToControl;
    private VehicleCollisionEffects[] _sparkSoundsToControl;
    private SphereCarController[] _sphereCarController;
    private VehicleInput[] _vehicleInput;

    private void Start()
    {
        arrV = FindObjectsOfType<VehicleInput>();
        _eventSystem = FindObjectOfType<EventSystem>();
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
                if (!Countdown.activeSelf)
                {
                    turnOnOff(true);
                }
                _vehicleInput = FindObjectsOfType<VehicleInput>();
                foreach(VehicleInput car in _vehicleInput)
                {
                    car.setStatus(true);
                }
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                UnpauseSoundHandle();
            }
            else
            {
                turnOnOff(false);
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
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        if (Countdown.activeSelf == false)
        {
            turnOnOff(true);
        }
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
}
