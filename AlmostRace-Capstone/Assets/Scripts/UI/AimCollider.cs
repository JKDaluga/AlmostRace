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
    public LayerMask layer;

    public float maxDist;

    public List<GameObject> colliding;
    private Camera cam;
    Plane[] planes;

    private GameObject[] obj;
    public List<Collider> interactables;

    int aimIndex = 0;

    bool canSwap;

    private void Start()
    {
        cam = aim.transform.parent.GetComponentInChildren<Camera>();
        interactables = new List<Collider>();

        obj = GameObject.FindGameObjectsWithTag("Interactable");

        for(int i = 0; i < obj.Length; i++)
        {
            interactables.Add(obj[i].GetComponent<Collider>());
        }

        obj = GameObject.FindGameObjectsWithTag("Vehicle");

        for(int i = 0; i < obj.Length; i++)
        {
            if(obj[i] != aim.self && obj[i] != aim.gameObject)
            {
                try
                {
                    interactables.Add(obj[i].GetComponent<Collider>());
                }
                catch
                {
                    print("Object doesn't have collider");
                }
            }
        }


    }

    private void FixedUpdate()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);


        if (aimIndex >= colliding.Count)
        {
            aimIndex = 0;
        }

        if (aimIndex < 0)
        {
            aimIndex = colliding.Count - 1;
        }

        if (colliding.Count > 0 && aimIndex < colliding.Count)
        {
            aim.target = colliding[aimIndex];
        } else
        {
            aim.target = null;
        }
        

        for(int i = 0; i < interactables.Count; i++)
        {
            
            if (GeometryUtility.TestPlanesAABB(planes, interactables[i].bounds))
            {
                if (!colliding.Contains(interactables[i].gameObject) && interactables[i].GetComponent<Collider>().enabled)
                {
                    if (Vector3.Distance(interactables[i].transform.position, aim.transform.position) < maxDist)
                    {
                        if (!Physics.Raycast(aim.transform.position, interactables[i].ClosestPoint(aim.transform.position) - aim.transform.position, maxDist, layer))
                        {
                            colliding.Add(interactables[i].gameObject);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    colliding.Remove(interactables[i].gameObject);
                }
                catch { }
            }
        }

        for(int i = 0; i < colliding.Count; i++)
        {
            if(Vector3.Distance(colliding[i].transform.position, aim.transform.position) > maxDist)
            {
                colliding.Remove(colliding[i]);
            }
            if (!colliding[i].GetComponent<Collider>().enabled)
            {
                colliding.Remove(colliding[i]);
            }
            if (Physics.Raycast(aim.transform.position, colliding[i].GetComponent<Collider>().ClosestPoint(aim.transform.position) - aim.transform.position, Vector3.Distance(colliding[i].transform.position, aim.transform.position), layer))
            {
                colliding.Remove(colliding[i]);
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


    }

}
