using System.Collections.Generic;
using UnityEngine;

public class TubeMesh : MonoBehaviour
{
    public Data _Data = new Data();
    public MeshFilter Mesh;
    public MeshRenderer MeshRenderer;
    public Material Material;
    public SplinePlus SplinePlus;
    public bool FlipFaces = false;
    public bool TwoSided = false;

    public float Radius = 0.2f;
    public int Segments = 4;
    public string AssetName = "";
 
    public void DrawMeshOnEachBranch()
    {
        var finalMeshes = new List<CombineInstance>();
        foreach (var branch in SplinePlus.SPData.DictBranches)
        {
            if (branch.Value.Nodes.Count <= 1) continue;
            var temp = new CombineInstance();
            var mesh = DrawMesh(branch.Value, branch.Key);
            temp.mesh = mesh;
            temp.transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;

            finalMeshes.Add(temp);

        }

        var finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalMeshes.ToArray(), true, true);
        finalMesh.RecalculateNormals(60.0f);

        if (TwoSided) finalMesh = FacesSettings.TwoSided(finalMesh);
        if (FlipFaces) finalMesh = FacesSettings.FlipFaces(finalMesh);
        Mesh.sharedMesh = finalMesh;
        MeshRenderer.sharedMaterial = Material;
    }

    public Mesh DrawMesh(Branch branch, int branchKey)
    {
        var rings = branch.Vertices.Count;

        var vertexNumber = ((Segments + 1) * rings);
        _Data.Vertices = new Vector3[vertexNumber];
        _Data.Triangles = new int[(Segments * (rings - 1) * 6)];
        _Data.Uvs = new Vector2[vertexNumber];

        for (int n = 0, t = 0; n < rings; n++)
        {
            var vert = branch.Vertices[n];
            var normal = branch.Normals[n];
            var tangent = branch.Tangents[n];

            Quaternion rot = Quaternion.LookRotation(tangent, normal);

            for (int i = 0; i <= Segments; i++, t++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / (Segments));
                var vertex = new Vector3(Mathf.Cos(rad) * Radius, Mathf.Sin(rad) * Radius, 0);

                var v = vert + rot * vertex;
                _Data.Vertices[t] = transform.InverseTransformPoint(v);
                Uvs(t, branch.VertexDistance[n], i);
            }
        }

        int u = 0;
        for (int n = 0; n < _Data.Vertices.Length - (Segments + 1); n++)
        {
            if (n % (Segments + 1) == 0) continue;
            u = Triangles(n, u);
        }

        return CreateMesh();
    }

    void Uvs(int t, float vertexDist, int i)
    {
        var x = Mathf.InverseLerp(0, Segments, i);
        var y = vertexDist;
        _Data.Uvs[t] = new Vector2(x, y);
    }

    int Triangles(int n, int u)
    {
        _Data.Triangles[u] = n;
        _Data.Triangles[u + 1] = n + Segments;
        _Data.Triangles[u + 2] = n - 1;
        u += 3;

        _Data.Triangles[u] = n;
        _Data.Triangles[u + 1] = n + Segments + 1;
        _Data.Triangles[u + 2] = n + Segments;

        u += 3;
        return u;
    }


    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = _Data.Vertices;
        mesh.triangles = _Data.Triangles;
        mesh.uv = _Data.Uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    [System.Serializable]
    public class Data
    {
        public Vector3[] Vertices;
        public Vector2[] Uvs;
        public int[] Triangles;
    }
}

