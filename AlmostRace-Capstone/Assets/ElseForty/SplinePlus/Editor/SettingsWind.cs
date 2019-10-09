using UnityEditor;
using UnityEngine;


public enum PackageType { SplinePlus, SplineMeshDeform, SplinePathFinding }
public enum SupportAndContactType { Documentation, Support, Tutorials }
public enum WindType { Support, Packages }

public class SettingsWind : EditorWindow
{

    public static SettingsData settingsData;
    public static GUIContent On;
    public static GUIContent Off;
    public PackageType packageType = PackageType.SplinePlus;
    public SupportAndContactType supportAndContactType = SupportAndContactType.Support;
    public WindType windType = WindType.Packages;

    [MenuItem("Tools/General settings", false, 10)]
    static void Init()
    {
        if (On == null) On = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/On.png"));
        if (Off == null) Off = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Off.png"));

        if (settingsData == null)
        {
            settingsData = (SettingsData)ScriptableObject.CreateInstance("SettingsData");
            AssetDatabase.CreateAsset(settingsData, "Assets/ElseForty/SplinePlus/Editor/Data.asset");
            AssetDatabase.SaveAssets();
        }

        if (EditorPrefs.HasKey("Assets/ElseForty/SplinePlus/GeneralSettings/Data.asset"))
        {
            settingsData = AssetDatabase.LoadAssetAtPath("Assets/ElseForty/SplinePlus/Editor/Data.asset", typeof(SettingsData)) as SettingsData;
        }

        SettingsWind window = (SettingsWind)EditorWindow.GetWindow(typeof(SettingsWind), true, "Settings", true);
        window.Show();
    }

    void OnGUI()
    {

        if (settingsData == null) return;

        if (windType == WindType.Packages)
        {
            EditorGUI.DrawRect(new Rect(0, 10, 120, 20), Color.white);
            EditorGUI.DrawRect(new Rect(120, 10, 120, 20), Color.grey);
        }
        else if (windType == WindType.Support)
        {
            EditorGUI.DrawRect(new Rect(0, 10, 120, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(120, 10, 120, 20), Color.white);
        }


        if (GUI.Button(new Rect(120, 10, 120, 20), "Contact & support", GUIStyle.none))
        {
            windType = WindType.Support;
        }
        if (GUI.Button(new Rect(0, 10, 100, 20), "Packages", GUIStyle.none))
        {
            windType = WindType.Packages;
        }


        EditorGUI.DrawRect(new Rect(0, 30, this.position.width, 2), Color.gray);

        GUI.Label(new Rect(this.position.width - 100, this.position.height - 20, 100, 20), "By Else forty", EditorStyles.boldLabel);
        EditorGUI.DrawRect(new Rect(152, 30, 1, this.position.height), Color.gray);

        if (windType == WindType.Packages)
        {
            Packages();
        }
        else
        {
            SupportAndContact();
        }
    }

    void Packages()
    {
        if (packageType == PackageType.SplinePlus)
        {
            EditorGUI.DrawRect(new Rect(2, 32, 150, 20), Color.white);
            EditorGUI.DrawRect(new Rect(2, 52, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 72, 150, 20), Color.grey);
        }
        else if (packageType == PackageType.SplineMeshDeform)
        {
            EditorGUI.DrawRect(new Rect(2, 32, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 52, 150, 20), Color.white);
            EditorGUI.DrawRect(new Rect(2, 72, 150, 20), Color.grey);
        }
        else if (packageType == PackageType.SplinePathFinding)
        {
            EditorGUI.DrawRect(new Rect(2, 32, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 52, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 72, 150, 20), Color.white);
        }

        if (GUI.Button(new Rect(2, 32, 150, 20), "Spline plus", GUIStyle.none))
        {
            packageType = PackageType.SplinePlus;
        }

        if (GUI.Button(new Rect(2, 52, 150, 20), "Spline mesh deform", GUIStyle.none))
        {
            packageType = PackageType.SplineMeshDeform;
        }
        if (GUI.Button(new Rect(2, 72, 150, 20), "Spline path finding", GUIStyle.none))
        {
            packageType = PackageType.SplinePathFinding;
        }

        GUI.BeginGroup(new Rect(165, 35, this.position.width - 170, this.position.height));

        if (packageType == PackageType.SplinePlus)
        {
            settingsData.NodeType = (NodeType)EditorGUILayout.EnumPopup("Node type", settingsData.NodeType, GUILayout.MaxWidth(this.position.width - 175));

            EditorGUILayout.BeginHorizontal();
            var showGizmosContent = (settingsData.ShowGizmos) ? On : Off;
            if (GUILayout.Button(showGizmosContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                settingsData.ShowGizmos = !settingsData.ShowGizmos;
                UpdateAllSPDataObjects();
            }

            EditorGUILayout.LabelField("Gizmos", GUILayout.MaxWidth(105));

            if (settingsData.ShowGizmos)
            {
                EditorGUI.BeginChangeCheck();

                var gizmosSize = EditorGUILayout.FloatField(settingsData.GizmosSize);
                if (EditorGUI.EndChangeCheck())
                {
                    settingsData.GizmosSize = gizmosSize;
                    UpdateAllSPDataObjects();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var helpersContent = (settingsData.ShowHelper) ? On : Off;
            if (GUILayout.Button(helpersContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                settingsData.ShowHelper = !settingsData.ShowHelper;
                UpdateAllSPDataObjects();
            }
            EditorGUILayout.LabelField("Helpers", GUILayout.MaxWidth(105));

            if (settingsData.ShowHelper)
            {
                EditorGUI.BeginChangeCheck();

                var HelperSize = EditorGUILayout.FloatField(settingsData.HelperSize);
                if (EditorGUI.EndChangeCheck())
                {
                    settingsData.HelperSize = HelperSize;
                    UpdateAllSPDataObjects();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var showSecondaryHandlesContent = (settingsData.ShowSecondaryHandles) ? On : Off;
            if (GUILayout.Button(showSecondaryHandlesContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                settingsData.ShowSecondaryHandles = !settingsData.ShowSecondaryHandles;
                UpdateAllSPDataObjects();
            }
            EditorGUILayout.LabelField("Secondary Handles");
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            var showRaycastCont = (settingsData.ShowRaycast) ? On : Off;
            if (GUILayout.Button(showRaycastCont, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                settingsData.ShowRaycast = !settingsData.ShowRaycast;
                UpdateAllSPDataObjects();
            }
            EditorGUILayout.LabelField("Show projection raycast");
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            var standard = EditorGUILayout.ColorField("Simple path point ", settingsData.StandardPathPointColor);

            if (EditorGUI.EndChangeCheck())
            {
                settingsData.StandardPathPointColor = standard;
                UpdateAllSPDataObjects();
            }

            EditorGUILayout.LabelField("Shared path point:", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var random = EditorGUILayout.ColorField("Random ", settingsData.RandomSharedNodeColor); 

            if (EditorGUI.EndChangeCheck())
            {
                settingsData.RandomSharedNodeColor = random;
                UpdateAllSPDataObjects();
            }

            EditorGUI.BeginChangeCheck();
            var Defined = EditorGUILayout.ColorField("Defined ", settingsData.DefinedSharedNodeColor); 

            if (EditorGUI.EndChangeCheck())
            {
                settingsData.DefinedSharedNodeColor = Defined;
                UpdateAllSPDataObjects();
            }

        }

        GUI.EndGroup();
    }

    void SupportAndContact()
    {
        if (supportAndContactType == SupportAndContactType.Support)
        {
            EditorGUI.DrawRect(new Rect(2, 32, 150, 20), Color.white);
            EditorGUI.DrawRect(new Rect(2, 52, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 72, 150, 20), Color.grey);
        }
        else if (supportAndContactType == SupportAndContactType.Documentation)
        {
            EditorGUI.DrawRect(new Rect(2, 32, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 52, 150, 20), Color.white);
            EditorGUI.DrawRect(new Rect(2, 72, 150, 20), Color.grey);
        }
        else if (supportAndContactType == SupportAndContactType.Tutorials)
        {
            EditorGUI.DrawRect(new Rect(2, 32, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 52, 150, 20), Color.grey);
            EditorGUI.DrawRect(new Rect(2, 72, 150, 20), Color.white);
        }

        if (GUI.Button(new Rect(2, 32, 150, 20), "Support", GUIStyle.none))
        {
            supportAndContactType = SupportAndContactType.Support;
        }

        if (GUI.Button(new Rect(2, 52, 150, 20), "Online documentation", GUIStyle.none))
        {
            supportAndContactType = SupportAndContactType.Documentation;
        }
        if (GUI.Button(new Rect(2, 72, 150, 20), "Tutorials", GUIStyle.none))
        {
            supportAndContactType = SupportAndContactType.Tutorials;
        }

        GUI.BeginGroup(new Rect(165, 35, this.position.width - 170, this.position.height));
        if (supportAndContactType == SupportAndContactType.Support)
        {
            var urlForum = "forum.unity.com/threads/published-spline-plus-spline-mesh-deform.532476";
            EditorGUILayout.TextArea("Email  : elseforty@gmail.com \nForum : " + urlForum, GUIStyle.none);
        }
        else if (supportAndContactType == SupportAndContactType.Documentation)
        {
            var urlDocSplinePlus = "docs.google.com/document/d/12hSl3tHb-RvlGV8KYTqW7ArM5s9LLyT7flEHgN_YWss/edit?usp=sharing";
            var urlDocSplineMeshDeform = "docs.google.com/document/d/19Slm0TeCWLuDVKDnhIlXycsORqmGWUuA1A2UDntQPPY/edit?usp=sharing";
            EditorGUILayout.TextArea("Spline plus             : " + urlDocSplinePlus +
                                     "\nSpline mesh deform: " + urlDocSplineMeshDeform, GUIStyle.none);
        }
        else if (supportAndContactType == SupportAndContactType.Tutorials)
        {
            var urlTutorials = "www.youtube.com/channel/UCwc_oOuV5x5lPVxI75BDB6A/videos";
            EditorGUILayout.TextArea("Tutorials : " + urlTutorials, GUIStyle.none);
        }
        GUI.EndGroup();
    }

    void UpdateAllSPDataObjects()
    {
        var splinePlusObjects = FindObjectsOfType<SplinePlus>();
        for (int i = 0; i < splinePlusObjects.Length; i++)
        { 
            splinePlusObjects[i].SPData.SharedSettings.ShowHelper = settingsData.ShowHelper;
            splinePlusObjects[i].SPData.SharedSettings.ShowRaycast = settingsData.ShowRaycast;
            splinePlusObjects[i].SPData.SharedSettings.ShowGizmos = settingsData.ShowGizmos;
            splinePlusObjects[i].SPData.SharedSettings.HelperSize = settingsData.HelperSize;
            splinePlusObjects[i].SPData.SharedSettings.GizmosSize = settingsData.GizmosSize;
            splinePlusObjects[i].SPData.SharedSettings.ShowSecondaryHandles = settingsData.ShowSecondaryHandles;

            splinePlusObjects[i].SPData.NodeType = settingsData.NodeType;
            splinePlusObjects[i].SPData.SharedSettings.StandardPathPointColor = settingsData.StandardPathPointColor;
            splinePlusObjects[i].SPData.SharedSettings.RandomSharedNodeColor = settingsData.RandomSharedNodeColor;
            splinePlusObjects[i].SPData.SharedSettings.DefinedSharedNodeColor = settingsData.DefinedSharedNodeColor;
        }
        SceneView.RepaintAll();
    }
}
