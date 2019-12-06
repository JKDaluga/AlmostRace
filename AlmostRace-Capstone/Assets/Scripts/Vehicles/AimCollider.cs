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

    float shortest;

    private void Start()
    {
        shortest = Mathf.Infinity;
    }

    private void FixedUpdate()
    {
        //If the colliding list isn't empty, finds the nearest object in the list
        if (colliding.Count > 0)
        {
            foreach (GameObject i in colliding)
            {
                if (Vector3.Distance(aim.gameObject.transform.position, i.transform.position) < shortest)
                {
                    shortest = Vector3.Distance(aim.gameObject.transform.position, i.transform.position);
                    aim.nearest = i;
                }
            }
        }
    }

    //Trigger Enter and Exit are used to add and remove values, and resets variables to default if necessary

    private void OnTriggerEnter(Collider other)
    {
        if(aim.shootable == (aim.shootable | ( 1 << other.gameObject.layer)) && other.gameObject != aim.self)
        {
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
                shortest = Mathf.Infinity;
            }
            if(aim.nearest == other.gameObject)
            {
                aim.nearest = null;
            }
        }
    }

}
