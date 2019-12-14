using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if(leftDriftParticles != null)
            leftDriftParticles.SetActive(false);
            if(rightDriftParticles != null)
            rightDriftParticles.SetActive(false);
        }
        if (!voidWaspBoost)
        {
            if (!car.getDrifting())
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(car.getTurning() * maxPitch, transform.eulerAngles.y, transform.eulerAngles.z)), pitchSpeed * Time.deltaTime);
            }
            else
            {
                //print(Mathf.Sign(car.getTurning()) + "==" + Mathf.Sign(dir));
                if (Mathf.Sign(car.getTurning()) == Mathf.Sign(dir))
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(car.getTurning() * maxPitch, transform.eulerAngles.y, transform.eulerAngles.z)), pitchSpeed * Time.deltaTime);
                }
                else
                {
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), pitchSpeed * Time.deltaTime);
                }

                if(Mathf.Abs(transform.eulerAngles.x) > (maxPitch * .9f))
                {
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
            }

            if (car.getTurning() == 0)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 90, 0)), pitchSpeed * Time.deltaTime);
            }
        }



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
