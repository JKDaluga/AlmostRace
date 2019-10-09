using UnityEngine;

public static class DistanceDataClass
{
    public static DistanceData DataExtraction(  SPData SPData,   Follower follower)
    {
        int f = 1;
        Branch branch = SPData.DictBranches[follower._BranchKey];

     
        if (!follower.FlipDirection)
        {
            if (follower.IsForward) f = follower.Reverse ? 1 : -1;
            else f = follower.Reverse ? -1 : 1;
        }
        else
        {
            f = follower.Reverse ? -1 : 1;
        }

        follower.DistanceData.Position = Vector3.zero;
        follower.DistanceData.Rotation = Quaternion.identity;
        follower.DistanceData.Index = 0;

        for (int i = branch.VertexDistance.Count - 2; i >= 0; i--)
        {
            if (follower.Progress >= branch.VertexDistance[i])
            {
                follower.DistanceData.Index = i;
                break;
            }
        }

        var a = follower.DistanceData.Index;
        var b = follower.DistanceData.Index + 1;

        var vertexA = branch.Vertices[a];
        var vertexB = branch.Vertices[b];

        var tangentA = branch.Tangents[a];
        var tangentB = branch.Tangents[b];

        var normalA = branch.Normals[a];
        var normalB = branch.Normals[b];

        var vertexDistanceA = branch.VertexDistance[a];
        var vertexDistanceB = branch.VertexDistance[b];

        var EdgeProgress = Mathf.InverseLerp(vertexDistanceA, vertexDistanceB, follower.Progress);

        var pos = Vector3.Lerp(vertexA, vertexB, EdgeProgress);
        follower.DistanceData.Position =  (follower.Trans)?  pos : (pos + follower.Position);


        follower.DistanceData.Normal = Vector3.Lerp(normalA, normalB, EdgeProgress);
        follower.DistanceData.Tangent = Vector3.Lerp(tangentA, tangentB, EdgeProgress);

        if (SPData.InterpolateRotation)
        {
            Quaternion FirstNodeRot = (SPData.Is2D) ? Quaternion.LookRotation(normalA, f * tangentA)
                : Quaternion.LookRotation(f * tangentA, normalA);
            Quaternion SecondNodeRot = (SPData.Is2D) ? Quaternion.LookRotation(normalB, f * tangentB)
                : Quaternion.LookRotation(f * tangentB, normalB);
            follower.DistanceData.Rotation = Quaternion.Lerp(FirstNodeRot, SecondNodeRot, EdgeProgress);
        }
        else
        {
            var dir = vertexB - vertexA;
            follower.DistanceData.Rotation = (SPData.Is2D) ? Quaternion.LookRotation(normalA, f * dir) : Quaternion.LookRotation(f * dir, normalA);
        }
        follower.DistanceData.Rotation *= Quaternion.Euler(follower.Rotation);
        return follower.DistanceData;
    }

    public static DistanceData DataExtraction( SPData SPData,  Train train,  Follower wagen)
    {
        int f = 1;
        Branch branch = SPData.DictBranches[wagen._BranchKey];

        if (train.IsForward) f = wagen.Reverse ? 1 : -1;
        else f = wagen.Reverse ? -1 : 1;

        wagen.DistanceData.Position = Vector3.zero;
        wagen.DistanceData.Rotation = Quaternion.identity;
        wagen.DistanceData.Index = 0;

        for (int i = branch.VertexDistance.Count - 2; i >= 0; i--)
        {
            if (wagen.Progress >= branch.VertexDistance[i])
            {
                wagen.DistanceData.Index = i;
                break;
            }
        }

        var a = wagen.DistanceData.Index;
        var b = wagen.DistanceData.Index + 1;

        var vertexA = branch.Vertices[a];
        var vertexB = branch.Vertices[b];

        var tangentA = branch.Tangents[a];
        var tangentB = branch.Tangents[b];


        var normalA = branch.Normals[a];
        var normalB = branch.Normals[b];

        var vertexDistanceA = branch.VertexDistance[a];
        var vertexDistanceB = branch.VertexDistance[b];

        var EdgeProgress = Mathf.InverseLerp(vertexDistanceA, vertexDistanceB, wagen.Progress);

        var pos = Vector3.Lerp(vertexA, vertexB, EdgeProgress);
        wagen.DistanceData.Position = (wagen.Trans) ? pos : (pos + wagen.Position);

        if (SPData.InterpolateRotation)
        {

            Quaternion FirstNodeRot = (SPData.Is2D) ? Quaternion.LookRotation(normalA, f * tangentA) : Quaternion.LookRotation(f * tangentA, normalA);
            Quaternion SecondNodeRot = (SPData.Is2D) ? Quaternion.LookRotation(normalB, f * tangentB) : Quaternion.LookRotation(f * tangentB, normalB);
            wagen.DistanceData.Rotation = Quaternion.Lerp(FirstNodeRot, SecondNodeRot, EdgeProgress);
        }
        else
        {
            var dir = vertexB - vertexA;
            wagen.DistanceData.Rotation = (SPData.Is2D) ? Quaternion.LookRotation(normalA, f * dir) : Quaternion.LookRotation(f * dir, normalA);
        }
        wagen.DistanceData.Rotation *= Quaternion.Euler(wagen.Rotation);
        return wagen.DistanceData;
    }


    public static DistanceData DataExtraction(Branch branch, float progress = 0)
    {
        DistanceData distanceData = new DistanceData();

        distanceData.Position = Vector3.zero;
        distanceData.Rotation = Quaternion.identity;
        distanceData.Index = 0;
        for (int i = branch.VertexDistance.Count - 2; i >= 0; i--)
        {
            if (progress >= branch.VertexDistance[i])
            {
                distanceData.Index = i;
                break;
            }
        }

        var a = distanceData.Index;
        var b = distanceData.Index + 1;

        var vertexA = branch.Vertices[a];
        var vertexB = branch.Vertices[b];

        var tangentA = branch.Tangents[a];
        var tangentB = branch.Tangents[b];


        var normalA = branch.Normals[a];
        var normalB = branch.Normals[b];

        var vertexDistanceA = branch.VertexDistance[a];
        var vertexDistanceB = branch.VertexDistance[b];

        var EdgeProgress = Mathf.InverseLerp(vertexDistanceA, vertexDistanceB, progress);
        distanceData.Position = Vector3.Lerp(vertexA, vertexB, EdgeProgress);


        Quaternion FirstNodeRot = Quaternion.LookRotation(tangentA, normalA);
        Quaternion SecondNodeRot = Quaternion.LookRotation(tangentB, normalB);
        distanceData.Rotation = Quaternion.Lerp(FirstNodeRot, SecondNodeRot, EdgeProgress);
        return distanceData;
    }
}
