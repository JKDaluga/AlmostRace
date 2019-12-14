using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley 12/3/19
 * Handler class for Aim Assist mechanic, checks collisions and assigns necessary values
 */

public class AimCollider : MonoBehaviour
{
    public AimAssistant aim;

    public List<GameObject> colliding = new List<GameObject>();

    int count;

    bool canSwap;

    private void Start()
    {
        count = -1;
    }

    private void FixedUpdate()
    {
        if(colliding.Count != 0)
        {
            aim.target = colliding[count];
        }


        if (Mathf.Abs(Input.GetAxis(aim.gameObject.GetComponent<VehicleInput>().rightHorizontal)) >= .2)
        {
            if (canSwap)
            {
                int sign = (int)Mathf.Sign(Input.GetAxisRaw(aim.gameObject.GetComponent<VehicleInput>().rightHorizontal));

                count += sign;

                if (count >= colliding.Count)
                {
                    count = 0;
                }

                if (count < 0)
                {
                    count = colliding.Count - 1;
                }

                canSwap = false;
            }
        }
        else canSwap = true;

    }

    //Trigger Enter and Exit are used to add and remove values, and resets variables to default if necessary

    private void OnTriggerEnter(Collider other)
    {
        if(aim.shootable == (aim.shootable | ( 1 << other.gameObject.layer)) && other.gameObject != aim.gameObject)
        {
            Debug.Log(other.gameObject == aim.gameObject);
            if(colliding.Count == 0)
            {
                count = 0;
            }
            colliding.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliding.Contains(other.gameObject))
        {
            colliding.Remove(other.gameObject);
            if(colliding.Count == 0)
            {
                count = -1;
                aim.target = null;
            }
        }
    }

}
