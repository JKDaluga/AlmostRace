using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley, Script that manages model movements when different actions are taken
 * Script accesses the spherecarcontroller to get vehicle states and inputs to play particle effects 
 * and pitch the vehicle an appropriate amount.
 */

public class ModelBehavior : MonoBehaviour
{
    public RaycastCar car;
    public VehicleInput ins;

    public int carType;

    public GameObject leftDriftParticles, rightDriftParticles;

    public float turnPitch;

    public float pitchSpeed;

    private float maxPitch;

    int dir;
    private void Start()
    {
        maxPitch = turnPitch;
    }
    private void Update()
    {

        if (Input.GetAxis(ins.horizontal) > 0)
        {
            dir = 1;
        }
        else dir = -1;

        //A set of code to pitch the car sideways based on if it's drifting/turning

        //Turns off drift particles if not drifting
        if (leftDriftParticles != null)
        {
            leftDriftParticles.SetActive(false);
        }
        if (rightDriftParticles != null)
        {
            rightDriftParticles.SetActive(false);
        }

        //If drifting, locks vehicle rotation based on the direction of the drift. Pitches as normal otherwise
  
        if (Mathf.Sign(Input.GetAxis(ins.horizontal)) == Mathf.Sign(dir))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(Input.GetAxis(ins.horizontal) * maxPitch, transform.eulerAngles.y, transform.eulerAngles.z)), pitchSpeed * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), pitchSpeed * Time.deltaTime);
        }

        //Sets appropriate drifting particles based on direction
        if (dir > 0)
        {
            if (rightDriftParticles != null)
                rightDriftParticles.SetActive(true);
        }
        else
        {
            if (leftDriftParticles != null)
                leftDriftParticles.SetActive(true);
        }

        if (Input.GetAxis(ins.horizontal) == 0)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), pitchSpeed * Time.deltaTime);
        }
   
    }

}
