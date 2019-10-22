using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabMesh
{
    public string LayerName;
    public Material  Material;
    public GameObject Prefab;
    public CombineInstance[] LayerCombineInstances;

    public Vector3 Offset;
    public Vector3 Rotation;
    public Vector3 LockRotation;
    public Vector3 Scale = new Vector3(1, 1, 1);
    public float UniformScale = 1;

    public Vector3 ROffset;
    public Vector3 RRotation;
    public Vector3 RScale = new Vector3(1, 1, 1);
    public float RUniformScale = 1;

    public bool RandomOffset = false;
    public bool RandomRotation = false;
    public bool RandomScale = false;
    public bool Uniform = false;
    public bool RandomSpacing= false;
    
    public float Spacing = 1;
    public int Tiling = 3;

    public bool LockRot= false;
    public bool Visible= true;
    public bool Mirror= false;

    public Vector3[] VerticesPosition= new Vector3[0];
    public List<Vector3> RandomWeights = new List<Vector3>();
    public List<Vector3> OffsetRandomWeights = new List<Vector3>();
 
    public bool LinkedSpacing = false;
    public bool UniqueMat = true;

    public float Placement = 0;

    public DeformationType _DeformationType = DeformationType.Alignement;
    public MeshTrim _MeshTrim = MeshTrim.Both;
    public Axes _Axis = Axes.Y;
    public MirrorAxes _MirrorAxis = MirrorAxes.none;

    public float MirrorOffset = 0;

    public int _mat =0;
}
