using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotBotBehavior : MonoBehaviour
{
    public float moveSpeed;
    public static HotSpotBotBehavior instance;
    private List<Node> branchNodes = new List<Node>();
    private SplinePlus _splinePlusScript;
    private bool _beingHeld;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _splinePlusScript = GameObject.FindGameObjectWithTag("HotSpotSpline").GetComponent<SplinePlus>();
        _splinePlusScript.SetSpeed(moveSpeed);
        _splinePlusScript.SPData.Followers[0].Reverse = true;
        foreach (KeyValuePair<int, Branch> entry in _splinePlusScript.SPData.DictBranches)
        {
            // do something with entry.Value or entry.Key
            for(int i = 0; i < entry.Value.Nodes.Count; i++)
            {
                if(!branchNodes.Contains(entry.Value.Nodes[i]))
                {
                    branchNodes.Add(entry.Value.Nodes[i]);
                }
            }
        }
        SetBeingHeld(false);
    }

    public bool GetBeingHeld()
    {
        return _beingHeld;
    }

    public void SetBeingHeld(bool isBeingHeld)
    {
        _beingHeld = isBeingHeld;
    }

    public Vector3 GetNearestPointOnSpline(Vector3 givenPosition, int vectorsBack)
    {
        Vector3 closestWorldPoint = new Vector3(9999, 9999, 9999);
        float lastDistance = 9999;
        int vectorsBackAdjustment;

        List<Branch> currentBranches = new List<Branch>();
        foreach (KeyValuePair<int, Branch> entry in _splinePlusScript.SPData.DictBranches)
        {
            currentBranches.Add(entry.Value);
        }
        
        for (int i = 0; i < currentBranches.Count; i++)
        {
            for (int j = 0; j < currentBranches[i].Vertices.Count; j++)
            {
                float distance = Vector3.Distance(currentBranches[i].Vertices[j], givenPosition);
                if (distance <= lastDistance)
                {
                    lastDistance = distance;
                    vectorsBackAdjustment = Mathf.Clamp(j + vectorsBack, 0, currentBranches[i].Vertices.Count - 1);
                    closestWorldPoint = currentBranches[i].Vertices[vectorsBackAdjustment];
                }
            }
        }
        return closestWorldPoint;
    }

    public void SetPosition(Vector3 vehiclesPosition)
    {
        Node positionToPlace = branchNodes[0];
        float distance = Vector3.Distance(branchNodes[0].Point.position, vehiclesPosition);

        for (int i = 0; i < branchNodes.Count; i++)
        {
            if(distance > Vector3.Distance(branchNodes[i].Point.position, vehiclesPosition))
            {
                distance = Vector3.Distance(branchNodes[i].Point.position, vehiclesPosition);
                positionToPlace = branchNodes[i];
            }
        }
        Node pathPoint1 = SplinePlusAPI.CreateNode(_splinePlusScript.SPData, vehiclesPosition + new Vector3(0,80,0));
        int branchKey = SplinePlusAPI.ConnectTwoNodes(_splinePlusScript.SPData, pathPoint1, positionToPlace);
        
        _splinePlusScript.GoToNewBranch(branchKey);
        _splinePlusScript.SPData.Followers[0].DistanceData.Index = branchKey;
    }

}
