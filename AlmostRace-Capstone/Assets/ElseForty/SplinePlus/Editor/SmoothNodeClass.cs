using System.Collections.Generic;
using UnityEngine;

public class SmoothNodeClass
{
    public void SmoothNode(SPData SPData)
    {
        var node = SPData.Selections._PathPoint;
        var branch = SPData.DictBranches[SPData.Selections._BranchKey];
        var nodeIndex = SPData.Selections._LocalNodeIndex; 

        SPData.SmoothData.Nodes = new Node[2];

        if (branch.Nodes.Count > 2 && nodeIndex > 0 && nodeIndex < (branch.Nodes.Count - 1))
        {
            SPData.SmoothData.InitNodePos = node.Point.position;
            SPData.SmoothData.InitNodePoint1Pos = node.Point1.position;
            SPData.SmoothData.Nodes[0] = node;

            SPData.SmoothData.Nodes[1] = SplinePlusAPI.DuplicatePathPoint(SPData, node);

            branch.Nodes.Insert(nodeIndex + 1, SPData.SmoothData.Nodes[1]);
        }

        EditSmoothNode(SPData,  SPData.SmoothRadius);
    }

    public void EditSmoothNode(SPData SPData, float radius)
    {
        var branch = SPData.DictBranches[SPData.Selections._BranchKey];
        var nodeIndex = SPData.Selections._LocalNodeIndex;
        SPData.SmoothRadius = radius;
        var backPos = branch.Nodes[nodeIndex - 1].Point.position;
        var forwardPos = branch.Nodes[nodeIndex + 2].Point.position;


        SPData.SmoothData.Nodes[0].Point.position = Vector3.Lerp(SPData.SmoothData.InitNodePos, backPos, SPData.SmoothRadius);
        SPData.SmoothData.Nodes[1].Point.position = Vector3.Lerp(SPData.SmoothData.InitNodePos, forwardPos, SPData.SmoothRadius);


        SPData.SmoothData.Nodes[0].Point2.position = SPData.SmoothData.InitNodePos;
        SPData.SmoothData.Nodes[0].Point1.localPosition = -SPData.SmoothData.Nodes[0].Point2.localPosition;

        SPData.SmoothData.Nodes[1].Point1.position = SPData.SmoothData.InitNodePos;
        SPData.SmoothData.Nodes[1].Point2.localPosition = -SPData.SmoothData.Nodes[1].Point1.localPosition;

        SPData.SplinePlus.SplineCreationClass.UpdateBranch(  SPData, branch);
    }

    public void ResetSmoothPathPoint(SPData SPData )
    {
        var branch = SPData.DictBranches[SPData.Selections._BranchKey];

        SPData.SmoothData.SmoothNode = false;

        var nodeIndex = SPData.SmoothData.Nodes[1].LocalIndex(SPData, SPData.Selections._BranchKey);
        var gO = SPData.SmoothData.Nodes[1].Point.gameObject;
        MonoBehaviour.DestroyImmediate(gO);
        SPData.DictBranches[SPData.Selections._BranchKey].Nodes.RemoveAt(nodeIndex);


        SPData.SmoothData.Nodes[0].Point.position = SPData.SmoothData.InitNodePos;
        SPData.SmoothData.Nodes[0].Point1.position = SPData.SmoothData.InitNodePoint1Pos;
        SPData.SmoothData.Nodes[0].Point2.localPosition = -SPData.SmoothData.Nodes[0].Point1.localPosition;

        SPData.SplinePlus.SplineCreationClass.UpdateBranch(  SPData, branch);
    }

    public void SmoothSharedPathPoint(SPData SPData)
    {
        var node = SPData.Selections._PathPoint;

        //cach data for restore procedure
        SPData.SmoothData.InitNodePos = node.Point.position;
        var sharedPathPointIndex = SPData.Selections._SharedPathPointIndex;
        SPData.SmoothData.Nodes = new Node[SPData.SharedNodes[sharedPathPointIndex].ConnectedBranches.Count];
        SPData.SmoothData.BranchesIndices = SPData.SharedNodes[sharedPathPointIndex].ConnectedBranches.ToArray();
   
        //shared path point breaking
        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)
        {
            var branchKey = SPData.SharedNodes[sharedPathPointIndex].ConnectedBranches[i];
            var localIndex = node.LocalIndex(SPData, branchKey);
 
            var duplicate = SplinePlusAPI.DuplicatePathPoint(SPData, node);
            SPData.SmoothData.Nodes[i] = duplicate;
            SPData.DictBranches[branchKey].Nodes[localIndex] = duplicate;
            if (localIndex != 0) BranchesClass.FlipHandles(SPData, branchKey, localIndex);
        }
        SPData.SmoothData.newBranchesIndices = new List<int>();
        //path points welding
        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)// path points created after chamfer,
        {
            for (int n = i; n < SPData.SmoothData.Nodes.Length; n++)
            {
                if (n == i) continue;
                //UpdateAllLinks
                var t = SplinePlusAPI.ConnectTwoNodes(SPData, SPData.SmoothData.Nodes[n], SPData.SmoothData.Nodes[i]);
                SPData.SmoothData.newBranchesIndices.Add(t);
                BranchesClass.FlipHandles(SPData, t, 0);
                BranchesClass.FlipHandles(SPData, t, 1);
            }
        }
        EditSmoothNodePoint(SPData, SPData.SmoothRadius);
    }

    public void EditSmoothNodePoint(SPData SPData, float radius)
    {
        SPData.SmoothRadius = radius;
        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)
        {
            var branchKey = SPData.SmoothData.BranchesIndices[i];
            var localIndex = SPData.SmoothData.Nodes[i].LocalIndex(SPData, branchKey);

            var t = (localIndex == 0) ? 1 : SPData.DictBranches[branchKey].Nodes.Count - 2;
            SPData.SmoothData.Nodes[i].Point.position = Vector3.Lerp(SPData.SmoothData.InitNodePos,
                SPData.DictBranches[branchKey].Nodes[t].Point.position, SPData.SmoothRadius);

            SPData.SmoothData.Nodes[i].Point1.position = SPData.SmoothData.InitNodePos;
            SPData.SmoothData.Nodes[i].Point2.localPosition = -SPData.SmoothData.Nodes[i].Point1.localPosition;
        }
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }

    public void ResetSmoothSharedPathPoint(SPData SPData)
    {
        SPData.SmoothData.SmoothNode = false;
 
        for (int i = 0; i < SPData.SmoothData.Nodes.Length; i++)
        {
            var branchKey = SPData.SmoothData.BranchesIndices[i];
            var localIndex = SPData.SmoothData.Nodes[i].LocalIndex(SPData, branchKey);

            SPData.DictBranches[branchKey].Nodes[localIndex] = SPData.SharedNodes[SPData.Selections._SharedPathPointIndex].Node;//restore old center path point
        }

        BranchesClass.AddRefreshSharedNode(SPData, SPData.SharedNodes[SPData.Selections._SharedPathPointIndex].Node);

        //delete created branches 
        for (int i = 0; i < SPData.SmoothData.newBranchesIndices.Count; i++)
        {
            BranchesClass.DeleteBranch(SPData, SPData.SmoothData.newBranchesIndices[i]);
        }
 
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }
}
