using UnityEngine;
using UnityEditor;
using System.Linq;

public class SceneViewUI
{
    public delegate void pivotEdit();
    public static event pivotEdit PivotEditDel;

    GUIStyle style = new GUIStyle();

    public void Draw(SPData SPData, SplinePlusEditor SplinePlusEditor)
    {

        if (SPData.IsEditingPivot) PivotEdit(SPData);
        SceneViewGUI(SPData, SplinePlusEditor);
    }

    void MessageBox(SPData SPData, SplinePlusEditor SplinePlusEditor)
    {
        GUI.BeginGroup(new Rect(240, 20, 200, 100));
        GUI.Label(new Rect(0, 0, 200, 100), SplinePlusEditor.SceneViewMenu);
        GUI.Label(new Rect(10, 5, 50, 20), "Radius:");
        var radius = GUI.HorizontalSlider(new Rect(60, 5, 130, 20), SPData.SmoothRadius, 0, 1);
        if (SPData.SmoothRadius != radius)
        {

            Undo.RecordObject(SPData.SplinePlus, "Smooth node radius changed");

            if (SPData.SmoothData.IsShared) SplinePlusEditor.SmoothNodeClass.EditSmoothNodePoint(SPData, radius);
            else SplinePlusEditor.SmoothNodeClass.EditSmoothNode(SPData, radius);

            EditorUtility.SetDirty(SPData.SplinePlus);

        }

        if (GUI.Button(new Rect(5, 25, 95, 20), "Cancel "))
        {
            Undo.RecordObject(SPData.SplinePlus, "Smooth node canceled ");
            if (SPData.SmoothData.IsShared) SplinePlusEditor.SmoothNodeClass.ResetSmoothSharedPathPoint(SPData);
            else SplinePlusEditor.SmoothNodeClass.ResetSmoothPathPoint(SPData);
            EditorUtility.SetDirty(SPData.SplinePlus);


        }
        if (GUI.Button(new Rect(100, 25, 95, 20), "Apply "))
        {
            Undo.RecordObject(SPData.SplinePlus, "Smooth node ");

            SPData.SmoothData.SmoothNode = false;
            SPData.SmoothData.Nodes = new Node[0];
            if (SPData.Selections._SharedPathPointIndex != -1) SPData.SharedNodes.RemoveAt(SPData.Selections._SharedPathPointIndex);

            EditorUtility.SetDirty(SPData.SplinePlus);

        }
        GUI.EndGroup();
    }


    void SceneViewGUI(SPData SPData, SplinePlusEditor splinePlusEditor)
    {
        Handles.BeginGUI();

        Event e = Event.current;
        if (e == null) return;
        if (SPData.DictBranches.Count == 0 || SPData.DictBranches.Count == 1 && SPData.DictBranches.FirstOrDefault().Value.Nodes.Count <= 1)
        {
            style.fontSize = 15;
            style.normal.textColor = Color.green;
            var sceneViewRect = SceneView.currentDrawingSceneView.position;
            GUI.Label(new Rect((sceneViewRect.width / 2) - 250, (sceneViewRect.height / 2) - 10, 500, 20),
               "Please press 'shift' on keyboard to start adding nodes", style);
        }

        if (GUI.Button(new Rect(10, 30, 30, 30), new GUIContent(SplinePlusEditor.Snap.image, "Snap all nodes and their handles to Unity grid"), GUIStyle.none))//snap
        {
            foreach (var branch in SPData.DictBranches)
            {
                for (int n = 0; n < branch.Value.Nodes.Count; n++)
                {
                    if (Camera.current.transform.eulerAngles == new Vector3(0, 0, 0) || Camera.current.transform.eulerAngles == new Vector3(0, 180, 0))
                    {

                        branch.Value.Nodes[n].Point.position = new Vector3(branch.Value.Nodes[n].Point.position.x,
                           branch.Value.Nodes[n].Point.position.y, SPData.DataParent.transform.position.z);
                        branch.Value.Nodes[n].Point1.position = new Vector3(branch.Value.Nodes[n].Point1.position.x,
                            branch.Value.Nodes[n].Point1.position.y, SPData.DataParent.transform.position.z);
                    }
                    else if (Camera.current.transform.eulerAngles == new Vector3(90, 0, 0) || Camera.current.transform.eulerAngles == new Vector3(270, 0, 0))
                    {

                        branch.Value.Nodes[n].Point.position = new Vector3(branch.Value.Nodes[n].Point.position.x, SPData.DataParent.transform.position.y,
                           branch.Value.Nodes[n].Point.position.z);
                        branch.Value.Nodes[n].Point1.position = new Vector3(branch.Value.Nodes[n].Point1.position.x, SPData.DataParent.transform.position.y,
                           branch.Value.Nodes[n].Point1.position.z);
                    }
                    else if (Camera.current.transform.eulerAngles == new Vector3(0, 90, 0) || Camera.current.transform.eulerAngles == new Vector3(0, 270, 0))
                    {

                        branch.Value.Nodes[n].Point.position = new Vector3(SPData.DataParent.transform.position.x, branch.Value.Nodes[n].Point.position.y,
                            branch.Value.Nodes[n].Point.position.z);
                        branch.Value.Nodes[n].Point1.position = new Vector3(SPData.DataParent.transform.position.x, branch.Value.Nodes[n].Point1.position.y,
                            branch.Value.Nodes[n].Point1.position.z);
                    }
                    else
                    {

                        branch.Value.Nodes[n].Point.position = new Vector3(branch.Value.Nodes[n].Point.position.x, SPData.DataParent.transform.position.y,
                            branch.Value.Nodes[n].Point.position.z);
                        branch.Value.Nodes[n].Point1.position = new Vector3(branch.Value.Nodes[n].Point1.position.x, SPData.DataParent.transform.position.y,
                           branch.Value.Nodes[n].Point1.position.z);
                    }
                    branch.Value.Nodes[n].Point2.localPosition = -branch.Value.Nodes[n].Point1.localPosition;
                }
            }
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }


        var pivotEditingContent = SPData.IsEditingPivot ? SplinePlusEditor.PivotEditOn : SplinePlusEditor.PivotEditOff;
        if (GUI.Button(new Rect(42, 30, 30, 30), new GUIContent(pivotEditingContent.image, "Pivot editing"), GUIStyle.none))//Edit
        {
            SPData.IsEditingPivot = !SPData.IsEditingPivot;
            Tools.hidden = SPData.IsEditingPivot ? true : false;
        }

        style.fontSize = 10;
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(10, 5, 30, 20), "Branch :", style);
        style.normal.textColor = Color.green;


        GUI.Label(new Rect(50, 5, 40, 20), ((SPData.DictBranches.Count == 0
            || SPData.DictBranches[SPData.Selections._BranchKey].Nodes.Count == 0) || SPData.DictBranches.Count == 0) ?
        "null" : SPData.Selections._BranchKey.ToString(), style);

        if (SPData.DictBranches.Count > 0)
        {
            if (GUI.Button(new Rect(10, 15, 12, 12), new GUIContent(SplinePlusEditor.Next.image, "Next branch"), GUIStyle.none))
            {
                int  nextBranchKey = SPData.DictBranches.FirstOrDefault().Key;
                foreach (var branch in SPData.DictBranches.Reverse())
                {
                    if (SPData.Selections._BranchKey == branch.Key)
                    {
                        SPData.Selections._BranchKey = nextBranchKey;
                        break;
                    }
                    nextBranchKey = branch.Key;
                }

                SPData.Selections._BranchKey = nextBranchKey;
            }

            if (GUI.Button(new Rect(25, 15, 12, 12), new GUIContent(SplinePlusEditor.Previous.image, "Previous branch"), GUIStyle.none))
            {
                int previousBranchKey = SPData.DictBranches.FirstOrDefault().Key;
                foreach (var branch in SPData.DictBranches)
                {
                    if (SPData.Selections._BranchKey == branch.Key)
                    {
                        SPData.Selections._BranchKey = previousBranchKey;
                        break;
                    }
                    previousBranchKey = branch.Key;
                }

                SPData.Selections._BranchKey = previousBranchKey;
            }
        }

        style.normal.textColor = Color.black;
        GUI.Label(new Rect(85, 5, 40, 20), "Node :", style);
        style.normal.textColor = Color.green;

        GUI.Label(new Rect(140, 5, 40, 20), ((SPData.DictBranches.Count == 0 || SPData.DictBranches[SPData.Selections._BranchKey].Nodes.Count == 0) || SPData.Selections._PathPoint.Equals(null)) ?
    "null" : SPData.Selections._LocalNodeIndex.ToString(), style);

        style.normal.textColor = Color.black;
        GUI.Label(new Rect(180, 5, 40, 20), "Shared :", style);
        style.normal.textColor = Color.green;

        GUI.Label(new Rect(225, 5, 40, 20), ((SPData.DictBranches.Count == 0 || SPData.DictBranches[SPData.Selections._BranchKey].Nodes.Count == 0) || SPData.Selections._PathPoint.Equals(null) || SPData.Selections._SharedPathPointIndex == -1) ?
    "null" : SPData.Selections._SharedPathPointIndex.ToString(), style);

        style.normal.textColor = Color.black;
        GUI.Label(new Rect(270, 5, 50, 20), "Connected branches :", style);
        style.normal.textColor = Color.green;

        if ((SPData.Selections._SharedPathPointIndex == -1 || SPData.DictBranches.Count == 0 || SPData.DictBranches[SPData.Selections._BranchKey].Nodes.Count == 0) || SPData.Selections._PathPoint.Equals(null))
        {
            GUI.Label(new Rect(380, 5, 50, 20), "null", style);
        }
        else
        {
            var array = SPData.SharedNodes[SPData.Selections._SharedPathPointIndex].ConnectedBranches;
            GUI.Label(new Rect(380, 5, 50, 20), string.Join(" - ", array.Select(i => i.ToString()).ToArray()), style);
        }

        if (!SPData.SmoothData.SmoothNode) BranchEdit(SPData);
        if (!SPData.Selections._PathPoint.Equals(null))
        {
            SelectedPathPointGUI(SPData, splinePlusEditor);
        }

        Handles.EndGUI();
    }

    void PivotEdit(SPData SPData)
    {
        EditorGUI.BeginChangeCheck();
        var pos = Handles.PositionHandle(SPData.Pivot, SPData.DataParent.transform.rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(SPData.SplinePlus, "Pivot position changed");
            SPData.Pivot = pos;

            Vector3[] pathPointsPosCach = new Vector3[SPData.DataParent.transform.childCount];
            for (int i = 0; i < SPData.DataParent.transform.childCount; i++)
            {
                pathPointsPosCach[i] = SPData.DataParent.transform.GetChild(i).position;
            }

            SPData.DataParent.transform.position = SPData.Pivot;

            for (int i = 0; i < SPData.DataParent.transform.childCount; i++)
            {
                SPData.DataParent.transform.GetChild(i).position = pathPointsPosCach[i];
            }
            //PivotEdit delegate
            if (PivotEditDel != null) PivotEditDel();

            EditorUtility.SetDirty(SPData.SplinePlus);
            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }
    }

    void SelectedPathPointGUI(SPData SPData, SplinePlusEditor splinePlusEditor)
    {
        if (GUI.Button(new Rect(76, 30, 30, 30), new GUIContent(SplinePlusEditor.SM.image, "Smooth selected regular-shared node"), GUIStyle.none))//smooth
        {
            var branch = SPData.DictBranches[SPData.Selections._BranchKey];
            SPData.SmoothData.IsShared = (SPData.Selections._SharedPathPointIndex != -1) ? true : false;

            if (SPData.SmoothData.IsShared)
            {
                SPData.SmoothData.SmoothNode = true;
                splinePlusEditor.SmoothNodeClass.SmoothSharedPathPoint(SPData);
            }
            else
            {
                if (branch.Nodes.Count > 2 &&
                       SPData.Selections._LocalNodeIndex != 0 &&
                       SPData.Selections._LocalNodeIndex != branch.Nodes.Count - 1)
                {
                    SPData.SmoothData.SmoothNode = true;
                    splinePlusEditor.SmoothNodeClass.SmoothNode(SPData);
                }
            }

        }

        if (SPData.SmoothData.SmoothNode) MessageBox(SPData, splinePlusEditor);

        if (!SPData.Selections._PathPoint.Equals(null))
        {
            var normalFactor = SPData.Selections._PathPoint.NormalFactor;
            var speedFactor = SPData.Selections._PathPoint.SpeedFactor;

            GUIStyle style = new GUIStyle
            {
                fontStyle = FontStyle.Italic,
                fontSize = 10
            };

            style.normal.textColor = Color.black;
            EditorGUI.LabelField(new Rect(10, 70, 50, 20), "Type", style);
            EditorGUI.BeginChangeCheck();
            var pathPointType = (NodeType)EditorGUI.EnumPopup(new Rect(60, 70, 160, 20), SPData.Selections._PathPoint._Type);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(SPData.SplinePlus, "Path point type changed");
                if (SPData.Selections._SharedPathPointIndex != -1)
                {
                    var sharedNode = SPData.SharedNodes[SPData.Selections._SharedPathPointIndex];
                    for (int i = 0; i < sharedNode.ConnectedBranches.Count; i++)
                    {
                        var bInd = sharedNode.ConnectedBranches[i];
                        var n = SPData.Selections._PathPoint.LocalIndex(SPData, bInd);
                        if (n != -1) SPData.DictBranches[bInd].Nodes[n]._Type = pathPointType;
                    }
                }
                else
                {
                    SPData.Selections._PathPoint._Type = pathPointType;
                }

                EditorUtility.SetDirty(SPData.SplinePlus);
                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }


            GUI.Label(new Rect(10, 90, 50, 20), "Normal", style);
            normalFactor = (int)GUI.HorizontalSlider(new Rect(60, 90, 115, 20), normalFactor, -180, 180);
            var normalFactorStr = GUI.TextField(new Rect(180, 90, 40, 15), normalFactor.ToString());

            System.Int32.TryParse(normalFactorStr, out normalFactor);
            if (normalFactor != SPData.Selections._PathPoint.NormalFactor)
            {
                SPData.Selections._PathPoint.NormalFactor = normalFactor;
                if (SPData.Selections._SharedPathPointIndex != -1)
                {
                    var sharedNode = SPData.SharedNodes[SPData.Selections._SharedPathPointIndex];
                    for (int i = 0; i < sharedNode.ConnectedBranches.Count; i++)
                    {
                        var branchKey = sharedNode.ConnectedBranches[i];
                        var localIndex = SPData.Selections._PathPoint.LocalIndex(SPData, branchKey);
                        if (localIndex == -1) continue;//path point not found on the branch 

                        var node = SPData.DictBranches[branchKey].Nodes[localIndex];
                        var tangent = (node.Point2.position - node.Point.position).normalized;
                        var t = Vector3.Dot(tangent, SPData.Selections._PathPoint.Tangent);

                        if (t > 0)
                        {
                            node.NormalFactor = normalFactor;
                        }
                        else
                        {
                            node.NormalFactor = -normalFactor;
                        }
                    }
                }
                else
                {
                    SPData.Selections._PathPoint.NormalFactor = normalFactor;
                }
                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }


            if (!SPData.ConstantSpeed)
            {
                GUI.Label(new Rect(10, 110, 50, 20), "Speed", style);
                speedFactor = (int)GUI.HorizontalSlider(new Rect(60, 110, 115, 20), speedFactor, 0, 100);
                var speedFactorStr = GUI.TextField(new Rect(180, 110, 40, 15), speedFactor.ToString());

                System.Int32.TryParse(speedFactorStr, out speedFactor);
                if (speedFactor != SPData.Selections._PathPoint.SpeedFactor)
                {
                    speedFactor = Mathf.Clamp(speedFactor, 0, 100);

                    if (SPData.Selections._SharedPathPointIndex != -1)
                    {
                        var sharedNode = SPData.SharedNodes[SPData.Selections._SharedPathPointIndex];
                        for (int i = 0; i < sharedNode.ConnectedBranches.Count; i++)
                        {
                            var branchKey = sharedNode.ConnectedBranches[i];
                            var localIndex = SPData.Selections._PathPoint.LocalIndex(SPData, branchKey);
                            if (localIndex == -1) continue;//path point not found on the branch 

                            SPData.DictBranches[branchKey].Nodes[localIndex].SpeedFactor = speedFactor;
                        }
                    }
                    else
                    {
                        SPData.DictBranches[SPData.Selections._BranchKey].Nodes[SPData.Selections._LocalNodeIndex].SpeedFactor = speedFactor;
                    }
                    SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
                }
            }
        }

        SharedPathPointGUI(SPData);
    }

    void AddBranch(SPData SPData)
    {
        if (GUI.Button(new Rect(240, 30, 30, 30), new GUIContent(SplinePlusEditor.Add.image, "Add a new branch at the selected node"), GUIStyle.none))
        {
            if (!SPData.Selections._PathPoint.Equals(null))
            {
                Undo.RecordObject(SPData.SplinePlus, "new branch added");
                BranchesClass.AddBranch(SPData);

                if (SceneView.sceneViews.Count > 0)
                {
                    SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                    sceneView.Focus();
                }
                EditorUtility.SetDirty(SPData.SplinePlus);
            }
            else
            {
                EditorUtility.DisplayDialog("Error",
                "You need to have a node selected before adding a branch"
               , "Ok");
            }
        }
    }

    void BranchEdit(SPData SPData)
    {
        if (!SPData.IsLooped && SPData.ObjectType != SPType.Extrude) AddBranch(SPData);

        if (SPData.DictBranches.Count == 0) return;
        if (GUI.Button(new Rect(272, 30, 30, 30), new GUIContent(SplinePlusEditor.Delete.image, "Delete the selected node "), GUIStyle.none) && EditorUtility.DisplayDialog("Branch deletion?",
            "Are you sure you want to delete this branch? "
           , "Yes", "No"))
        {
            BranchesClass.DeleteBranch(SPData, SPData.Selections._BranchKey);
            SceneView.RepaintAll();
        }

        if (GUI.Button(new Rect(304, 30, 30, 30), new GUIContent(SplinePlusEditor.FlipHandles.image, "Flips the handles of the selected node "), GUIStyle.none))
        {
            Undo.RecordObject(SPData.SplinePlus, "handles Fliped");
            BranchesClass.FlipHandles(SPData, SPData.Selections._BranchKey, SPData.Selections._LocalNodeIndex);

            EditorUtility.SetDirty(SPData.SplinePlus);
        }
        var rec = new Rect(336, 30, 30, 30);
        if (SPData.ObjectType != SPType.Extrude && !SPData.IsLooped)
        {

            rec = new Rect(368, 30, 30, 30);
            if (GUI.Button(new Rect(336, 30, 30, 30), new GUIContent(SplinePlusEditor.BreakAt.image, "Break the selected branch at the selected node"), GUIStyle.none))
            {
                if (!SPData.Selections._PathPoint.Equals(null))
                {
                    Undo.RecordObject(SPData.SplinePlus, "Break at branch");
                    var node = SPData.Selections._PathPoint;
                    BranchesClass.BreakBranch(SPData, node);
                    EditorUtility.SetDirty(SPData.SplinePlus);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error",
                "You need to have a node selected where you want your branch to be broken at "
               , "Ok");
                }
            }
        }
        else
        {
            rec = new Rect(336, 30, 30, 30);
        }

        if (GUI.Button(rec, new GUIContent(SplinePlusEditor.Reverse.image, "Reverse the direction of the selected branch "), GUIStyle.none))
        {
            Undo.RecordObject(SPData.SplinePlus, "branch Reversed");
            BranchesClass.ReverseBranch(SPData, SPData.Selections._BranchKey);
            EditorUtility.SetDirty(SPData.SplinePlus);
        }
    }

    void SharedPathPointGUI(SPData SPData)
    {
        if (SPData.Selections._SharedPathPointIndex == -1 || SPData.Selections._PathPoint.Equals(null)) return;

        GUIStyle style = new GUIStyle();

        var index = SPData.Selections._SharedPathPointIndex;
        style.fontSize = 10;

        GUI.Label(new Rect(10, 130, 50, 20), "Shared node:", style);

        style.normal.textColor = Color.black;
        style.fontStyle = FontStyle.Italic;
        EditorGUI.LabelField(new Rect(10, 150, 50, 20), "Type", style);

        EditorGUI.BeginChangeCheck();
        var sharedPathPointType = (SharedNodeType)EditorGUI.EnumPopup(new Rect(60, 150, 140, 20), SPData.SharedNodes[index]._Type);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(SPData.SplinePlus, "Shared node type changed");
            SPData.SharedNodes[index]._Type = sharedPathPointType;
            EditorUtility.SetDirty(SPData.SplinePlus);

        }

        if (GUI.Button(new Rect(205, 150, 20, 15), new GUIContent(SplinePlusEditor.Delete.image, "Initialize shared node keyboard data"), GUIStyle.none))
        {
            Undo.RecordObject(SPData.SplinePlus, "Forward branch changed");
            SPData.SharedNodes[index]._Forward = -1;
            SPData.SharedNodes[index]._Backward = -1;
            SPData.SharedNodes[index]._Right = -1;
            SPData.SharedNodes[index]._Left = -1;
            EditorUtility.SetDirty(SPData.SplinePlus);
        }

        if (sharedPathPointType == SharedNodeType.Defined)
        {
            GUI.Label(new Rect(10, 170, 50, 20), "Forward", style);
            GUI.TextField(new Rect(60, 170, 140, 15), (SPData.SharedNodes[index]._Forward == -1) ? "" : SPData.SharedNodes[index]._Forward.ToString());
            if (GUI.Button(new Rect(205, 170, 20, 15), SplinePlusEditor.Return, GUIStyle.none))
            {
                Undo.RecordObject(SPData.SplinePlus, "Forward branch changed");
                SPData.SharedNodes[index]._Forward = SPData.Selections._BranchKey;
                EditorUtility.SetDirty(SPData.SplinePlus);
            }

            GUI.Label(new Rect(10, 190, 50, 20), "Left", style);
            GUI.TextField(new Rect(60, 190, 40, 15), (SPData.SharedNodes[index]._Left == -1) ? "" : SPData.SharedNodes[index]._Left.ToString());
            if (GUI.Button(new Rect(105, 190, 20, 15), SplinePlusEditor.Return, GUIStyle.none))
            {
                Undo.RecordObject(SPData.SplinePlus, "backward branch changed");
                SPData.SharedNodes[index]._Left = SPData.Selections._BranchKey;
                EditorUtility.SetDirty(SPData.SplinePlus);
            }

            GUI.Label(new Rect(125, 190, 50, 20), "Right", style);
            GUI.TextField(new Rect(160, 190, 40, 15), (SPData.SharedNodes[index]._Right == -1) ? "" : SPData.SharedNodes[index]._Right.ToString());
            if (GUI.Button(new Rect(205, 190, 20, 15), SplinePlusEditor.Return, GUIStyle.none))
            {
                Undo.RecordObject(SPData.SplinePlus, "Right branch changed");
                SPData.SharedNodes[index]._Right = SPData.Selections._BranchKey;
                EditorUtility.SetDirty(SPData.SplinePlus);
            }

            GUI.Label(new Rect(10, 210, 50, 20), "Backward", style);
            GUI.TextField(new Rect(60, 210, 140, 15), (SPData.SharedNodes[index]._Backward == -1) ? "" : SPData.SharedNodes[index]._Backward.ToString());
            if (GUI.Button(new Rect(205, 210, 20, 15), SplinePlusEditor.Return, GUIStyle.none))
            {
                Undo.RecordObject(SPData.SplinePlus, "backward branch changed");
                SPData.SharedNodes[index]._Backward = SPData.Selections._BranchKey;
                EditorUtility.SetDirty(SPData.SplinePlus);
            }

        }
        
    }
}
