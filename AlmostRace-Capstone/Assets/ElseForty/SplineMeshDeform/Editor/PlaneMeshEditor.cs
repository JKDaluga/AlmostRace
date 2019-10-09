using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaneMesh))]
public class PlaneMeshEditor : Editor
{
    PlaneMesh PlaneMesh;
    public GUIContent PlaneMeshBanner;


    [MenuItem("Tools/Mesh deform/Plane mesh", false, 2)]
    static void CreateSplinePlus()
    {
        var NewSpline = new GameObject("Plane mesh");
        var sPData = NewSpline.AddComponent<SplinePlus>().SPData;
        sPData.DataParent = NewSpline;
        sPData.Followers.Add(new Follower());

        Selection.activeGameObject = sPData.DataParent;

        var planeMesh = NewSpline.AddComponent<PlaneMesh>();
        planeMesh.MeshRenderer = NewSpline.AddComponent<MeshRenderer>();
        planeMesh.Mesh = NewSpline.AddComponent<MeshFilter>();
        if (planeMesh.Material == null) planeMesh.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
        planeMesh.SplinePlus = NewSpline.GetComponent<SplinePlus>();
        planeMesh.SplinePlus.SPData.ObjectType = SPType.PlaneMesh;
    }

    private void OnEnable()
    {
        PlaneMesh = (PlaneMesh)target;
        if (PlaneMeshBanner == null) PlaneMeshBanner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/PlaneMeshBanner.png"));

        if (PlaneMesh.gameObject.GetComponent<MeshRenderer>() == null) PlaneMesh.gameObject.AddComponent<MeshRenderer>();
        PlaneMesh.MeshRenderer = PlaneMesh.gameObject.GetComponent<MeshRenderer>();

        if (PlaneMesh.gameObject.GetComponent<MeshFilter>() == null) PlaneMesh.gameObject.AddComponent<MeshFilter>();
        PlaneMesh.Mesh = PlaneMesh.gameObject.GetComponent<MeshFilter>();

        if (PlaneMesh.Material == null) PlaneMesh.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
        if (PlaneMesh.SplinePlus == null) PlaneMesh.SplinePlus = PlaneMesh.gameObject.GetComponent<SplinePlus>();
        PlaneMesh.SplinePlus.SPData.ObjectType = SPType.PlaneMesh;

        PlaneMesh.DrawMeshOnEachBranch();
    }


    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        var rect = EditorGUILayout.BeginHorizontal();
        var xx = rect.x + (rect.width - PlaneMeshBanner.image.width) * 0.5f;
        GUI.Label(new Rect(xx, rect.y, PlaneMeshBanner.image.width, PlaneMeshBanner.image.height), PlaneMeshBanner);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (PlaneMesh.SplinePlus.SPData.DictBranches.Count == 0 || PlaneMesh.SplinePlus == null) return;
        var branch = PlaneMesh.SplinePlus.SPData.DictBranches[PlaneMesh.SplinePlus.SPData.Selections._BranchKey];
        if (branch.Nodes.Count == 0) return;

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();

        var curve = EditorGUILayout.CurveField("Width", PlaneMesh.BranchData[PlaneMesh.SplinePlus.SPData.Selections._BranchIndex].Curve, Color.green, new Rect(0, 0, 1, 1));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Width changed");
            PlaneMesh.BranchData[PlaneMesh.SplinePlus.SPData.Selections._BranchIndex].Curve = curve;
            PlaneMesh.DrawMeshOnEachBranch();

            EditorUtility.SetDirty(PlaneMesh);
        }
        EditorGUI.BeginChangeCheck();
        var MaxWidth = EditorGUILayout.FloatField(PlaneMesh.MaxWidth, GUILayout.MaxWidth(80));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Max width changed");
            PlaneMesh.MaxWidth = MaxWidth;
            PlaneMesh.DrawMeshOnEachBranch();

            EditorUtility.SetDirty(PlaneMesh);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.BeginChangeCheck();
        var meshDecal = EditorGUILayout.FloatField("Offset", PlaneMesh.BranchData[PlaneMesh.SplinePlus.SPData.Selections._BranchIndex].BranchMeshDecal);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Offset changed");
            PlaneMesh.BranchData[PlaneMesh.SplinePlus.SPData.Selections._BranchIndex].BranchMeshDecal = meshDecal;
            PlaneMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(PlaneMesh);
        }

        EditorGUI.BeginChangeCheck();
        var flipFaces = EditorGUILayout.Toggle("Flip faces", PlaneMesh.FlipFaces);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Flip faces changed");
            PlaneMesh.FlipFaces = flipFaces;
            PlaneMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(PlaneMesh);
        }
        EditorGUI.BeginChangeCheck();
        var twoSided = EditorGUILayout.Toggle("Two sided", PlaneMesh.TwoSided);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Two sided changed");
            PlaneMesh.TwoSided = twoSided;
            PlaneMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(PlaneMesh);
        }
        EditorGUI.BeginChangeCheck();
        var brushTextureType = EditorGUILayout.Toggle("Brush texture type", PlaneMesh.IsBrushTextureType);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Brush texture type changed");
            PlaneMesh.IsBrushTextureType = brushTextureType;
            PlaneMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(PlaneMesh);
        }
        EditorGUI.BeginChangeCheck();
        var material = (Material)EditorGUILayout.ObjectField("material", PlaneMesh.Material, typeof(Material), true);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "materialchanged");
            PlaneMesh.Material = material;
            PlaneMesh.MeshRenderer.sharedMaterial = PlaneMesh.Material;
            EditorUtility.SetDirty(PlaneMesh);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Export settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var assetName = EditorGUILayout.TextField("Asset name", PlaneMesh.AssetName);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PlaneMesh, "Asset name changed");
            PlaneMesh.AssetName = assetName;
            EditorUtility.SetDirty(PlaneMesh);
        }
        if (GUILayout.Button("Export asset"))
        {
            SaveAsset.Save(PlaneMesh.gameObject, PlaneMesh.AssetName);
        }

        EditorGUILayout.EndHorizontal();
    }
}
