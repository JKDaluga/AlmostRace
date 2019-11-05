using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley 11/5/19
 * Rotates an empty gameobject around the vehicle model, with the camera set to always look at the object
 */

public class SwivelCamera : MonoBehaviour
{
    public float horiz;
    public float vert;


    public VehicleInput _vehicleInput;

    private void Start()
    {
       // _vehicleInput = GetComponentInParent<VehicleInput>();
    }

    private void FixedUpdate()
    {
        //If players are actively pushing the right joystick, sets the camera angles appropriately
        //Otherwise, allows players to face and aim forward
        if (Mathf.Abs( Input.GetAxis(_vehicleInput.rightHorizontal)) > .3f  || Mathf.Abs(Input.GetAxis(_vehicleInput.rightVertical)) > .3f)
        {

            horiz = -Input.GetAxis(_vehicleInput.rightHorizontal);
            vert = -Input.GetAxis(_vehicleInput.rightVertical);

            float angle = Mathf.Atan2(horiz, vert) * Mathf.Rad2Deg;


            transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x, transform.parent.parent.eulerAngles.y - angle, transform.parent.eulerAngles.z);
        } else
        {
            transform.parent.eulerAngles = transform.parent.parent.eulerAngles;
        }
    }
}
