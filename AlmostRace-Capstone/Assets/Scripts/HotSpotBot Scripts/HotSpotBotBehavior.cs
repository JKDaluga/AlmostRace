using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpotBotBehavior : MonoBehaviour
{
    public float moveSpeed;
    public static HotSpotBotBehavior instance;
    public List<Node> branchNodes;
    private SplinePlus _splinePlusScript;
    private bool _beingHeld;

    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        _splinePlusScript = GameObject.Find("Spline plus").GetComponent<SplinePlus>();
        _splinePlusScript.SetSpeed(moveSpeed);

        //SPData.Selection._BranchKey is the key of the currently selected branch in the editor,
        int n = _splinePlusScript.SPData.Selections._BranchKey;
        branchNodes = _splinePlusScript.SPData.DictBranches[0].Nodes;
    }

    public bool GetBeingHeld()
    {
        return _beingHeld;
    }

    public void SetBeingHeld(bool isBeingHeld)
    {
        _beingHeld = isBeingHeld;
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

        Node pathPoint1 = SplinePlusAPI.CreateNode(_splinePlusScript.SPData, vehiclesPosition);
        int branchKey = SplinePlusAPI.ConnectTwoNodes(_splinePlusScript.SPData, pathPoint1, positionToPlace);
        _splinePlusScript.GoToNewBranch(branchKey);
    }

}
