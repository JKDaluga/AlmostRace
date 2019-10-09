using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TubeMesh))]
public class TubeMeshEditor : Editor
{
    TubeMesh TubeMesh;
    public GUIContent TubeMeshBanner;

    [MenuItem("Tools/Mesh deform/Tube mesh", false, 2)]
    static void CreateSplinePlus()
    {
        var NewSpline = new GameObject("Tube mesh");
        var sPData = NewSpline.AddComponent<SplinePlus>().SPData;
        sPData.DataParent = NewSpline;
        sPData.Followers.Add(new Follower());
 
        Selection.activeGameObject = sPData.DataParent;

        var TubeMesh = NewSpline.AddComponent<TubeMesh>();
        TubeMesh.MeshRenderer = NewSpline.AddComponent<MeshRenderer>();
        TubeMesh.Mesh = NewSpline.AddComponent<MeshFilter>();
        TubeMesh.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
        TubeMesh.SplinePlus = NewSpline.GetComponent<SplinePlus>();
        TubeMesh.SplinePlus.SPData.ObjectType = SPType.TubeMesh;
    }

    private void OnEnable()
    {
        TubeMesh = (TubeMesh)target;
        if (TubeMeshBanner == null) TubeMeshBanner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/TubeMeshBanner.png"));

        if (TubeMesh.gameObject.GetComponent<MeshRenderer>() == null) TubeMesh.gameObject.AddComponent<MeshRenderer>();
        TubeMesh.MeshRenderer = TubeMesh.gameObject.GetComponent<MeshRenderer>();
       
        if (TubeMesh.gameObject.GetComponent<MeshFilter>() == null) TubeMesh.gameObject.AddComponent<MeshFilter>();
        TubeMesh.Mesh = TubeMesh.gameObject.GetComponent<MeshFilter>();
       
        if (TubeMesh.Material == null) TubeMesh.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
        if (TubeMesh.SplinePlus == null) TubeMesh.SplinePlus = TubeMesh.gameObject.GetComponent<SplinePlus>();
        TubeMesh.SplinePlus.SPData.ObjectType = SPType.TubeMesh;
        TubeMesh.DrawMeshOnEachBranch();
    }


    public override void OnInspectorGUI()
    {
         // DrawDefaultInspector();
        var rect = EditorGUILayout.BeginHorizontal();
        var xx = rect.x + (rect.width - TubeMeshBanner.image.width) * 0.5f;
        GUI.Label(new Rect(xx, rect.y, TubeMeshBanner.image.width, TubeMeshBanner.image.height), TubeMeshBanner);
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

        if (TubeMesh.SplinePlus.SPData.DictBranches.Count == 0) return;
        var branch = TubeMesh.SplinePlus.SPData.DictBranches[TubeMesh.SplinePlus.SPData.Selections._BranchKey];
        if (branch.Nodes.Count == 0) return;

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        var radius = EditorGUILayout.FloatField("Radius",TubeMesh.Radius);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(TubeMesh, "Radius changed");
            TubeMesh.Radius = radius;
            TubeMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(TubeMesh);
        }
        EditorGUI.BeginChangeCheck();
        var segments = EditorGUILayout.IntField("Segments",TubeMesh.Segments);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(TubeMesh, "Segments changed");
            if (segments < 1) segments = 1;
            TubeMesh.Segments = segments;
            TubeMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(TubeMesh);
        }
        EditorGUI.BeginChangeCheck();
        var flipFaces = EditorGUILayout.Toggle("Flip faces", TubeMesh.FlipFaces);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(TubeMesh, "Flip faces changed");
            TubeMesh.FlipFaces = flipFaces;
            TubeMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(TubeMesh);
        }
        EditorGUI.BeginChangeCheck();
        var twoSided = EditorGUILayout.Toggle("Two sided", TubeMesh.TwoSided);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(TubeMesh, "Two sided changed");
            TubeMesh.TwoSided = twoSided;
            TubeMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(TubeMesh);
        }
        EditorGUI.BeginChangeCheck();
        var material = (Material)EditorGUILayout.ObjectField("material", TubeMesh.Material, typeof(Material), true);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(TubeMesh, "materialchanged");
            TubeMesh.Material = material;
            TubeMesh.MeshRenderer.sharedMaterial = TubeMesh.Material;
            EditorUtility.SetDirty(TubeMesh);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Export settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        var assetName = EditorGUILayout.TextField("Asset name", TubeMesh.AssetName);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(TubeMesh, "Asset name changed");
            TubeMesh.AssetName = assetName;
            EditorUtility.SetDirty(TubeMesh);
        }
        if (GUILayout.Button("Export asset"))
        {
            SaveAsset.Save(TubeMesh.gameObject, TubeMesh.AssetName);
        }

        EditorGUILayout.EndHorizontal();
    }
}
