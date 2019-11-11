using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HypeGateBehavior : MonoBehaviour
{

    private float _currentHype;
    public float hypeLimit;
    public GameObject gateToOpen;
    private float _hypeLimitActual;
    public List<TextMeshProUGUI> displayTexts;
    private int _carsInGame;
    private List<GameObject> _carsInRange;

    // Start is called before the first frame update
    void Start()
    {
        _carsInGame = HypeManager.instance.vehicleList.Count;
        StartCoroutine(CheckCars());
    }

    public IEnumerator CheckCars()
    {
        while(true)
        {
            if(_carsInRange.Count < _carsInGame)
            {
                yield return null;
            }
            else if(_carsInRange.Count == _carsInGame)
            {
                StartCoroutine(TrackHype());
                StopCoroutine(CheckCars());
                yield return null;
            }
            yield return null;
        }
    }

    public IEnumerator TrackHype()
    {  
        while (true)
        {
            _currentHype = HypeManager.instance.totalHype;
            if (_currentHype < _hypeLimitActual)
            {
                yield return null;
            }
            else if(_currentHype >= _hypeLimitActual)
            {
                gateToOpen.SetActive(false);
                StopCoroutine(TrackHype());
                 yield return null;
            }
            yield return null;
        }
    }
}
