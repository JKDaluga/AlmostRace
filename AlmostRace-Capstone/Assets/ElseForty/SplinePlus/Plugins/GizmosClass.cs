using UnityEngine;

public class GizmosClass
{

    public void FollowerProjectionLines( Follower follower)
    {

        if (follower.FollowerGO == null) return;

        var GroundRayDiam = follower.FollowerProjection.GroundRayPos * follower.FollowerProjection.GroundRayLength;
        Gizmos.color =  Color.magenta;
        Gizmos.DrawLine( follower.DistanceData.Position- GroundRayDiam, follower.DistanceData.Position+ GroundRayDiam);

        var followerTransform = follower.FollowerGO.transform;
       // var ObstacleRayCenter = new Vector3(followerTransform.position.x,
   // followerTransform.position.y + follower.FollowerProjection.Height, followerTransform.position.z);

        var ObstacleRayCenter = followerTransform.position + followerTransform.up * follower.FollowerProjection.ObstacleRayHeight;


        Gizmos.color = Color.red;
        Gizmos.DrawLine(ObstacleRayCenter, follower.FollowerProjection.FwObstacleRayPos);

    }
    public void DrawBranch(SPData SPData, Branch branch, int branchInd)
    {
        var unselectedCol = Color.gray;
        if (SPData.FollowerType == FollowerType.PathFinding && SPData.Selections._Follower < SPData.PFFollowers.Count)
        {
            var pFGoal = SPData.PFFollowers[SPData.Selections._Follower];
            if (pFGoal.Agents.Count > 0 && SPData.Selections._Agent < pFGoal.Agents.Count)
            {
                unselectedCol = pFGoal.Agents[SPData.Selections._Agent].PathFindingPath.Exists(x => x.BranchIndex == branchInd) ? Color.blue : Color.gray;
            }
        }

        for (int z = 0; z < branch.Vertices.Count; z++)
        {
            if (z > 0)
            {
                var a = branch.Vertices[z - 1];
                var b = branch.Vertices[z];

                if (branchInd == SPData.Selections._BranchKey)
                {
                    Gizmos.color = SPData.ConstantSpeed ? Color.green : Color.Lerp(Color.red, Color.green, branch.SpeedFactor[z] / 100.0f);
                    Gizmos.DrawLine(a, b);
                }
                else
                {
                    Gizmos.color = SPData.ConstantSpeed ? unselectedCol : Color.Lerp(Color.red, unselectedCol, branch.SpeedFactor[z] / 100.0f);
                    Gizmos.DrawLine(a, b);
                }
            }

            var c = branch.Vertices[z];
            if (SPData.SharedSettings.ShowHelper)
            {
                var n = branch.Normals[z] * SPData.SharedSettings.HelperSize;
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(c, c + n);

                var t = branch.Tangents[z] * SPData.SharedSettings.HelperSize;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(c, c + t);
            }
        }

        if (SPData.SplineProjection && SPData.SharedSettings.ShowRaycast)
        {
            Gizmos.color = Color.yellow;
            for (int z = 0; z < branch.Vertices.Count; z++)
            {
                for (int n = 0; n < branch.Nodes.Count; n++)
                {
                    var origin = branch.Nodes[n].Point.position;
                    Gizmos.DrawLine(origin + Vector3.up * SPData.RaycastLength, origin - Vector3.up * SPData.RaycastLength);
                }
            }
        }
    }


    public void  NodesGizmos(SPData SPData)
    {
        if (SPData.SharedSettings.ShowGizmos)
        {
            foreach (var branch in SPData.DictBranches)
            {
                for (int i = 0; i < branch.Value.Nodes.Count; i++)
                {
                     Gizmos.color = SPData.SharedSettings.StandardPathPointColor;
                     Gizmos.DrawSphere(branch.Value.Nodes[i].Point.position, SPData.SharedSettings.GizmosSize);
                }
            }

            for (int i = 0; i < SPData.SharedNodes.Count; i++)
            {
                if (SPData.SharedNodes[i].Node.Equals(null)) continue;
 
                if (SPData.SharedNodes[i]._Type == SharedNodeType.Random) Gizmos.color = SPData.SharedSettings.RandomSharedNodeColor;
                else Gizmos.color = SPData.SharedSettings.DefinedSharedNodeColor;

                Gizmos.DrawSphere(SPData.SharedNodes[i].Node.Point.position, SPData.SharedSettings.GizmosSize);
            }
        }
    }
}
