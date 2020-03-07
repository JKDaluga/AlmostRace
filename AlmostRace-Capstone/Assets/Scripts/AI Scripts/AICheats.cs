using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheats : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    RaycastCar rearPlayer;
    bool playersIn;

    private RaceManager _raceManager;
    public HypeGateTimeBehavior arena;

    [Tooltip("Number of Vertices away the AI can be before being warped")]
    public float killDist = 40;
    private void Start()
    {
        arena = FindObjectOfType<HypeGateTimeBehavior>();

        _raceManager = FindObjectOfType<RaceManager>();
        if (_raceManager == null)
        {
            Debug.LogError("Race Manager not found!");
        }

        StartCoroutine(arenaWarp());
        StartCoroutine(distanceKill());
    }

    private void Update()
    {
        if (arena != null && arena.isActivated)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator arenaWarp()
    {
        while (true)
        {
            yield return null;
            if (arena != null && arena.isActivated)
            {
                playersIn = true;
                foreach (GameObject i in players)
                {
                    if (!arena.carsInRange.Contains(i))
                    {
                        playersIn = false;
                    }
                }

                if (playersIn && !arena.carsInRange.Contains(gameObject))
                {
                    GetComponent<CarHealthBehavior>().Kill();
                    GetComponent<AIBehaviour>().SwapSpline();
                }
            }
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator distanceKill()
    {
        while (true)
        {
            yield return null;
            if (_raceManager != null)
            {
                rearPlayer = _raceManager.cars[0];
                foreach (GameObject i in players)
                {
                    if (i.GetComponent<RaycastCar>().closestIndex < rearPlayer.closestIndex)
                    {
                        rearPlayer = i.GetComponent<RaycastCar>();
                    }
                }

                if (rearPlayer.closestIndex - GetComponent<AIBehaviour>().closestIndex > 40)
                {
                    GetComponent<CarHealthBehavior>().Kill();
                }
                yield return new WaitForSeconds(2.0f);
            }
        }
    }

}
