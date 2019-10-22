using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;

public class TrainEditor
{
    public void Follower(SPData SPData, SplinePlusEditor SPEditor)
    {
        if (SPEditor.TrainsList == null) Train(SPData, SPEditor);
        var followersRect = EditorGUILayout.BeginHorizontal();

        SPEditor.TrainsList.DoList(new Rect(followersRect.x, followersRect.y + 10, followersRect.width, followersRect.height));
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(SPEditor.TrainsList.GetHeight());
    }

    public void Train(SPData SPData, SplinePlusEditor SPEditor)
    {
        SerializedObject serializedObject = new SerializedObject(SPData.SplinePlus);
        SerializedProperty trainsSP = serializedObject.FindProperty("SPData").FindPropertyRelative("Trains");

        SPEditor.TrainsList = new ReorderableList(serializedObject, trainsSP, true, false, true, true);
        SPEditor.TrainsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, SplinePlusEditor.ElementHeight), "Trains", EditorStyles.boldLabel);
        };

        SPEditor.TrainsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty train = trainsSP.GetArrayElementAtIndex(index);
            SerializedProperty WagonsSP = train.FindPropertyRelative("Wagons");

            //wagons ReorderableList setup
            // add new wagons ReorderableList to the WagonsList whenever new train is added
            if (SPEditor.WagonsList.Count <= index)
            {
                SPEditor.WagonsList.Add(new ReorderableList(train.serializedObject, WagonsSP, true, false, true, true));
                WagonsDraw(SPData, SPEditor, train, index);

            }

            var trainExpandContent = (train.FindPropertyRelative("Show").boolValue) ? SplinePlusEditor.Plus : SplinePlusEditor.Minus;
            if (GUI.Button(new Rect(rect.x, rect.y, 20, SplinePlusEditor.ElementHeight), trainExpandContent, GUIStyle.none))
            {
                train.FindPropertyRelative("Show").boolValue = !train.FindPropertyRelative("Show").boolValue;
                trainsSP.serializedObject.ApplyModifiedProperties();

            }

            train.FindPropertyRelative("Name").stringValue = EditorGUI.TextField(new Rect(rect.x + 40, rect.y, rect.width - 120, SplinePlusEditor.ElementHeight), train.FindPropertyRelative("Name").stringValue, EditorStyles.boldLabel);
            if (GUI.Button(new Rect(rect.x + 20, rect.y, 20, SplinePlusEditor.ElementHeight), new GUIContent(SplinePlusEditor.Settings.image, "Follower settings"), GUIStyle.none))
            {
                FollowerSettingsWind wind = (FollowerSettingsWind)EditorWindow.GetWindow(typeof(FollowerSettingsWind), true, "Follower settings", true);
                wind.Show();
                wind.ShowWindow(SPData, train, FollowerSettingsType.Train);
            }
            var trainIsActiveContent = (train.FindPropertyRelative("IsActive").boolValue) ? SplinePlusEditor.On : SplinePlusEditor.Off;
            if (GUI.Button(new Rect(rect.width, rect.y, 40, SplinePlusEditor.ElementHeight), new GUIContent(trainIsActiveContent.image, "Enable/disable train animation"), GUIStyle.none))
            {
                train.FindPropertyRelative("IsActive").boolValue = !train.FindPropertyRelative("IsActive").boolValue;
                trainsSP.serializedObject.ApplyModifiedProperties();

            }
            if (train.FindPropertyRelative("Show").boolValue)
            {
                rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
                EditorGUI.BeginChangeCheck();
                var progress = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width - 38, SplinePlusEditor.ElementHeight), "Progress", train.FindPropertyRelative("Progress").floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (progress > SPData.DictBranches[train.FindPropertyRelative("_BranchKey").intValue].BranchDistance)
                    {
                        train.FindPropertyRelative("Progress").floatValue = SPData.DictBranches[train.FindPropertyRelative("_BranchKey").intValue].BranchDistance;
                        trainsSP.serializedObject.ApplyModifiedProperties();

                        return;
                    }

                    train.FindPropertyRelative("Progress").floatValue = progress;

                    UpdateAllWagons(SPData, train);

                }

                rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
                EditorGUI.BeginChangeCheck();
                var speed = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width - 38, SplinePlusEditor.ElementHeight), "Speed", train.FindPropertyRelative("Speed").floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    train.FindPropertyRelative("Speed").floatValue = speed;
                    trainsSP.serializedObject.ApplyModifiedProperties();
                }
                rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
                EditorGUI.IntField(new Rect(rect.x, rect.y, rect.width - 38, SplinePlusEditor.ElementHeight), "Branch key", train.FindPropertyRelative("_BranchKey").intValue);


                if (GUI.Button(new Rect(rect.width, rect.y, 20, SplinePlusEditor.ElementHeight), new GUIContent(SplinePlusEditor.Return.image, "Add branch index"), GUIStyle.none))
                {
                    serializedObject.Update();
                    train.FindPropertyRelative("_BranchKey").intValue = SPData.Selections._BranchKey;
                    UpdateAllWagons(SPData, train);
                }

                rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
                SPEditor.WagonsList[index].DoList(new Rect(rect.x, rect.y, rect.width, rect.height));
            }

        };


        SPEditor.TrainsList.elementHeightCallback = (int index) =>
        {
            SerializedProperty train = trainsSP.GetArrayElementAtIndex(index);
            if (!train.FindPropertyRelative("Show").boolValue) return SplinePlusEditor.ElementHeight + 2;

            var d = 7;
            if (train.FindPropertyRelative("Wagons").arraySize == 0) return (SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace) * (d + 1);
            for (int i = 0; i < train.FindPropertyRelative("Wagons").arraySize; i++)
            {
                d += (train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(i).FindPropertyRelative("Show").boolValue) ? 5 : 1;
            }
            var height = (SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace) * d;
            return height;
        };

        SPEditor.TrainsList.onAddCallback = (ReorderableList list) =>
        {
            var t = trainsSP.arraySize;
            trainsSP.InsertArrayElementAtIndex(t);

            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("AnimationEvents").boolValue = true;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("IsForward").boolValue = true;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Name").stringValue = "Train name";
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Wagons").arraySize = 0;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Show").boolValue = true;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("IsActive").boolValue = true;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("_BranchKey").intValue = 0;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Progress").floatValue = 0;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Speed").floatValue = 2.5f;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Acceleration").floatValue = 0;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("BrakesForce").floatValue = 2;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("_FollowerAnimation").enumValueIndex = 0;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Position").vector3Value = Vector3.zero;
            trainsSP.GetArrayElementAtIndex(t).FindPropertyRelative("Rotation").vector3Value = Vector3.zero;

            trainsSP.serializedObject.ApplyModifiedProperties();
        };

        SPEditor.TrainsList.onRemoveCallback = (ReorderableList list) =>
        {
            trainsSP.DeleteArrayElementAtIndex(SPEditor.TrainsList.index);
            if (FollowerSettingsWind.oldWindw != null) FollowerSettingsWind.oldWindw.Close();
            trainsSP.serializedObject.ApplyModifiedProperties();
            return;
        };
    }



    void WagonsDraw(SPData SPData, SplinePlusEditor SPEditor, SerializedProperty train, int i)
    {
        var progressList = new List<float>();
        SerializedProperty WagonsSP = train.FindPropertyRelative("Wagons");

        SPEditor.WagonsList[i].drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (index == 0) progressList = new List<float>();
            SerializedProperty wagon = WagonsSP.GetArrayElementAtIndex(index);
            progressList.Add(wagon.FindPropertyRelative("Progress").floatValue);
            DrawWagon(SPData, wagon, rect);
        };

        SPEditor.WagonsList[i].elementHeightCallback = (int index) =>
        {
            SerializedProperty wagon = WagonsSP.GetArrayElementAtIndex(index);
            if (!wagon.FindPropertyRelative("Show").boolValue) return SplinePlusEditor.ElementHeight + 2;
            var height = ((SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace) * 5);
            return height;
        };

        SPEditor.WagonsList[i].onAddCallback = (ReorderableList list) =>
        {
            var t = train.FindPropertyRelative("Wagons").arraySize;
            train.FindPropertyRelative("Wagons").InsertArrayElementAtIndex(t);

            train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(t).FindPropertyRelative("IsForward").boolValue = true;
            train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(t).FindPropertyRelative("Reverse").boolValue = false;
            train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(t).FindPropertyRelative("Rot").boolValue = true;
            train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(t).FindPropertyRelative("FlipDirection").boolValue = false;
            train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(t).FindPropertyRelative("IsActive").boolValue = true;
            train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(t).FindPropertyRelative("Rot").boolValue = true;

            WagonsSP.serializedObject.ApplyModifiedProperties();
        };

        SPEditor.WagonsList[i].onRemoveCallback = (ReorderableList list) =>
        {
            WagonsSP.DeleteArrayElementAtIndex(list.index);
            WagonsSP.serializedObject.ApplyModifiedProperties();
            return;
        };

        SPEditor.WagonsList[i].onReorderCallback = (ReorderableList list) =>
        {
            UpdateAllWagons(SPData, train);
            //progress auto setting when auto spacing is off
            for (int n = 0; n < train.FindPropertyRelative("Wagons").arraySize; n++)
            {
                var wagon = train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(n);
                wagon.FindPropertyRelative("Progress").floatValue = progressList.Min();
                progressList.Remove(progressList.Min());
            }
        };
    }


    public void DrawWagon(SPData SPData, SerializedProperty wagon, Rect rect)
    {
        var followerExpandContent = (wagon.FindPropertyRelative("Show").boolValue) ? SplinePlusEditor.Plus : SplinePlusEditor.Minus;
        if (GUI.Button(new Rect(rect.x, rect.y, 20, SplinePlusEditor.ElementHeight), followerExpandContent, GUIStyle.none))
        {
            wagon.FindPropertyRelative("Show").boolValue = !wagon.FindPropertyRelative("Show").boolValue;
            wagon.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUI.ObjectField(new Rect(80, rect.y, rect.width - 20, SplinePlusEditor.ElementHeight),
              wagon.FindPropertyRelative("FollowerGO"), typeof(GameObject), new GUIContent(""));
        if (EditorGUI.EndChangeCheck())
        {
            wagon.serializedObject.ApplyModifiedProperties();
            SPData.SplinePlus.TrainClass.Follow(SPData);
        }
        if (!wagon.FindPropertyRelative("Show").boolValue) return;


        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        EditorGUI.BeginChangeCheck();
        var progress = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, SplinePlusEditor.ElementHeight), "Progress", wagon.FindPropertyRelative("Progress").floatValue);
        if (EditorGUI.EndChangeCheck())
        {

            var branchDist = SPData.DictBranches[wagon.FindPropertyRelative("_BranchKey").intValue].BranchDistance;
            if (progress < 0) progress = 0;
            if (progress > branchDist) progress = branchDist;

            wagon.FindPropertyRelative("Progress").floatValue = progress;
            wagon.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.TrainClass.Follow(SPData);
        }
        var transButtonString = wagon.FindPropertyRelative("Trans").boolValue ? "Local" : "World";

        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        EditorGUI.BeginChangeCheck();

        var position = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width-40, SplinePlusEditor.ElementHeight), "Position", wagon.FindPropertyRelative("Position").vector3Value);
        if (EditorGUI.EndChangeCheck())
        {
            wagon.FindPropertyRelative("Position").vector3Value = position;
            wagon.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.TrainClass.Follow(SPData);
        }
        if (GUI.Button(new Rect(rect.width+20, rect.y, 40, 15), new GUIContent(transButtonString)))
        {
            wagon.FindPropertyRelative("Trans").boolValue = !wagon.FindPropertyRelative("Trans").boolValue;
            wagon.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.TrainClass.Follow(SPData);
        }

        GUI.enabled = wagon.FindPropertyRelative("Rot").boolValue ? true : false;

        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        EditorGUI.BeginChangeCheck();

        var rotation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width - 40, SplinePlusEditor.ElementHeight), "Rotation", wagon.FindPropertyRelative("Rotation").vector3Value);
        if (EditorGUI.EndChangeCheck())
        {
            wagon.FindPropertyRelative("Rotation").vector3Value = rotation;
            wagon.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.TrainClass.Follow(SPData);
        }

        GUI.enabled = true;

        GUIStyle RotButtonStyle = new GUIStyle(GUI.skin.button);
        RotButtonStyle.normal = wagon.FindPropertyRelative("Rot").boolValue ? RotButtonStyle.onNormal : RotButtonStyle.normal;
        if (GUI.Button(new Rect(rect.width + 20, rect.y, 40, 15), new GUIContent("Rot"), RotButtonStyle))
        {
            wagon.FindPropertyRelative("Rot").boolValue = !wagon.FindPropertyRelative("Rot").boolValue;
            wagon.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.TrainClass.Follow(SPData);
        }
    }


    public void UpdateAllWagons(SPData SPData, SerializedProperty train)
    {
        for (int i = 0; i < train.FindPropertyRelative("Wagons").arraySize; i++)
        {
            var wagon = train.FindPropertyRelative("Wagons").GetArrayElementAtIndex(i);
            wagon.FindPropertyRelative("Progress").floatValue = train.FindPropertyRelative("Progress").floatValue * i / (train.FindPropertyRelative("Wagons").arraySize - 1);
            wagon.FindPropertyRelative("_BranchKey").intValue = train.FindPropertyRelative("_BranchKey").intValue;
            wagon.FindPropertyRelative("Speed").floatValue = train.FindPropertyRelative("Speed").floatValue;
            wagon.serializedObject.ApplyModifiedProperties();

        }
        SPData.SplinePlus.TrainClass.Follow(SPData);
    }

}
