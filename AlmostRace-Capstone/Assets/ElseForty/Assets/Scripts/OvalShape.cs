using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvalShape : MonoBehaviour
{
    public float Power = 20;
    SPData SPData;

    void Start()
    {
        //create the game object that will hold the spline plus component and catch the SPData to use further
        SPData = SplinePlusAPI.CreateSplinePlus(Vector3.zero);

        //create the 2 nodes
        var node1 = SplinePlusAPI.CreateNode(SPData, new Vector3(10, 0, 0));
        var node2 = SplinePlusAPI.CreateNode(SPData, new Vector3(-10, 0, 0));

        //connect the 2 node
        SplinePlusAPI.ConnectTwoNodes(SPData, node1, node2);

        //change the node points to get the oval shape,
        // just copy the values from the editor example it's easier this way
        node1.Point1.position = new Vector3(10, 0, Power);
        node1.Point2.position = new Vector3(10, 0, -Power);

        node2.Point1.position = new Vector3(-10, 0, Power);
        node2.Point2.position = new Vector3(-10, 0, -Power);

        //set looped to true to close the shape
        SPData.IsLooped = true;

        //trigger a spline plus update
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);

    }
}
