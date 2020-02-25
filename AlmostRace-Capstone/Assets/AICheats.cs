using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AICheats : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    RaycastCar rearPlayer;

    public HypeGateBehavior arena;

    [Tooltip("Number of Vertices away the AI can be before being warped")]
    public float killDist = 40;

    private void Start()
    {
        arena = FindObjectOfType<HypeGateBehavior>();
        RaycastCar[] temp = FindObjectsOfType<RaycastCar>();

        foreach(RaycastCar i in temp)
        {
            if (i.GetComponent<VehicleInput>())
            {
                players.Add(i.gameObject);
            }
        }
        rearPlayer = players[0].GetComponent<RaycastCar>();

        InvokeRepeating("distanceKill", 0, 2);
    }

    
    private void arenaWarp()
    {
        if(arena.carsInRange.Intersect(players) == players && !arena.carsInRange.Contains(gameObject))
        {
            GetComponent<CarHealthBehavior>().Kill();
            GetComponent<AIBehaviour>().SwapSpline();
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
