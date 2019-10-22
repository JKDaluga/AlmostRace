using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public List<GameObject> Points = new List<GameObject>();
    public float Radius = 0;

    public void CreatePath()
    {
        var SplinePlus = SplinePlusAPI.CreateSplinePlus(Vector3.zero);

        Node pathPoint1 = new Node();
        Node pathPoint2 = new Node();

        for (int i = 0; i < Points.Count - 1; i = i + 2)
        {
            if (Points[i] == null || Points[i + 1] == null)
            {
                DestroyImmediate(SplinePlus.DataParent);
                return;
            }

            pathPoint1 = SplinePlusAPI.CreateNode(SplinePlus, Points[i].transform.position);
            pathPoint2 = SplinePlusAPI.CreateNode(SplinePlus, Points[i + 1].transform.position);

            SplinePlusAPI.ConnectTwoNodes(SplinePlus, pathPoint1, pathPoint2);
        }
       SplinePlusAPI.SmoothAllSharedNodes(SplinePlus, Radius);
    }
}
