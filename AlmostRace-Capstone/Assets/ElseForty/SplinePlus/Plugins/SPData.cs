using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum addPathPointPos { Beginning, End, Middle };
public enum FollowerAnimation { AutoAnimated, KeyboardInput, SceneClick };
public enum SharedNodeType { Random, Defined};
public enum RefAxis { X, Y, Z };
public enum NodeType { Free, Smooth };
public enum BranchWeldState { none, First, Last, Both }
public enum SPType { SplinePlus, MeshDeformer, Extrude, PlaneMesh, TubeMesh }
public enum FollowerType { Simple, Train, PathFinding }
public enum FollowerSettingsType { Follower, Agent, Train  }
public enum UpdateType { None, SharedNodes }
public enum PathFollowingType { Strict, Projected }



[System.Serializable]
public class SPData : ISerializationCallbackReceiver
{
    public RefAxis ReferenceAxis = RefAxis.Y;
    public SPType ObjectType;
    public FollowerType FollowerType;
    public NodeType NodeType = NodeType.Smooth;
   

    public List<Follower> Followers = new List<Follower>();
    public List<Train> Trains = new List<Train>();
    public List<PathFindingGoal> PFFollowers = new List<PathFindingGoal>();
    public List<SharedNode> SharedNodes = new List<SharedNode>();

    public SmoothData SmoothData;
    public Selections Selections;
    public SharedSettings SharedSettings;


    public int Smoothness = 20;
    public int PathPointCount = 0;
    public int BranchesCount = 0;
    public int BranchesSelectionCount = 0;

    public float Offset;
    public static float KeyboardInputValue=0;
    public float RaycastLength = 30;
    public float SmoothRadius = 0.2f;

    public float ElapsedTime;

    public bool InterpolateRotation = true;
    public bool SplineProjection = false;
    public bool HandlesProjection = false;
    public bool MeshOrientation = false;
    public bool ConstantSpeed = false;
    public bool EditSpline = false;
    public bool ShowNodeSettings = false;
    public bool ShowProjectionSettings = true;
    public bool ShowSplineSettings = true;
    public bool ShowEvents = true;
    public bool IsEditingPivot = false;
    public bool IsLooped = false;
    public bool Is2D = false;

    public Vector3 Pivot;
    public SplinePlus AttachedSplinePlus;
    public GameObject DataParent;
    public SplinePlus SplinePlus;

    public Dictionary<int, Branch> DictBranches = new Dictionary<int, Branch>();

    [SerializeField]
    List<int> Keys = new List<int>();
    [SerializeField]
    List<Branch> Values = new List<Branch>();

    public void OnBeforeSerialize()
    {
        Keys.Clear();
        Values.Clear();

        foreach (var branch in DictBranches)
        {
            Keys.Add(branch.Key);
            Values.Add(branch.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        DictBranches = new Dictionary<int, Branch>();

        for (int i = 0; i != Math.Min(Keys.Count, Values.Count); i++)
            DictBranches.Add(Keys[i], Values[i]);
    }
}
[System.Serializable]
public class SharedSettings
{
    public bool ShowHelper = false;
    public bool ShowRaycast = false;
    public bool ShowGizmos = true;
    public bool ShowSecondaryHandles = true;
    public float HelperSize = 1.0f;
    public float GizmosSize = 0.2f;

    public Color StandardPathPointColor;
    public Color RandomSharedNodeColor;
    public Color DefinedSharedNodeColor;
    public Color KeyboardSharedPathPointColor;
}



[System.Serializable]
public class Selections
{
    public int _BranchKey = 0;
    public int _BranchIndex = 0;
    public int _Follower = 0;
    public int _Agent = 0;
    public int _SharedPathPointIndex = -1;
    public BranchWeldState _BranchWeldState = BranchWeldState.none;
    public Node _PathPoint = new Node();
    public int _LocalNodeIndex;
}

[System.Serializable]
public class Branch
{
    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> Tangents = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();

    public List<Node> Nodes = new List<Node>();
    public List<float> SpeedFactor = new List<float>();
    public List<float> VertexDistance = new List<float>();
    public float BranchDistance = 0;
}

[System.Serializable]
public class SharedNode
{
    public Node Node = new Node();
    public List<int> ConnectedBranches = new List<int>();
    public SharedNodeType _Type = SharedNodeType.Random;
    public int _Left = -1;
    public int _Right = -1;
    public int _Forward = -1;
    public int _Backward = -1;
}
[System.Serializable]
public class DistanceData
{
    public int Index = 0;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Tangent;
    public Vector3 Normal;
}

[System.Serializable]
public class Follower
{
    public int _BranchKey = 0;
    public GameObject FollowerGO;

    public float Progress = 0;
    public float Acceleration = 0.01f;
    public float BrakesForce = 1;
    public float UpdateTime = 0.1f;

    public FollowerAnimation _FollowerAnimation = FollowerAnimation.AutoAnimated;
    public UpdateType UpdateType = UpdateType.None;
    public PathFollowingType PathFollowingType = PathFollowingType.Strict;

    public Vector3 Position;
    public Vector3 Rotation;

    public float Speed = 2.5f;
    public float KeyGravity = 0;
    public float MinDistance = 30;
    public float OnAwakeDelayTime = 0;
    public float Delta = 0;

    public bool IsForward = true;
    public bool Reverse = false;
    public bool Rot = true;
    public bool Trans = false;
    public bool FlipDirection = true;
    public bool IsActive = false;
    public bool AnimationEvents = false;
    public bool Show = false;

    public bool GoalReached = false;
    public bool GoalFound = false;

    public bool IsMinDist = false;
    public bool ConsiderTangents = true;

    public DistanceData DistanceData = new DistanceData();
    public UnityEvent OnMoveEvent = new UnityEvent();
    public UnityEvent IDLEEvent = new UnityEvent();
    public UnityEvent SpaceEvent = new UnityEvent();
    public UnityEvent OnAwakeEvent = new UnityEvent();

    public FollowerProjection FollowerProjection = new FollowerProjection() ;

    public List<SplinePlusEvent> Events = new List<SplinePlusEvent>();
    public List<PathFindingPathPoint> PathFindingPath = new List<PathFindingPathPoint>();

  
}


[System.Serializable]
public class PathFindingGoal
{
    public Follower Goal;
    public List<Follower> Agents = new List<Follower>();
    public bool Show = false;
}


[System.Serializable]
public class Train
{
    public string Name = "Your name here";
    public List<Follower> Wagons = new List<Follower>();
    public float Step = 0;

    public bool Show = false;
    public bool IsForward = true;
    public bool IsActive = false;
    public bool IsEndRoad = false;
    public bool AnimationEvents = false;


    public int _BranchKey = 0;
    public float Progress = 0;
    public float Speed = 2.5f;
    public float KeyGravity = 0;

    public float OnAwakeDelayTime = 0;
    public float Acceleration = 0;
    public float BrakesForce = 1;

    public FollowerAnimation _FollowerAnimation = FollowerAnimation.AutoAnimated;

    public Vector3 Position;
    public Vector3 Rotation;

    public UnityEvent OnMoveEvent = new UnityEvent();
    public UnityEvent IDLEEvent = new UnityEvent();
    public UnityEvent SpaceEvent = new UnityEvent();
    public UnityEvent OnAwakeEvent = new UnityEvent();

    public List<SplinePlusEvent> Events = new List<SplinePlusEvent>();
}


[System.Serializable]
public class Node
{
    public Transform Point;
    public Transform Point1;
    public Transform Point2;
    public NodeType _Type = NodeType.Smooth;

    public int SpeedFactor = 100;
    public int NormalFactor = 0;
    public Vector3 Normal;
    public Vector3 Tangent;

    public int LocalIndex(SPData SPData, int branchKey)
    {
        for (int n = 0; n < SPData.DictBranches[branchKey].Nodes.Count; n++)
        {
            if (SPData.DictBranches[branchKey].Nodes[n].Equals(this))
            {
                return n;
            }
        }
        return -1;
    }

    public override bool Equals(object obj)
    {
        if (obj == null && (this.Point == null)) return true;
        if ((obj == null) || !(obj is Node)) return false;

        var node = ((Node)obj);
        return Node.Equals(this.Point.gameObject, node.Point.gameObject);
    }

    public override int GetHashCode()
    {
        return Point.GetHashCode();
    }
}

[System.Serializable]
public class SplinePlusEvent
{
    public string EventName = "Your event name here";
    public UnityEvent MyEvents = new UnityEvent();
    public string BranchIndexEndStr;
    public string BranchIndexStartStr;
    public int _Condition;
    public List<string> Conditions = new List<string>();
    public bool AnimationEvents = true;

}

[System.Serializable]
public class SmoothData
{
    public bool IsShared = false;
    public bool SmoothNode = false;
    public Vector3 InitNodePos = new Vector3();
    public Vector3 InitNodePoint1Pos = new Vector3();
    public Node[] Nodes = new Node[0];
    public int[] BranchesIndices = new int[0];
    public List<int> newBranchesIndices = new List<int>();

    public bool[] FlippedPathPoint = new bool[0];
    public int InitBranchesCount = 0;
}

[System.Serializable]
public class PathFindingPathPoint
{
    public Node Curr;
    public int BranchIndex;
}

[System.Serializable]
public class FollowerProjection
{
    public float GroundRayLength=10;
    public float ObstacleRayLength = 10;
    public float FollowerGroundOffset=1;
    public float ObstacleRayHeight=1;

    public Vector3 FwObstacleRayPos ;
    public Vector3 GroundRayPos ;
 
    public bool GroundColDetect;
    public bool ObstacleColDetect;

    public bool FollowGroundNormal;
}


