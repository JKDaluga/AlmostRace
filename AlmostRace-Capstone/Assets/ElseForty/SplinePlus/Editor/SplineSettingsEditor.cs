using UnityEngine;
using UnityEditor;


public class SplineSettingsEditor
{
    public void SplineSettings(SPData SPData)
    {
        GUIContent buttonContent;
        if (!SPData.ShowSplineSettings) buttonContent = SplinePlusEditor.Minus;
        else buttonContent = SplinePlusEditor.Plus;
     
        var rect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(new Rect(rect.x - 15, rect.y, rect.width + 15, 1), Color.gray);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(buttonContent, GUIStyle.none, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(SPData.SplinePlus, "Show spline settings changed");
            SPData.ShowSplineSettings = !SPData.ShowSplineSettings;
            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        EditorGUILayout.LabelField("Spline settings", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        if (!SPData.ShowSplineSettings) 
        {
            EditorGUILayout.Space();
            return; 
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        var refAxis = (RefAxis)EditorGUILayout.EnumPopup(new GUIContent("Reference axis", "Spline normals reference direction by default"), SPData.ReferenceAxis);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(SPData.SplinePlus, "refAxis changed");
            SPData.ReferenceAxis = refAxis;
            EditorUtility.SetDirty(SPData.SplinePlus);
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }

        var Smoothness = (int)EditorGUILayout.IntField("Smoothness", SPData.Smoothness);
        if (Smoothness != SPData.Smoothness)
        {
            Undo.RecordObject(SPData.SplinePlus, "Smoothness changed");
            SPData.Smoothness = Mathf.Abs(Smoothness);
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        EditorGUILayout.BeginHorizontal();
        var attachedSplineplus = (SplinePlus)EditorGUILayout.ObjectField("Attach Spline+ object", SPData.AttachedSplinePlus, typeof(SplinePlus), true);

        if (attachedSplineplus != SPData.AttachedSplinePlus)
        {
            Undo.RecordObject(SPData.SplinePlus, "Attach Spline+ data changed");
            SPData.AttachedSplinePlus = attachedSplineplus;
            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        if (GUILayout.Button("Attach", GUILayout.MaxWidth(60)))
        {
            if (SPData.AttachedSplinePlus != null && EditorUtility.DisplayDialog("Confirm",
            "The attached spline plus object will be completely merged after this, do you want to continue! ",
           "Yes", "Cancel"))
            {
                Undo.RecordObject(SPData.SplinePlus, "Attach Spline+ data changed");
                Undo.RecordObject(SPData.AttachedSplinePlus, "Recover attach Spline+ data changed");

                SplinePlusAPI.Attach(SPData, SPData.AttachedSplinePlus.SPData);
                Undo.DestroyObjectImmediate(SPData.AttachedSplinePlus.SPData.DataParent);

                EditorUtility.SetDirty(SPData.SplinePlus);
            }
            else
            {
                EditorUtility.DisplayDialog("Error",
            "Spline plus object is null !! "
           , "Okey");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        var interpolateRotationContent = (SPData.InterpolateRotation) ? SplinePlusEditor.On : SplinePlusEditor.Off;

        if (GUILayout.Button(interpolateRotationContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(SPData.SplinePlus, "Interpolate rotationchanged");
            SPData.InterpolateRotation = !SPData.InterpolateRotation;
            EditorUtility.SetDirty(SPData.SplinePlus);
        }
        EditorGUILayout.LabelField("Interpolate rotation");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var constantSpeedContent = (SPData.ConstantSpeed) ? SplinePlusEditor.On : SplinePlusEditor.Off;

        if (GUILayout.Button(constantSpeedContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(SPData.SplinePlus, "Constant speed value changed");
            SPData.ConstantSpeed = !SPData.ConstantSpeed;
            EditorUtility.SetDirty(SPData.SplinePlus);
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData); ;
        }
        EditorGUILayout.LabelField("Constant speed");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var splineProjectionContent = (SPData.SplineProjection) ? SplinePlusEditor.On : SplinePlusEditor.Off;
        if (GUILayout.Button(splineProjectionContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(SPData.SplinePlus, "Spline Projection changed");
            SPData.SplineProjection = !SPData.SplineProjection;
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }
        EditorGUILayout.LabelField("Spline Projection");
        EditorGUILayout.EndHorizontal();

        if (SPData.DictBranches.Count <= 1)
        {
            EditorGUILayout.BeginHorizontal();
            var isLoopedContent = (SPData.IsLooped) ? SplinePlusEditor.On : SplinePlusEditor.Off;
            if (GUILayout.Button(isLoopedContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(SPData.SplinePlus, "Looped value changed");
                SPData.IsLooped = !SPData.IsLooped;

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
                SceneView.RepaintAll();
            }
            EditorGUILayout.LabelField("Looped");
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(2);
    }
}
