using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 
    Eddie B

    Script that handles how the Hype Gates behave in each arena. More specific functionality will probably
    need to be added once the other arenas are finalized.

     */

public class HypeGateBehavior : MonoBehaviour
{
    public bool isFinalHypeGate = false;
    private HypeManager _hypeManager;
    private float _currentHype; // tracks current total hype of cars inside of arena
    private float _displayHype; //used in showing hype in text

    [Tooltip("The amount of Hype players need to generate in this arena to proceed")]
    public float hypeLimit;

    [Tooltip("The gameobject to remove once hypeLimit is reached")]
    public GameObject gateToOpen;//might be changed per hype gate for specific behavior
    public GameObject gateToClose;
    public Transform hotSpotLocation;
    public Transform[] spawnPoints;
    private float _hypeLimitActual;
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
        if(_hypeManager != null)
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
        _hotSpotBotScript.DetachFromSpline(this);
        if (_hypeManager != null)
        {
            _carsInGame = _hypeManager.vehicleList.Count;
        }
        else
        {
            Debug.LogError("Hype Manager not found!");
        }
        float playerPercentage;
        while(true)
        {
            playerPercentage = ((float)carsInRange.Count / (float)_carsInGame);
            //Debug.Log((float)(carsInRange.Count / _carsInGame));
            if (carsInRange.Count < _carsInGame)
            {
                foreach(GameObject car in _hypeManager.vehicleList)
                {
                    car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.ActivateArenaHypeDisplay();
                    car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.arenaHypeText.text = "Arena Locked";
                    car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.lockBottomFill.fillAmount = playerPercentage;
                    eventPanel.SetActive(true);
                    arenaActiveText.SetActive(true);

                    Invoke("DisableEvents", 3);
                }
                yield return null;
            }
            else if(carsInRange.Count == _carsInGame)
            {
                foreach (GameObject car in _hypeManager.vehicleList)
                {
                    car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.lockBottomFill.fillAmount = 1;
                    

                    car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.arenaHypeText.text = "Arena Hype";
                }

                gateToClose.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(TrackHype());
                _hotSpotBotScript.SetVehiclesIn(true);
                yield return null;

            }
            yield return null;
        }
    }
    
    public void InitializeHypeGate(GameObject aggroSphere)
    {
        _aggroSphere = aggroSphere;
        _currentHype = _hypeManager.totalHype; //initial get of total hype, for display purposes
        _displayHype = _currentHype;
        _hypeLimitActual = _currentHype + hypeLimit;
    }

    public IEnumerator TrackHype()
    {    
        while (true)
        {
            _currentHype = _hypeManager.totalHype;
            if (_currentHype < _hypeLimitActual)
            {
                UpdateDisplays();
                yield return null;
            }
            else if(_currentHype >= _hypeLimitActual)
            {
                if(isFinalHypeGate)
                {
                    FinishDisplays();
                    _hypeManager.StartCoroutine(_hypeManager.EndGameCountDown(5));
                    isFinalHypeGate = false;
                    //StopAllCoroutines();
                }
                else
                {
                    gateToOpen.SetActive(false);
                    AudioManager.instance.Play("ArenaCleared");
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
        foreach(TextMeshProUGUI displayText in displayTexts)
        {
            displayText.text = "Hype: " + ((_currentHype - _displayHype) /  hypeLimit * 100).ToString("F0") + "%";
        }
        foreach (GameObject car in _hypeManager.vehicleList)
        {
            car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.lockTopFill.fillAmount = (_currentHype - _displayHype) / hypeLimit; 
            car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.SetArenaHypeDisplayNumber(((_currentHype - _displayHype) / hypeLimit * 100));
        }
    }

    public void FinishDisplays()
    {
        foreach (TextMeshProUGUI displayText in displayTexts)
        {
            displayText.text = "Hype: 100%";
        }
        foreach (GameObject car in _hypeManager.vehicleList)
        {
            car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.SetArenaHypeDisplayNumber(100);
            car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.UnlockArena();

        }
        Invoke("DisableDisplays", 1);
    }

    public void DisableDisplays()
    {
        foreach (GameObject car in _hypeManager.vehicleList)
        {
            car.gameObject.GetComponent<VehicleHypeBehavior>().playerUIManagerScript.DeactivateArenaHypeDisplay();
        }
    }

    public void DisableEvents()
    {
        eventPanel.SetActive(false);
        arenaActiveText.SetActive(false);
        arenaEndText.SetActive(false);
    }
}
