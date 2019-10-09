using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Info))]
public class InfoEditor : Editor
{
	static Info Info;
    static GUIStyle style = new GUIStyle();
   

    void OnEnable()
	{
        Info = (Info)target;
        style.normal.textColor = Color.green;

    }

    private void OnSceneGUI()
    {
        Handles.BeginGUI();
        var sceneViewRect = SceneView.currentDrawingSceneView.position;

        GUI.Label(new Rect(10, sceneViewRect.height - (sceneViewRect.height * 0.2f),
            (sceneViewRect.width * 0.4f), (sceneViewRect.height * 0.2f)), Info.Text, style);

        Handles.EndGUI();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        var infoText = EditorGUILayout.TextArea(Info.Text);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Info, "Info text changed");
            Info.Text = infoText;
            EditorUtility.SetDirty(Info);
        }
    }

 
}