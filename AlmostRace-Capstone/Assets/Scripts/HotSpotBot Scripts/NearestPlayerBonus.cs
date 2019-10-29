using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Robyn Riley
    Purpose: Checks for which player is nearest to the HotSpotBot
    And gives them a small hype bonus every few seconds.
    */

public class NearestPlayerBonus : MonoBehaviour
{
    GameObject[] players;

    float[] distances;

    int closest;

    [SerializeField] private float gainTime;

    public bool beingHeld;

    private void Start()
    {
        //Finds all existing objects with the car controller script, then saves the objects into an array
        SphereCarController[] temp = FindObjectsOfType<SphereCarController>();
        players = new GameObject[temp.Length];
        distances = new float[players.Length];
        for(int i = 0; i < temp.Length; i++)
        {
            players[i] = temp[i].gameObject;
        }


        //Begins invoking the bonus hype immediately
        InvokeRepeating("AddBonus", gainTime, gainTime);
    }


    private void FixedUpdate()
    {
        //Replace beingHeld with GetComponent<HotSpotBotBehavior>.getBeingHeld() when that is implemented
        if (!beingHeld)
        {
            //If the method has stopped, begin calling it again
            if (!IsInvoking("AddBonus"))
            {
                InvokeRepeating("AddBonus", gainTime, gainTime);
            }
            //Saves the distance between each player and the HotSpotBot
            for (int i = 0; i < players.Length; i++)
            {
                distances[i] = Vector3.Distance(transform.position, players[i].transform.position);
            }


            //Saves which member of the array is closest to the HotSpotBot
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[closest] > distances[i])
                {
                    closest = i;
                }
            }
        }
        else
        {
            CancelInvoke();
        }
    }

    void AddBonus()
    {
        players[closest].GetComponent<VehicleHypeBehavior>().AddHype(5);
    }

}
