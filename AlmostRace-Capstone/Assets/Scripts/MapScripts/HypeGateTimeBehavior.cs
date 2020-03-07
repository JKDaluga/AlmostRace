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
    public bool isFinalArenaGate = false;
    private HypeManager _hypeManager;
    public float startTime = 60;
    private float _currentTime;
    private float _displayTime;

    [Tooltip("The gameobject to remove once time is counted down")]
    public GameObject gateToOpen;
    public GameObject gateToClose;
    public Transform hotSpotLocation;
    public Transform[] spawnPoints;
    public List<TextMeshProUGUI> displayTexts;
    private int _carsInGame;
    public List<GameObject> carsInRange;
    private GameObject _aggroSphere;
    private HotSpotBotBehavior _hotSpotBotScript;
    public GameObject eventPanel;
    public GameObject playerWinText;
    public GameObject arenaActiveText;
    public GameObject arenaEndText;

    // Start is called before the first frame update
    void Start()
    {
        _hypeManager = FindObjectOfType<HypeManager>();
        _hotSpotBotScript = GameObject.Find("HotSpotBot").GetComponent<HotSpotBotBehavior>();
        if (_hypeManager != null)
        {
            _carsInGame = _hypeManager.vehicleList.Count;
        }
        else
        {
            Debug.LogError("Hype Manager not found!");
        }
        gateToClose.SetActive(false);
    }

    public IEnumerator CheckCars()
    {
        if (_hypeManager != null)
        {
            _carsInGame = _hypeManager.vehicleList.Count;
        }
        else
        {
            Debug.LogError("Hype Manager not found!");
        }
        float playerPercentage;
        while (true)
        {
            playerPercentage = ((float)carsInRange.Count / (float)_carsInGame);
            //Debug.Log((float)(carsInRange.Count / _carsInGame));
            if (carsInRange.Count < _carsInGame)
            {
                foreach (GameObject car in _hypeManager.vehicleList)
                {
                    if (car.gameObject.GetComponent<VehicleInput>() != null)
                    {
                        car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.ActivateArenaHypeDisplay();
                        car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.arenaHypeText.text = "Arena Locked";
                        car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.lockBottomFill.fillAmount = playerPercentage;
                    }

                }
                yield return null;
            }
            else if (carsInRange.Count == _carsInGame)
            {
                eventPanel.SetActive(true);
                arenaActiveText.SetActive(true);

                Invoke("DisableEvents", 3);

                foreach (GameObject car in _hypeManager.vehicleList)
                {
                    if (car.gameObject.GetComponent<VehicleInput>() != null)
                    {
                        car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.lockBottomFill.fillAmount = 1;
                        car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.arenaHypeText.text = "Arena Hype";
                    }
                }

                Invoke("CloseGate", 1);


                StopAllCoroutines();
                StartCoroutine(TrackHype());
                _hotSpotBotScript.SetVehiclesIn(true);
                yield return null;

            }
            yield return null;
        }
    }

    public void CloseGate()
    {
        gateToClose.SetActive(true);
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
                if (isFinalArenaGate)
                {
                    FinishDisplays();
                    _hypeManager.StartCoroutine(_hypeManager.EndGameCountDown(5));
                    isFinalArenaGate = false;
                    //StopAllCoroutines();
                }
                else
                {
                    gateToOpen.SetActive(false);
                    AudioManager.instance.PlayWithoutSpatial("ArenaCleared");
                    FinishDisplays();
                    _aggroSphere.SetActive(false);
                    _hotSpotBotScript.ReAttachToSpline();
                    eventPanel.SetActive(true);
                    arenaEndText.SetActive(true);

                    eventPanel.SetActive(true);
                    arenaEndText.SetActive(true);

                    Invoke("DisableEvents", 3);
                }
                //StopCoroutine(TrackHype());
                StopAllCoroutines();
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

        foreach (GameObject car in _hypeManager.vehicleList)
        {
            if (car.gameObject.GetComponent<VehicleInput>() != null)
            {
                car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.lockTopFill.fillAmount = _currentTime / startTime;
                car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.SetArenaHypeDisplayNumber(minutes, seconds);
            }
        }
    }

    public void FinishDisplays()
    {
        foreach (TextMeshProUGUI displayText in displayTexts)
        {
            displayText.text = "100%";
        }
        foreach (GameObject car in _hypeManager.vehicleList)
        {
            if (car.gameObject.GetComponent<VehicleInput>() != null)
            {
                car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.SetArenaHypeDisplayNumber(0, 0);
                car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.UnlockArena();
            }

        }
        Invoke("DisableDisplays", 1);
    }

    public void DisableDisplays()
    {
        foreach (GameObject car in _hypeManager.vehicleList)
        {
            if (car.gameObject.GetComponent<VehicleInput>() != null)
            {
                car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.DeactivateArenaHypeDisplay();
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
