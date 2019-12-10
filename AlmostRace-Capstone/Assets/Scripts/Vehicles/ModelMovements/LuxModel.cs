using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxModel : MonoBehaviour
{
    public SphereCarController car;
    public VehicleInput ins;

    public GameObject child;

    public float turnPitch, driftPitch;

    public float pitchSpeed;

    private float maxPitch;


    private void Update()
    {
        //A set of code to pitch the car sideways based on if it's drifting/turning
        if (car.getDrifting())
        {
            maxPitch = driftPitch;
        }
        else
        {
            maxPitch = turnPitch;
        }
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(car.getTurning() * maxPitch, transform.eulerAngles.y, transform.eulerAngles.z)), pitchSpeed * Time.deltaTime);
        

        print(child.transform.localEulerAngles.z);


        /*if (Input.GetButtonDown(ins.pickupInput) && !car.GetIsBoosting())
        {
            StartCoroutine(pitchForward());
        }*/
        
    }

    public float rotateTime;

    public IEnumerator pitchForward()
    {
        float time = 0;
        while (time < rotateTime)
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, -90) * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        time = 0;


        while(time < (rotateTime * 2))
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, 45) * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
    }

}
