using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeformedMesh))]
public class DeformedMeshEditor : Editor
{
    public DeformedMesh DeformedMesh;
    public GUIContent Delete;
    public GUIContent Visible;
    public GUIContent Unvisible;
    public GUIContent SelectionFrame;
    public GUIContent MeshDeformBanner;
    public GUIContent Link;
    public GUIContent Unlink;

    Vector2 scrollPosition = Vector2.zero;

    public bool MeshAdded = false;
    public bool FieldChanged = false;
    public GameObject NewMeshAdded;

    [MenuItem("Tools/Mesh deform/Mesh deform", false, 2)]
    static void CreateSplinePlus()
    {
        var NewSpline = new GameObject("Mesh deform");
        var sPData = NewSpline.AddComponent<SplinePlus>().SPData;
        sPData.DataParent = NewSpline;
        sPData.Followers.Add(new Follower());

        Selection.activeGameObject = sPData.DataParent;

        var meshDeform = NewSpline.AddComponent<DeformedMesh>();
        meshDeform.SplinePlus = NewSpline.GetComponent<SplinePlus>();
        meshDeform.SplinePlus.SPData.ObjectType = SPType.MeshDeformer;
    }

    void OnEnable()
    {
        DeformedMesh = target as DeformedMesh;

        if (Delete == null) Delete = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Delete.png"));
        if (Visible == null) Visible = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Visible.png"));
        if (Unvisible == null) Unvisible = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Unvisible.png"));
        if (SelectionFrame == null) SelectionFrame = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/SelectionFrame.png"));
        if (MeshDeformBanner == null) MeshDeformBanner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/MeshDeformBanner.png"));
        if (Link == null) Link = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Link.png"));
        if (Unlink == null) Unlink = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Unlink.png"));

        EditorApplication.update += Update;
        SceneViewUI.PivotEditDel += PivotEdit;
        SplineEditing.BranchSelectionChangedDel += BranchSelectionChanged;

        DeformedMesh.IsUpdateBase = true;
        DeformedMesh.DrawMeshOnEachBranch();
    }
    private void OnDisable()
    {
        EditorApplication.update -= Update;
        SceneViewUI.PivotEditDel -= PivotEdit;
        SplineEditing.BranchSelectionChangedDel -= BranchSelectionChanged;
    }

    void BranchSelectionChanged()
    {
        DeformedMesh._Layer = 0;
    }

    void PivotEdit()
    {
        foreach (var dBBranch in DeformedMesh.DMBranches)
        {
            dBBranch.Value.MeshHolder.transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        if (MeshAdded)
        {
            MeshAdded = false;

            var prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;
            var prefabMesh = new PrefabMesh();
            prefabMesh.Prefab = NewMeshAdded;
            prefabMesh.LayerName = NewMeshAdded.name;
            var importedMat = prefabMesh.Prefab.GetComponent<MeshRenderer>().sharedMaterial;
            if (importedMat != null) prefabMesh.Material = importedMat;
            else prefabMesh.Material = (Material)EditorGUIUtility.Load("Assets/ElseForty/Assets/Materials/Base.mat");
            prefabMeshes.Add(prefabMesh);
            DeformedMesh._Layer = prefabMeshes.Count - 1;

            UpdateMaterials();
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
        }
    }



    void DuplicateMeshLayer()
    {
        var prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;
        var origMeshLayer = prefabMeshes[DeformedMesh._Layer];

        var prefabMesh = new PrefabMesh();
        prefabMesh.Prefab = origMeshLayer.Prefab;
        prefabMesh.LayerName = origMeshLayer.Prefab.name;

        prefabMesh.UniqueMat = origMeshLayer.UniqueMat;
        prefabMesh.Material = origMeshLayer.Material;
        prefabMesh._mat = origMeshLayer._mat;

        prefabMesh.RandomOffset = origMeshLayer.RandomOffset;
        prefabMesh.RandomRotation = origMeshLayer.RandomRotation;
        prefabMesh.RandomScale = origMeshLayer.RandomScale;

        prefabMesh.Offset = origMeshLayer.Offset;
        prefabMesh.Rotation = origMeshLayer.Rotation;
        prefabMesh.Scale = origMeshLayer.Scale;
        prefabMesh.UniformScale = origMeshLayer.UniformScale;


        prefabMesh.ROffset = origMeshLayer.ROffset;
        prefabMesh.RRotation = origMeshLayer.RRotation;
        prefabMesh.RScale = origMeshLayer.RScale;

        prefabMesh.RUniformScale = origMeshLayer.RUniformScale;

        prefabMesh.Uniform = origMeshLayer.Uniform;

        prefabMesh.Tiling = origMeshLayer.Tiling;

        prefabMesh.Spacing = origMeshLayer.Spacing;
        prefabMesh.RandomSpacing = origMeshLayer.RandomSpacing;
        prefabMesh.LinkedSpacing = origMeshLayer.LinkedSpacing;

        prefabMesh.Placement = origMeshLayer.Placement;

        prefabMesh.LockRot = origMeshLayer.LockRot;
        prefabMesh.LockRotation = origMeshLayer.LockRotation;

        prefabMesh._Axis = origMeshLayer._Axis;
        prefabMesh._DeformationType = origMeshLayer._DeformationType;

        prefabMesh.Mirror = origMeshLayer.Mirror;
        prefabMesh.MirrorOffset = origMeshLayer.MirrorOffset;
        prefabMesh._MirrorAxis = origMeshLayer._MirrorAxis;

        prefabMesh.Visible = origMeshLayer.Visible;

        prefabMeshes.Add(prefabMesh);
        DeformedMesh._Layer = prefabMeshes.Count - 1;

        UpdateMaterials();
        DeformedMesh.IsUpdateBase = true;
        DeformedMesh.DrawMeshOnEachBranch();
    }

    public void UpdateMaterials()
    {
        var dMBranch = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey];
        List<Material> mats = new List<Material>();
        dMBranch.Mats = new List<string>();
        for (int i = 0; i < dMBranch.PrefabMeshes.Count; i++)
        {
            if (dMBranch.PrefabMeshes[i].UniqueMat)
            {
                mats.Add(dMBranch.PrefabMeshes[i].Material);

                if (dMBranch.PrefabMeshes[i].Material != null)
                {
                    dMBranch.Mats.Add(dMBranch.PrefabMeshes[i].Material.name);
                }
                dMBranch.PrefabMeshes[i]._mat = dMBranch.Mats.Count - 1;
            }
        }
        dMBranch.MeshHolder.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
    }


    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        if (Event.current != null)// avoid updating until mouse key is up or return key is down in value fields 
        {
            if ((Event.current.type == EventType.KeyDown && Event.current.keyCode == (KeyCode.Return))
               || (Event.current.type == EventType.MouseUp && Event.current.button == 0))
            {
                if (FieldChanged)
                {
                    FieldChanged = false;
                    DeformedMesh.IsUpdateBase = true;
                    DeformedMesh.DrawMeshOnEachBranch();
                }
            }
        }

        var rect = EditorGUILayout.BeginHorizontal();
        var xx = rect.x + (rect.width - MeshDeformBanner.image.width) * 0.5f;
        GUI.Label(new Rect(xx, rect.y, MeshDeformBanner.image.width, MeshDeformBanner.image.height), MeshDeformBanner);
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

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Prefab meshes", EditorStyles.boldLabel);

        Rect lastRect = GUILayoutUtility.GetLastRect();
        var dropArea = new Rect(lastRect.x, lastRect.y + 30, lastRect.width, 120);
        GUI.Box(dropArea, "");

        List<PrefabMesh> prefabMeshes = new List<PrefabMesh>();
        if (DeformedMesh.DMBranches.Count > 0)
        {
            prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;
        }

        DropAreaGUI(dropArea);

        if (prefabMeshes.Count == 0)
        {
            var str = (prefabMeshes.Count == 0) ? "Please drag and drop you prefab meshes here!" : "";
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;

            GUI.Label(new Rect(dropArea.x, dropArea.y + (dropArea.height / 2), dropArea.width, dropArea.height), str, centeredStyle);
        }
        var rectScrollView = new Rect(lastRect.x, lastRect.y + 30, lastRect.width, 130);
        var viewRect = new Rect(0, 0, 110 * (prefabMeshes.Count), 100);

        scrollPosition = GUI.BeginScrollView(rectScrollView, scrollPosition, viewRect);
        for (int i = 0; i < prefabMeshes.Count; i++)
        {
            var assetPreview = AssetPreview.GetAssetPreview(prefabMeshes[i].Prefab);

            var x = (110 * i) + 10;
            var y = 10;

            if (GUI.Button(new Rect(x, y, 100, 100), assetPreview, GUIStyle.none))//selection
            {
                DeformedMesh._Layer = i;
            }
            if (DeformedMesh._Layer == i) GUI.Box(new Rect(x, y, 100, 100), SelectionFrame, GUIStyle.none);
        }
        GUI.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.Space(160);

        if (prefabMeshes.Count > 0) SelectedLayer();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    void SelectedLayer()
    {
        var branchKey = DeformedMesh.SplinePlus.SPData.Selections._BranchKey;
        var branch = DeformedMesh.SplinePlus.SPData.DictBranches[branchKey];
        var dMBranch = DeformedMesh.DMBranches[branchKey];
        var prefabMeshes = dMBranch.PrefabMeshes;

        var prefabMesh = prefabMeshes[DeformedMesh._Layer];
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent(Delete.image, "Delete selected Mesh layer"), GUIStyle.none, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to delete this mesh layer?", "Yes", "Cancel"))
            {
                prefabMeshes.RemoveAt(DeformedMesh._Layer);
                DeformedMesh._Layer = 0;

                UpdateMaterials();
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                return;
            }
        }

        GUILayout.Space(2);
        var texture = (prefabMesh.Visible) ? Visible : Unvisible;
        if (GUILayout.Button(new GUIContent(texture.image, "Hide/Unhide selected Mesh layer"), GUIStyle.none, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            Undo.RecordObject(DeformedMesh, "mesh layer visibility changed");
            prefabMesh.Visible = !prefabMesh.Visible;

            UpdateMaterials();
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }

        GUILayout.Space(2);
        if (GUILayout.Button(new GUIContent(SplinePlusEditor.Duplicate.image, "Duplicate selected Mesh layer"), GUIStyle.none, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            Undo.RecordObject(DeformedMesh, "Duplicate mesh layer changed");
            DuplicateMeshLayer();
            EditorUtility.SetDirty(DeformedMesh);
        }


        EditorGUI.BeginChangeCheck();
        var layerName = EditorGUILayout.TextField(prefabMesh.LayerName, EditorStyles.boldLabel);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "PrefabMesh name changed");
            prefabMesh.LayerName = layerName;
            EditorUtility.SetDirty(DeformedMesh);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(2);
        EditorGUI.BeginChangeCheck();
        var mesh = (GameObject)EditorGUILayout.ObjectField("Mesh prefab", prefabMesh.Prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Prefab value changed");
            prefabMesh.Prefab = mesh;
            FieldChanged = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }


        EditorGUILayout.BeginVertical();
        if (prefabMesh.Material == null && prefabMesh.UniqueMat)
        {
            EditorGUILayout.HelpBox("Mesh will not be visible because material is empty, please assign it !!", MessageType.Error);
        }
        EditorGUILayout.BeginHorizontal();


        if (prefabMesh.UniqueMat)
        {
            EditorGUI.BeginChangeCheck();
            var material = (Material)EditorGUILayout.ObjectField("Material", prefabMesh.Material, typeof(Material), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "material changed");
                Undo.RecordObject(DeformedMesh.DMBranches[branchKey].MeshHolder.GetComponent<MeshRenderer>(), "material changed");
                prefabMesh.Material = material;

                UpdateMaterials();
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            var mat = EditorGUILayout.Popup("Materials", prefabMesh._mat, dMBranch.Mats.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "materials changed");
                prefabMesh._mat = mat;

                UpdateMaterials();
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }

        GUIStyle originalMatStyle = new GUIStyle(GUI.skin.button);
        originalMatStyle.normal = prefabMesh.UniqueMat ? originalMatStyle.onNormal : originalMatStyle.normal;
        if (GUILayout.Button("Unique", originalMatStyle, GUILayout.MaxWidth(80), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Unique material value changed");
            prefabMesh.UniqueMat = !prefabMesh.UniqueMat;

            UpdateMaterials();
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();

            EditorUtility.SetDirty(DeformedMesh);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUI.BeginChangeCheck();
        var type = EditorGUILayout.EnumPopup("Type", prefabMesh._DeformationType);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Type value changed");
            prefabMesh._DeformationType = (DeformationType)type;

            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }

        GUI.enabled = prefabMesh.LockRot ? false : true;

        EditorGUI.BeginChangeCheck();
        var axis = EditorGUILayout.EnumPopup("Axis", prefabMesh._Axis);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Axis changed");
            prefabMesh._Axis = (Axes)axis;

            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }

        GUI.enabled = true;

        if (prefabMesh._DeformationType == DeformationType.Alignement)
        {
            GUI.enabled = prefabMesh.LockRot ? true : false;

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var lockRot = EditorGUILayout.Vector3Field("Lock rotation", prefabMesh.LockRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "Lock rotation value changed");
                prefabMesh.LockRotation = lockRot;
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
            GUI.enabled = true;

            GUIStyle LockStyle = new GUIStyle(GUI.skin.button);
            LockStyle.normal = prefabMesh.LockRot ? LockStyle.onNormal : LockStyle.normal;
            if (GUILayout.Button(new GUIContent("Lock", "Lock rotation"), LockStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(DeformedMesh, "Lock value changed");
                prefabMesh.LockRot = !prefabMesh.LockRot;
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
            EditorGUILayout.EndHorizontal();

        }
        else
        {
            prefabMesh.LockRot = false;
        }
        if (prefabMesh._DeformationType==DeformationType.Deformation)
        {
            EditorGUI.BeginChangeCheck();
            var meshTrim = EditorGUILayout.EnumPopup("Mesh trim", prefabMesh._MeshTrim);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "Mesh trim value changed");
                prefabMesh._MeshTrim = (MeshTrim)meshTrim;

                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }
  

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var offset = EditorGUILayout.Vector3Field("Offset", prefabMesh.Offset);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Offset changed");
            prefabMesh.Offset = offset;
            FieldChanged = true;
            EditorUtility.SetDirty(DeformedMesh);
        }
        GUIStyle ROffsetStyle = new GUIStyle(GUI.skin.button);
        ROffsetStyle.normal = prefabMesh.RandomOffset ? ROffsetStyle.onNormal : ROffsetStyle.normal;
        if (GUILayout.Button("Rand", ROffsetStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Random offset value changed");
            prefabMesh.RandomOffset = !prefabMesh.RandomOffset;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }
        EditorGUILayout.EndHorizontal();

        if (prefabMesh.RandomOffset)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            var rOffset = EditorGUILayout.Vector3Field("Offset 2", prefabMesh.ROffset);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "Offset changed");
                prefabMesh.ROffset = rOffset;
                FieldChanged = true;
                EditorUtility.SetDirty(DeformedMesh);
            }

            if (GUILayout.Button("Seed", GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(DeformedMesh, "Random offset value changed");

                OffsetSeedUpdate();
                EditorUtility.SetDirty(DeformedMesh);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var rotation = EditorGUILayout.Vector3Field("Rotation", prefabMesh.Rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "rotation changed");
            prefabMesh.Rotation = rotation;
            FieldChanged = true;
            EditorUtility.SetDirty(DeformedMesh);
        }
        GUIStyle RRotationStyle = new GUIStyle(GUI.skin.button);
        RRotationStyle.normal = prefabMesh.RandomRotation ? RRotationStyle.onNormal : RRotationStyle.normal;
        if (GUILayout.Button("Rand", RRotationStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Random rotation value changed");
            prefabMesh.RandomRotation = !prefabMesh.RandomRotation;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }

        EditorGUILayout.EndHorizontal();
        if (prefabMesh.RandomRotation)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var rRotation = EditorGUILayout.Vector3Field("Rotation 2", prefabMesh.RRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "rotation changed");
                prefabMesh.RRotation = rRotation;
                FieldChanged = true;
                EditorUtility.SetDirty(DeformedMesh);
            }

            if (GUILayout.Button("Seed", GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(DeformedMesh, "Random rotation value changed");
                RotationSeedUpdate();
                EditorUtility.SetDirty(DeformedMesh);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (prefabMesh.Uniform)
        {
            EditorGUI.BeginChangeCheck();
            var uniformScale = EditorGUILayout.FloatField("Scale", prefabMesh.UniformScale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "scale changed");
                prefabMesh.UniformScale = uniformScale;
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            var scale = EditorGUILayout.Vector3Field("Scale", prefabMesh.Scale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "scale changed");
                prefabMesh.Scale = scale;
                FieldChanged = true;
                EditorUtility.SetDirty(DeformedMesh);
            }
        }

        GUIStyle RUniformStyle = new GUIStyle(GUI.skin.button);
        RUniformStyle.normal = prefabMesh.Uniform ? RUniformStyle.onNormal : RUniformStyle.normal;
        if (GUILayout.Button("Unif", RUniformStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Unifrom value changed");
            prefabMesh.Uniform = !prefabMesh.Uniform;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);

        }
        GUIStyle RScaleStyle = new GUIStyle(GUI.skin.button);
        RScaleStyle.normal = prefabMesh.RandomScale ? RScaleStyle.onNormal : RScaleStyle.normal;
        if (GUILayout.Button("Rand", RScaleStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Random scale value changed");
            prefabMesh.RandomScale = !prefabMesh.RandomScale;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (prefabMesh.RandomScale)
        {
            if (prefabMesh.Uniform)
            {
                EditorGUI.BeginChangeCheck();
                var rUniformScale = EditorGUILayout.FloatField("Scale 2", prefabMesh.RUniformScale);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(DeformedMesh, "scale changed");
                    prefabMesh.RUniformScale = rUniformScale;
                    FieldChanged = true;
                    EditorUtility.SetDirty(DeformedMesh);
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var rScale = EditorGUILayout.Vector3Field("Scale 2", prefabMesh.RScale);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(DeformedMesh, "scale changed");
                    prefabMesh.RScale = rScale;
                    FieldChanged = true;
                    EditorUtility.SetDirty(DeformedMesh);
                }
            }

            if (GUILayout.Button("Seed", GUILayout.MaxWidth(84), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(DeformedMesh, "Random scale value changed");
                ScaleSeedUpdate();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        var tiling = EditorGUILayout.IntField("Tiling", prefabMesh.Tiling);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Tiling changed");
            prefabMesh.Tiling = tiling;
            if (prefabMesh.Tiling <= 0) prefabMesh.Tiling = 0;
            FieldChanged = true;
            EditorUtility.SetDirty(DeformedMesh);
        }
        if (GUILayout.Button("Auto tile", GUILayout.MaxWidth(84), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Auto tiling value changed");

            prefabMesh.Tiling = (int)(branch.BranchDistance / prefabMesh.Spacing) + 1;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        var spacing = EditorGUILayout.FloatField("Spacing", prefabMesh.Spacing);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Spacing changed");
            if (spacing <= 0.05f) spacing = 0.05f;

            if (prefabMesh.LinkedSpacing)
            {
                var t = spacing - prefabMesh.Spacing;

                for (int i = 0; i < DeformedMesh.DMBranches.Count; i++)
                {
                    var _prefabMeshes = DeformedMesh.DMBranches[i].PrefabMeshes;
                    for (int n = 0; n < _prefabMeshes.Count; n++)
                    {
                       _prefabMeshes[n].Spacing += t;
                    }
                }
            }
            else
            {
                prefabMesh.Spacing = spacing;
            }
           
            FieldChanged = true;
            EditorUtility.SetDirty(DeformedMesh);
        }


        if (!prefabMesh.RandomSpacing)
        {
            GUIContent LinkStyle = prefabMesh.LinkedSpacing ? Link : Unlink;

            if (GUILayout.Button(LinkStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(DeformedMesh, "Linked spacing value changed");
                prefabMesh.LinkedSpacing = !prefabMesh.LinkedSpacing;
                DeformedMesh.IsUpdateBase = true;
                DeformedMesh.DrawMeshOnEachBranch();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }

        if (prefabMesh.RandomSpacing)
        {
            if (GUILayout.Button("Seed", GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                Undo.RecordObject(DeformedMesh, "Seed value changed");
                SpacingSeedUpdate();
                EditorUtility.SetDirty(DeformedMesh);
            }
        }

        GUIStyle RSpacingStyle = new GUIStyle(GUI.skin.button);
        RSpacingStyle.normal = prefabMesh.RandomSpacing ? RSpacingStyle.onNormal : RSpacingStyle.normal;
        if (GUILayout.Button("Rand", RSpacingStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Random Spacing value changed");
            prefabMesh.RandomSpacing = !prefabMesh.RandomSpacing;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        var placement = EditorGUILayout.FloatField("Placement", prefabMesh.Placement);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Placement changed");
            if (placement <= 0) placement = 0;
            prefabMesh.Placement = placement;
            FieldChanged = true;
            EditorUtility.SetDirty(DeformedMesh);
        }

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = dMBranch.SmoothNormals ? true : false;

        EditorGUI.BeginChangeCheck();
        var smoothNormalsAngle = EditorGUILayout.FloatField(new GUIContent("Normals", "Normals angle"), dMBranch.SmoothNormalsAngle);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Normals angle value changed");
            dMBranch.SmoothNormalsAngle = smoothNormalsAngle;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }
        GUI.enabled = true;

        GUIStyle normalStyle = new GUIStyle(GUI.skin.button);
        normalStyle.normal = dMBranch.SmoothNormals ? normalStyle.onNormal : normalStyle.normal;
        if (GUILayout.Button(new GUIContent("Smth", "Smooth normals"), normalStyle, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            Undo.RecordObject(DeformedMesh, "Smooth normals value changed");
            dMBranch.SmoothNormals = !dMBranch.SmoothNormals;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();
            EditorUtility.SetDirty(DeformedMesh);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        var mirrorAxis = EditorGUILayout.EnumPopup("Mirror axis", prefabMesh._MirrorAxis);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(DeformedMesh, "Mirror axis changed");
            prefabMesh._MirrorAxis = (MirrorAxes)mirrorAxis;

            prefabMesh.Mirror = prefabMesh._MirrorAxis != 0 ? true : false;
            DeformedMesh.IsUpdateBase = true;
            DeformedMesh.DrawMeshOnEachBranch();

            EditorUtility.SetDirty(DeformedMesh);
        }

        if (prefabMesh._MirrorAxis != 0)
        {
            EditorGUI.BeginChangeCheck();
            var mirrorOffset = EditorGUILayout.FloatField("Mirror offset", prefabMesh.MirrorOffset);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(DeformedMesh, "mirror  offset changed");
                prefabMesh.MirrorOffset = mirrorOffset;
                FieldChanged = true;
                EditorUtility.SetDirty(DeformedMesh);
            }
        }
    }

    public void DropAreaGUI(Rect dropArea)
    {

        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (System.Object dragged_object in DragAndDrop.objectReferences)
                    {
                        NewMeshAdded = (GameObject)dragged_object;
                        if (NewMeshAdded.gameObject.scene.name != null)
                        {
                            EditorUtility.DisplayDialog("Error", "Please drag prefabs from your project window ", "Okey");
                            return;
                        }
                        if (DeformedMesh.SplinePlus.SPData.DictBranches.Count == 0)
                        {
                            EditorUtility.DisplayDialog("Error", "Please make sure you have your spline drawn before proceeding!", "Ok");
                            return;
                        }
                        MeshAdded = true;
                    }
                }
                break;
        }
    }


    void OffsetSeedUpdate()
    {
        var prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;

        var offsetRandomWeights = prefabMeshes[DeformedMesh._Layer].OffsetRandomWeights;
        for (int i = 0; i < offsetRandomWeights.Count; i++)
        {
            var x = UnityEngine.Random.Range(0, 1.0f);
            var y = UnityEngine.Random.Range(0, 1.0f);
            var z = UnityEngine.Random.Range(0, 1.0f);

            offsetRandomWeights[i] = new Vector4(x, y, z);
        }

        DeformedMesh.IsUpdateBase = true;
        DeformedMesh.DrawMeshOnEachBranch();
    }


    void RotationSeedUpdate()
    {
        var prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;

        var randomWeights = prefabMeshes[DeformedMesh._Layer].RandomWeights;
        for (int i = 0; i < randomWeights.Count; i++)
        {
            var x = UnityEngine.Random.Range(0, 1.0f);
            var y = randomWeights[i].x;
            var z = randomWeights[i].z;
            randomWeights[i] = new Vector4(x, y, z);
        }

        DeformedMesh.IsUpdateBase = true;
        DeformedMesh.DrawMeshOnEachBranch();
    }

    void ScaleSeedUpdate()
    {
        var prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;

        var randomWeights = prefabMeshes[DeformedMesh._Layer].RandomWeights;
        for (int i = 0; i < randomWeights.Count; i++)
        {
            var x = randomWeights[i].x;
            var y = UnityEngine.Random.Range(0, 1.0f);
            var z = randomWeights[i].z;
            randomWeights[i] = new Vector4(x, y, z);
        }

        DeformedMesh.IsUpdateBase = true;
        DeformedMesh.DrawMeshOnEachBranch();
    }

    void SpacingSeedUpdate()
    {
        var prefabMeshes = DeformedMesh.DMBranches[DeformedMesh.SplinePlus.SPData.Selections._BranchKey].PrefabMeshes;
        var randomWeights = prefabMeshes[DeformedMesh._Layer].RandomWeights;
        for (int i = 0; i < randomWeights.Count; i++)
        {
            var x = randomWeights[i].x;
            var y = randomWeights[i].y;
            var z = UnityEngine.Random.Range(0, 1.0f);

            randomWeights[i] = new Vector4(x, y, z);
        }

        DeformedMesh.IsUpdateBase = true;
        DeformedMesh.DrawMeshOnEachBranch();
    }
}
