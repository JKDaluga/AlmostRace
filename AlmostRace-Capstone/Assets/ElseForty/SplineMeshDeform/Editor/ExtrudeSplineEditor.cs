using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Extrude))]
public class ExtrudeEditor : Editor
{
    Extrude Extrude;
    public GUIContent ExtrudeSplineBanner;


    [MenuItem("Tools/Mesh deform/Extrude", false, 2)]
    static void CreateSplinePlus()
    {
        var NewSpline = new GameObject("Extrude");
        var sPData = NewSpline.AddComponent<SplinePlus>().SPData;
        sPData.DataParent = NewSpline;
        sPData.Followers.Add(new Follower());

        Selection.activeGameObject = sPData.DataParent;

        var Extrude = NewSpline.AddComponent<Extrude>();
        Extrude.MeshRenderer = NewSpline.AddComponent<MeshRenderer>();
        Extrude.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
        Extrude.Mesh = NewSpline.AddComponent<MeshFilter>();
        Extrude.SplinePlus = NewSpline.GetComponent<SplinePlus>();
        Extrude.SplinePlus.SPData.ObjectType = SPType.Extrude;
    }


    private void OnEnable()
    {
        Extrude = target as Extrude;
        if (ExtrudeSplineBanner == null) ExtrudeSplineBanner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/ExtrudeSplineBanner.png"));

        if (Extrude.gameObject.GetComponent<MeshRenderer>() == null) Extrude.gameObject.AddComponent<MeshRenderer>();
        Extrude.MeshRenderer = Extrude.gameObject.GetComponent<MeshRenderer>();

        if (Extrude.gameObject.GetComponent<MeshFilter>() == null) Extrude.gameObject.AddComponent<MeshFilter>();
        Extrude.Mesh = Extrude.gameObject.GetComponent<MeshFilter>();

        if (Extrude.Material == null) Extrude.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
        if (Extrude.SplinePlus == null) Extrude.SplinePlus = Extrude.gameObject.GetComponent<SplinePlus>();
        Extrude.SplinePlus.SPData.ObjectType = SPType.Extrude;
        Extrude.DrawMeshOnEachBranch();
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        var rect = EditorGUILayout.BeginHorizontal();
        var xx = rect.x + (rect.width - ExtrudeSplineBanner.image.width) * 0.5f;
        GUI.Label(new Rect(xx, rect.y, ExtrudeSplineBanner.image.width, ExtrudeSplineBanner.image.height), ExtrudeSplineBanner);
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

        EditorGUILayout.LabelField("Extrude Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        var extrudeHeight = EditorGUILayout.FloatField("Extrude height", Extrude.Height);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Extrude height changed");
            Extrude.Height = extrudeHeight;
            Extrude.DrawMeshOnEachBranch();

            EditorUtility.SetDirty(Extrude);
        }
        EditorGUI.BeginChangeCheck();
        var rings = EditorGUILayout.IntField("Rings", Extrude.Rings);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Rings changed");
            if (rings < 0) rings = 0;
            Extrude.Rings = rings;
            Extrude.DrawMeshOnEachBranch();

            EditorUtility.SetDirty(Extrude);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shell Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        var shell = EditorGUILayout.Toggle("Shell", Extrude.Shell);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Shell value changed");
            Extrude.Shell = shell;
            if (Extrude.Shell) Extrude.CapHoles = true;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }

        if (Extrude.Shell)
        {
            EditorGUI.BeginChangeCheck();
            var shellPower = EditorGUILayout.FloatField("Shell power", Extrude.ShellPower);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Extrude, "Shell power  changed");
                Extrude.ShellPower = shellPower;
                Extrude.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(Extrude);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Faces Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        var CapHoles = EditorGUILayout.Toggle("Cap holes", Extrude.CapHoles);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Cap holes changed");
            Extrude.CapHoles = CapHoles;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }
        EditorGUI.BeginChangeCheck();
        var twoSided = EditorGUILayout.Toggle("Two sided", Extrude.TwoSided);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Two sided changed");
            Extrude.TwoSided = twoSided;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }
        var FlipFaces = EditorGUILayout.Toggle("Flip faces", Extrude.FlipFaces);
        if (FlipFaces != Extrude.FlipFaces)
        {
            Undo.RecordObject(Extrude, "Flip faces changed");
            Extrude.FlipFaces = FlipFaces;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        var curvature = EditorGUILayout.CurveField("Curvature", Extrude.Curvature, Color.green, new Rect(0, -1, 1, 2));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Curvature changed");
            Extrude.Curvature = curvature;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }
        EditorGUI.BeginChangeCheck();
        var curvaturePower = EditorGUILayout.FloatField(Extrude.CurvaturePower, GUILayout.MaxWidth(70));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Curvature power value changed");
            Extrude.CurvaturePower = curvaturePower;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Material Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        var mat = (Material)EditorGUILayout.ObjectField("Material", Extrude.Material, typeof(Material), true);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Material changed");
            Extrude.Material = mat;
            Extrude.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(Extrude);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Export settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var assetName = EditorGUILayout.TextField("Asset name", Extrude.AssetName);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Extrude, "Asset name changed");
            Extrude.AssetName = assetName;
            EditorUtility.SetDirty(Extrude);
        }

        if (GUILayout.Button("Export Asset"))
        {
            SaveAsset.Save(Extrude.gameObject, Extrude.AssetName);
        }

        EditorGUILayout.EndHorizontal();
    }
}
