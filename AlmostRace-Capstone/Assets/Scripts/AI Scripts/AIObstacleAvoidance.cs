using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObstacleAvoidance : MonoBehaviour
{
    public LayerMask wall;
    public int obstLayer;
    RaycastHit left, right;
    public AIBehaviour handler;

    public bool turnL, turnR;

    float rayAngle, forwardAngle;

    public float turnAmount;

    Vector3 vel;

    public float turnOverride;
    public float oneWallOvrd;

    private void FixedUpdate()
    {
        vel = GetComponentInParent<Rigidbody>().velocity;
        vel = new Vector3(vel.x, 0, vel.z);
        transform.parent.rotation = Quaternion.LookRotation(vel, transform.parent.up);

        if (turnL)
        {
            handler.inputTurn = -turnAmount;
        }
        if (turnR)
        {
            handler.inputTurn = turnAmount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.Raycast(transform.parent.position, transform.right, out right, wall);
        Physics.Raycast(transform.parent.position, -transform.right, out left, wall);

        //The Car only finds a wall on the left side
        if ((left.collider != null && right.collider == null))
        {
            //If the car is too far away, turn left
            if (left.distance > oneWallOvrd)
            {
                turnL = true;
                turnR = false;
            }
            //Otherwise, turn right
            else
            {
                turnL = false;
                turnR = true;
            }
        }
        else if ((left.collider == null && right.collider != null))
        {
            //Code block opposite to left wall only
            if (right.distance > oneWallOvrd)
            {
                turnL = false;
                turnR = true;
            }
            else
            {
                turnL = true;
                turnR = false;
            }
        }
        //Checks if the left wall is closer than the right
        else if (left.distance > right.distance)
        {
            //Gets the angle between the left wall normal and the drawn ray
            rayAngle = Vector3.Angle(left.normal, -transform.right);
            
            //Checks if the car is moving diagonal to the wall
            if (rayAngle > turnOverride)
            {
                //Gets the angle between the car forward and the wall normal
                forwardAngle = Vector3.Angle(left.normal, transform.forward);
                
                //If the car is heading toward the wall, turn away, else turn toward it
                if (forwardAngle > 90)
                {
                    turnL = false;
                    turnR = true;
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
        else
        {
            rayAngle = Vector3.Angle(right.normal, transform.right);
            if (rayAngle > turnOverride)
            {
                forwardAngle = Vector3.Angle(right.normal, transform.forward);
                if (forwardAngle > 90)
                {
                    turnL = true;
                    turnR = false;
                }
                else
                {
                    turnL = false;
                    turnR = true;
                }
            }
            else
            {
                turnL = true;
                turnR = false;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        turnL = false;
        turnR = false;
    }
}
