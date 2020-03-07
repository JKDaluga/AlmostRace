using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObstacleAvoidance : MonoBehaviour
{
    public LayerMask wall;
    RaycastHit left, right;
    public AIBehaviour handler;

    public bool turnL, turnR;

    public bool avoiding = true;

    float rayAngle;
    public float turnAmount, turnOverride;

    Vector3 vel;

    float timer = 0;

    private void FixedUpdate()
    {
        vel = handler.GetComponent<Rigidbody>().velocity;
        if(vel.magnitude != 0) transform.parent.rotation = Quaternion.LookRotation(vel, transform.parent.up);

        if (turnL)
        {
            handler.inputTurn = -turnAmount;
        } 
        if (turnR)
        {
            handler.inputTurn = turnAmount;
        }
        AIBehaviour behavior = GetComponent<AIBehaviour>();
        if (behavior != null && behavior._inArena)
        {
            avoiding = false;
        }

        if (!avoiding && behavior != null && !behavior._inArena)
        {
           
            timer += Time.deltaTime;
            if(timer > .5f)
            {
                avoiding = true;
                timer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (avoiding)
        {
            Physics.Raycast(handler.transform.position, handler.transform.right, out right, wall);
            Physics.Raycast(handler.transform.position, -handler.transform.right, out left, wall);

            //Left Wall is Closer than Right Wall
            if (left.distance > right.distance)
            {
                rayAngle = Vector3.SignedAngle(right.normal, -handler.transform.right, handler.transform.forward);

                //Car is moving at an angle
                if (Mathf.Abs(rayAngle) > turnOverride)
                {
                    //Sign determines movement direction, negative is right, positive is left
                    if (rayAngle > 0)
                    {
                        turnR = true;
                        turnL = false;
                    }
                    else
                    {
                        turnL = true;
                        turnR = false;
                    }
                }
                else
                {
                    turnL = true;
                    turnR = false;
                }

            }
            else
            {
                rayAngle = Vector3.SignedAngle(left.normal, handler.transform.right, handler.transform.forward);

                if (Mathf.Abs(rayAngle) > turnOverride)
                {
                    //Sign determines movement direction, negative is right, positive is left
                    if (rayAngle > 0)
                    {
                        turnR = true;
                        turnL = false;
                    }
                    else
                    {
                        turnL = true;
                        turnR = false;
                    }
                }
                else
                {
                    turnL = false;
                    turnR = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        turnR = false;
        turnL = false;
        rayAngle = 0;
    }
}
