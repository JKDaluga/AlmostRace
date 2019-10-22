using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowerClass
{
    public void Follow(SPData SPData, Follower follower)
    {
        if (!follower.IsActive || follower.FollowerGO == null || SPData.DictBranches.Count <= 0
            || SPData.DictBranches[follower._BranchKey].Nodes.Count <= 1) return;
        AnimationType(SPData, follower);
    }

    public void Follow(SPData SPData)
    {
        if (SPData.DictBranches.Count <= 0) return;

        for (int i = 0; i < SPData.Followers.Count; i++)
        {
            var follower = SPData.Followers[i];
            if ((!follower.IsActive && Application.isPlaying) || follower.FollowerGO == null
               || SPData.DictBranches[follower._BranchKey].Nodes.Count <= 1) continue;
            AnimationType(SPData, follower);
        }
    }


    public void FollowerProjection(SPData SPData, Follower follower)
    {
        var followerTransform = follower.FollowerGO.transform;
        var groundRayLength = follower.FollowerProjection.GroundRayLength;
        var obstacleRayHeight = follower.FollowerProjection.ObstacleRayHeight;
        var obstacleRayLength = follower.FollowerProjection.ObstacleRayLength;

        follower.FollowerProjection.GroundRayPos = new Vector3(0, -groundRayLength, 0).normalized;

        RaycastHit groundRay, obstacleRay;

        if (Physics.Raycast(follower.DistanceData.Position - follower.FollowerProjection.GroundRayPos * groundRayLength
            , follower.FollowerProjection.GroundRayPos, out groundRay, groundRayLength * 2))
        {
            var u = SPData.DataParent.transform.TransformPoint(groundRay.point);
            followerTransform.position = new Vector3(groundRay.point.x,
                groundRay.point.y + follower.FollowerProjection.FollowerGroundOffset, groundRay.point.z);

            if (follower.FollowerProjection.FollowGroundNormal)
            {
                var biNormal = Vector3.Cross(follower.DistanceData.Tangent, follower.DistanceData.Normal);
                var fw = Vector3.Cross(biNormal, groundRay.normal);
                fw = (!follower.Reverse) ? -fw : fw;
                followerTransform.rotation = Quaternion.LookRotation(fw, groundRay.normal);
            }
            else
            {
                var fw = new Vector3(follower.DistanceData.Tangent.x, 0, follower.DistanceData.Tangent.z);
                fw = (follower.Reverse) ? -fw : fw;
                followerTransform.rotation = Quaternion.LookRotation(fw, Vector3.up);
            }

            follower.FollowerProjection.GroundColDetect = true;
        }
        else
        {
            follower.FollowerProjection.GroundColDetect = false;
        }

        var ObstacleRayCenter = followerTransform.position + followerTransform.up * obstacleRayHeight;
        if (Physics.Raycast(ObstacleRayCenter, followerTransform.forward, out obstacleRay, obstacleRayLength))
        {
            follower.KeyGravity = 0;
            if (!follower.FollowerProjection.ObstacleColDetect)
            {
                follower.IDLEEvent.Invoke();
            }

            follower.FollowerProjection.ObstacleColDetect = true;
        }
        else
        {
            if (follower.FollowerProjection.ObstacleColDetect)
            {
                follower.OnMoveEvent.Invoke();
            }

            follower.FollowerProjection.ObstacleColDetect = false;
        }

        follower.FollowerProjection.FwObstacleRayPos = ObstacleRayCenter
        + followerTransform.forward * obstacleRayLength;
    }


    public void AnimationType(SPData SPData, Follower follower)
    {
        if (follower._FollowerAnimation == FollowerAnimation.AutoAnimated)//AutoAnimated
        {
            AutoAnimated(SPData, follower);
        }
        else if (follower._FollowerAnimation == FollowerAnimation.KeyboardInput)//Keyboard input
        {
            KeyboardAnimationType(SPData, follower);
        }
        else if (follower._FollowerAnimation == FollowerAnimation.SceneClick)//Keyboard input
        {
            if (Input.GetMouseButtonDown(0))
            {
                int branchInd = 0, vertexInd = 0;
                float minDist = float.MaxValue;
                foreach (var branch in SPData.DictBranches)
                {
                    for (int n = 0; n < branch.Value.Vertices.Count; n++)
                    {
                        var dist = Vector2.Distance(Camera.main.WorldToScreenPoint(branch.Value.Vertices[n]), Input.mousePosition);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            branchInd = branch.Key;
                            vertexInd = n;
                        }
                    }
                }

                follower.Progress = SPData.DictBranches[branchInd].VertexDistance[vertexInd];
                follower._BranchKey = branchInd;

                SPData.SplinePlus.PFFollow();
                SPData.SplinePlus.PFFindAllShortestPaths();
            }
            TransformFollower(SPData, follower);
        }

        if (ProgressCheck(SPData, follower) && Application.isPlaying) IsAtBranchFork(SPData, follower);
    }

    public void AutoAnimated(SPData SPData, Follower follower)
    {
        if (Application.isPlaying && !follower.FollowerProjection.ObstacleColDetect)
        {
            var branch = SPData.DictBranches[follower._BranchKey];
            var speedFactor = SPData.ConstantSpeed ? 1 : (branch.SpeedFactor[follower.DistanceData.Index] / 100.0f);

            // acceleration 
            float acceleration = 1;
            if (follower.KeyGravity < follower.Acceleration)
            {
                follower.KeyGravity += Time.deltaTime;
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, 0, follower.Acceleration);

                var t = Mathf.InverseLerp(0, follower.Acceleration, follower.KeyGravity);
                acceleration = Mathf.Lerp(0, 1, t);
            }

            if (!follower.Reverse) follower.Progress += follower.Speed * acceleration * speedFactor * Time.deltaTime;
            else follower.Progress -= follower.Speed * speedFactor * acceleration * Time.deltaTime;
        }
        //transform assign
        TransformFollower(SPData, follower);
    }



    public void IsAtBranchFork(SPData SPData, Follower follower)
    {

        if (SPData.IsLooped)
        {
            var delta = (follower.Reverse ? Mathf.Abs(follower.Progress) : follower.Progress - SPData.DictBranches[follower._BranchKey].BranchDistance);
            follower.Progress = (follower.Reverse ? SPData.DictBranches[follower._BranchKey].BranchDistance - delta : delta);
        }
        else
        {
            var index = follower.Reverse ? 0 : SPData.DictBranches[follower._BranchKey].Nodes.Count - 1;
            Node lastPathPoint = SPData.DictBranches[follower._BranchKey].Nodes[index];

            for (int i = 0; i < SPData.SharedNodes.Count; i++)
            {
                if (SPData.SharedNodes[i].Node.Equals(lastPathPoint))
                {
                    BranchPicking(SPData, SPData.SharedNodes[i], follower);
                    return;
                }
            }
            follower.Reverse = !follower.Reverse;
        }
    }

    public bool ProgressCheck(SPData SPData, Follower follower)
    {
        var pathLength = SPData.DictBranches[follower._BranchKey].BranchDistance;


        if (!follower.Reverse)
        {
            return (follower.Progress > pathLength) ? true : false;
        }
        else
        {
            return (follower.Progress < 0) ? true : false;
        }
    }

    public void KeyboardAnimationType(SPData SPData, Follower follower)
    {
        var branch = SPData.DictBranches[follower._BranchKey];
        float inputValue = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            follower.SpaceEvent.Invoke();
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            follower.OnMoveEvent.Invoke();
            inputValue = InputGravity(follower, "forward");

        }
        else if (Input.GetKey(KeyCode.P))
        {
            if (SPData.FollowerType == FollowerType.PathFinding)
            {
                SPData.SplinePlus.PFFindAllShortestPaths();
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            follower.OnMoveEvent.Invoke();
            inputValue = InputGravity(follower, "backward");
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.Space))
        {
            follower.IDLEEvent.Invoke();
        }
        else if (!Input.GetKey(KeyCode.UpArrow) || !Input.GetKey(KeyCode.DownArrow))
        {
             inputValue = InputGravity(follower, "neutral");
        }
        if (Application.isPlaying && !follower.FollowerProjection.ObstacleColDetect)
        {
            var speedFactor = SPData.ConstantSpeed ? 1 : (branch.SpeedFactor[follower.DistanceData.Index] / 100.0f);
 
            if (follower.Reverse) follower.Progress -= inputValue * follower.Speed * speedFactor * Time.deltaTime;
            else follower.Progress += inputValue * follower.Speed * speedFactor * Time.deltaTime;
        }
        TransformFollower(SPData, follower);
    }


    public void TransformFollower(SPData SPData, Follower follower)
    {
        follower.DistanceData = DistanceDataClass.DataExtraction(SPData, follower);

        //transform assign
        follower.FollowerGO.transform.position = follower.DistanceData.Position;
        if (follower.Trans) follower.FollowerGO.transform.Translate(follower.Position);

        follower.FollowerGO.transform.rotation = (follower.Rot) ? follower.DistanceData.Rotation : Quaternion.Euler(follower.Rotation);


        if (follower.PathFollowingType == PathFollowingType.Projected) FollowerProjection(SPData, follower);
    }

    public float InputGravity(Follower follower, string state)
    {
     
        float inputValue = 0;

        if (state == "forward")
        {
            if (follower.KeyGravity < 0)
            {
                follower.KeyGravity += (Time.deltaTime * follower.BrakesForce);
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, -follower.Acceleration, 0);

                var t = Mathf.InverseLerp(0, -follower.Acceleration, follower.KeyGravity);
                inputValue = Mathf.Lerp(0, 1, t);
            }
            else 
            {
                if (!follower.IsForward) follower.Reverse = !follower.Reverse;
                follower.IsForward = true;
                follower.KeyGravity += Time.deltaTime;
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, 0, follower.Acceleration);

                var t = Mathf.InverseLerp(0, follower.Acceleration, follower.KeyGravity);
                inputValue = Mathf.Lerp(0, 1, t);
            }
            if (follower.Acceleration == 0) return 1;
        }
        else if (state == "backward")
        {
             if (follower.KeyGravity > 0)
            {
                follower.KeyGravity -= (Time.deltaTime * follower.BrakesForce);
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, 0, follower.Acceleration);

                var t = Mathf.InverseLerp(0, follower.Acceleration, follower.KeyGravity);
                inputValue = Mathf.Lerp(0, 1, t);
            }
            else  
            {
               if (follower.IsForward) follower.Reverse = !follower.Reverse;
                 follower.IsForward = false;
                follower.KeyGravity -= Time.deltaTime;
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, -follower.Acceleration, 0);

                var t = Mathf.InverseLerp(0, -follower.Acceleration, follower.KeyGravity);
                inputValue = Mathf.Lerp(0, 1, t);
            }
            if (follower.Acceleration == 0) return 1;
        }
        else
        {
            if (follower.IsForward && follower.KeyGravity > 0)
            {
                follower.KeyGravity -= Time.deltaTime;
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, 0, follower.Acceleration);
               var t = Mathf.InverseLerp(follower.Acceleration, 0, follower.KeyGravity);
               inputValue = Mathf.Lerp(1, 0, t);
            }
       
            else if (!follower.IsForward && follower.KeyGravity < 0)
            {
                follower.KeyGravity += Time.deltaTime;
                follower.KeyGravity = Mathf.Clamp(follower.KeyGravity, -follower.Acceleration, 0);
               var t = Mathf.InverseLerp(-follower.Acceleration, 0, follower.KeyGravity);
               inputValue = Mathf.Lerp(1, 0, t);
            }
        }
        return inputValue;
    }

    public static bool IsBranchValid(SPData SPData, int previousBranchIndex, int newBranchKey, Node node)
    {
        // path point of the shared path point follower is at
        var pathPoint1Ind = node.LocalIndex(SPData, previousBranchIndex);
        var pathPoint2Ind = node.LocalIndex(SPData, newBranchKey);

        Node pathPoint1 = SPData.DictBranches[previousBranchIndex].Nodes[pathPoint1Ind];
        Node pathPoint2 = SPData.DictBranches[newBranchKey].Nodes[pathPoint2Ind];

        if ((pathPoint1Ind == 0 && pathPoint2Ind == 0) || (pathPoint1Ind != 0 && pathPoint2Ind != 0))
        {
            if (pathPoint1.Point1.gameObject != pathPoint2.Point1.gameObject) return true;
            return false;
        }

        if (pathPoint1.Point1.gameObject == pathPoint2.Point1.gameObject) return true;
        return false;

    }

    void BranchPicking(SPData SPData, SharedNode sharedNode, Follower follower)
    {
        List<int> connectedBranches = new List<int>();
        for (int i = 0; i < sharedNode.ConnectedBranches.Count; i++)// filter eligible branches that follower can switch to
        {
            var _newBranch = sharedNode.ConnectedBranches[i];
            if (_newBranch == follower._BranchKey) continue;

            if (sharedNode.Node._Type == NodeType.Smooth) // smooth
            {
                if (IsBranchValid(SPData, follower._BranchKey, _newBranch, sharedNode.Node) && SPData.DictBranches[_newBranch].Nodes.Count > 1)
                {
                    connectedBranches.Add(_newBranch);
                }
            }
            else // free
            {
                connectedBranches.Add(_newBranch);
            }
        }

        //branch picking decision
        Branch_Picking_SharedNode(SPData, sharedNode, follower, connectedBranches);
    }

    public void Branch_Picking_SharedNode(SPData SPData, SharedNode sharedNode, Follower follower, List<int> connectedBranches)
    {

        var rand = UnityEngine.Random.Range(0, connectedBranches.Count);
        int branchKey = 0;

        if (sharedNode._Type == SharedNodeType.Defined)// defined shared node type
        {
            branchKey = DefinedSharedNodeType(SPData, connectedBranches, sharedNode, follower);// keyboard sharedPath Point type
        }
        else if (sharedNode._Type == SharedNodeType.Random)
        {
            if (connectedBranches.Count != 0) branchKey = connectedBranches[rand];// random shared node type
            else branchKey = follower._BranchKey;// if no eligible branch is found then pick previous branch back
        }

        SetFollowerBranchKey(SPData, sharedNode, follower, branchKey);
    }

    public void SetFollowerBranchKey(SPData SPData, SharedNode sharedNode, Follower follower, int branchKey)
    {
        //concerve constant spacing values between wagons
        // this should be caculated before new branch option is selected
        if (follower.Progress > SPData.DictBranches[follower._BranchKey].BranchDistance)
        {
            follower.Delta = follower.Progress - SPData.DictBranches[follower._BranchKey].BranchDistance;
        }
        else if (follower.Progress < 0)
        {
            follower.Delta = -follower.Progress;
        }
        SPData.SplinePlus.EventClass.EventsTriggering(follower, follower._BranchKey, branchKey);

        //set new branch
        follower._BranchKey = branchKey;


        //follower new progress value calculation
        var pathLength = SPData.DictBranches[follower._BranchKey].BranchDistance;
        if (!SPData.DictBranches[follower._BranchKey].Nodes[0].Equals(sharedNode.Node))
        {
            follower.Reverse = true;
            follower.Progress = pathLength - follower.Delta;
        }
        else
        {
            follower.Reverse = false;
            follower.Progress = follower.Delta;
        }

        TransformFollower(SPData, follower);
    }

    public int DefinedSharedNodeType(SPData SPData, List<int> connectedBranches, SharedNode sharedNode, Follower follower)
    {
        //if follower current branch is not _Backward or _Forward or _Left or _Right then pick the more legitimate choice if available
        Func<int> FollowerOnUndefinedBranch = delegate
        {
            var definedBranches = new List<int>();
            if (sharedNode._Left != -1) definedBranches.Add(sharedNode._Left);
            if (sharedNode._Right != -1) definedBranches.Add(sharedNode._Right);
            if (sharedNode._Backward != -1) definedBranches.Add(sharedNode._Backward);
            if (sharedNode._Forward != -1) definedBranches.Add(sharedNode._Forward);


            for (int i = 0; i < definedBranches.Count; i++)
            {
                if (IsBranchValid(SPData, follower._BranchKey, definedBranches[i], sharedNode.Node)) return definedBranches[i];

            }
            return -1;
        };


        int branch = -1;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (follower.IsForward)
            {
                if (follower._BranchKey == sharedNode._Forward) branch = sharedNode._Left;
                else if (follower._BranchKey == sharedNode._Backward) branch = sharedNode._Left;
                else if (follower._BranchKey == sharedNode._Left) branch = sharedNode._Forward;
                else if (follower._BranchKey == sharedNode._Right) branch = sharedNode._Backward;
                else branch = FollowerOnUndefinedBranch();
            }
            else
            {
                if (follower._BranchKey == sharedNode._Forward) branch = sharedNode._Right;
                else if (follower._BranchKey == sharedNode._Backward) branch = sharedNode._Right;
                else if (follower._BranchKey == sharedNode._Left) branch = sharedNode._Forward;
                else if (follower._BranchKey == sharedNode._Right) branch = sharedNode._Backward;
                else branch = FollowerOnUndefinedBranch();
            }


        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (follower.IsForward)
            {
                if (follower._BranchKey == sharedNode._Forward) branch = sharedNode._Right;
                else if (follower._BranchKey == sharedNode._Backward) branch = sharedNode._Right;
                else if (follower._BranchKey == sharedNode._Left) branch = sharedNode._Backward;
                else if (follower._BranchKey == sharedNode._Right) branch = sharedNode._Forward;
                else branch = FollowerOnUndefinedBranch();
            }
            else
            {
                if (follower._BranchKey == sharedNode._Forward) branch = sharedNode._Left;
                else if (follower._BranchKey == sharedNode._Backward) branch = sharedNode._Left;
                else if (follower._BranchKey == sharedNode._Left) branch = sharedNode._Backward;
                else if (follower._BranchKey == sharedNode._Right) branch = sharedNode._Forward;
                else branch = FollowerOnUndefinedBranch();
            }
        }

        else
        {
            if (follower._BranchKey == sharedNode._Forward)
            {
                branch = sharedNode._Backward;
            }
            else if (follower._BranchKey == sharedNode._Backward)
            {
                branch = sharedNode._Forward;
            }
            else if (follower._BranchKey == sharedNode._Left)
            {
                branch = sharedNode._Right;
            }
            else if (follower._BranchKey == sharedNode._Right)
            {
                branch = sharedNode._Left;
            }
            else branch = FollowerOnUndefinedBranch();
        }
        //if no branch choice is defined then pick a random choice if not then return branch value
        if (branch == -1)
        {
            var rand = UnityEngine.Random.Range(0, connectedBranches.Count);
            return connectedBranches[rand];
        }

        if (IsBranchValid(SPData, follower._BranchKey, branch, sharedNode.Node))
        {
            return branch;
        }
        else
        {
            branch = FollowerOnUndefinedBranch();
            return branch;
        }


    }
}
