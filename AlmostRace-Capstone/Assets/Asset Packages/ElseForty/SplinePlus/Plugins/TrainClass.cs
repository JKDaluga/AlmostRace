using System.Collections.Generic;
using UnityEngine;

public class TrainClass
{

    public struct data
    {
        public Follower wagon;
        public Train train;
        public int i;
    }

    public void Follow(SPData SPData)
    {
        for (int i = 0; i < SPData.Trains.Count; i++)
        {
            var train = SPData.Trains[i];
            if (!train.IsActive) continue;
            if (train.IsForward)
            {
                for (int n = train.Wagons.Count-1; n >=0 ; n--)//subfollowers
                {
                    if (train.Wagons[n].FollowerGO == null) continue;
                    data data;
                    data.train = train;
                    data.wagon = train.Wagons[n];
                    data.i = n;

                    AnimationType(SPData, data);
                }
            }
            else
            {
                for (int n = 0; n < train.Wagons.Count; n++)//subfollowers
                {
                    if (train.Wagons[n].FollowerGO == null) continue;
                    data data;
                    data.train = train;
                    data.wagon = train.Wagons[n];
                    data.i = n;

                    AnimationType(SPData, data);
                }
            }
        }

    }

    public void AnimationType(SPData SPData, data data)
    {
        if (data.train._FollowerAnimation == FollowerAnimation.AutoAnimated)//autoUpdate strict
        {
            AutoAnimated(SPData, data);
        }
        else if (data.train._FollowerAnimation == FollowerAnimation.KeyboardInput)//Keyboard input
        {
            KeyboardAnimationType(SPData, data);
        }

        if (ProgressCheck(SPData, data) && Application.isPlaying)
        {
            AtBranchFork(SPData, data);
        }
    }

    public bool ProgressCheck(SPData SPData, data data)
    {
        var pathLength = SPData.DictBranches[data.wagon._BranchKey].BranchDistance;

        if (!data.wagon.Reverse)
        {
            return (data.wagon.Progress > pathLength) ? true : false;
        }
        else
        {
            return (data.wagon.Progress < 0) ? true : false;
        }
    }

    public void AutoAnimated(SPData SPData, data data)
    {
        if (Application.isPlaying)
        {

            var trainHeadIndex = (data.train.IsForward) ? data.train.Wagons.Count - 1 : 0;
            var trainHead = data.train.Wagons[trainHeadIndex];

            var trainHeadBranch = SPData.DictBranches[trainHead._BranchKey];
            var speedFactor = SPData.ConstantSpeed ? 1 : (trainHeadBranch.SpeedFactor[trainHead.DistanceData.Index] / 100.0f);

            // acceleration 
            float acceleration = 1;
            if (data.train.KeyGravity < data.train.Acceleration)
            {
                data.train.KeyGravity += Time.deltaTime/ data.train.Wagons.Count;
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, 0, data.train.Acceleration);

                var t = Mathf.InverseLerp(0, data.train.Acceleration, data.train.KeyGravity);
                acceleration = Mathf.Lerp(0, 1, t);
            }

            if (!data.train.IsEndRoad)
            {
                if (!data.wagon.Reverse) data.wagon.Progress += data.train.Speed* acceleration * speedFactor * Time.deltaTime;
                else data.wagon.Progress -= data.train.Speed * speedFactor * acceleration * Time.deltaTime;
            }
        }

        //transform assign
        TransformFollower(SPData,data);
    }

    public void KeyboardAnimationType(SPData SPData, data data)
    {
        if (Application.isPlaying)
        {
            var trainHeadIndex = (data.train.IsForward) ? data.train.Wagons.Count - 1 : 0;
            var trainHead = data.train.Wagons[trainHeadIndex];

            var trainHeadBranch = SPData.DictBranches[trainHead._BranchKey];
            var speedFactor = SPData.ConstantSpeed ? 1 : (trainHeadBranch.SpeedFactor[trainHead.DistanceData.Index] / 100.0f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                data.wagon.SpaceEvent.Invoke();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                data.wagon.OnMoveEvent.Invoke();

                InputGravity(data, "forward", trainHeadIndex);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                data.wagon.OnMoveEvent.Invoke();

                InputGravity(data, "backward", trainHeadIndex);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space))
            {
                data.wagon.SpaceEvent.Invoke();

            }
            else if (!Input.GetKey(KeyCode.UpArrow) || !Input.GetKey(KeyCode.DownArrow))
            {
                InputGravity(data, "neutral", trainHeadIndex);
            }


            if (trainHead.Progress < SPData.DictBranches[trainHead._BranchKey].BranchDistance &&
               trainHead.Progress > 0) data.train.IsEndRoad = false;


            if (!data.train.IsEndRoad)
            {
                if (!data.wagon.Reverse) data.wagon.Progress += SPData.KeyboardInputValue * data.train.Speed * speedFactor * Time.deltaTime;
                else data.wagon.Progress -= SPData.KeyboardInputValue * data.train.Speed * speedFactor * Time.deltaTime;
            }
        }
        TransformFollower(SPData,data);
    }

    public static void TransformFollower(SPData SPData,data data)
    {
        data.wagon.DistanceData = DistanceDataClass.DataExtraction(SPData, data.train, data.wagon);

        //transform assign
        data.wagon.FollowerGO.transform.position = data.wagon.DistanceData.Position;
        if (data.wagon.Trans) data.wagon.FollowerGO.transform.Translate(data.wagon.Position);

        data.wagon.FollowerGO.transform.rotation = (data.wagon.Rot) ? data.wagon.DistanceData.Rotation : Quaternion.Euler(data.wagon.Rotation);
        
    }

    public void InputGravity(data data, string state, int trainHeadIndex)
    {
        if (trainHeadIndex != data.i) return;
        if (state == "forward")
        {
            if (data.train.KeyGravity < 0)
            {
                data.train.KeyGravity += (Time.deltaTime * data.train.BrakesForce);
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, -data.train.Acceleration, 0);

                var t = Mathf.InverseLerp(0, -data.train.Acceleration, data.train.KeyGravity);
                SPData.KeyboardInputValue = Mathf.Lerp(0, 1, t);
            }
            else
            {
                if (!data.train.IsForward)
                {
                    for (int i = 0; i < data.train.Wagons.Count; i++)
                    {
                        data.train.Wagons[i].Reverse = !data.train.Wagons[i].Reverse;
                    }
                }
                data.train.IsForward = true;

                data.train.KeyGravity += Time.deltaTime;
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, 0, data.train.Acceleration);

                var t = Mathf.InverseLerp(0, data.train.Acceleration, data.train.KeyGravity);
                SPData.KeyboardInputValue = Mathf.Lerp(0, 1, t);
            }

            if (data.train.Acceleration == 0) SPData.KeyboardInputValue = 1;
        }
        else if (state == "backward")
        {
            if (data.train.KeyGravity > 0)
            {
                data.train.KeyGravity -= (Time.deltaTime * data.train.BrakesForce);
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, 0, data.train.Acceleration);

                var t = Mathf.InverseLerp(0, data.train.Acceleration, data.train.KeyGravity);
                SPData.KeyboardInputValue = Mathf.Lerp(0, 1, t);

            }
            else
            {
                if (data.train.IsForward)
                {
                    for (int i = 0; i < data.train.Wagons.Count; i++)
                    {
                        data.train.Wagons[i].Reverse = !data.train.Wagons[i].Reverse;
                    }
                }
                data.train.IsForward = false;


                data.train.KeyGravity -= Time.deltaTime;
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, -data.train.Acceleration, 0);
                var t = Mathf.InverseLerp(0, -data.train.Acceleration, data.train.KeyGravity  );
                SPData.KeyboardInputValue = Mathf.Lerp(0, 1, t);
            }

            if (data.train.Acceleration == 0) SPData.KeyboardInputValue = 1;
        }
        else
        {
            if (data.train.IsForward && data.train.KeyGravity > 0)
            {
                data.train.KeyGravity -= Time.deltaTime;
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, 0, data.train.Acceleration);
                var t = Mathf.InverseLerp(data.train.Acceleration, 0, data.train.KeyGravity );
                SPData.KeyboardInputValue = Mathf.Lerp(1, 0, t);
            }

            else if (!data.train.IsForward && data.train.KeyGravity < 0)
            {
                data.train.KeyGravity += Time.deltaTime;
                data.train.KeyGravity = Mathf.Clamp(data.train.KeyGravity, -data.train.Acceleration, 0);
                var t = Mathf.InverseLerp(-data.train.Acceleration, 0, data.train.KeyGravity );
                SPData.KeyboardInputValue = Mathf.Lerp(1, 0, t);
            }
        }
        
    }

    public void AtBranchFork(SPData SPData, data data)
    {
        if (SPData.IsLooped)
        {
            var delta = (data.wagon.Reverse ? Mathf.Abs(data.wagon.Progress) : data.wagon.Progress - SPData.DictBranches[data.wagon._BranchKey].BranchDistance);
            data.wagon.Progress = (data.wagon.Reverse ? SPData.DictBranches[data.wagon._BranchKey].BranchDistance - delta : delta);
        }
        else
        {
            var trainHeadIndex = (data.train.IsForward) ? data.train.Wagons.Count - 1 : 0;
            var index = data.wagon.Reverse ? 0 : SPData.DictBranches[data.wagon._BranchKey].Nodes.Count - 1;
            Node lastPathPoint = SPData.DictBranches[data.wagon._BranchKey].Nodes[index];
            for (int n = 0; n < SPData.SharedNodes.Count; n++)
            {
                if (SPData.SharedNodes[n].Node.Equals(lastPathPoint))
                {
                    BranchPicking(SPData, SPData.SharedNodes[n], data);
                    data.train.IsEndRoad = false;
                    return;
                }
            }
            // correct spacing between wagons when train is on a dead end

            data.train.IsEndRoad = true;

            data.train.KeyGravity = 0;
            SPData.KeyboardInputValue = 0;

            var trainHead = data.train.Wagons[trainHeadIndex];
            var trainHeadProgress = trainHead.Progress;
            var trainBranchDist = SPData.DictBranches[trainHead._BranchKey].BranchDistance;

            for (int i = 0; i < data.train.Wagons.Count; i++)
            {
                if (trainHeadProgress > trainBranchDist)
                {
                    var t = data.train.Wagons.Count - 1 - i;
                    data.train.Wagons[t].Progress = trainBranchDist - (data.train.Progress * i) / (data.train.Wagons.Count - 1);
                }
                else if (trainHeadProgress < 0)
                {
                    data.train.Wagons[i].Progress = (data.train.Progress * i) / (data.train.Wagons.Count - 1);
                }
            }
        }
    }

    void BranchPicking(SPData SPData, SharedNode sharedNode, data data)
    {
        List<int> connectedBranches = new List<int>();

        for (int n = 0; n < sharedNode.ConnectedBranches.Count; n++)// filter eligible branches that subFollower can switch to
        {
            var _newBranch = sharedNode.ConnectedBranches[n];
            if (_newBranch == data.wagon._BranchKey) continue;

            if (sharedNode.Node._Type == NodeType.Smooth)//smooth
            {
                if (FollowerClass.IsBranchValid(SPData, data.wagon._BranchKey, _newBranch, sharedNode.Node)
                    && SPData.DictBranches[_newBranch].Nodes.Count > 1)// 
                {
                    connectedBranches.Add(_newBranch);
                }
            }
            else//free
            {
                connectedBranches.Add(_newBranch);
            }
        }

        int branchKey = 0;
        var r = (data.train.IsForward) ? data.train.Wagons.Count - 1 : 0;
        //branch picking decision
        if (data.i == r)
        {
            branchKey = Branch_Picking_SharedNode(SPData, sharedNode, data, connectedBranches);
        }
        else
        {
            if (data.train.IsForward) branchKey = data.train.Wagons[data.i + 1]._BranchKey;
            else branchKey = data.train.Wagons[data.i - 1]._BranchKey;
        }

        SetTrainBranchKey(SPData, sharedNode, data, branchKey);
     }

    public static void SetTrainBranchKey(SPData SPData, SharedNode sharedNode, data data, int branchKey)
    {
        //concerve constant spacing values between wagons
        // this should be caculated before new branch option is selected
        if (data.wagon.Progress > SPData.DictBranches[data.wagon._BranchKey].BranchDistance)
        {
            data.wagon.Delta = data.wagon.Progress - SPData.DictBranches[data.wagon._BranchKey].BranchDistance;
        }
        else if (data.wagon.Progress < 0)
        {
            data.wagon.Delta = -data.wagon.Progress;
        }
        SPData.SplinePlus.EventClass.EventsTriggering(data.wagon, data.wagon._BranchKey, branchKey);

        //set new branch
        data.wagon._BranchKey = branchKey;


        //follower new progress value calculation
        var pathLength = SPData.DictBranches[data.wagon._BranchKey].BranchDistance;
        if (!SPData.DictBranches[data.wagon._BranchKey].Nodes[0].Equals(sharedNode.Node))
        {
            data.wagon.Reverse = true;
            data.wagon.Progress = pathLength - data.wagon.Delta;
        }
        else
        {
            data.wagon.Reverse = false;
            data.wagon.Progress = data.wagon.Delta;
        }

        TransformFollower(SPData, data);
    }

    public int Branch_Picking_SharedNode(SPData SPData, SharedNode sharedNode, data data, List<int> connectedBranches)
    {
        int branchKey = 0;
        var rand = UnityEngine.Random.Range(0, connectedBranches.Count);
        if (sharedNode._Type == SharedNodeType.Defined)// defined shared node type
        {
            branchKey = SPData.SplinePlus.FollowerClass.DefinedSharedNodeType(SPData, connectedBranches, sharedNode, data.wagon);// keyboard sharedPath Point type
        }
        else if (sharedNode._Type == SharedNodeType.Random)
        {
            if (connectedBranches.Count != 0) branchKey = connectedBranches[rand];// random shared node type
            else branchKey = data.wagon._BranchKey;// if no eligible branch is found then pick previous branch back
        }

        //events only for the trainhead
        SPData.SplinePlus.EventClass.EventsTriggering(data.train, data.wagon._BranchKey, branchKey);

        return branchKey;
    }

}
