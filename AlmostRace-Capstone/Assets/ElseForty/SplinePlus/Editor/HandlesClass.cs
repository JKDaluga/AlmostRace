using UnityEditor;
using UnityEngine;

public class HandlesClass
{
    public void DrawGizmos(SPData SPData)
    {
        BranchesDirrection(SPData);
        if (SPData.DictBranches.Count > 0 && !SPData.Selections._PathPoint.Equals(null)) PositionHandlers(SPData);
    }
   
    public void BranchesDirrection(SPData SPData)
    {
        var e = SPData.Selections._BranchKey;
        if (SPData.DictBranches.Count == 0 || SPData.DictBranches[e].Vertices.Count < 2) return;
        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0, SPData.DictBranches[e].Vertices[0], Quaternion.LookRotation(SPData.DictBranches[e].Vertices[1] - SPData.DictBranches[e].Vertices[0]), SPData.SharedSettings.GizmosSize * 3, EventType.Repaint);
        Handles.color = Color.white;
    }
  
    public void PositionHandlers(SPData SPData)
    {
        if (SPData.DictBranches.Count == 0) return;
        if (SPData.EditSpline) return;

        var Point = SPData.Selections._PathPoint.Point;
        var Point1 = SPData.Selections._PathPoint.Point1;
        var Point2 = SPData.Selections._PathPoint.Point2;

        Vector3 p;

        EditorGUI.BeginChangeCheck();
        p = Handles.PositionHandle(Point.position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Point.transform, "Position point changed");
            Point.position = p;
            EditorUtility.SetDirty(SPData.SplinePlus);
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }

        if (SPData.SharedSettings.ShowSecondaryHandles && (SPData.Selections._PathPoint._Type == NodeType.Smooth))
        {
            Vector3 pos, pos2;
            EditorGUI.BeginChangeCheck();

            pos = Handles.PositionHandle(Point1.position, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Point1.transform, "Position point1 changed");
                Undo.RecordObject(Point2.transform, "Position point2 changed");
                Point1.position = pos;
                Point2.localPosition = -Point1.localPosition;
                EditorUtility.SetDirty(SPData.SplinePlus);

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }
            EditorGUI.BeginChangeCheck();
            pos2 = Handles.PositionHandle(Point2.position, Quaternion.Euler(0, 180f, 0));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Point1.transform, "Position point1 changed");
                Undo.RecordObject(Point2.transform, "Position point2 changed");
                Point2.position = pos2;
                Point1.localPosition = -Point2.localPosition;
                EditorUtility.SetDirty(SPData.SplinePlus);

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }
            SecondaryHandlerGizmos(Point, Point1, Point2);
        }
    }

    public void SecondaryHandlerGizmos(Transform Point, Transform Point1, Transform Point2)
    {
        Handles.color = Color.yellow;
        Handles.DrawLine(Point.position, Point1.position);
        Handles.DrawLine(Point.position, Point2.position);
        Handles.color = Color.white;
    }
}
