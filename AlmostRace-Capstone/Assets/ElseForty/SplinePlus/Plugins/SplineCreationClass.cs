using System.Reflection;
using UnityEngine;

public class SplineCreationClass
{
    public void UpdateAllBranches(SPData SPData)
    {
        foreach (var branch in SPData.DictBranches)
        {
            if (SPData.SplineProjection) SPData.SplinePlus.ProjectionClass.Raycasts(  SPData,branch.Value);
            UpdateBranch(  SPData, branch.Value);
        }
        SPData.SplinePlus.FollowerClass.Follow(SPData);
        SPData.SplinePlus.TrainClass.Follow(SPData);
        SPData.SplinePlus.PFFollow();

        UpdateComponents(SPData);
    }

    public void UpdateBranch(  SPData SPData, Branch branch)
    {
        if (SPData.DictBranches.Count == 0) return;
        branch.Vertices.Clear();
        branch.Tangents.Clear();
        branch.Normals.Clear();
        branch.SpeedFactor.Clear();
        branch.VertexDistance.Clear();

        branch.BranchDistance = 0;
        for (int j = 0; j < branch.Nodes.Count - 1; j++)
        {
            var n = (j > 0) ? 1 : 0;
            CubicBezier(SPData,branch, n,branch.Nodes[j + 1], branch.Nodes[j]);
        }

        if (SPData.IsLooped) CubicBezier(SPData,branch, 1,branch.Nodes[0],
          branch.Nodes[branch.Nodes.Count - 1]);
    }

    public void CubicBezier(SPData SPData, Branch branch, int u, Node pointA, Node pointB)
    {
        Vector3 vertex = Vector3.zero;
        Vector3 tangent = Vector3.zero;
        Vector3 normal = Vector3.up;
        Quaternion quaternion = Quaternion.identity;

        Vector3 mid = Vector3.Lerp(pointB.Point.position, pointA.Point.position, 0.5f);
        Vector3 _pointA1 = pointA.Point1.position;
        Vector3 _pointB2 = pointB.Point2.position;
        float speedFactor = 0;
        float t = 0;

        if (pointA._Type == NodeType.Free) _pointA1 = mid;
        if (pointB._Type == NodeType.Free) _pointB2 = mid;

        for (int i = u; i < SPData.Smoothness; i++)
        {
            t = Mathf.InverseLerp(0, SPData.Smoothness - 1, i);

            vertex = CalculateCubicBezier(t, pointB.Point.position, _pointB2,
                                  _pointA1, pointA.Point.position);
            tangent = CalculateTangent(t, pointB.Point.position, _pointB2,
                                                      _pointA1, pointA.Point.position);
            speedFactor = Mathf.Lerp(pointB.SpeedFactor, pointA.SpeedFactor, t);


            var angle = Mathf.Lerp(pointB.NormalFactor, pointA.NormalFactor, t);
            if (!SPData.MeshOrientation)
            {
                var dir = Vector3.up;
                if (SPData.ReferenceAxis == RefAxis.X) dir = Vector3.right;
                else if (SPData.ReferenceAxis == RefAxis.Y) dir = Vector3.up;
                else if (SPData.ReferenceAxis == RefAxis.Z) dir = Vector3.forward;

                Vector3 biNormal = Vector3.Cross(dir, tangent);
                var n = Vector3.Cross(tangent, biNormal).normalized;
                normal = Quaternion.AngleAxis(angle, tangent) * n;
            }
            else
            {
                normal = Vector3.Lerp(pointB.Normal, pointA.Normal, t);
            }

            if (branch.Vertices.Count > 0)
            {
                branch.BranchDistance += Vector3.Distance(vertex, branch.Vertices[branch.Vertices.Count - 1]);
            }

            branch.Vertices.Add(vertex);
            branch.Normals.Add(normal);
            branch.Tangents.Add(tangent);
 
            branch.SpeedFactor.Add(speedFactor);
            branch.VertexDistance.Add(branch.BranchDistance);

            //cach path point tangent and normal
            if (i == u)
            {
                pointB.Normal = normal;
                pointB.Tangent = tangent;
            }
            else if (i == (SPData.Smoothness - 1))
            {
                pointA.Normal = normal;
                pointA.Tangent = tangent;
            }
        }
    }

    public void UpdateComponents(SPData SPData)
    {
        if (SPData.ObjectType == SPType.MeshDeformer)
        {
            FindSubPackages(SPData, "DeformedMesh");
        }
        else if (SPData.ObjectType == SPType.PlaneMesh)
        {
            FindSubPackages(SPData, "PlaneMesh");
        }
        else if (SPData.ObjectType == SPType.TubeMesh)
        {
            FindSubPackages(SPData, "TubeMesh");
        }
        else if (SPData.ObjectType == SPType.Extrude)
        {
            FindSubPackages(SPData, "Extrude");
        }
    }

    void FindSubPackages(SPData SPData, string type)
    {
        Component myType = null;
        myType = SPData.DataParent.GetComponent(type);

        if (myType != null)
        {
            MethodInfo myMethod = myType.GetType().GetMethod("DrawMeshOnEachBranch");
            if (myMethod != null) myMethod.Invoke(myType, new object[] { });
        }
        else
        {
            SPData.ObjectType = SPType.SplinePlus;
            MonoBehaviour.DestroyImmediate(SPData.DataParent.GetComponent<MeshFilter>());
            MonoBehaviour.DestroyImmediate(SPData.DataParent.GetComponent<MeshRenderer>());
        }
    }

    private Vector3 CalculateCubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var u = 1 - t;
        var uu = u * u;
        var uuu = u * uu;
        var tt = t * t;
        var ttt = t * tt;
        var p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    private Vector3 CalculateTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var tt = t * t;
        var u = (1 - t);
        var p = -u * u * p0;
        p += (3 * tt - 4 * t + 1) * p1;
        p += (-3 * tt + 2 * t) * p2;
        p += tt * p3;

        return p.normalized;
    }
}
