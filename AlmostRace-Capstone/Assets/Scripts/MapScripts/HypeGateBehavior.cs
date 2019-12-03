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
    public Transform hotSpotLocation;
    private float _hypeLimitActual;
    public List<TextMeshProUGUI> displayTexts;
    private int _carsInGame;
    public List<GameObject> carsInRange;
    private GameObject _aggroSphere;
    private HotSpotBotBehavior _hotSpotBotScript;

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
    }

    public IEnumerator CheckCars()
    {
        _hotSpotBotScript.DetachFromSpline(hotSpotLocation);
        while(true)
        {
            if(carsInRange.Count < _carsInGame)
            {
                yield return null;
            }
            else if(carsInRange.Count == _carsInGame)
            {
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
                    _hypeManager.StartCoroutine(_hypeManager.EndGameCountDown(5));
                    isFinalHypeGate = false;
                    //StopAllCoroutines();
                }
                else
                {
                    gateToOpen.SetActive(false);
                    AudioManager.instance.Play("");
                    FinishDisplays();
                    _aggroSphere.SetActive(false);
                    _hotSpotBotScript.ReAttachToSpline();
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
    }

    public void FinishDisplays()
    {
        foreach (TextMeshProUGUI displayText in displayTexts)
        {
            displayText.text = "Hype: 100%";
        }
    }
}
