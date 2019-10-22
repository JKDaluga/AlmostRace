using UnityEngine;
using UnityEditor;
using System.Linq;

public class SplineEditing
{
    RaycastHit Hit;
    Ray ray = new Ray();
    Plane m_Plane;
    Vector3 hitPoint;

    public delegate void branchSelectionChanged();
    public static event branchSelectionChanged BranchSelectionChangedDel;

    public void EditingFunctions(SPData SPData)
    {
        Event e = Event.current;
        if (e == null) return;

        if (Camera.current.transform.eulerAngles == new Vector3(0, 0, 0) || Camera.current.transform.eulerAngles == new Vector3(0, 180, 0))
            m_Plane = new Plane(Vector3.forward, Vector3.zero);
        else if (Camera.current.transform.eulerAngles == new Vector3(90, 0, 0) || Camera.current.transform.eulerAngles == new Vector3(270, 0, 0))
            m_Plane = new Plane(Vector3.up, Vector3.zero);
        else if (Camera.current.transform.eulerAngles == new Vector3(0, 90, 0) || Camera.current.transform.eulerAngles == new Vector3(0, 270, 0))
            m_Plane = new Plane(Vector3.right, Vector3.zero);
        else m_Plane = new Plane(Vector3.up, Vector3.zero);

        //if (e.type == EventType.keyDown && e.keyCode == KeyCode.X)
        if (e.shift)
        {
            SPData.EditSpline = true;
            if (SPData.DictBranches.Count == 0)
            {
                Branch branch = new Branch();
                SPData.DictBranches.Add(SPData.BranchesCount, branch);
                SPData.Selections._BranchKey = SPData.BranchesCount;
                SPData.Selections._BranchIndex = SPData.DictBranches.Count-1;
                SPData.BranchesCount++;

                //rearange followers branches 
                for (int i = 0; i < SPData.Followers.Count; i++)
                {
                    if (!SPData.DictBranches.ContainsKey(SPData.Followers[i]._BranchKey))
                    {
                        SPData.Followers[i]._BranchKey = SPData.DictBranches.Keys.FirstOrDefault();
                    }
                }
            }
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            SPData.Selections._PathPoint = new Node();
            SPData.EditSpline = false;
        }

        if (!SPData.EditSpline && !SPData.SmoothData.SmoothNode) SelectPoint(SPData);

        if (SPData.EditSpline)
        {
            SceneView.currentDrawingSceneView.Repaint();
            if (SPData.DictBranches.Count == 0) return;
            int n = 0;
            float distance = 0.0f;
            ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            var branch = SPData.DictBranches[SPData.Selections._BranchKey];


            if (e.shift && (branch.Nodes.Count >= 2)) //Adding path points in the midlle
            {
                if (EditorWindow.focusedWindow == null || EditorWindow.focusedWindow.titleContent.text != "Scene") return;

                var sceneViewSize = EditorWindow.focusedWindow.position;
                var t = Mathf.InverseLerp(sceneViewSize.y * 0.8f, (sceneViewSize.y + sceneViewSize.height) * 0.8f, e.mousePosition.y);
                var p = Mathf.Lerp(0, branch.VertexDistance[branch.VertexDistance.Count - 1], t);
                var distanceData = DistanceDataClass.DataExtraction(branch, p);

                for (int i = 0; i < branch.Nodes.Count; i++)
                {
                    if (Vector3.Distance(distanceData.Position, branch.Nodes[i].Point.position) <= 0.4f)
                    {
                        EditorGUIUtility.AddCursorRect(Camera.current.pixelRect, MouseCursor.ArrowMinus);
                        Handles.color = Color.red;
                        Handles.SphereHandleCap(0, branch.Nodes[i].Point.position, Quaternion.identity, SPData.SharedSettings.GizmosSize * 2, EventType.Repaint);
                        if (e.type == EventType.MouseDown && e.button == 0)
                            DeletePoint(SPData,  branch.Nodes[i], i);
                        return;
                    }
                }

                var v = Mathf.InverseLerp(0, branch.Vertices.Count - 1, distanceData.Index);
                n = (int)Mathf.Lerp(0, SPData.IsLooped ? branch.Nodes.Count : branch.Nodes.Count - 1, v);

                Handles.color = Color.yellow;
                Handles.SphereHandleCap(0, distanceData.Position, Quaternion.identity, SPData.SharedSettings.GizmosSize*2, EventType.Repaint);
                if (e.type == EventType.MouseDown && e.button == 0)
                    AddNewPoint(SPData, distanceData.Position, addPathPointPos.Middle, n + 1);
            }
            else
            {
                //Welding 
                foreach (var _branch in SPData.DictBranches)
                {
                    for (int r = 0; r < _branch.Value.Nodes.Count; r++)
                    {
                        var dist = Vector2.Distance(HandleUtility.WorldToGUIPoint(_branch.Value.Nodes[r].Point.position), e.mousePosition);
                        if (dist < 10)
                        {
                            if (!branch.Nodes.Exists(x => x.Equals(_branch.Value.Nodes[r])))
                            {
                                var wTangent1 = _branch.Value.Nodes[r].Point1.position;
                                var wTangent2 = _branch.Value.Nodes[r].Point2.position;
                                EditorGUIUtility.AddCursorRect(Camera.current.pixelRect, MouseCursor.Link);
                                switch (SPData.Selections._BranchWeldState)
                                {
                                    case BranchWeldState.First:
                                        n = (branch.Nodes.Count - 1);
                                        Preview(SPData, _branch.Value.Nodes[r].Point.position, branch.Nodes[n].Point2.position, wTangent1, n);
                                        if (e.type == EventType.MouseDown && e.button == 0) WeldBranch(SPData, _branch.Value.Nodes[r], n);
                                        break;
                                    case BranchWeldState.Last:
                                        n = 0;
                                        Preview(SPData, _branch.Value.Nodes[r].Point.position, branch.Nodes[n].Point1.position, wTangent2, n);
                                        if (e.type == EventType.MouseDown && e.button == 0) WeldBranch(SPData, _branch.Value.Nodes[r], n);
                                        break;
                                    case BranchWeldState.none:
                                        var dist1 = Vector3.Distance(hitPoint, branch.Nodes[0].Point.position);
                                        var dist2 = Vector3.Distance(hitPoint, branch.Nodes[branch.Nodes.Count - 1].Point.position);
                                        if (dist1 <= dist2)
                                        {
                                            n = 0;
                                            Preview(SPData,  _branch.Value.Nodes[r].Point.position, branch.Nodes[n].Point1.position, wTangent2, n);
                                            if (e.type == EventType.MouseDown && e.button == 0) WeldBranch(SPData, _branch.Value.Nodes[r], n);
                                        }
                                        else
                                        {
                                            n = (branch.Nodes.Count - 1);
                                            Preview(SPData, _branch.Value.Nodes[r].Point.position, branch.Nodes[n].Point2.position, wTangent1, n);
                                            if (e.type == EventType.MouseDown && e.button == 0) WeldBranch(SPData, _branch.Value.Nodes[r], n);
                                        }

                                        break;
                                }

                                return;
                            }
                        }
                    }

                }
                EditorGUIUtility.AddCursorRect(Camera.current.pixelRect, MouseCursor.ArrowPlus);
                // regular path point adding
                if (Physics.Raycast(ray, out Hit)) hitPoint = Hit.point;
                else if (m_Plane.Raycast(ray, out distance)) hitPoint = ray.GetPoint(distance);
                else return;

                if (branch.Nodes.Count == 0) //first branch path points adding
                {
                    if (e.type == EventType.MouseDown && e.button == 0) AddNewPoint(SPData, hitPoint, addPathPointPos.Beginning, 0);
                    return;
                }
                else if (branch.Nodes.Count == 1) //first branch path points adding
                {
                    n = (branch.Nodes.Count - 1);
                    Preview(SPData, hitPoint, branch.Nodes[n].Point2.position, Vector3.zero, n);
                    if (e.type == EventType.MouseDown && e.button == 0) AddNewPoint(SPData, hitPoint, addPathPointPos.End, n);
                    return;
                }


                if (SPData.Selections._BranchWeldState == BranchWeldState.none)// add path point to both sides
                {
                    var dist1 = Vector3.Distance(hitPoint, branch.Nodes[0].Point.position);
                    var dist2 = Vector3.Distance(hitPoint, branch.Nodes[branch.Nodes.Count - 1].Point.position);

                    if (dist1 <= dist2)
                    {
                        n = 0;
                        if (!SPData.IsLooped) Preview(SPData,hitPoint, branch.Nodes[n].Point1.position, Vector3.zero, n);
                        if (e.type == EventType.MouseDown && e.button == 0) AddNewPoint(SPData, hitPoint, addPathPointPos.Beginning, n);
                    }
                    else
                    {
                        n = (branch.Nodes.Count - 1);
                        if (!SPData.IsLooped) Preview(SPData, hitPoint, branch.Nodes[n].Point2.position, Vector3.zero, n);
                        if (e.type == EventType.MouseDown && e.button == 0) AddNewPoint(SPData, hitPoint, addPathPointPos.End, n);
                    }
                }
                else if (SPData.Selections._BranchWeldState == BranchWeldState.First)//add path point to the first side
                {
                    n = (branch.Nodes.Count - 1);
                    Preview(SPData, hitPoint, branch.Nodes[n].Point2.position, Vector3.zero, n);
                    if (e.type == EventType.MouseDown && e.button == 0) AddNewPoint(SPData, hitPoint, addPathPointPos.End, n);
                }
                else if (SPData.Selections._BranchWeldState == BranchWeldState.Last)// add path point to the last side
                {
                    n = 0;
                    Preview(SPData,  hitPoint, branch.Nodes[n].Point1.position, Vector3.zero, n);
                    if (e.type == EventType.MouseDown && e.button == 0) AddNewPoint(SPData, hitPoint, addPathPointPos.Beginning, n);
                }
            }
        }
    }


    public void Preview(SPData SPData, Vector3 newPathPointPos, Vector3 tangent1, Vector3 tangent2, int n)
    {
        Event e = Event.current;
        if (e == null) return;

        var branch = SPData.DictBranches[SPData.Selections._BranchKey];

        Handles.color = Color.yellow;
        Handles.SphereHandleCap(0, newPathPointPos, Quaternion.identity, SPData.SharedSettings.GizmosSize*2, EventType.Repaint);
        var tan2 = (tangent2 == Vector3.zero) ? Vector3.Lerp(branch.Nodes[n].Point.position, newPathPointPos, 0.5f) : tangent2;

        Handles.DrawBezier(branch.Nodes[n].Point.position, newPathPointPos, tangent1, tan2, Color.yellow, null, 2.0f);
    }


    void DeletePoint(SPData SPData, Node node, int i)
    {
        var branch = SPData.DictBranches[SPData.Selections._BranchKey];
        if (!SPData.SharedNodes.Exists(x => x.Node.Equals(node)))
        {
            Undo.DestroyObjectImmediate(node.Point.gameObject);
            Undo.RecordObject(SPData.SplinePlus, "path point deleted");

            SPData.EditSpline = false;
            branch.Nodes.RemoveAt(i);
            EditorUtility.SetDirty(SPData.SplinePlus);

            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }
    }

    public void WeldBranch(SPData SPData, Node node, int n)
    {
        var branch = SPData.DictBranches[SPData.Selections._BranchKey];
        Undo.RecordObject(SPData.SplinePlus, "branch welded");
        if (n == 0) branch.Nodes.Insert(0, node);
        else branch.Nodes.Add(node);

        if (SPData.NodeType == NodeType.Free) node._Type = NodeType.Free;
        else node._Type = NodeType.Smooth;

        BranchesClass.BreakBranch(SPData, node);
        BranchesClass.AddRefreshSharedNode(SPData, node);
        BranchesClass.BranchWeldSt(SPData);
        EditorUtility.SetDirty(SPData.SplinePlus);
        SPData.SplinePlus.SplineCreationClass.UpdateBranch(SPData,branch);
    }

    void SelectPoint(SPData SPData)
    {
        if (SPData.DictBranches.Count == 0) return;
        if (Event.current.type == EventType.MouseDown)
        {
            var branch = SPData.DictBranches[SPData.Selections._BranchKey];
            for (int i = 0; i < branch.Nodes.Count; i++)
            {
                var dist = Vector2.Distance(HandleUtility.WorldToGUIPoint(branch.Nodes[i].Point.position), Event.current.mousePosition);
                if (dist < 10)
                {
                    SPData.Selections._PathPoint = branch.Nodes[i];
                    SPData.Selections._LocalNodeIndex = i;

                    var sharedPathPointIndex = SPData.SharedNodes.FindIndex(x => x.Node.Equals(SPData.Selections._PathPoint));
                    SPData.Selections._SharedPathPointIndex = sharedPathPointIndex;
                    return;
                }
            }
            SelectBranch(SPData);
        }
    }


    void SelectBranch(SPData SPData)
    {
        int n = 0;
        foreach (var branch in SPData.DictBranches)
        {
            for (int i = 0; i < branch.Value.Vertices.Count - 1; i++)
            {
                var dist = Vector2.Distance(HandleUtility.WorldToGUIPoint(branch.Value.Vertices[i]), Event.current.mousePosition);
                if (dist < 20)
                {
                    Undo.RecordObject(SPData.SplinePlus, "Branch selected");

                    if (SPData.Selections._BranchKey == branch.Key) return;
                    SPData.Selections._BranchKey = branch.Key;
                    SPData.Selections._BranchIndex = n;
                    BranchesClass.BranchWeldSt(SPData);

                    //init mesh layer selection when branch selection is changed to avoid index out of range exception
                    if (SPData.ObjectType == SPType.MeshDeformer)
                    {
                        //Change branch selection delegete
                        if (BranchSelectionChangedDel != null) BranchSelectionChangedDel();
                    }
                    //keep path point selection when changing branches selection
                    if (SPData.Selections._SharedPathPointIndex != -1)
                    {
                        if (!SPData.SharedNodes[SPData.Selections._SharedPathPointIndex].ConnectedBranches.Contains(branch.Key)) SPData.Selections._PathPoint = new Node();
                        else
                        {
                            SPData.Selections._LocalNodeIndex = SPData.SharedNodes[SPData.Selections._SharedPathPointIndex].Node.LocalIndex(SPData, SPData.Selections._BranchKey);
                            SPData.Selections._PathPoint = SPData.DictBranches[SPData.Selections._BranchKey].Nodes[SPData.Selections._LocalNodeIndex];
                        }
                    }
                    else
                    {
                        SPData.Selections._PathPoint = new Node();
                    }
                    EditorUtility.SetDirty(SPData.SplinePlus);

                    return;
                }
            }
            n++;
        }
    }

    public void AddNewPoint(SPData SPData,Vector3 position, addPathPointPos addPathPointPos, int targetIndex)
    {
        Undo.RecordObject(SPData.SplinePlus, "Node added");


        var branch = SPData.DictBranches[SPData.Selections._BranchKey];
        SPData.EditSpline = false;
        var node = new Node();

        node._Type = SPData.NodeType;
     
        node.Point = CreatePoint(SPData, "p" + SPData.PathPointCount, position, Quaternion.identity, SPData.DataParent.transform);

        if (addPathPointPos == addPathPointPos.Beginning)
        {
            branch.Nodes.Insert(0, node);

            node.Point1 = CreatePoint(SPData, "p1", branch.Nodes[targetIndex].Point.position, Quaternion.identity, node.Point);
            node.Point2 = CreatePoint(SPData, "p2", branch.Nodes[targetIndex].Point.position, Quaternion.identity, node.Point);

            if (branch.Nodes.Count > 1)
            {
                node.Point2.position = Vector3.Lerp(branch.Nodes[targetIndex + 1].Point.position, node.Point.position, 0.5f);
                node.Point1.localPosition = -node.Point2.localPosition;
            }
            else
            {
                node.Point1.localPosition = new Vector3(5, 0, 0);
                node.Point2.localPosition = -node.Point1.localPosition;
            }
        }
        else if (addPathPointPos == addPathPointPos.End)//end 
        {
            branch.Nodes.Add(node);
            targetIndex = branch.Nodes.Count - 1;
            node.Point1 = CreatePoint(SPData, "p1", branch.Nodes[targetIndex].Point.position, Quaternion.identity, node.Point);
            node.Point2 = CreatePoint(SPData, "p2", branch.Nodes[targetIndex].Point.position, Quaternion.identity, node.Point);
            node.Point1.position = Vector3.Lerp(branch.Nodes[targetIndex - 1].Point.position, node.Point.position, 0.5f);
            node.Point2.localPosition = -node.Point1.localPosition;

        }
        else if (addPathPointPos == addPathPointPos.Middle)//middle
        {
            var a = (targetIndex - 1) < 0 ? branch.Nodes.Count - 1 : targetIndex - 1;
            var b = (targetIndex + 1) >= branch.Nodes.Count ? 0 : (targetIndex + 1);

            branch.Nodes.Insert(targetIndex, node);
            node.Point1 = CreatePoint(SPData, "p1", branch.Nodes[targetIndex].Point.position, Quaternion.identity, node.Point);
            node.Point2 = CreatePoint(SPData, "p2", branch.Nodes[targetIndex].Point.position, Quaternion.identity, node.Point);
            var dir = (branch.Nodes[a].Point.position - branch.Nodes[b].Point.position).normalized;
            node.Point1.localPosition = dir * Vector3.Distance(branch.Nodes[a].Point.position, branch.Nodes[b].Point.position) * 0.2f;
            node.Point2.localPosition = -node.Point1.localPosition;
        }

        BranchesClass.BranchWeldSt(SPData);
        EditorUtility.SetDirty(SPData.SplinePlus);
        SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);

        SPData.PathPointCount++;
    }


    public Transform CreatePoint(SPData SPData, string name, Vector3 Pos, Quaternion Rot, Transform Parent)
    {
        var Obj = new GameObject(name);

        Obj.hideFlags = HideFlags.HideInHierarchy;
        Obj.transform.position = Pos;
        Obj.transform.rotation = Rot;
        Obj.transform.parent = Parent;
        return Obj.transform;
    }
}
