using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObstacleAvoidance : MonoBehaviour
{
    public LayerMask wall;
    RaycastHit left, right;
    public AIBehaviour handler;

    public bool turnL, turnR;

    float rayAngle;
    public float turnAmount, turnOverride;

    Vector3 vel;

    private void FixedUpdate()
    {
        vel = GetComponentInParent<Rigidbody>().velocity;
        vel = new Vector3(vel.x, 0, vel.y);
        transform.parent.rotation = Quaternion.LookRotation(vel, transform.parent.up);

        if (turnL)
        {
            handler.inputTurn = -turnAmount;
        } 
        else if (turnR)
        {
            handler.inputTurn = turnAmount;
        }
    }

    private void OnTriggerEnter(Collider other)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        turnL = false;
        turnR = false;
        rayAngle = 0;
    }
}
