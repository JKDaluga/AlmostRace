using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor
{
    PathGenerator PathGenerator;

    public GUIContent Banner;
    public GUIContent Delete;
    public GUIContent Add;
    public GUIContent Reverse;

    public void OnEnable()
    {
        PathGenerator = (PathGenerator)target;

        if (Banner == null) Banner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Banner.png"));
        if (Delete == null) Delete = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Delete.png"));
        if (Add == null) Add = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Add.png"));
        if (Reverse == null) Reverse = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Reverse.png"));
    }

    [MenuItem("Tools/Spline plus/Path generator",false,1)]
    static void CreatePathGenerator()
    {
        var PathGeneratorGO = new GameObject("Path generator");
        var pathGenerator = PathGeneratorGO.AddComponent<PathGenerator>();
        Selection.activeGameObject = PathGeneratorGO.gameObject;

        pathGenerator.Points.Add(null);
        pathGenerator.Points.Add(null);
    }

    public override void OnInspectorGUI()
    {
        var rect = EditorGUILayout.BeginHorizontal();
        var x = rect.x + (rect.width - Banner.image.width) * 0.5f;
        GUI.Label(new Rect(x, rect.y, Banner.image.width, Banner.image.height), Banner);
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


        if (GUILayout.Button("Add branch", GUILayout.Height(20)))
        {

            Undo.RecordObject(PathGenerator, "branch added");
            PathGenerator.Points.Add(null);
            PathGenerator.Points.Add(null);
            EditorUtility.SetDirty(PathGenerator);
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        EditorGUILayout.LabelField("Path point1", EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.LabelField("Path point2", EditorStyles.boldLabel, GUILayout.Width(200));
        GUILayout.EndHorizontal();
        for (int i = 0; i < PathGenerator.Points.Count - 1; i = i + 2)
        {

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Branch " + ((i / 2) + 1), EditorStyles.boldLabel, GUILayout.Width(100));
            var point = (GameObject)EditorGUILayout.ObjectField(PathGenerator.Points[i], typeof(GameObject), true);
            if (point != PathGenerator.Points[i])
            {
                Undo.RecordObject(PathGenerator, "Point changed");
                PathGenerator.Points[i] = point;


                EditorUtility.SetDirty(PathGenerator);
            }
            var point2 = (GameObject)EditorGUILayout.ObjectField(PathGenerator.Points[i + 1], typeof(GameObject), true);
            if (point2 != PathGenerator.Points[i + 1])
            {
                Undo.RecordObject(PathGenerator, "Point changed");
                PathGenerator.Points[i + 1] = point2;


                EditorUtility.SetDirty(PathGenerator);
            }
            GUILayout.Space(2);
            if (GUILayout.Button(Delete, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
            {

                Undo.RecordObject(PathGenerator, "Points removed");
                PathGenerator.Points.RemoveAt(i);
                PathGenerator.Points.RemoveAt(i);
                EditorUtility.SetDirty(PathGenerator);
                break;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        var Radius = EditorGUILayout.Slider("Radius", PathGenerator.Radius, 0, 1);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PathGenerator, "radius changed");
            PathGenerator.Radius = Mathf.Abs(Radius);
            EditorUtility.SetDirty(PathGenerator);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Create path", GUILayout.MaxHeight(20)))
        {
            PathGenerator.CreatePath();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //  DrawDefaultInspector();
    }
}
