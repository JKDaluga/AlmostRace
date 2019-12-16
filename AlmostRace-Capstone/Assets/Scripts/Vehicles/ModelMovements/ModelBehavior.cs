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
    public SphereCarController car;
    public VehicleInput ins;

    public int carType;

    public GameObject leftDriftParticles, rightDriftParticles;

    public float turnPitch, driftPitch;

    public float pitchSpeed;

    private float maxPitch;

    public float voidWaspSpin, voidSpinDuration;

    bool voidWaspBoost;

    int dir;
    private void Update()
    {

        if (Input.GetButtonDown(ins.brake))
        {
            if (car.getTurning() > 0)
            {
                dir = 1;
            }
            else dir = -1;
        }
        //A set of code to pitch the car sideways based on if it's drifting/turning
        if (car.getDrifting())
        {
            maxPitch = driftPitch;

        }
        else
        {
            maxPitch = turnPitch;

            //Turns off drift particles if not drifting
            if (leftDriftParticles != null)
            {
                leftDriftParticles.SetActive(false);
            }
            if (rightDriftParticles != null)
            {
                rightDriftParticles.SetActive(false);
            }
        }
        //Check for void wasp boost, because it overrides the pitch of the vehicle
        if (!voidWaspBoost)
        {
            //If the car isn't drifting, set the x rotation based on turning input
            if (!car.getDrifting())
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(car.getTurning() * maxPitch, transform.eulerAngles.y, transform.eulerAngles.z)), pitchSpeed * Time.deltaTime);
            }
            else
            {   
                //If drifting, locks vehicle rotation based on the direction of the drift. Pitches as normal otherwise
                if (Mathf.Sign(car.getTurning()) == Mathf.Sign(dir))
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(car.getTurning() * maxPitch, transform.eulerAngles.y, transform.eulerAngles.z)), pitchSpeed * Time.deltaTime);
                }
                else
                {
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), pitchSpeed * Time.deltaTime);
                }
                
                    //Sets appropriate drifting particles based on direction
                    if(dir > 0)
                    {
                        if(rightDriftParticles != null)
                            rightDriftParticles.SetActive(true);
                    } else
                    {
                        if (leftDriftParticles != null)
                            leftDriftParticles.SetActive(true);
                    }
            }

            if (car.getTurning() == 0)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), pitchSpeed * Time.deltaTime);
            }
        }


        //Checks the car type and uses the correct boost visual for each.
        if (Input.GetButtonDown(ins.pickupInput) && !car.GetIsBoosting())
        {
            switch (carType)
            {
                case 0:
                    StartCoroutine(pitchForward());
                    break;
                case 1:
                    StartCoroutine(rotate());
                    break;
            }
        }
        
    }

    public float rotateTime;

    //Routine allowing void wasp to do spins while boosting
    public IEnumerator rotate()
    {
        voidWaspBoost = true;
        float time = 0;
        while(time < voidSpinDuration)
        {
            transform.Rotate(new Vector3(voidWaspSpin, 0.0f, 0.0f) * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 90, 0);
        voidWaspBoost = false;
    }

    //Adds small shake effect to Lux boosting
    public IEnumerator pitchForward()
    {
        float time = 0;
        while (time < rotateTime)
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, -40) * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        
        int wobble = 0;
        while (wobble < 6)
        {
            time = 0;
            if (wobble % 2 == 0)
            {
                while (time < .05f)
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, 25) * Time.deltaTime);
                    time += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (time < .05f)
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, -25) * Time.deltaTime);
                    time += Time.deltaTime;
                    yield return null;
                }
            }
            wobble++;
            yield return null;
        }


        time = 0;


        while(time < (rotateTime * 2))
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, 20) * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
    }

}
