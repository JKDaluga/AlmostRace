using System;
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

    public List<GameObject> colliding;

    int aimIndex = 0;

    bool canSwap;


    private void FixedUpdate()
    {
        //Code to set and reset target based on colliding placement
        if (colliding.Count > 0)
        {
            if(aimIndex >= 0 && aimIndex < colliding.Count)
            {
                aim.target = colliding[aimIndex];
            }
        
        }
        else aim.target = null;
        
        if(aimIndex >= colliding.Count)
        {
            aimIndex = -1;
        }

        //Removes objects that are behind the car
        try
        {
            foreach (GameObject i in colliding)
            {
                Vector3 dir = i.transform.position - aim.gameObject.transform.position;
                dir.Normalize();

                float dot = Vector3.Dot(dir, aim.gameObject.transform.forward);
                if (dot < 0)
                {
                    colliding.Remove(i);
                }
            }
        }
        catch(InvalidOperationException e)
        {
            colliding.Clear();
        }

        //Removes targets that are no longer in colliding list
        if (!colliding.Contains(aim.target))
        {
            aim.target = null;
        }

        //If an object becomes null, remove it from the list
        colliding.RemoveAll(GameObject => GameObject == null);
        
        if (colliding.Count > 0 && aimIndex >= 0 && aimIndex < colliding.Count)
        {
            if (colliding[aimIndex].GetComponent<Interactable>() != null)
            {
                if (colliding[aimIndex].GetComponent<Interactable>().interactableHealth <= 0)
                {
                    for (int i = 0; i < colliding.Count; i++)
                    {
                        if (colliding[i].GetComponent<Interactable>() != null && colliding[i].GetComponent<Interactable>().interactableHealth > 0)
                        {
                            aimIndex = i;
                            break;
                        }

                    }
                    aimIndex = -1;
                    aim.target = null;
                }
            }
            if (aim.target == null && colliding.Count > 1)
            {
                for (int i = 0; i < colliding.Count; i++)
                {
                    if (colliding[i].GetComponent<Interactable>() != null && colliding[i].GetComponent<Interactable>().interactableHealth > 0)
                    {
                        aimIndex = i;
                        break;
                    }
                }
            }

        }

        //Allows for swapping between possible targets
        if (Mathf.Abs(Input.GetAxis(aim.gameObject.GetComponent<VehicleInput>().rightHorizontal)) >= .2)
        {
            if (canSwap)
            {
                int sign = (int)Mathf.Sign(Input.GetAxisRaw(aim.gameObject.GetComponent<VehicleInput>().rightHorizontal));

                aimIndex += sign;


                canSwap = false;
            }
        }
        else canSwap = true;


        if (aimIndex >= colliding.Count)
        {
            aimIndex = 0;
        }

        if (aimIndex < 0)
        {
            aimIndex = colliding.Count - 1;
        }
    }

    //Trigger Enter and Exit are used to add and remove values, and resets variables to default if necessary

    private void OnTriggerEnter(Collider other)
    {
        if(aim.shootable == (aim.shootable | ( 1 << other.gameObject.layer)) && other.gameObject != aim.gameObject)
        {

            if(colliding.Count == 0)
            {
                aimIndex = 0;
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
                aimIndex = -1;
                aim.target = null;
            }
        }
    }

}
