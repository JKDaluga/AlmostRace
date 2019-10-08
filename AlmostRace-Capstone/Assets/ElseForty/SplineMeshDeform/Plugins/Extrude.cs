using System.Collections.Generic;
using UnityEngine;


public class Extrude : MonoBehaviour
{
    public List<CapVIndices> CapVIndices = new List<CapVIndices>();
    public List<int> CapVerticesIndices;

    public Material Material;
    
    public MeshFilter Mesh = null;
    public MeshRenderer MeshRenderer = null;

    public bool CapHoles = false;
    public bool TwoSided = false;
    public bool FlipFaces;
    public bool Shell = false;

    public string AssetName;

    public float Height = 10;
    public int Rings = 0;
    public float CurvaturePower = 1.0f;
    public float ShellPower = 1;

    public AnimationCurve Curvature = AnimationCurve.Linear(0, 0, 1, 0);

    public SplinePlus SplinePlus;
    public Extrusion Extrusion;

    public Body BodyData = new Body();
    public Cap CapData = new Cap();

    public Extrude()
    {
        Extrusion = SplinePlus.CreateInstance(Extrusion);
    }

    public void DrawMeshOnEachBranch()
    {
        CombineInstance[] finalMeshes = new CombineInstance[ SplinePlus.SPData.DictBranches.Count];
        foreach (var branch in  SplinePlus.SPData.DictBranches)
        {
            var mesh = DrawMesh(branch.Value);
            finalMeshes[branch.Key].mesh = mesh;
            finalMeshes[branch.Key].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;
        }

        var finalMesh = new Mesh();

        finalMesh.CombineMeshes(finalMeshes, true, true);
        finalMesh.RecalculateNormals(60.0f);

         if (TwoSided)
        {
            var tempMesh = FacesSettings.TwoSided(finalMesh);
            finalMesh = tempMesh;
        }
        if (FlipFaces) finalMesh = FacesSettings.FlipFaces(finalMesh);
        Mesh.sharedMesh = finalMesh;
    }



    public Mesh DrawMesh(Branch branch)
    {
        if (branch.Vertices.Count > 1) Extrusion.Body(this, branch);
        return CreateMesh(branch);
    }


    public Mesh CreateMesh(Branch branch)
    {
        Mesh returnedMesh = new Mesh();
        Mesh body1 = new Mesh(), body2 = new Mesh();

        body1.vertices = BodyData.Vertices;
        body1.triangles = BodyData.Triangles;
        body1.tangents = BodyData.Tangents;
        body1.normals = BodyData.Normals;
        body1.uv = BodyData.Uvs;

        if (Shell)
        {
            body2.vertices = BodyData.Vertices;
            body2.tangents = BodyData.Tangents;
            body2.normals = BodyData.Normals;
            body2.triangles = BodyData.Triangles;
            body2.uv = BodyData.Uvs;

            var vert = new Vector3[body2.vertices.Length];

            for (int n = 0, t = 0; n <= Rings + 1; n++)
            {
                for (int i = 0; i < branch.Vertices.Count; i++, t++)
                {
                    vert[t] = body2.vertices[t] + body2.normals[t] * ShellPower;
                }
            }

            body2.vertices = vert;
            body1 = FacesSettings.FlipFaces(body1);
        }

        if (CapHoles)
        {
            Mesh cap1 = new Mesh(), cap2 = new Mesh();
            cap1.vertices = CapData.Vertices;
            cap1.triangles = CapData.Triangles;
            cap1.tangents = CapData.Tangents;
            cap1.uv = CapData.Uvs;

            cap2.vertices = CapData.Vertices;
            cap2.triangles = CapData.Triangles;
            cap2.tangents = CapData.Tangents;
            cap2.uv = CapData.Uvs;


            var vert = new Vector3[cap2.vertices.Length];

            for (int i = 0, n = 0; i < CapData.Vertices.Length && n < branch.Vertices.Count; i = i + 2, n++)
            {
                var normal = Vector3.Cross(branch.Tangents[n], branch.Normals[n]).normalized;
                vert[i] = cap1.vertices[i] + Vector3.up * Height + normal * Curvature.Evaluate(1) - normal * Curvature.Evaluate(0);
                if ((i + 1) < CapData.Vertices.Length) vert[i + 1] = cap1.vertices[i + 1] + Vector3.up * Height + normal * Curvature.Evaluate(1) - normal * Curvature.Evaluate(0);
            }

            cap2.vertices = vert;
            cap2 = FacesSettings.FlipFaces(cap2);

            CombineInstance[] bodyMesh = new CombineInstance[2];

            bodyMesh[0].mesh = body1;
            bodyMesh[0].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

            bodyMesh[1].mesh = body2;
            bodyMesh[1].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

            Mesh bodyComMesh = new Mesh();
            bodyComMesh.CombineMeshes(bodyMesh, true);

            CombineInstance[] capMesh = new CombineInstance[2];
            capMesh[0].mesh = cap1;
            capMesh[0].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

            capMesh[1].mesh = cap2;
            capMesh[1].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;


            Mesh capComMesh = new Mesh();
            capComMesh.CombineMeshes(capMesh, true);

            CombineInstance[] totalMesh = new CombineInstance[2];

            totalMesh[0].mesh = bodyComMesh;
            totalMesh[0].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

            totalMesh[1].mesh = capComMesh;
            totalMesh[1].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

            Mesh totalComMesh = new Mesh();
            totalComMesh.CombineMeshes(totalMesh, true);

            totalComMesh.RecalculateNormals();
            returnedMesh = totalComMesh;
        }
        else
        {
            if (Shell)
            {
                CombineInstance[] totalMesh = new CombineInstance[2];

                totalMesh[0].mesh = body1;
                totalMesh[0].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;


                totalMesh[1].mesh = body2;
                totalMesh[1].transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

                Mesh combinedAllMesh = new Mesh();
                combinedAllMesh.CombineMeshes(totalMesh, true);

                combinedAllMesh.RecalculateNormals();
                returnedMesh = combinedAllMesh;
            }
            else
            {
                body1.RecalculateNormals();
                returnedMesh = body1;
            }
        }
        MeshRenderer.sharedMaterial = Material;

        return returnedMesh;
    }
}

[System.Serializable]
public class Body
{
    public int VertexNumber;
    public int segments = 5;

    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] Uvs;
    public Vector3[] Normals;
    public Vector4[] Tangents;

}

[System.Serializable]
public class Cap
{
    public int VertexNumber;
    public int segments = 5;
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] Uvs;
    public Vector4[] Tangents;

    public GameObject Orient;
}

[System.Serializable]
public class CapVIndices
{
    public List<int> vIndices = new List<int>();
}


