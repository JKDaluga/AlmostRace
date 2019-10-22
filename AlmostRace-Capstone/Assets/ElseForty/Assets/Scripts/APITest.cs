using UnityEngine;

public class APITest : MonoBehaviour
{
    public GameObject Follower1;
    public GameObject Follower2;
    SPData SPData;
    public void Start()
    {

        SPData = SplinePlusAPI.CreateSplinePlus(Vector3.zero);

        var pathPoint1 = SplinePlusAPI.CreateNode(SPData, new Vector3(0, 0, 0));
        var pathPoint2 = SplinePlusAPI.CreateNode(SPData, new Vector3(10, 0, 0));
        var pathPoint3 = SplinePlusAPI.CreateNode(SPData, new Vector3(10, 0, 10));
        var pathPoint4 = SplinePlusAPI.CreateNode(SPData, new Vector3(0, 0, 10));

        SplinePlusAPI.ConnectTwoNodes(SPData, pathPoint1, pathPoint2);
        SplinePlusAPI.ConnectTwoNodes(SPData, pathPoint2, pathPoint3);
        SplinePlusAPI.ConnectTwoNodes(SPData, pathPoint3, pathPoint4);
        SplinePlusAPI.ConnectTwoNodes(SPData, pathPoint4, pathPoint1);

        SplinePlusAPI.SmoothAllSharedNodes(SPData, 0.5f);
        FollowerSettings(SPData);
    }
    void FollowerSettings(SPData SPData)
    {
        SPData.Followers[0]._FollowerAnimation = FollowerAnimation.AutoAnimated;
        SPData.Followers[0].FollowerGO = Follower1;
        SPData.Followers[0].Speed = 10;
        SPData.Followers[0].IsActive = true;

         SPData.Followers.Add(new Follower());
  
        SPData.Followers[1]._FollowerAnimation = FollowerAnimation.AutoAnimated;
        SPData.Followers[1].FollowerGO = Follower2;
        SPData.Followers[1].Speed = 5;
        SPData.Followers[1].IsActive = true;

    }
}

