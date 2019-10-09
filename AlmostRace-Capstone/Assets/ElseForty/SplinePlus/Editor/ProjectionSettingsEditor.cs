using UnityEngine;
using UnityEditor;

public class ProjectionSettingsEditor
{
    public void Projection(SPData SPData)
    {
        if (!SPData.SplineProjection) return;

        GUIContent buttonContent = !SPData.ShowProjectionSettings ? SplinePlusEditor.Minus : SplinePlusEditor.Plus;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(buttonContent, GUIStyle.none, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(SPData.SplinePlus, "Show Projection settings changed");
            SPData.ShowProjectionSettings = !SPData.ShowProjectionSettings;
            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        EditorGUILayout.LabelField("Projection settings", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        if (!SPData.ShowProjectionSettings) return;

        if (EditorApplication.isPlaying) SPData.SplineProjection = false;
        if (!SPData.SplineProjection && SPData.MeshOrientation) EditorGUILayout.HelpBox("Mesh orientation is still on, enable projection and disable it in case you don't want to use it anymore", MessageType.Warning);

        if (SPData.SplineProjection)
        {
            EditorGUI.BeginChangeCheck();
            var RaycastLength = EditorGUILayout.FloatField("Raycast length", SPData.RaycastLength);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(SPData.SplinePlus, "Raycast length changed");
                SPData.RaycastLength = RaycastLength;
                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
                EditorUtility.SetDirty(SPData.SplinePlus);
            }

            EditorGUI.BeginChangeCheck();
            var Offset = EditorGUILayout.FloatField("Offset", SPData.Offset);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(SPData.SplinePlus, "Offset changed");
                SPData.Offset = Offset;
                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
                EditorUtility.SetDirty(SPData.SplinePlus);
            }

            EditorGUILayout.BeginHorizontal();

            var handlesProjectionContent = (SPData.HandlesProjection) ? SplinePlusEditor.On : SplinePlusEditor.Off;

            if (GUILayout.Button(handlesProjectionContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(SPData.SplinePlus, "Handles Projection changed");
                SPData.HandlesProjection = !SPData.HandlesProjection;
                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
                EditorUtility.SetDirty(SPData.SplinePlus);
            }
            EditorGUILayout.LabelField("Handles projection");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            var meshOrientationContent = (SPData.MeshOrientation) ? SplinePlusEditor.On : SplinePlusEditor.Off;
            if (GUILayout.Button(meshOrientationContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(SPData.SplinePlus, "Mesh orientation changed");
                SPData.MeshOrientation = !SPData.MeshOrientation;
                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
                EditorUtility.SetDirty(SPData.SplinePlus);
            }
            EditorGUILayout.LabelField("Mesh orientation");
            EditorGUILayout.EndHorizontal();
        }
    }
}
