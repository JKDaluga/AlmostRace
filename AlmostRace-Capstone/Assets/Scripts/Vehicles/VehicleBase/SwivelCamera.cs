using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley 11/5/19
 * Rotates an empty gameobject around the vehicle model, with the camera set to always look at the object
 */

public class SwivelCamera : MonoBehaviour
{
    float horiz;
    float vert;

    public float turnTime = 100;
    public float turnAmount = 1;

    public VehicleInput _vehicleInput;

    bool rearFacing = false;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        //If players are actively pushing the right joystick, sets the camera angles appropriately
        //Otherwise, allows players to face and aim forward

        //When the rightJoystick button is pressed, make the camera face backwards
        if (Input.GetButtonDown(_vehicleInput.rightStickButton))
        {
            Debug.Log("hit");
            rearFacing = true;
        }
        //when released, reset the camera
        if (Input.GetButtonUp(_vehicleInput.rightStickButton) || Input.GetAxis(_vehicleInput.rightVertical) < -.3f)
        {
            rearFacing = false;
            Vector3 target = transform.parent.parent.eulerAngles;

            transform.parent.rotation = Quaternion.Euler(target);
        }

        if (rearFacing)
        {
            Vector3 target = new Vector3(transform.parent.eulerAngles.x, transform.parent.parent.eulerAngles.y - 180, transform.parent.eulerAngles.z);

            transform.parent.rotation = Quaternion.Euler(target);
        } 


        //if a direction is pushed on the right joystick, find the respective angle, and smoothly rotate the camera to the new position
        if (Mathf.Abs(Input.GetAxis(_vehicleInput.rightHorizontal)) > .2f)
        {
            CancelInvoke();
            horiz = Input.GetAxis(_vehicleInput.rightHorizontal);
            

            float angle = Mathf.Atan2(horiz, vert) * Mathf.Rad2Deg;

            Vector3 target = new Vector3(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y + (turnAmount * Mathf.Sign( horiz)), transform.parent.eulerAngles.z);

            if (transform.parent.rotation != Quaternion.Euler(target))
            {
                transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.Euler(target), turnTime * Time.deltaTime);
            }
        }
        //if No direction is pressed, set the camera behind the player again.
        else 
        {
            if (!Input.GetButton(_vehicleInput.basicAbilityInput) && !Input.GetButton(_vehicleInput.signatureAbilityInput))
            Invoke("Forward", 3.0f);
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
