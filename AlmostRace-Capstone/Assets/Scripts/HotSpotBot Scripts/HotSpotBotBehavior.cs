using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie Borissov
 
                                        VVV PLEASE READ VVV

    A very rough prototype of our hotspot bot. DO NOT KEEP THIS CODE.

    This code is meant to VERY roughly simulate an "ai" agent traveling a "nav mesh'.
    What it really is is a little ball that follows a series of nodes.

    This version does not feature players OBTAINING the hotspot bot, and is intended ONLY to test the combat.
    This version also does not acount for branching paths or shortcuts...again VERY rough, specifically 
    for playtesting combat.
     
     */

public class HotSpotBotBehavior : MonoBehaviour
{

    public float moveSpeed = 10; // How fast the bot moves
    public bool isOnLinearMap = true;

    public List<Transform> destinationNodes; //List of places bot can go to

    [Header("NAV POINT STUFF")]
    private int nextNodeIndex; // The index currently traveling to.
    public Transform nextNode; // The node currently traveling to.

    private int previousNodeIndex; //the index previously visited, really only used for non-linear maps
    public Transform previousNode; //the node previously visited, really only used for non-linear maps
 

    private Rigidbody objectRigidbody;

    private List<VehicleHypeBehavior> vehiclesInRange; // vehicles to give hype to.

    private float startTime = 0f;
    private float journeyLength = 1f;

    // Start is called before the first frame update
    void Start()
    {
        objectRigidbody = gameObject.GetComponent<Rigidbody>();
        SelectFirstNode();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distCovered = (Time.time - startTime) * moveSpeed;
        // Fraction of journey completed = current distance divided by total distance.
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(previousNode.position, nextNode.position, fracJourney);
        if(transform.position == nextNode.position)
        {
            SelectNextNode();
        }
    }


    private void SelectFirstNode()
    {
        previousNode = gameObject.transform;
        previousNodeIndex = 0;
        nextNode = destinationNodes[0];
        nextNodeIndex = 0;     
        gameObject.transform.LookAt(previousNode); //Placed here to avoid it happening in Update
        startTime = Time.time;
        journeyLength = Vector3.Distance(previousNode.position, nextNode.position);
    }

    private void SelectNextNode()
    {
        if (isOnLinearMap)
        {
            previousNode = nextNode;
            previousNodeIndex = nextNodeIndex;
            if (previousNodeIndex + 1 >= destinationNodes.Count)
            {
                //Checks to see if we've reached the end of the list, loops back around to the start.
                nextNode = destinationNodes[0];
                nextNodeIndex = 0;
            }
            else
            {
                //Haven't reached the end of the list yet.
                nextNode = destinationNodes[previousNodeIndex + 1];
                nextNodeIndex = previousNodeIndex + 1;
            }
            
        }
        else
        {
            int randNum = Random.Range(1, destinationNodes.Count - 1); //Chooses initial random node, avoids needing a non-recursive version of SelectRandomNextNode().
            previousNode = nextNode;
            previousNodeIndex = nextNodeIndex;

            SelectRandomNextNode(randNum); //Recursively finds a suitable random next node.          
        }
        gameObject.transform.LookAt(nextNode); //Placed here to avoid it happening in Update
        startTime = Time.time;
        journeyLength = Vector3.Distance(previousNode.position, nextNode.position);
        //objectRigidbody.velocity = (transform.forward * moveSpeed * Time.deltaTime);
    }

    private void SelectRandomNextNode(int nodeIndex)//Recursive function that ensures the hotspot bot doesn't go back and forth between nodes.
    {
        if(nodeIndex == previousNodeIndex) //checks if the next random node was the same as the previous one (we don't want this)
        {
            int randNum = Random.Range(1, destinationNodes.Count - 1);
            SelectRandomNextNode(randNum);
        }
        else // Break condition;
        {
            nextNode = destinationNodes[nodeIndex];
            nextNodeIndex = nodeIndex;
        }     
    }

    public Transform GetPreviousNode()
    {
        return previousNode;
    }

    public Transform GetNextNode()
    {
        return nextNode;
    }
}
