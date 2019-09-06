using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]

public class Wheel
{
    //The collider this wheel uses
    public WheelCollider collider;

    //Whether this wheel should be powered by the engine
    public bool powered;

    //Whether this wheel is steerable
    public bool steerable;

    //Whether this wheel can apply brakes
    public bool hasBrakes;
}

public class Vehicle : MonoBehaviour
{
    [SerializeField] Wheel[] wheels = { };

    //The settings used for controlling the wheels:

    //Maximum motor torque
    [SerializeField] float motorTorque = 1000;

    //Maximum brake torque
    [SerializeField] float brakeTorque = 2000;

    //Maximum steering angle
    [SerializeField] float steeringAngle = 45;
    [SerializeField] float trueSteeringAngle;


    [SerializeField] WheelFrictionCurve moving;
    [SerializeField] WheelFrictionCurve atRest;

    [SerializeField] float topSpeed = 70;

    private void Start()
    {
        for (int wheelNum = 0; wheelNum < wheels.Length; wheelNum++)
        {
            wheels[wheelNum].collider.ConfigureVehicleSubsteps(5, 20, 25);
            if (wheels[wheelNum].steerable)
            {

                moving = atRest = wheels[wheelNum].collider.sidewaysFriction;
            }
        }
        trueSteeringAngle = steeringAngle;
        moving.stiffness = 100;
    }
    // Update is called once per frame
    void Update()
    {
        //If the vertical axis is positie, apply motor torque and no
        //brake torque. If it's negative, apply brake torque and no
        //motor torque

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        var vertical = Input.GetAxis("Vertical");

        float motorTorqueToApply;
        float brakeTorqueToApply;

        if(vertical >= 0 && GetComponentInParent<Rigidbody>().velocity.magnitude < topSpeed) {
            

            motorTorqueToApply = vertical * motorTorque;
            brakeTorqueToApply = 0;

        } else {
            //If the vertical axis is negative, cut the engine and step
            //on the brakes.

            //We use mathf.abs here to ensure that we use the positive 
            //value of "vertical" (because applying negative braking
            //torque would lead to weirdness).
            motorTorqueToApply = 0;
            brakeTorqueToApply = Mathf.Abs(vertical) * brakeTorque;
        }

        //Sale the maximum steering angle by the horizontal axis.
        var currentSteeringAngle = Input.GetAxis("Horizontal") * trueSteeringAngle;

        Debug.Log(GetComponentInParent<Rigidbody>().velocity.magnitude);
        

        //Update all wheels

        //Using a foor loop, rather than a foreach loop, because foreach
        //loops allocate temporary memory, which is turned into garbage
        //at the end of the frame. We want to minimize garbage, because
        //the more garbage that gets generated, the more often the 
        //garbage collector has to run, which causes performance problems.
        for (int wheelNum = 0; wheelNum < wheels.Length; wheelNum++)
        {
            var wheel = wheels[wheelNum];

            //If a wheel is powered, it updates its motor torque
            if (wheel.powered)
            {
                wheel.collider.motorTorque = motorTorqueToApply;
            }
            //If a wheel is steerable, it updates its steer angle
            if (wheel.steerable)
            {
                wheel.collider.steerAngle = currentSteeringAngle;
              
            }
            //If a wheel has brakes, it updates its brake torque
            if (wheel.hasBrakes)
            {
                wheel.collider.brakeTorque = brakeTorqueToApply;
            }

            /*if (GetComponentInParent<Rigidbody>().velocity.magnitude > 5.0f)
            {
                wheel.collider.sidewaysFriction = moving;
            }
            else
            {
                wheel.collider.sidewaysFriction = atRest;
            }*/
        }


        if(GetComponentInParent<Rigidbody>().velocity.magnitude >= 1)
        {
            trueSteeringAngle = steeringAngle / (GetComponentInParent<Rigidbody>().velocity.magnitude / 2);
        } else
        {
            trueSteeringAngle = steeringAngle / 1;
        }
    }

    

}
