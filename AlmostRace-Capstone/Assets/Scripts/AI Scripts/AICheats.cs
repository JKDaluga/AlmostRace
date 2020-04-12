using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheats : MonoBehaviour
{
    RaycastCar rearPlayer;
    public bool playersIn;

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
        if (arena != null && arena.carsInRange.Count == 4)
        {
            StopAllCoroutines();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<CarHealthBehavior>().AICheatKill();
        }
    }

    public void startCheating()
    {
        StartCoroutine(distanceKill());
    }

    IEnumerator arenaWarp()
    {
        while (true)
        {
            yield return null;
            if (arena != null && arena.isActivated)
            {
                playersIn = true;
                foreach (RaycastCar i in _raceManager.playerCars)
                {
                    if (!arena.carsInRange.Contains(i.gameObject))
                    {
                        playersIn = false;
                    }
                }

                if (playersIn && !arena.carsInRange.Contains(gameObject))
                {
                    print("Arena Warping");
                    GetComponent<CarHealthBehavior>().AICheatKill();
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
                foreach (RaycastCar i in _raceManager.playerCars)
                {
                    if (i.closestIndex < rearPlayer.closestIndex)
                    {
                        rearPlayer = i.GetComponent<RaycastCar>();
                    }
                }

                if (rearPlayer.closestIndex - GetComponent<AIBehaviour>().closestIndex > 40)
                {
                    GetComponent<CarHealthBehavior>().AICheatKill();
                }
                yield return new WaitForSeconds(2.0f);
            }
        }
    }

    public RaycastCar getRearPlayer()
    {
        return rearPlayer;
    }

    public bool getPlayersIn()
    {
        return playersIn;
    }

}
