using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley 11/5/19
 * Rotates an empty gameobject around the vehicle model, with the camera set to always look at the object
 * 
 * Edited 12/2/2019: Eddie B
 * Added a float called "resetDelay" that replaced the magic number used for resetting camera after no input
 */

public class SwivelCamera : MonoBehaviour
{
    float horiz;
    float vert;

    public float turnTime = 100;
    public float turnAmount = 1;
    public float resetDelay = 3.0f;

    public float maxAngle;

    public VehicleInput _vehicleInput;

    Quaternion orig;

    bool rearFacing = false;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        //If players are actively pushing the right joystick, sets the camera angles appropriately
        //Otherwise, allows players to face and aim forward
        

        //When the rightJoystick button is pressed, make the camera face backwards
        if (Input.GetButtonDown(_vehicleInput.rightStickButton))
        {
            rearFacing = true;

            Quaternion target = transform.parent.rotation * Quaternion.Euler(0, 180, 0);

            transform.parent.rotation = target;
        }
        //when released, reset the camera
        if (Input.GetButtonUp(_vehicleInput.rightStickButton) || Input.GetAxis(_vehicleInput.rightVertical) < -.3f)
        {
            rearFacing = false;
            Vector3 target = transform.parent.parent.eulerAngles;

            transform.parent.rotation = Quaternion.Euler(target);


        }


        //if a direction is pushed on the right joystick, find the respective angle, and smoothly rotate the camera to the new position
        if (Mathf.Abs(Input.GetAxis(_vehicleInput.rightHorizontal)) > .2f)
        {
            CancelInvoke();
            horiz = Input.GetAxis(_vehicleInput.rightHorizontal);

            Quaternion target;

            target = transform.parent.rotation * Quaternion.Euler(0, turnAmount * Mathf.Sign(horiz), 0);
            

            float actualY = transform.parent.localEulerAngles.y;

            if(actualY > 180)
            {
                actualY -= 360;
            }

            //Debug.Log(actualY);

            if(Mathf.Abs(actualY) > maxAngle)
            {
                if(Mathf.Sign(actualY) == Mathf.Sign(horiz))
                {
                    target = transform.parent.rotation;
                }
            }

            if (transform.parent.rotation != target)
            {
                transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, target, turnTime * Time.deltaTime);
                
            }
        }
        //if No direction is pressed, set the camera behind the player again.
        else 
        {
            if (!Input.GetButton(_vehicleInput.basicAbilityInput) && !Input.GetButton(_vehicleInput.signatureAbilityInput) && !Input.GetButton(_vehicleInput.rightStickButton))
            Invoke("Forward", resetDelay);
            else
            {
                CancelInvoke();
            }
        }

    }

    private void Forward()
    {
        Vector3 target = transform.parent.parent.eulerAngles;

        transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(target), turnTime * Time.deltaTime);
    }
}
