using System.Linq;

public static class BranchesClass
{
    public static void AddBranch(SPData SPData)
    {
        var node = SPData.Selections._PathPoint;
        if (node.Equals(null)) return;
        SPData.EditSpline = false;
        BreakBranch(SPData, node);

        var newBranch = new Branch();
        newBranch.Nodes.Add(node);

        SPData.DictBranches.Add(SPData.BranchesCount, newBranch);

        SPData.Selections._BranchKey = SPData.BranchesCount;
        SPData.Selections._BranchIndex = SPData.DictBranches.Count - 1;

        SPData.BranchesCount++;

        AddRefreshSharedNode(SPData, node);
        SPData.Selections._PathPoint = new Node();


        BranchWeldSt(SPData);
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }

    public static void DeleteBranch(SPData SPData, int branchKey)
    {
        var branch = SPData.DictBranches[branchKey];
        var index = SPData.SharedNodes.FindIndex(x => x.Node.Equals(branch.Nodes[0]));
        if (index != -1)
        {
            if (SPData.SharedNodes[index].ConnectedBranches.Count <= 2) SPData.SharedNodes.Remove(SPData.SharedNodes[index]);
        }
        index = SPData.SharedNodes.FindIndex(x => x.Node.Equals(branch.Nodes[branch.Nodes.Count - 1]));
        if (index != -1)
        {
            if (SPData.SharedNodes[index].ConnectedBranches.Count <= 2) SPData.SharedNodes.Remove(SPData.SharedNodes[index]);
        }

        SPData.Selections._PathPoint = new Node();
        SPData.Selections._SharedPathPointIndex = -1;

        SPData.DictBranches.Remove(branchKey);


        SPData.Selections._BranchKey = SPData.DictBranches.Keys.LastOrDefault();
        SPData.Selections._BranchIndex = SPData.DictBranches.Count - 1;
        BranchWeldSt(SPData);

        // readdapt shared path points connected branches
        RearrageBranchesIndices(SPData, branchKey);
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }

    public static void BranchWeldSt(SPData SPData)
    {
        if (SPData.DictBranches.Count == 0) return;
        SPData.Selections._BranchWeldState = BranchWeldState.none;
        var branch = SPData.DictBranches[SPData.Selections._BranchKey];
        if (branch.Nodes.Count == 0) return;
        var pathPoint1 = branch.Nodes[0];
        var u = branch.Nodes.Count - 1;
        var pathPoint2 = branch.Nodes[u];


        if (branch.Nodes.Count > 1 && SPData.SharedNodes.Exists(v => v.Node.Equals(pathPoint1) && SPData.SharedNodes.Exists(x => x.Node.Equals(pathPoint2))))
        {
            SPData.Selections._BranchWeldState = BranchWeldState.Both;
        }
        else if (SPData.SharedNodes.Exists(x => x.Node.Equals(pathPoint2))) SPData.Selections._BranchWeldState = BranchWeldState.Last;
        else if (SPData.SharedNodes.Exists(x => x.Node.Equals(pathPoint1))) SPData.Selections._BranchWeldState = BranchWeldState.First;

    }

    static void RearrageBranchesIndices(SPData SPData, int branchKey)
    {
        ConnectedBranches(SPData);
        for (int i = 0; i < SPData.SharedNodes.Count; i++)//refresh all shared path points
        {
            // rearrange keyboard branches indices
            if (SPData.SharedNodes[i]._Backward == branchKey) SPData.SharedNodes[i]._Backward = -1;
            else if (SPData.SharedNodes[i]._Forward == branchKey) SPData.SharedNodes[i]._Forward = -1;
            else if (SPData.SharedNodes[i]._Left == branchKey) SPData.SharedNodes[i]._Left = -1;
            else if (SPData.SharedNodes[i]._Right == branchKey) SPData.SharedNodes[i]._Right = -1;
        }

        //rearrange followers branches
        for (int n = 0; n < SPData.Followers.Count; n++)
        {
            var follower = SPData.Followers[n];
            if (follower._BranchKey == branchKey) follower._BranchKey = SPData.DictBranches.Keys.LastOrDefault();


            //rearrange events
            for (int nn = 0; nn < SPData.Followers[n].Events.Count; nn++)
            {
                for (int i = follower.Events[nn].Conditions.Count - 1; i >= 0; i--)
                {
                    string[] indices = follower.Events[nn].Conditions[i].Split(';');

                    follower.Events[nn].BranchIndexStartStr = indices[0];
                    follower.Events[nn].BranchIndexEndStr = indices[1];

                    var start = int.Parse(follower.Events[nn].BranchIndexStartStr);
                    var end = int.Parse(follower.Events[nn].BranchIndexEndStr);
                    if (start == branchKey || end == branchKey)
                    {
                        follower.Events[nn].Conditions.RemoveAt(i);
                    }
                }

                follower.Events[nn].BranchIndexStartStr = "";
                follower.Events[nn].BranchIndexEndStr = "";
            }
        }

        //rearrange trains branches
        for (int n = 0; n < SPData.Trains.Count; n++)
        {
            var train = SPData.Trains[n];

            if (train._BranchKey == branchKey) train._BranchKey = SPData.DictBranches.Keys.LastOrDefault();
            //update all train wagons branchKey
            for (int u = 0; u < train.Wagons.Count; u++)
            {
                train.Wagons[u]._BranchKey = train._BranchKey;
            }

            //rearrange events
            for (int nn = 0; nn < train.Events.Count; nn++)
            {
                for (int i = train.Events[nn].Conditions.Count - 1; i >= 0; i--)
                {
                    string[] indices = train.Events[nn].Conditions[i].Split(';');

                    train.Events[nn].BranchIndexStartStr = indices[0];
                    train.Events[nn].BranchIndexEndStr = indices[1];

                    var start = int.Parse(train.Events[nn].BranchIndexStartStr);
                    var end = int.Parse(train.Events[nn].BranchIndexEndStr);
                    if (start == branchKey || end == branchKey)
                    {
                        train.Events[nn].Conditions.RemoveAt(i);
                    }
                }

                train.Events[nn].BranchIndexStartStr = "";
                train.Events[nn].BranchIndexEndStr = "";
            }

        }

    }

    public static void BreakBranch(SPData SPData, Node node)
    {
        if (SPData.SharedNodes.Exists(x => x.Node.Equals(node))) return;
        foreach (var branch in SPData.DictBranches)
        {
            var localIndex = node.LocalIndex(SPData, branch.Key);

            if (localIndex != -1)
            {
                if (localIndex == 0 || localIndex == (branch.Value.Nodes.Count - 1)) continue;// avoid breaking branch at first and last path point , it creates additionnal branch element

                var newBranch = new Branch();

                newBranch.Nodes = branch.Value.Nodes.GetRange(localIndex, branch.Value.Nodes.Count - localIndex);
                branch.Value.Nodes = branch.Value.Nodes.GetRange(0, localIndex + 1);

                SPData.DictBranches.Add(SPData.BranchesCount, newBranch);
                SPData.Selections._BranchKey = SPData.BranchesCount;
                SPData.Selections._BranchIndex = SPData.DictBranches.Count - 1;

                SPData.BranchesCount++;

                branch.Value.Nodes[branch.Value.Nodes.Count - 1] = node;
                AddRefreshSharedNode(SPData, node);

                SPData.Selections._PathPoint = new Node();

                BranchWeldSt(SPData);

                break;
            }
        }
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }


    public static void ConnectedBranches(SPData SPData)// update connected branches list of all shared path points
    {
        for (int ii = 0; ii < SPData.SharedNodes.Count; ii++)
        {
            SPData.SharedNodes[ii].ConnectedBranches.Clear();
            foreach (var branch in SPData.DictBranches)
            {
                if (branch.Value.Nodes.Exists(x => x.Equals(SPData.SharedNodes[ii].Node)))
                {
                    if (!SPData.SharedNodes[ii].ConnectedBranches.Contains(branch.Key)) SPData.SharedNodes[ii].ConnectedBranches.Add(branch.Key);
                }
            }
        }
    }

    public static void AddRefreshSharedNode(SPData SPData, Node node)
    {
        var index = SPData.SharedNodes.FindIndex(x => x.Node.Equals(node));
        if (index == -1)//create new shared path point
        {
            SharedNode sharedNodes = new SharedNode();
            sharedNodes.Node = node;
            SPData.SharedNodes.Add(sharedNodes);
        }
        ConnectedBranches(SPData);
    }


    public static void ReverseBranch(SPData SPData, int branchKey)
    {
        var branch = SPData.DictBranches[branchKey];
        branch.Nodes.Reverse();

        for (int i = 0; i < branch.Nodes.Count; i++)
        {
            var temp = branch.Nodes[i].Point2;
            branch.Nodes[i].Point2 = branch.Nodes[i].Point1;
            branch.Nodes[i].Point1 = temp;
        }

        BranchWeldSt(SPData);
        SPData.SplinePlus.SplineCreationClass.UpdateBranch(SPData, branch);
        SPData.SplinePlus.FollowerClass.Follow(SPData);
    }


    public static void FlipHandles(SPData SPData, int branchKey, int nodeIndex)
    {
        if (nodeIndex == -1) return;
        var pathPointOnBranch = SPData.DictBranches[branchKey].Nodes[nodeIndex];
        Node newPathPoint = new Node();

        newPathPoint.Point = pathPointOnBranch.Point;
        newPathPoint.Point1 = pathPointOnBranch.Point2;
        newPathPoint.Point2 = pathPointOnBranch.Point1;

        newPathPoint._Type = pathPointOnBranch._Type;

        newPathPoint.Normal = pathPointOnBranch.Normal;
        newPathPoint.Tangent = pathPointOnBranch.Tangent;

        newPathPoint.SpeedFactor = pathPointOnBranch.SpeedFactor;
        newPathPoint.NormalFactor = pathPointOnBranch.NormalFactor;

        SPData.DictBranches[branchKey].Nodes[nodeIndex] = newPathPoint;
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
    }
}