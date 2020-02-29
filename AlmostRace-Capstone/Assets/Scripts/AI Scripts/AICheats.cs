using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheats : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    RaycastCar rearPlayer;
    RaycastCar[] allCars;
    bool playersIn;

    public HypeGateBehavior arena;

    [Tooltip("Number of Vertices away the AI can be before being warped")]
    public float killDist = 40;

    private void Start()
    {
        arena = FindObjectOfType<HypeGateBehavior>();
         allCars = FindObjectsOfType<RaycastCar>();

        foreach(RaycastCar i in allCars)
        {
            if (i.GetComponent<VehicleInput>())
            {
                players.Add(i.gameObject);
            }
        }
        rearPlayer = players[0].GetComponent<RaycastCar>();

        InvokeRepeating("distanceKill", 0, 2);
        InvokeRepeating("arenaWarp", 0, 2);
    }

    
    private void arenaWarp()
    {
        playersIn = true;
        foreach(GameObject i in players)
        {
            if (!arena.carsInRange.Contains(i))
            {
                playersIn = false;
            }
        }

        if(playersIn && arena.carsInRange.Count < allCars.Length && !arena.isActiveAndEnabled)
        {
            GetComponent<CarHealthBehavior>().Kill();
            GetComponent<AIBehaviour>().SwapSpline();
            print("arena Warp");
        }
    }

    private void distanceKill()
    {
        foreach(GameObject i in players)
        {
            if(i.GetComponent<RaycastCar>().closestIndex < rearPlayer.closestIndex)
            {
                rearPlayer = i.GetComponent<RaycastCar>();
            }
        }

        if(rearPlayer.closestIndex - GetComponent<AIBehaviour>().closestIndex > 40)
        {
            GetComponent<CarHealthBehavior>().Kill();
        }
    }

}
