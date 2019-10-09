using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Axes { X, Y, Z };
public enum MirrorAxes { none, X, Y, Z };
public enum DeformationType { Deformation, Alignement };
public enum MeshTrim { None, Left,Right,Both };

public class DeformedMesh : MonoBehaviour, ISerializationCallbackReceiver
{
    public Dictionary<int, DMBranch> DMBranches = new Dictionary<int, DMBranch>();
    public int _Layer = 0;
    public string AssetName;

    public SplinePlus SplinePlus;
    public bool IsUpdateBase = false;
    public Axes Axes;
    public MirrorAxes MirrorAxes;
    public DeformationType DeformationType;

    [SerializeField]
    List<int> Keys = new List<int>();
    [SerializeField]
    List<DMBranch> Values = new List<DMBranch>();

    public void OnBeforeSerialize()
    {
        Keys.Clear();
        Values.Clear();

        foreach (var dMBranch in DMBranches)
        {
            Keys.Add(dMBranch.Key);
            Values.Add(dMBranch.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        DMBranches = new Dictionary<int, DMBranch>();

        for (int i = 0; i != Math.Min(Keys.Count, Values.Count); i++)
            DMBranches.Add(Keys[i], Values[i]);
    }

    private void Start()
    {
        IsUpdateBase = true;
        DrawMeshOnEachBranch();
    }

    void TransformMesh(GameObject mesh, DMBranch dMBranch, int dMBranchIndex, int i)
    {
        var prefabMesh = dMBranch.PrefabMeshes[dMBranchIndex];
        Vector3 offset = Vector3.zero;
        if (prefabMesh.RandomOffset)
        {
            offset.x = Mathf.Lerp(prefabMesh.Offset.x, prefabMesh.ROffset.x, prefabMesh.OffsetRandomWeights[i].x);
            offset.y = Mathf.Lerp(prefabMesh.Offset.y, prefabMesh.ROffset.y, prefabMesh.OffsetRandomWeights[i].y);
            offset.z = Mathf.Lerp(prefabMesh.Offset.z, prefabMesh.ROffset.z, prefabMesh.OffsetRandomWeights[i].z);
        }
        else
        {
            offset = prefabMesh.Offset;
        }

        Vector3 rotation = Vector3.zero;
        if (prefabMesh.RandomRotation)
        {
            rotation = Vector3.Lerp(prefabMesh.Rotation, prefabMesh.RRotation, prefabMesh.RandomWeights[i].x);
        }
        else
        {
            rotation = prefabMesh.Rotation;
        }

        Vector3 scale = Vector3.zero;
        if (prefabMesh.RandomScale)
        {
            if (prefabMesh.Uniform)
            {
                var s = Mathf.Lerp(prefabMesh.UniformScale, prefabMesh.RUniformScale, prefabMesh.RandomWeights[i].y);
                scale = new Vector3(s, s, s);
            }
            else scale = Vector3.Lerp(prefabMesh.Scale, prefabMesh.RScale, prefabMesh.RandomWeights[i].y);
        }
        else
        {
            if (prefabMesh.Uniform)
            {
                scale = new Vector3(prefabMesh.UniformScale, prefabMesh.UniformScale, prefabMesh.UniformScale);
            }
            else scale = prefabMesh.Scale;
        }

        mesh.transform.Translate(offset);
        mesh.transform.Rotate(rotation);
        mesh.transform.localScale = scale;
        if (prefabMesh.Mirror) Mirror(mesh, dMBranch, i, dMBranchIndex);
        if (prefabMesh.LockRot)
        {
            mesh.transform.rotation = Quaternion.LookRotation(mesh.transform.rotation * Vector3.forward);
            mesh.transform.rotation = Quaternion.Euler(0, mesh.transform.eulerAngles.y, 0);

            mesh.transform.rotation = mesh.transform.rotation * Quaternion.Euler(prefabMesh.LockRotation);
        }
    }

    void Randomization(DMBranch dMBranch, int tiling, int dMBranchIndex)
    {
        if (!IsUpdateBase) return;
        var prefabMesh = dMBranch.PrefabMeshes[dMBranchIndex];
        if (tiling > prefabMesh.RandomWeights.Count)
        {
            var t = tiling - prefabMesh.RandomWeights.Count;
            for (int r = 0; r < t; r++)
            {
                var x = UnityEngine.Random.Range(0, 1.0f);
                var y = UnityEngine.Random.Range(0, 1.0f);
                var z = UnityEngine.Random.Range(0, 1.0f);

                prefabMesh.RandomWeights.Add(new Vector3(x, y, z));


                x = UnityEngine.Random.Range(0, 1.0f);
                y = UnityEngine.Random.Range(0, 1.0f);
                z = UnityEngine.Random.Range(0, 1.0f);
                prefabMesh.OffsetRandomWeights.Add(new Vector3(x, y, z));
            }
        }
        else
        {
            var t = prefabMesh.RandomWeights.Count - tiling;
            prefabMesh.RandomWeights.RemoveRange(tiling, t);
            prefabMesh.OffsetRandomWeights.RemoveRange(tiling, t);
        }
    }

    public void UpdateBaseAlignement(DMBranch dMBranch, Branch branch, float spacing, int tiling, int dMBranchIndex)
    {
        var prefabMesh = dMBranch.PrefabMeshes[dMBranchIndex];
        var temp = (GameObject)Instantiate(prefabMesh.Prefab, transform.position, Quaternion.identity);

        var maxtiling = (int)(branch.BranchDistance / spacing);
        maxtiling = (prefabMesh.Mirror) ? maxtiling * 2 : maxtiling;
       // tiling = Mathf.Clamp(tiling, 0, maxtiling + 1);

        prefabMesh.LayerCombineInstances = new CombineInstance[tiling];
        float placement = prefabMesh.Placement;

        var alignementAxis = new Vector3();
        if (prefabMesh._Axis == Axes.X) alignementAxis = Vector3.right;
        else if (prefabMesh._Axis == Axes.Y) alignementAxis = Vector3.up;
        else if (prefabMesh._Axis == Axes.Z) alignementAxis = Vector3.forward;

        for (int i = 0; i < tiling; i++)
        {
            var rSpacing = (prefabMesh.RandomSpacing) ? Mathf.Lerp(0, spacing, prefabMesh.RandomWeights[i].z) : 0;

            int t = (prefabMesh.Mirror) ? i / 2 : i;
            var distanceData = DistanceDataClass.DataExtraction(branch, ((spacing * t) + rSpacing + placement));

            temp.transform.position = transform.TransformPoint(distanceData.Position);

            if (prefabMesh.LockRot) temp.transform.rotation = distanceData.Rotation;
            else temp.transform.rotation = distanceData.Rotation * Quaternion.LookRotation(alignementAxis);

            TransformMesh(temp, dMBranch, dMBranchIndex, i);


            prefabMesh.LayerCombineInstances[i].mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            prefabMesh.LayerCombineInstances[i].transform = transform.worldToLocalMatrix * temp.transform.localToWorldMatrix;
        }

        var finalMesh = new Mesh();
        finalMesh.CombineMeshes(prefabMesh.LayerCombineInstances, true);
        MonoBehaviour.DestroyImmediate(temp);
    }


    public void UpdateBaseDeformation(DMBranch dMBranch, Branch branch, float spacing, int tiling, int dMBranchIndex)
    {
        if (!IsUpdateBase) return;
        var prefabMesh = dMBranch.PrefabMeshes[dMBranchIndex];
        var temp = (GameObject)Instantiate(prefabMesh.Prefab, transform.position, Quaternion.identity);

        var maxtiling = (int)(branch.BranchDistance / spacing);

        maxtiling = (prefabMesh.Mirror) ? maxtiling * 2 : maxtiling;
        tiling = Mathf.Clamp(tiling, 0, maxtiling + 1);

        prefabMesh.LayerCombineInstances = new CombineInstance[tiling];
        float placement = prefabMesh.Placement;

        var deformatiomAxis = new Vector3();
        if (prefabMesh._Axis == Axes.X) deformatiomAxis = Vector3.right;
        else if (prefabMesh._Axis == Axes.Y) deformatiomAxis = Vector3.up;
        else if (prefabMesh._Axis == Axes.Z) deformatiomAxis = Vector3.forward;

        for (int i = 0; i < tiling; i++)
        {
            var rSpacing = (prefabMesh.RandomSpacing) ? Mathf.Lerp(0, spacing, prefabMesh.RandomWeights[i].z) : 0;

            int t = (prefabMesh.Mirror) ? i / 2 : i;
            var p = transform.position + (Vector3.forward * ((spacing * t) + rSpacing + placement));
            temp.transform.position = transform.rotation * p;
            temp.transform.rotation = Quaternion.LookRotation(deformatiomAxis);

            TransformMesh(temp, dMBranch, dMBranchIndex, i);

            prefabMesh.LayerCombineInstances[i].mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            prefabMesh.LayerCombineInstances[i].transform = transform.worldToLocalMatrix * temp.transform.localToWorldMatrix;

        }
        var finalMesh = new Mesh();
        finalMesh.CombineMeshes(prefabMesh.LayerCombineInstances, true);
        prefabMesh.VerticesPosition = finalMesh.vertices;

        MonoBehaviour.DestroyImmediate(temp);
    }

    public void DrawMeshOnEachBranch()
    {

        foreach (var branch in SplinePlus.SPData.DictBranches)
        {
            //update branches layers
            if (!DMBranches.ContainsKey(branch.Key))
            {
                var newBranchLayer = new DMBranch();
                newBranchLayer.MeshHolder = new GameObject("Mesh Holder");

                newBranchLayer.MeshHolder.AddComponent<MeshFilter>();
                newBranchLayer.MeshHolder.AddComponent<MeshRenderer>();
                newBranchLayer.MeshHolder.transform.parent = this.gameObject.transform;
                DMBranches.Add(branch.Key, newBranchLayer);
            }

            //Draw all branches meshesB
            if (branch.Value.Vertices.Count > 0 && DMBranches[branch.Key].PrefabMeshes.Count > 0)
            {
                var dMBranch = DMBranches[branch.Key];

                var branchFinalMesh = DrawMesh(branch.Value, dMBranch);
                branchFinalMesh.RecalculateBounds();

                if (dMBranch.SmoothNormals) NormalSolver.RecalculateNormals(branchFinalMesh, dMBranch.SmoothNormalsAngle);
                else branchFinalMesh.RecalculateNormals();
                dMBranch.MeshHolder.GetComponent<MeshFilter>().sharedMesh = branchFinalMesh;
            }
        }


        foreach (var dMBranch in DMBranches.Reverse())
        {
            if (!SplinePlus.SPData.DictBranches.ContainsKey(dMBranch.Key))
            {
                MonoBehaviour.DestroyImmediate(dMBranch.Value.MeshHolder);
                DMBranches.Remove(dMBranch.Key);
            }
        }
        if (IsUpdateBase) IsUpdateBase = false;
    }

    public Mesh DrawMesh(Branch branch, DMBranch dMBranch)
    {
        CombineInstance[] unfilteredPrefabMeshes = new CombineInstance[dMBranch.PrefabMeshes.Count];

        float spacing = 0;
        int tiling = 0;

        for (int i = 0; i < dMBranch.PrefabMeshes.Count; i++)
        {
            spacing = dMBranch.PrefabMeshes[i].Spacing;
            tiling = (dMBranch.PrefabMeshes[i].Mirror) ? dMBranch.PrefabMeshes[i].Tiling * 2 : dMBranch.PrefabMeshes[i].Tiling;

            Randomization(dMBranch, tiling, i);

            if (dMBranch.PrefabMeshes[i]._DeformationType == DeformationType.Deformation) UpdateBaseDeformation(dMBranch, branch, spacing, tiling, i);
            else if (dMBranch.PrefabMeshes[i]._DeformationType == DeformationType.Alignement) UpdateBaseAlignement(dMBranch, branch, spacing, tiling, i);

            var unfilteredPrefabMesh = new Mesh();
            unfilteredPrefabMesh.CombineMeshes(dMBranch.PrefabMeshes[i].LayerCombineInstances, true);
            if (dMBranch.PrefabMeshes[i]._DeformationType == 0)
            {
                unfilteredPrefabMesh = MeshDeformation(unfilteredPrefabMesh, branch, dMBranch, dMBranch.PrefabMeshes[i], i);
            }

            if (dMBranch.PrefabMeshes[i].Visible)
            {
                unfilteredPrefabMeshes[i].mesh = unfilteredPrefabMesh;
            }
            else
            {
                var temp = new Mesh();
                var vert = new Vector3[1];
                vert[0] = Vector3.zero;
                temp.vertices = vert;
                unfilteredPrefabMeshes[i].mesh = temp;
            }
            unfilteredPrefabMeshes[i].transform = transform.worldToLocalMatrix;
        }

        return Filter(dMBranch, unfilteredPrefabMeshes);
    }

    Mesh Filter(DMBranch dMBranch, CombineInstance[] unfilteredPrefabMeshes)
    {
        /*
        filtering is done by combining unfilteredPrefabMeshes that shares the same material together into a mesh called MaterialLayer
        MaterialLayer will group all prefab meshes that shares the same material
        then it will all be combined into one final mesh called MaterialLayers ,
        MaterialLayers is the final mesh on the  mesh renderer component,
        it is basicaly a list of sub meshes each sub mesh is a group of prefab meshes that shares the same material
         */

        CombineInstance[] MaterialLayers = new CombineInstance[dMBranch.Mats.Count];

        for (int i = 0; i < dMBranch.Mats.Count; i++)
        {
            List<CombineInstance> prefabMeshesSharingMaterial = new List<CombineInstance>();

            for (int n = 0; n < unfilteredPrefabMeshes.Length; n++)
            {
                if (dMBranch.PrefabMeshes[n]._mat == i) // if prefabe mesh material is shared or not
                {
                    var prefabMesh = new CombineInstance();
                    prefabMesh.mesh = unfilteredPrefabMeshes[n].mesh;
                    prefabMesh.transform = unfilteredPrefabMeshes[n].transform;
                    if (prefabMesh.mesh == null) continue;

                    prefabMeshesSharingMaterial.Add(prefabMesh);
                }
            }

            Mesh MaterialLayer = new Mesh();
            MaterialLayer.CombineMeshes(prefabMeshesSharingMaterial.ToArray(), true);
            MaterialLayers[i].mesh = MaterialLayer;
            MaterialLayers[i].transform = transform.localToWorldMatrix * transform.worldToLocalMatrix;

        }

        Mesh branchFinalMesh = new Mesh();
        branchFinalMesh.CombineMeshes(MaterialLayers, false, true);
        return branchFinalMesh;
    }

    Mesh MeshDeformation(Mesh mesh, Branch branch, DMBranch dMBranch, PrefabMesh prefabMesh, int n)
    {

        var newVertices = new Vector3[mesh.vertices.Length];
        float verDistOnAxis = 0;
        Vector3 VerPosOnAxis = Vector3.zero;
        for (int i = 0; i < newVertices.Length; i++)
        {
            verDistOnAxis  = dMBranch.PrefabMeshes[n].VerticesPosition[i].z;
            VerPosOnAxis   = dMBranch.PrefabMeshes[n].VerticesPosition[i];

            if (prefabMesh._MeshTrim == MeshTrim.Both)
            {
                VerPosOnAxis.z = 0;
            }
            else if (prefabMesh._MeshTrim == MeshTrim.Left)
            {
                if (verDistOnAxis > 0) VerPosOnAxis.z = 0;
            }
            else if (prefabMesh._MeshTrim == MeshTrim.Right)
            {
                if (verDistOnAxis < branch.BranchDistance) VerPosOnAxis.z = 0;
                if (verDistOnAxis > branch.BranchDistance) VerPosOnAxis.z -= branch.BranchDistance;
            }
            else
            {
                if (verDistOnAxis > 0 && verDistOnAxis < branch.BranchDistance) VerPosOnAxis.z = 0;
                if (verDistOnAxis > branch.BranchDistance) VerPosOnAxis.z -= branch.BranchDistance;
            }

           
                var distanceData = DistanceDataClass.DataExtraction(branch, verDistOnAxis);
            newVertices[i] = (distanceData.Rotation * VerPosOnAxis) + distanceData.Position;
        }
        mesh.vertices = newVertices;
        return mesh;
    }

    void Mirror(GameObject mesh, DMBranch dMBranch, int i, int dMBranchIndex)
    {
        Vector3 offset = Vector3.zero, scale = Vector3.zero;
        int t = (i % 2 == 0) ? 1 : -1;
        if (dMBranch.PrefabMeshes[dMBranchIndex]._MirrorAxis == MirrorAxes.Y)
        {
            scale = new Vector3(t * mesh.transform.localScale.x, mesh.transform.localScale.y, mesh.transform.localScale.z);
            offset = t * Vector3.right * dMBranch.PrefabMeshes[dMBranchIndex].MirrorOffset;
        }
        else if (dMBranch.PrefabMeshes[dMBranchIndex]._MirrorAxis == MirrorAxes.Z)
        {
            scale = new Vector3(mesh.transform.localScale.x, t * mesh.transform.localScale.y, mesh.transform.localScale.z);
            offset = t * Vector3.up * dMBranch.PrefabMeshes[dMBranchIndex].MirrorOffset;
        }
        else
        {
            scale = new Vector3(mesh.transform.localScale.x, mesh.transform.localScale.y, t * mesh.transform.localScale.z);
            offset = t * Vector3.forward * dMBranch.PrefabMeshes[dMBranchIndex].MirrorOffset;
        }

        mesh.transform.Translate(offset);
        mesh.transform.localScale = scale;
    }
}

[System.Serializable]
public class DMBranch
{
    public GameObject MeshHolder;
    public List<string> Mats = new List<string>();
    public List<PrefabMesh> PrefabMeshes = new List<PrefabMesh>();
    public bool SmoothNormals = false;
    public float SmoothNormalsAngle = 60.0f;

}
