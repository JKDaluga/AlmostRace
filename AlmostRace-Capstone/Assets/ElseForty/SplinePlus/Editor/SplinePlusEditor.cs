using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System;
using System.Reflection;

[CustomEditor(typeof(SplinePlus))]
public class SplinePlusEditor : Editor
{
    public SPData SPData;

    public SplineEditing SplineEditing;
    public NodeCoordinatesEditor NodeCoordinatesEditor;
    public SplineSettingsEditor SplineSettingsEditor;
    public FollowerEditor FollowerEditor;
    public TrainEditor TrainEditor;
    public Type PFFollowerEditor;
    public ProjectionSettingsEditor ProjectionSettingsEditor;
    public SceneViewUI SceneViewUI;
    public SplineCreationClass SplineCreationClass;
    public SmoothNodeClass SmoothNodeClass;
    public HandlesClass HandlesClass;

    public Vector2 FollowerScrolPos;
    public Vector2 FollowerSettingsScrolPos = new Vector2(0, 0);

   
    public static GUIContent Banner;
    public static GUIContent Plus;
    public static GUIContent Minus;
    public static GUIContent Delete;
    public static GUIContent Add;
    public static GUIContent Reverse;
    public static GUIContent TrainHead;
    public static GUIContent BreakAt;
    public static GUIContent FlipHandles;
    public static GUIContent SceneViewMenu;
    public static GUIContent Edit;
    public static GUIContent SM;
    public static GUIContent Snap;
    public static GUIContent On;
    public static GUIContent Off;
    public static GUIContent Selected;
    public static GUIContent Deselected;
    public static GUIContent PivotEditOn;
    public static GUIContent PivotEditOff;
    public static GUIContent Previous;
    public static GUIContent Next;
    public static GUIContent Duplicate;
    public static GUIContent Settings;
    public static GUIContent Return;

    public static SettingsData settingsData;
 
    public ReorderableList FollowersList;
    public ReorderableList TrainsList;
    public List<ReorderableList> WagonsList = new List<ReorderableList>();
    public List<ReorderableList> AgentsList = new List<ReorderableList>();
    public ReorderableList PFFollowersList;

    public static float ElementHeight;
    public static float ElementHeightSpace;

    [MenuItem("Tools/Spline plus/Spline plus", false, 0)]
    static void CreateSplinePlus()
    {
        var NewSpline = new GameObject("Spline plus");
        var sPData = NewSpline.AddComponent<SplinePlus>().SPData;
        sPData.DataParent = NewSpline;

        Selection.activeGameObject = sPData.DataParent;
    }

    public void OnEnable()
    {
        var SplinePlus = (SplinePlus)target;
        SPData = SplinePlus.SPData;

        if (Banner == null) Banner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Banner.png"));
        if (Plus == null) Plus = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Plus.png"));
        if (Minus == null) Minus = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Minus.png"));
        if (Delete == null) Delete = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Delete.png"));
        if (Add == null) Add = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Add.png"));
        if (Reverse == null) Reverse = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Reverse.png"));
        if (BreakAt == null) BreakAt = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/BreakAt.png"));
        if (FlipHandles == null) FlipHandles = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/FlipHandles.png"));
        if (SceneViewMenu == null) SceneViewMenu = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/SceneViewMenu.png"));
        if (Edit == null) Edit = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Edit.png"));
        if (SM == null) SM = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/SM.png"));
        if (Snap == null) Snap = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Snap.png"));
        if (On == null) On = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/On.png"));
        if (Off == null) Off = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Off.png"));
        if (Selected == null) Selected = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Selected.png"));
        if (Deselected == null) Deselected = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Deselected.png"));
        if (PivotEditOn == null) PivotEditOn = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/PivotEditOn.png"));
        if (PivotEditOff == null) PivotEditOff = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/PivotEditOff.png"));
        if (Duplicate == null) Duplicate = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Duplicate.png"));
        if (Settings == null) Settings = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Settings.png"));
        if (Return == null) Return = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Return.png"));
        if (Next == null) Next = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Next.png"));
        if (Previous == null) Previous = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Previous.png"));

        SplineEditing = SplinePlus.CreateInstance(SplineEditing);
        NodeCoordinatesEditor = SplinePlus.CreateInstance(NodeCoordinatesEditor);
        SplineSettingsEditor = SplinePlus.CreateInstance(SplineSettingsEditor);
        FollowerEditor = SplinePlus.CreateInstance(FollowerEditor);
        TrainEditor = SplinePlus.CreateInstance(TrainEditor);
        ProjectionSettingsEditor = SplinePlus.CreateInstance(ProjectionSettingsEditor);
        SceneViewUI = SplinePlus.CreateInstance(SceneViewUI);
        SplineCreationClass = SplinePlus.CreateInstance(SplineCreationClass);
        SmoothNodeClass = SplinePlus.CreateInstance(SmoothNodeClass);
        HandlesClass = SplinePlus.CreateInstance(HandlesClass);

        settingsData = AssetDatabase.LoadAssetAtPath("Assets/ElseForty/SplinePlus/Editor/Data.asset", typeof(SettingsData)) as SettingsData;
        if (settingsData == null)
        {
            settingsData = (SettingsData)ScriptableObject.CreateInstance("SettingsData");
            AssetDatabase.CreateAsset(settingsData, "Assets/ElseForty/SplinePlus/Editor/Data.asset");
            AssetDatabase.SaveAssets();
        }


        SPData.SharedSettings.ShowHelper = settingsData.ShowHelper;
        SPData.SharedSettings.ShowRaycast = settingsData.ShowRaycast;
        SPData.SharedSettings.ShowGizmos = settingsData.ShowGizmos;
        SPData.SharedSettings.HelperSize = settingsData.HelperSize;
        SPData.SharedSettings.GizmosSize = settingsData.GizmosSize;
        SPData.SharedSettings.ShowSecondaryHandles = settingsData.ShowSecondaryHandles;
        SPData.NodeType = settingsData.NodeType;

        SPData.SharedSettings.StandardPathPointColor = settingsData.StandardPathPointColor;
        SPData.SharedSettings.RandomSharedNodeColor = settingsData.RandomSharedNodeColor;
        SPData.SharedSettings.RandomSharedNodeColor = settingsData.RandomSharedNodeColor;

        EditorApplication.playmodeStateChanged += ModeChanged;

        ElementHeight = EditorGUIUtility.singleLineHeight;
        ElementHeightSpace = 2;
    }

    private void OnDisable()
    {
        EditorApplication.playmodeStateChanged -= ModeChanged;
    }

    void ModeChanged()//reset selection data when play mode is off
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode &&
             !EditorApplication.isPlaying)
        {
            SPData.Selections._SharedPathPointIndex = -1;
            SPData.Selections._PathPoint = new Node();
            EditorUtility.SetDirty(SPData.SplinePlus);
        }
    }

    void TwoDMode()
    {

        SceneView sv = SceneView.sceneViews[0] as SceneView;
        if (sv.in2DMode && !SPData.Is2D)//2D mode
        {
            SPData.Is2D = true;
            SPData.ReferenceAxis = RefAxis.Z;
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);

        }
        else if (!sv.in2DMode && SPData.Is2D)//3D mode
        {
            SPData.Is2D = false;
            SPData.ReferenceAxis = RefAxis.Y;
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }
    }

    public void PFFollower(SPData SPData)
    {
        Type myType = null;
        try
        {
            myType = Type.GetType(" PFFollowerEditor,Assembly-CSharp-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", true);
        }
        catch
        {
            myType = null;
            SPData.FollowerType = FollowerType.Simple;
        }

        if (myType != null)
        {
            object instance = Activator.CreateInstance(myType);
            MethodInfo myMethod = myType.GetMethod("Follower");
            if (myMethod != null) myMethod.Invoke(instance, new object[] { SPData, this });
            else Debug.Log("Method PathFinding not found");
        }
        else
        {
            Debug.Log(" Path finding package is not found");
        }
    }


    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
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


        if (!EditorApplication.isPlaying)
        {

            var followerType = (FollowerType)EditorGUILayout.EnumPopup("Followers type", SPData.FollowerType);
            if (SPData.FollowerType != followerType)
            {
                Undo.RecordObject(SPData.SplinePlus, "Follower type changed");
                SPData.FollowerType = followerType;
                EditorUtility.SetDirty(SPData.SplinePlus);
            }


            if (SPData.FollowerType == FollowerType.Simple) FollowerEditor.Follower(SPData, this);
            else if (SPData.FollowerType == FollowerType.Train) TrainEditor.Follower(SPData, this);
            else if (SPData.FollowerType == FollowerType.PathFinding) PFFollower(SPData);

            NodeCoordinatesEditor.PathSetup(SPData);
            ProjectionSettingsEditor.Projection(SPData);
            SplineSettingsEditor.SplineSettings(SPData);
            if (SPData.SplineProjection || Event.current.commandName == "UndoRedoPerformed" || SPData.DataParent.transform.hasChanged) //
            {
                if (SPData.DataParent.transform.hasChanged) SPData.DataParent.transform.hasChanged = false;
                SplineCreationClass.UpdateAllBranches(SPData);
                SPData.Selections._SharedPathPointIndex = -1;
            }

        }
        else
        {
            EditorGUILayout.LabelField("Editor not available in playMode", EditorStyles.boldLabel);
            if (SPData.FollowerType == FollowerType.PathFinding)
            {
                if (GUILayout.Button("Update shoretest paths or press 'P'"))
                {
                    SPData.SplinePlus.PFFindAllShortestPaths();
                }
            }
        }
    }

    private void OnSceneGUI()
    {
        if (!EditorApplication.isPlaying) HandlesClass.DrawGizmos(SPData);
        TwoDMode();

        SceneViewUI.Draw(SPData, this);
        SplineEditing.EditingFunctions(SPData);
        Selection.activeGameObject = SPData.DataParent;

        //Free node type 
        if (Event.current.commandName == "FrameSelected")
        {
            Undo.RecordObject(SPData.SplinePlus, "Node type changed to free");
            Event.current.commandName = "";
            SPData.Selections._PathPoint._Type = NodeType.Free;
            SPData.SplinePlus.SplineCreationClass.UpdateBranch(SPData,SPData.DictBranches[SPData.Selections._BranchKey]);
            EditorUtility.SetDirty(SPData.SplinePlus);
        }
        //Smooth node type
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.S)
        {
            Undo.RecordObject(SPData.SplinePlus, "Node type changed to smooth");
             SPData.Selections._PathPoint._Type = NodeType.Smooth;
            SPData.SplinePlus.SplineCreationClass.UpdateBranch(SPData, SPData.DictBranches[SPData.Selections._BranchKey]);

            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        //focus on path point
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z)
       {
          if(SPData.Selections._PathPoint!=null) SceneView.lastActiveSceneView.LookAt(SPData.Selections._PathPoint.Point.position);
       }


    }
}


