using System.Collections.Generic;
using UnityEngine;

public class Extrusion
{
    public void Body(Extrude Extrude, Branch branch)
    {
        var segments = branch.Vertices.Count;

        Extrude.BodyData.VertexNumber = (segments * (Extrude.Rings + 2));
        Extrude.BodyData.Vertices = new Vector3[Extrude.BodyData.VertexNumber];
        Extrude.BodyData.Triangles = new int[((segments - 1) * (Extrude.Rings + 1) * 6)];//+24

        Extrude.BodyData.Normals = new Vector3[Extrude.BodyData.VertexNumber];
        Extrude.BodyData.Tangents = new Vector4[Extrude.BodyData.VertexNumber];
        Extrude.BodyData.Uvs = new Vector2[Extrude.BodyData.VertexNumber];

        Body_Data(Extrude,branch);

        int u = 0;
        for (int n = 1; n < (Extrude.BodyData.Vertices.Length - segments); n++)
        {
            if (n % segments == 0) continue;
            u = Body_Triangles(Extrude, n, segments, u);
        }
        if (Extrude.CapHoles) Cap(Extrude,branch);
    }

    void Body_Data(Extrude Extrude, Branch branch)
    {
        for (int n = 0, t = 0; n <= Extrude.Rings + 1; n++)
        {
            float u = 0;
            u = Mathf.InverseLerp(0, Extrude.Rings + 1, n);

            var curvature = Extrude.Curvature.Evaluate(u);

            for (int i = 0; i < branch.Vertices.Count; i++, t++)
            {
                // normals
                var normal = Vector3.Cross(branch.Tangents[i], branch.Normals[i]).normalized;
                Extrude.BodyData.Normals[t] = normal;
                // tangents
                Extrude.BodyData.Tangents[t] = branch.Tangents[i].normalized;

                //vertices
                var vertex = Extrude.SplinePlus.SPData.SplinePlus.transform.InverseTransformPoint(branch.Vertices[i]);
                float heigth = (Extrude.Height * (n / (float)(Extrude.Rings + 1)));
                Extrude.BodyData.Vertices[t] = vertex + Vector3.up * heigth + normal * curvature * Extrude.CurvaturePower;
                //UV
                Body_Uvs(Extrude, branch, i, t, n);
            }
        }
    }

    void Body_Uvs(Extrude Extrude, Branch branch ,int i, int t, int n)
    {
        float heigth = (Extrude.Height * (n / (float)(Extrude.Rings + 1)));
        Extrude.BodyData.Uvs[t] = new Vector2(heigth , branch.VertexDistance[i] );
    }

    int Body_Triangles(Extrude Extrude, int n, int Segments, int u)
    {
        Extrude.BodyData.Triangles[u] = n;
        Extrude.BodyData.Triangles[u + 1] = n - 1 + Segments;
        Extrude.BodyData.Triangles[u + 2] = n - 1;
        u += 3;

        Extrude.BodyData.Triangles[u] = n;
        Extrude.BodyData.Triangles[u + 1] = n + Segments;
        Extrude.BodyData.Triangles[u + 2] = n - 1 + Segments;

        u += 3;
        return u;
    }

    public void Cap(Extrude Extrude, Branch branch)
    {
        if (Extrude.Shell)
        {
            Extrude.CapData.VertexNumber = branch.Vertices.Count * 2;
            Extrude.CapData.Vertices = new Vector3[Extrude.CapData.VertexNumber];
            Extrude.CapData.Tangents = new Vector4[Extrude.CapData.VertexNumber];
            Extrude.CapData.Uvs = new Vector2[Extrude.CapData.VertexNumber];
            Extrude.CapData.Triangles = new int[(Extrude.CapData.VertexNumber - 1) * 3];


            for (int i = 0, n = 0; i < Extrude.CapData.Vertices.Length && n < branch.Vertices.Count; i = i + 2, n++)
            {
                var normal = Vector3.Cross(branch.Tangents[n], branch.Normals[n]).normalized;
                Extrude.CapData.Vertices[i] = branch.Vertices[n] + normal * Extrude.Curvature.Evaluate(0);
                Extrude.CapData.Vertices[i + 1] = branch.Vertices[n] + normal * Extrude.Curvature.Evaluate(0) + normal * Extrude.ShellPower;

                Extrude.CapData.Vertices[i] = Extrude.SplinePlus.SPData.SplinePlus.transform.InverseTransformPoint(Extrude.CapData.Vertices[i]);
                Extrude.CapData.Vertices[i + 1] = Extrude.SplinePlus.SPData.SplinePlus.transform.InverseTransformPoint(Extrude.CapData.Vertices[i + 1]);

                Cap_Uvs(Extrude, i);
                Cap_Uvs(Extrude, i + 1);
            }
            Cap_Triangles(Extrude);
        }
        else
        {
            var vert = (Extrude.SplinePlus.SPData.IsLooped) ? (branch.Vertices.Count - 1) : branch.Vertices.Count;
            Extrude.CapData.VertexNumber = vert;
            Extrude.CapData.Vertices = new Vector3[Extrude.CapData.VertexNumber];
            Extrude.CapData.Tangents = new Vector4[Extrude.CapData.VertexNumber];
            Extrude.CapData.Uvs = new Vector2[Extrude.CapData.VertexNumber];
            Extrude.CapData.Triangles = new int[(branch.Vertices.Count - 2) * 3];


            for (int i = 0; i < vert; i++)
            {
                Extrude.CapData.Vertices[i] = Extrude.BodyData.Vertices[i];
                Extrude.CapData.Tangents[i] = branch.Tangents[i];
                Cap_Uvs(Extrude, i);
            }
            Cap_Triangles(Extrude);
        }
    }

    void Cap_Triangles(Extrude Extrude )
    {
        if (Extrude.Shell)
        {
            for (int i = 0, c = 0; (i + 3) < Extrude.CapData.Vertices.Length && (c + 5) < Extrude.CapData.Triangles.Length; i = i + 2, c += 6)
            {
                Extrude.CapData.Triangles[c] = i;
                Extrude.CapData.Triangles[c + 1] = i + 2;
                Extrude.CapData.Triangles[c + 2] = i + 1;

                Extrude.CapData.Triangles[c + 3] = i + 1;
                Extrude.CapData.Triangles[c + 4] = i + 2;
                Extrude.CapData.Triangles[c + 5] = i + 3;
            }
        }
       else
       {
           Extrude.CapVerticesIndices = new List<int>();
           for (int i = 0; i < Extrude.CapData.Vertices.Length; i++)
           {
               Extrude.CapVerticesIndices.Add(i);
           }
           EarCliping(Extrude, 0);
       }
    }

    void EarCliping(Extrude Extrude, int c)
    {
        int indexToRemove = 0;
        bool earFound = false;

        for (int i = 0; i < Extrude.CapVerticesIndices.Count; i++)
        {
            if (FindConcaveVertex(Extrude, i))
            {
                int x, y, z;
                x = (i == Extrude.CapVerticesIndices.Count - 1) ? 0 : i + 1;
                y = i;
                z = (i == 0) ? (Extrude.CapVerticesIndices.Count - 1) : i - 1;

                Extrude.CapData.Triangles[c] = Extrude.CapVerticesIndices[x];
                Extrude.CapData.Triangles[c + 1] = Extrude.CapVerticesIndices[y];
                Extrude.CapData.Triangles[c + 2] = Extrude.CapVerticesIndices[z];
                c += 3;

                indexToRemove = i;
                earFound = true;
                break;
            }
            else
            {
                earFound = false;
            }
        }

        if (earFound)
        {
            Extrude.CapVerticesIndices.RemoveAt(indexToRemove);

            if (c < Extrude.CapData.Triangles.Length)
            {

                EarCliping(Extrude, c);
            }
        }
    }

    public bool FindConcaveVertex(Extrude Extrude, int r)
    {
        var vertices = Extrude.CapVerticesIndices.ToArray();
        Vector3 v, v1, v2;
        int i, i1, i2;

        i = vertices[r];
        i1 = (r == 0) ? vertices[vertices.Length - 1] : vertices[r - 1];
        i2 = (r == vertices.Length - 1) ? vertices[0] : vertices[r + 1];

        v = Extrude.CapData.Vertices[i];
        v1 = Extrude.CapData.Vertices[i1];
        v2 = Extrude.CapData.Vertices[i2];


        for (int u = 0; u < vertices.Length; u++)//check if triangle contains a point
        {
            var t = vertices[u];
            if (t == i || t == i1 || t == i2) continue;
            if (PointInTriangle(Extrude.CapData.Vertices[t], v, v1, v2)) return false;
        }

        var a = (v - v2);
        var b = (v - v1);

        float fAngle = Vector3.Cross(a.normalized, b.normalized).y;
        fAngle *= 180.0f;
        return (fAngle < 0) ? false : true;
    }

    bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
    {
        var cp1 = Vector3.Cross(b - a, p1 - a);
        var cp2 = Vector3.Cross(b - a, p2 - a);
        if (Vector3.Dot(cp1, cp2) >= 0) return true;
        else return false;
    }

    bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        if (SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b)) return true;
        else return false;
    }

    void Cap_Uvs(Extrude Extrude, int i)
    {
        var x = Extrude.CapData.Vertices[i].x;
        var z = Extrude.CapData.Vertices[i].z;

        Extrude.CapData.Uvs[i] = new Vector2(x , z );
    }
}
