using UnityEngine;
using UnityEditor;

public class NodeCoordinatesEditor
{
    public void PathSetup(SPData SPData)
    {
        GUIContent buttonContent;
        if (!SPData.ShowNodeSettings) buttonContent = SplinePlusEditor.Minus;
        else buttonContent = SplinePlusEditor.Plus;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

       var rect = EditorGUILayout.BeginHorizontal();
       EditorGUI.DrawRect(new Rect(rect.x - 15, rect.y, rect.width + 15, 1), Color.gray);
       EditorGUILayout.EndHorizontal();

        if (SPData.Selections._PathPoint.Equals(null))
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

            EditorGUILayout.LabelField("No node is selected!", style );
            return;
        }

        var point = SPData.Selections._PathPoint.Point;
        var point1 = SPData.Selections._PathPoint.Point1;
        var point2 = SPData.Selections._PathPoint.Point2;

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Positions", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        var pathPointPosition = EditorGUILayout.Vector3Field("Point", point.transform.position);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point.transform, "Path Point position changed");
            point.transform.position = pathPointPosition;
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        EditorGUI.BeginChangeCheck();
        var point1Position = EditorGUILayout.Vector3Field("Point 1", point1.transform.position);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point1.transform, "Point1  position changed");
            point1.transform.position = point1Position;
            point2.transform.localPosition = -point1.transform.localPosition;

            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            EditorUtility.SetDirty(SPData.SplinePlus);

        }
        EditorGUI.BeginChangeCheck();
        var point2Position = EditorGUILayout.Vector3Field("Point 2", point2.transform.position);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(point2.transform, "Point2  position changed");
            point2.transform.position = point2Position;
            point1.transform.localPosition = -point2.transform.localPosition;
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            EditorUtility.SetDirty(SPData.SplinePlus);
        }
    }
}
