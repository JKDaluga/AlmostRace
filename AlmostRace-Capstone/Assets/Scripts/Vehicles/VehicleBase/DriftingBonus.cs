using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Author: Robyn Riley
 Purpose: Gives player a bonus to hype for drifting safely, giving larger bonuses for longer drifts

        V V V V IMPORTANT V V V V
     
This script is currently on the Collider object of the cars. Once we implement collisions based off the model of the car,
     
this script will need to be moved onto the car's Model field instead. Other aspects of the script will likely be changed at that time.
*/

public class DriftingBonus : MonoBehaviour
{
    //Boolean values to check if the drift has just started 
    //and if the drift is ended safely
    public bool safeDrift, startDrift;


    //values for base hype and the multiplier that will grow over time
    public float hypeToAdd;
    [SerializeField] float hypeMultiplier;
    float maxMultiplier;

    //Holder for the multiplied hype amount
    float finalHype;

    public SphereCarController car;

    private void Start()
    {
        startDrift = true;
    }

    private void FixedUpdate()
    {
        //If the car begins drifting, set the booleans and reset the multiplier
        if (car.getDrifting() && startDrift)
        {
            safeDrift = true;
            startDrift = false;
            hypeMultiplier = 0;
        }


        //While the car is drifting safely, add to the multiplier
        if (safeDrift)
        {
            if(hypeMultiplier < 5)
            {
                hypeMultiplier += Time.deltaTime;
            }
        }

        //when the car stops drifting, check if the drift was safe,
        //add hype if the drift was safe, and reset all other values
        if(!car.getDrifting())
        {
            startDrift = true;
            if (safeDrift)
            {
                finalHype =Mathf.Floor( hypeToAdd * hypeMultiplier);
                car.gameObject.GetComponent<VehicleHypeBehavior>().AddHype(finalHype);
                safeDrift = false;
            }
        }
    }


    //If the player collides with anything, they lose the safedrifting bonus
    private void OnCollisionEnter(Collision collision)
    {
        safeDrift = false;
    }
}
