using System.Linq;
using UnityEngine;

public static class SplinePlusAPI
{
    public static SPData CreateSplinePlus(Vector3 pos)
    {
        var NewSpline = new GameObject("SplinePlus");
        NewSpline.transform.position = pos;
        var SPData = NewSpline.AddComponent<SplinePlus>().SPData;
        SPData.DataParent = NewSpline;
        SPData.Followers.Add(new Follower());
 
        return SPData;
    }

    public static void SmoothAllSharedNodes(SPData SPData, float radius)
    {
        for (int i = SPData.SharedNodes.Count - 1; i >= 0; i--)
        {
            SmoothSharedPathPoint(SPData, SPData.SharedNodes[i], radius);
        }
        // update all branches 
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }

    public static void SmoothSharedPathPoint(SPData SPData, SharedNode sharedNode, float radius)
    {
        SPData.SmoothData.InitNodePos = sharedNode.Node.Point.position;
        SPData.SmoothData.Nodes = new Node[sharedNode.ConnectedBranches.Count];
        SPData.SmoothData.BranchesIndices = sharedNode.ConnectedBranches.ToArray();

        //shared path point breaking
        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)
        {
            var branchKey = sharedNode.ConnectedBranches[i];

            var localIndex = sharedNode.Node.LocalIndex(SPData, branchKey);

            if (localIndex != 0) BranchesClass.ReverseBranch(SPData, branchKey);

            var duplicate = DuplicatePathPoint(SPData, SPData.DictBranches[branchKey].Nodes[0]);
            SPData.SmoothData.Nodes[i] = duplicate;
            SPData.DictBranches[branchKey].Nodes[0] = duplicate;
        }

        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)
        {
            var branchKey = SPData.SmoothData.BranchesIndices[i];
            BranchesClass.FlipHandles(SPData, branchKey, 0);

            var mid = Vector3.Lerp(SPData.DictBranches[branchKey].Nodes[0].Point.position,
            SPData.DictBranches[branchKey].Nodes[1].Point.position, 0.5f);

            SPData.SmoothData.Nodes[i].Point.position = Vector3.Lerp(SPData.SmoothData.InitNodePos,
            mid, radius);

            SPData.SmoothData.Nodes[i].Point2.position = SPData.SmoothData.InitNodePos;
            SPData.SmoothData.Nodes[i].Point1.localPosition = -SPData.SmoothData.Nodes[i].Point2.localPosition;
        }

        // path points welding
        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)// path points created after chamfer,
        {
            for (int n = i; n < SPData.SmoothData.Nodes.Length; n++)
            {
                if (n == i) continue;
                ConnectTwoNodes(SPData, SPData.SmoothData.Nodes[n], SPData.SmoothData.Nodes[i]);
            }
        }

        // remove initiale shared path point
        SPData.SharedNodes.Remove(sharedNode);
        MonoBehaviour.DestroyImmediate(sharedNode.Node.Point.gameObject);
    }


    public static int AddNewBranchAtPathPoint(SPData SPData, Node Node)
    {
        var branch = new Branch();
        branch.Nodes.Add(Node);


        SPData.DictBranches.Add(SPData.BranchesCount, branch);
        SPData.BranchesCount++;
 
        SPData.Selections._BranchKey =SPData.DictBranches.LastOrDefault().Key;
        IsSharedPathPoint(SPData, Node, SPData.Selections._BranchKey);
        return SPData.Selections._BranchKey;
    }

    public static void AddNodeAtEndOfBranch(SPData SPData, int branchKey, Node node)
    {
        SPData.DictBranches[branchKey].Nodes.Add(node);
        SPData.SplinePlus.SplineCreationClass.UpdateBranch( SPData, SPData.DictBranches[branchKey]);
    }

    public static int ConnectTwoNodes(SPData SPData, Node pathPoint1, Node pathPoint2)
    {
        foreach (var branch in SPData.DictBranches)
        {// avoid creating duplicate branches
            if (branch.Value.Nodes[0].Equals(pathPoint1) &&
                branch.Value.Nodes[branch.Value.Nodes.Count - 1].Equals(pathPoint2))
                return branch.Key;

            else if (branch.Value.Nodes[0].Equals(pathPoint2) &&
                branch.Value.Nodes[branch.Value.Nodes.Count - 1].Equals(pathPoint1))
                return branch.Key;

        }

        var newBranch = new Branch();
        newBranch.Nodes.Add(pathPoint1);
        newBranch.Nodes.Add(pathPoint2);

         SPData.DictBranches.Add(SPData.BranchesCount, newBranch);
        SPData.Selections._BranchKey = SPData.BranchesCount;
        SPData.BranchesCount++;

 
        BranchesClass.FlipHandles(SPData, SPData.Selections._BranchKey, 1);

        // detect if newly created branch has shared path points
        IsSharedPathPoint(SPData, pathPoint1, SPData.Selections._BranchKey);
        IsSharedPathPoint(SPData, pathPoint2, SPData.Selections._BranchKey);

        SPData.SplinePlus.SplineCreationClass.UpdateBranch( SPData, newBranch);


        SPData.Selections._PathPoint = SPData.DictBranches[SPData.Selections._BranchKey].Nodes[0];
        return SPData.Selections._BranchKey;
    }

    public static void IsSharedPathPoint(SPData SPData, Node node, int currentBranchIndex)
    {
        foreach (var branch in SPData.DictBranches)
        {

            if (branch.Key  == currentBranchIndex) continue;
            if (branch.Value.Nodes.Exists(x => x.Equals(node)))
            {
                BranchesClass.AddRefreshSharedNode(SPData, node);
            }
        }
    }

    public static Node CreateNode(SPData SPData, Vector3 position)
    {
        foreach (var branch in SPData.DictBranches)
        {
            for (int n = 0; n < branch.Value.Nodes.Count; n++)//shared path points
            {
                if (Vector3.Distance(branch.Value.Nodes[n].Point.transform.position, position) < Vector3.kEpsilon)
                {
                    return branch.Value.Nodes[n];
                }
            }
        }


        var node = new Node();
        var point = new GameObject("p");
        var point1 = new GameObject("p1");
        var point2 = new GameObject("p2");

        point.transform.parent = SPData.DataParent.transform;
        point1.transform.parent = point.transform;
        point2.transform.parent = point.transform;

        point.transform.position = position;
        point1.transform.localPosition = Vector3.zero;
        point2.transform.localPosition = Vector3.zero;

        point.hideFlags = HideFlags.HideInHierarchy;

        node.Point = point.transform;
        node.Point1 = point1.transform;
        node.Point2 = point2.transform;

        node._Type = NodeType.Smooth;

        SPData.Selections._PathPoint = node;

        return node;
    }


    public static Node DuplicatePathPoint(SPData SPData, Node originPathPoint)
    {
        var node = new Node();
        var point = new GameObject("p");
        var point1 = new GameObject("p1");
        var point2 = new GameObject("p2");

        point.transform.parent = SPData.DataParent.transform;
        point1.transform.parent = point.transform;
        point2.transform.parent = point.transform;

        point.transform.position = originPathPoint.Point.position;
        point1.transform.localPosition = originPathPoint.Point1.transform.localPosition;
        point2.transform.localPosition = originPathPoint.Point2.transform.localPosition;

        point.hideFlags = HideFlags.HideInHierarchy;

        node.Point = point.transform;
        node.Point1 = point1.transform;
        node.Point2 = point2.transform;

        node._Type = originPathPoint._Type;

        return node;
    }


    public static void Attach(SPData originSplinePlus, SPData attachedSplinePlus)
    {
        foreach (var branch in attachedSplinePlus.DictBranches)// import branches
        {
            Branch _branch = new Branch();
            for (int ii = 0; ii < branch.Value.Nodes.Count; ii++)
            {
                var node = new Node();

                node = branch.Value.Nodes[ii];
                node.Point.parent = originSplinePlus.SplinePlus.transform;
                _branch.Nodes.Add(node);
            }

             originSplinePlus.DictBranches.Add(originSplinePlus.BranchesCount, _branch);
            originSplinePlus.BranchesCount++;

        }

        for (int i = 0; i < attachedSplinePlus.SharedNodes.Count; i++)// import sharedNodes
        {
            var sharedNode = attachedSplinePlus.SharedNodes[i];
            originSplinePlus.SharedNodes.Add(sharedNode);
        }

        if (originSplinePlus.DictBranches.Count == 0) return;
        originSplinePlus.Selections._BranchKey = originSplinePlus.DictBranches.LastOrDefault().Key;
        originSplinePlus.Selections._BranchIndex = 0;
        originSplinePlus.Selections._SharedPathPointIndex = -1;
        originSplinePlus.Selections._PathPoint = new Node();

        originSplinePlus.SplinePlus.SplineCreationClass.UpdateAllBranches(originSplinePlus);
    }
}
