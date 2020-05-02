using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

    /*
    Author: Eddie B & Jake Velicer
    Purpose: Contains the behavior for the hot spot bot. This includes its behavior for setting
    its position back on the spline after it has been dropped, if it is being held, a function
    for finding the nearest point on the hot spot bot spline, and the hot spot bot behavior in arenas.
    Script that handles how the Arena Gates behave in each arena. Tracks vehicles in arena, closes the door to the arena, counts down the timer, and opens up the area after it is done
    */

public class HypeGateTimeBehavior : MonoBehaviour
{
    private HypeManager _hypeManager;
    private RaceManager _raceManager;
    public float startTime = 60;
    private float _currentTime;
    private float _displayTime;

    [Tooltip("The gameobject to remove once time is counted down")]
    public GameObject[] gateToOpen;
    public GameObject[] gateToClose;
    public Transform[] spawnPoints;
    public List<TextMeshProUGUI> displayTexts;
    private int _carsInGame;
    public List<GameObject> carsInRange;
    private GameObject _aggroSphere;
    public GameObject eventPanel;
    public GameObject arenaActiveText;
    public GameObject arenaEndText;
    public bool isActivated;

    public SplineSwapTrigger aiMan;

    // Start is called before the first frame update
    void Start()
    {
        _raceManager = FindObjectOfType<RaceManager>();
        aiMan = _raceManager.aiMan;
        if (_raceManager != null)
        {
            _carsInGame = _raceManager.cars.Length;
        }
        else
        {
            Debug.LogError("Race Manager not found!");
        }
        _hypeManager = FindObjectOfType<HypeManager>();
        if (_hypeManager == null)
        {
            Debug.LogError("Hype Manager not found!");
        }
    }


    public IEnumerator CheckCars()
    {
        if (_raceManager != null)
        {
            _carsInGame = _raceManager.cars.Length;
        }
        else
        {
            Debug.LogError("Race Manager not found!");
        }

        float playerPercentage;
        while (true)
        {
            playerPercentage = ((float)carsInRange.Count / (float)_carsInGame);
            if (carsInRange.Count < _carsInGame)
            {
                foreach (RaycastCar car in _raceManager.cars)
                {
                    if (car != null && car.gameObject.GetComponent<VehicleInput>() != null)
                    {
                        car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.ActivateArenaHypeDisplay();
                        car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.arenaHypeText.text = "Arena Locked";
                        car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.lockBottomFill.fillAmount = playerPercentage;
                        car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.TriggerTimePopup();
                    }

                }
                yield return null;
            }
            else if (carsInRange.Count == _carsInGame)
            {
                eventPanel.SetActive(true);
                arenaActiveText.SetActive(true);

                Invoke("DisableEvents", 3);

                foreach (RaycastCar car in _raceManager.cars)
                {
                    if (car.gameObject.GetComponent<VehicleInput>() != null)
                    {
                        car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.lockBottomFill.fillAmount = 1;
                        car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.arenaHypeText.text = "Arena Time";
                    }
                }

                Invoke("CloseArena", 1);

                StopAllCoroutines();

                _raceManager.inArena = true;
                StartCoroutine(TrackHype());
                yield return null;

            }
            yield return null;
        }
    }

    public void CloseArena()
    {
        foreach (GameObject gate in gateToClose)
        {
            gate.SetActive(true);
            gate.GetComponent<Animator>().Play("Close");
        }
    }

    public void OpenArena()
    {
        foreach (GameObject gate in gateToOpen)
        {
            gate.GetComponent<Animator>().Play("Open");
        }
    }

    public void InitializeHypeGate(GameObject aggroSphere)
    {
        _aggroSphere = aggroSphere;
        _currentTime = startTime; //initial get of total hype, for display purposes
        _displayTime = _currentTime;
    }

    public IEnumerator TrackHype()
    {
        while (true)
        {
            if (_currentTime > 0)
            {
                _currentTime--;
                UpdateDisplays();
                yield return new WaitForSeconds(1);
            }
            else if (_currentTime <= 0)
            {
                    OpenArena();
                    AudioManager.instance.PlayWithoutSpatial("ArenaCleared");
                    FinishDisplays();
                    _aggroSphere.SetActive(false);
                    eventPanel.SetActive(true);
                    arenaEndText.SetActive(true);

                    eventPanel.SetActive(true);
                    arenaEndText.SetActive(true);

                    Invoke("DisableEvents", 3);

                _raceManager.time = 0;
                _raceManager.inArena = false;
             
                //StopCoroutine(TrackHype());
                StopAllCoroutines();
                isActivated = false;

                foreach (RaycastCar i in _raceManager.cars)
                {
                    i.inArena = false;
                    i.activeSpline++;
                    if (!i.GetComponent<VehicleInput>())
                    {
                        i.GetComponent<AICheats>().startCheating();
                        aiMan.updateAI(i.GetComponent<AIBehaviour>());
                    }
                }
                yield return null;
            }
            yield return null;
        }
    }

    public void UpdateDisplays()
    {
        float minutes = Mathf.Floor(_currentTime / 60);
        float seconds = (_currentTime % 60);

        foreach (TextMeshProUGUI displayText in displayTexts)
        {
            displayText.text = string.Format("{0}:{1}", minutes.ToString("0"), seconds.ToString("00"));
        }

        foreach (RaycastCar car in _raceManager.cars)
        {
            if (car.gameObject.GetComponent<VehicleInput>() != null)
            {
                car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.lockTopFill.fillAmount = _currentTime / startTime;
                car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.SetArenaHypeDisplayNumber(minutes, seconds);
            }
        }
    }

    public void FinishDisplays()
    {
        foreach (TextMeshProUGUI displayText in displayTexts)
        {
            displayText.text = "0:00";
        }
        foreach (RaycastCar car in _raceManager.cars)
        {
            if (car.gameObject.GetComponent<VehicleInput>() != null)
            {
                car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.SetArenaHypeDisplayNumber(0, 0);
                car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.UnlockArena();
            }

        }
        Invoke("DisableDisplays", 1);
    }

    public void DisableDisplays()
    {
        foreach (RaycastCar car in _raceManager.cars)
        {
            if (car.gameObject.GetComponent<VehicleInput>() != null)
            {
                car.gameObject.GetComponent<RaycastCar>().playerUIManagerScript.DeactivateArenaHypeDisplay();
            }
        }
    }

    public void DisableEvents()
    {
        eventPanel.SetActive(false);
        arenaActiveText.SetActive(false);
        arenaEndText.SetActive(false);
    }
}
