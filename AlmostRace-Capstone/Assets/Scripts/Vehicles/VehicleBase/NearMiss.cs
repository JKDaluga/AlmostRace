using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Robyn Riley
    Purpose: Checks the area around each player for things they can collide with
    And gives them a small bonus to hype if they avoid those nearby objects.
    
     
     
                V V V V IMPORTANT V V V V
     
     This script is currently on the Collider object of the cars. Once we implement collisions based off the model of the car,
     
     this script will need to be moved onto the car's Model field instead.
*/




public class NearMiss : MonoBehaviour
{
    //NearHits holds nearby objects, while hits takes those objects if a collision is detected.
    public List<GameObject> nearHits = new List<GameObject>();
    public List<GameObject> hits = new List<GameObject>();
    public LayerMask missable;
    public VehicleHypeBehavior hype;

    private void FixedUpdate()
    {

        //Spherecast checks the entire area directly around the player
        RaycastHit[] closecall;

        closecall = Physics.SphereCastAll(transform.position, 5, transform.position, 10, missable);

        Debug.Log(closecall.Length);

        //If the objects detected by "CloseCall" aren't in nearHits or Hits, add them to nearHits;
        for (int i = 0; i < closecall.Length; i++)
        {
            if(!nearHits.Find(GameObject => GameObject == closecall[i].collider.gameObject) && !hits.Find(GameObject => GameObject == closecall[i].collider.gameObject))
            {
                nearHits.Add(closecall[i].collider.gameObject);
            }
        }

        //if the objects in nearhits move out of range, check if they're in hits, and finally add hype if they aren't.
        foreach (GameObject target in nearHits)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 12)
            {
                nearHits.Remove(target);
                if (!hits.Find(GameObject => GameObject == target))
                {
                    hype.AddHype(5.0f);
                }
                else
                {
                    hits.Remove(target);
                }
            }
        }

        
    }

    //checks for collisions and adds the object to hits if it happens.
    private void OnCollisionEnter(Collision collision)
    {
        if(nearHits.Find(GameObject => GameObject == collision.gameObject))
        {
            hits.Add(collision.gameObject);
        }
    }
}
