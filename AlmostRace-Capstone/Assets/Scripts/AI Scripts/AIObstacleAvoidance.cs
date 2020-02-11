using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObstacleAvoidance : MonoBehaviour
{
    public LayerMask wall;
    public int obstLayer;
    RaycastHit left, right;
    public AIBehaviour handler;

    public bool seeing;

    public float turnAmount;

    private void OnTriggerEnter(Collider other)
    {
        Physics.Raycast(transform.parent.position, transform.right, out right, wall);
        Physics.Raycast(transform.parent.position, -transform.right, out left, wall);

        seeing = true;
        if(left.distance > right.distance)
        {
            handler.inputTurn = turnAmount;
        }
        else
        {
            handler.inputTurn = -turnAmount;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        handler.inputTurn = 0;
        seeing = false;
    }
}
