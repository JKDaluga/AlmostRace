using System.Collections.Generic;
using UnityEngine;


public class PlaneMesh : MonoBehaviour
{
    public List<BranchData> BranchData = new List<BranchData>();

    public Data Data = new Data();
    public MeshFilter Mesh;
    public MeshRenderer MeshRenderer;
    public Material Material;
    public SplinePlus SplinePlus;
    public bool FlipFaces = false;
    public bool TwoSided = false;

    public float MaxWidth = 1f;
    public float MeshDecal = 0;

    public string AssetName = "";
    int j;

    public bool IsBrushTextureType = false;

    public PlaneMesh()
    {
        Data = SplinePlus.CreateInstance(Data);
    }

    public float WidthValue(int vertexIndex, Branch branch, int branchKey)
    {
       
        return BranchData[branchKey].Curve.Evaluate(Mathf.InverseLerp(0, branch.Vertices.Count, vertexIndex)) * MaxWidth;
    }
    public void DrawMeshOnEachBranch()
    {
        if (BranchData.Count <  SplinePlus.SPData.DictBranches.Count)
        {
            for (int n = BranchData.Count; n <  SplinePlus.SPData.DictBranches.Count; n++)
            {
                BranchData.Add(new BranchData());
            }
        }
        else if (BranchData.Count >  SplinePlus.SPData.DictBranches.Count)
        {
            BranchData.RemoveRange( SplinePlus.SPData.DictBranches.Count, (BranchData.Count -  SplinePlus.SPData.DictBranches.Count));
        }

        List<CombineInstance> finalMeshes = new List<CombineInstance>();
        int i = 0;
        foreach (var branch in SplinePlus.SPData.DictBranches)
        {
            if (branch.Value.Nodes.Count > 1)
            {
                var temp = new CombineInstance();

                var mesh = DrawMesh(branch.Value,i);
                temp.mesh = mesh;
                temp.transform = transform.worldToLocalMatrix;

                finalMeshes.Add(temp);

            }
            i++;
        }

        var finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalMeshes.ToArray(), true, true);


        if (TwoSided) finalMesh = FacesSettings.TwoSided(finalMesh);
        if (FlipFaces) finalMesh = FacesSettings.FlipFaces(finalMesh);
        Mesh.sharedMesh = finalMesh;
        MeshRenderer.sharedMaterial = Material;
    }

    public Mesh DrawMesh(Branch branch, int branchKey)
    {
        var meshDecal = BranchData[branchKey].BranchMeshDecal;
        Data.Vertices.Clear();
        Data.Normals.Clear();
        Data.Tangents.Clear();


        for (int i = 0; i < branch.Vertices.Count; i++)//vertices normals tangents
        {
            var width = WidthValue(i,branch, branchKey);

            var vertex1 = (branch.Vertices[i] + Vector3.Cross(branch.Tangents[i], branch.Normals[i]) * width) + branch.Normals[i] * meshDecal;
            var vertex2 = (branch.Vertices[i] + Vector3.Cross(branch.Tangents[i], branch.Normals[i]) * -width) + branch.Normals[i] * meshDecal;

            Data.Vertices.Add(vertex1);
            Data.Vertices.Add(vertex2);

            Data.Normals.Add(branch.Normals[i]);
            Data.Normals.Add(branch.Normals[i]);

            Data.Tangents.Add(branch.Tangents[i]);
            Data.Tangents.Add(branch.Tangents[i]);
        }

        if (Data.Vertices.Count < 4) return null;
        Triangles();
        Uvs(branch);

        return CreateMesh();
    }

    public void Uvs(Branch branch)
    {
        if (IsBrushTextureType)
        {
            Data.Uvs = new Vector2[Data.Vertices.Count];

            for (int i = 0; i < Data.Uvs.Length; i++)
            {
                if (Data.Uvs.Length > 3 && i == 0)
                {
                    Data.Uvs[0] = new Vector2(0, 1f);
                    Data.Uvs[1] = new Vector2(0, 0f);
                    Data.Uvs[2] = new Vector2(0.5f, 1f);
                    Data.Uvs[3] = new Vector2(0.5f, 0f);
                    i = 3;
                    continue;
                }
                else if (Data.Uvs.Length > 3 && i == (Data.Uvs.Length - 4))
                {
                    Data.Uvs[i] = new Vector2(0.5f, 1f);
                    Data.Uvs[i + 1] = new Vector2(0.5f, 0f);
                    Data.Uvs[i + 2] = new Vector2(1, 1f);
                    Data.Uvs[i + 3] = new Vector2(1, 0f);
                    i += 3;
                    continue;
                }
                if (i % 2 == 0) Data.Uvs[i] = new Vector2(0.5f, 1f);
                else Data.Uvs[i] = new Vector2(0.5f, 0f);
            }
        }
        else
        {
            Data.Uvs = new Vector2[Data.Vertices.Count];

            for (int i = 0; i < Data.Uvs.Length; i++)
            {
                int yValue;
                if (i % 2 == 0) yValue = 0;
                else yValue = 1;
                Data.Uvs[i] = new Vector2(Mathf.InverseLerp(0, branch.BranchDistance, branch.VertexDistance[Mathf.RoundToInt(i / 2)]), yValue);
            }
        }

    }

    public void Triangles()
    {
        Data.Triangles = new int[(Data.Vertices.Count - 2) * 3];
        for (int i = 0; i < Data.Vertices.Count - 2; i = i + 2)
        {
            Data.Triangles[j] = i;
            Data.Triangles[j + 1] = i + 2;
            Data.Triangles[j + 2] = i + 1;

            Data.Triangles[j + 3] = i + 1;
            Data.Triangles[j + 4] = i + 2;
            Data.Triangles[j + 5] = i + 3;

            j = j + 6;
            if (i == (Data.Vertices.Count - 2) || j >= Data.Triangles.Length) j = 0;
        }
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = Data.Vertices.ToArray();
        mesh.triangles = Data.Triangles;
        mesh.normals = Data.Normals.ToArray();
        mesh.tangents = Data.Tangents.ToArray();
        mesh.uv = Data.Uvs;
        return mesh;

    }
}

[System.Serializable]
public class BranchData
{
    public float BranchMeshDecal = 0;
    public AnimationCurve Curve = AnimationCurve.Linear(0, 1, 1, 1);
}

[System.Serializable]
public class Data
{
    public Vector2[] Uvs;
    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector4> Tangents = new List<Vector4>();
    public int[] Triangles;

}