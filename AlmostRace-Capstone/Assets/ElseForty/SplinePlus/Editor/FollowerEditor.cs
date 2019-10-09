using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

public class FollowerEditor
{
    public void Follower(SPData SPData, SplinePlusEditor SPEditor)
    {
        if (SPEditor.FollowersList == null) Simple(SPData, SPEditor);
        var followersRect = EditorGUILayout.BeginHorizontal();
        SPEditor.FollowersList.DoList(new Rect(followersRect.x + 5, followersRect.y + 10, followersRect.width - 5, followersRect.height));
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(SPEditor.FollowersList.GetHeight());
    }

    public void Simple(SPData SPData, SplinePlusEditor SPEditor)
    {
        SerializedObject serializedObject = new SerializedObject(SPData.SplinePlus);
        SerializedProperty FollowersSP = serializedObject.FindProperty("SPData").FindPropertyRelative("Followers");
        SPEditor.FollowersList = new ReorderableList(serializedObject, FollowersSP, true, false, true, true);

        SPEditor.FollowersList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, SplinePlusEditor.ElementHeight), "Simple followers", EditorStyles.boldLabel);
        };

        SPEditor.FollowersList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            serializedObject.Update();
            SerializedProperty follower = FollowersSP.GetArrayElementAtIndex(index);

            DrawFollower(SPData, follower, rect);
            serializedObject.ApplyModifiedProperties();
        };
        SPEditor.FollowersList.onAddCallback = (ReorderableList list) =>
        {
            FollowersSP.serializedObject.Update();

            var t = FollowersSP.arraySize;
            FollowersSP.InsertArrayElementAtIndex(t);

            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Show").boolValue = false;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("AnimationEvents").boolValue = true;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("IsActive").boolValue = true;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("FlipDirection").boolValue = false;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("IsForward").boolValue = true;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("_BranchKey").intValue = SPData.DictBranches.Keys.LastOrDefault();
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Progress").floatValue = 0;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Speed").floatValue = 2.5f;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Acceleration").floatValue = 0;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("BrakesForce").floatValue = 2;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("_FollowerAnimation").enumValueIndex = 0;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Rot").boolValue = true;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Position").vector3Value = Vector3.zero;
            FollowersSP.GetArrayElementAtIndex(t).FindPropertyRelative("Rotation").vector3Value = Vector3.zero;

            FollowersSP.serializedObject.ApplyModifiedProperties();

        };
        SPEditor.FollowersList.elementHeightCallback = (int index) =>
        {
            SerializedProperty follower = FollowersSP.GetArrayElementAtIndex(index);
            if (!follower.FindPropertyRelative("Show").boolValue) return SplinePlusEditor.ElementHeight + 2;

            var height = (SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace) * 7;
            return height;
        };

        SPEditor.FollowersList.onRemoveCallback = (ReorderableList list) =>
        {
            FollowersSP.DeleteArrayElementAtIndex(list.index);
            if (FollowerSettingsWind.oldWindw != null) FollowerSettingsWind.oldWindw.Close();
            FollowersSP.serializedObject.ApplyModifiedProperties();
            return;
        };
    }

    public void DrawFollower(SPData SPData, SerializedProperty follower, Rect rect)
    {
        var followerExpandContent = (follower.FindPropertyRelative("Show").boolValue) ? SplinePlusEditor.Plus : SplinePlusEditor.Minus;
        if (GUI.Button(new Rect(rect.x, rect.y, 20, SplinePlusEditor.ElementHeight), followerExpandContent, GUIStyle.none))
        {
            follower.FindPropertyRelative("Show").boolValue = !follower.FindPropertyRelative("Show").boolValue;
            follower.serializedObject.ApplyModifiedProperties();

        }

        if (!follower.FindPropertyRelative("Show").boolValue)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(new Rect(90, rect.y, rect.width - 100, SplinePlusEditor.ElementHeight),
               follower.FindPropertyRelative("FollowerGO"), typeof(GameObject), new GUIContent(""));
            if (EditorGUI.EndChangeCheck())
            {
                if (follower.FindPropertyRelative("FollowerGO") != null) follower.FindPropertyRelative("IsActive").boolValue = true;

                follower.serializedObject.ApplyModifiedProperties();
                SPData.SplinePlus.FollowerClass.Follow(SPData);
            }
            var FollowerIsActiveContent = (follower.FindPropertyRelative("IsActive").boolValue) ? SplinePlusEditor.On : SplinePlusEditor.Off;
            if (GUI.Button(new Rect(rect.width, rect.y, 40, SplinePlusEditor.ElementHeight), new GUIContent(FollowerIsActiveContent.image, "Enable/disable follower animation"), GUIStyle.none))
            {
                follower.FindPropertyRelative("IsActive").boolValue = !follower.FindPropertyRelative("IsActive").boolValue;
                follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.FollowerClass.Follow(SPData);
            }

            if (GUI.Button(new Rect(rect.x + 20, rect.y, 20, SplinePlusEditor.ElementHeight), new GUIContent(SplinePlusEditor.Settings.image, "Follower settings"), GUIStyle.none))
            {
                FollowerSettingsWind wind = (FollowerSettingsWind)EditorWindow.GetWindow(typeof(FollowerSettingsWind), true, "Follower settings", true);
                wind.Show();
                wind.ShowWindow(SPData, follower, FollowerSettingsType.Follower);
            }

            return;
        }


        var FollowerIsActiveContent2 = (follower.FindPropertyRelative("IsActive").boolValue) ? SplinePlusEditor.On : SplinePlusEditor.Off;
        if (GUI.Button(new Rect(rect.width, rect.y, 40, SplinePlusEditor.ElementHeight), new GUIContent(FollowerIsActiveContent2.image, "Enable/disable follower animation"), GUIStyle.none))
        {
            follower.FindPropertyRelative("IsActive").boolValue = !follower.FindPropertyRelative("IsActive").boolValue;
        }

        if (GUI.Button(new Rect(rect.x + 20, rect.y, 20, SplinePlusEditor.ElementHeight), new GUIContent(SplinePlusEditor.Settings.image, "Follower settings"), GUIStyle.none))
        {
            FollowerSettingsWind wind = (FollowerSettingsWind)EditorWindow.GetWindow(typeof(FollowerSettingsWind), true, "Follower settings", true);
            wind.Show();
            wind.ShowWindow(SPData, follower, FollowerSettingsType.Follower);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUI.ObjectField(new Rect(90, rect.y, rect.width - 100, SplinePlusEditor.ElementHeight),
           follower.FindPropertyRelative("FollowerGO"), typeof(GameObject), new GUIContent(""));
        if (EditorGUI.EndChangeCheck())
        {
            if (follower.FindPropertyRelative("FollowerGO") != null) follower.FindPropertyRelative("IsActive").boolValue = true;

            follower.serializedObject.ApplyModifiedProperties();
            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }

        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;

        EditorGUI.BeginChangeCheck();
        var progress = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width - 38, SplinePlusEditor.ElementHeight), "Progress", follower.FindPropertyRelative("Progress").floatValue);
        if (EditorGUI.EndChangeCheck())
        {
            var branchDist = SPData.DictBranches[follower.FindPropertyRelative("_BranchKey").intValue].BranchDistance;
            if (progress < 0) progress = 0;
            if (progress > branchDist) progress = branchDist;

            follower.FindPropertyRelative("Progress").floatValue = progress;
            follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }

        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 38, SplinePlusEditor.ElementHeight), follower.FindPropertyRelative("Speed"), new GUIContent("Speed"));

        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        EditorGUI.IntField(new Rect(rect.x, rect.y, rect.width - 38, SplinePlusEditor.ElementHeight), new GUIContent("Branch key"), follower.FindPropertyRelative("_BranchKey").intValue);


        if (GUI.Button(new Rect(rect.width, rect.y, 20, SplinePlusEditor.ElementHeight), new GUIContent(SplinePlusEditor.Return.image, "Add branch index"), GUIStyle.none))
        {
            follower.FindPropertyRelative("_BranchKey").intValue = SPData.Selections._BranchKey;
            follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }



        var transButtonString = follower.FindPropertyRelative("Trans").boolValue ? "Local" : "World";
        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        var position = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width - 40, SplinePlusEditor.ElementHeight), "Position", follower.FindPropertyRelative("Position").vector3Value);
        if (follower.FindPropertyRelative("Position").vector3Value != position)
        {
            follower.FindPropertyRelative("Position").vector3Value = position;
            follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }

        if (GUI.Button(new Rect(rect.width, rect.y, 40, 15), new GUIContent(transButtonString)))
        {
            follower.FindPropertyRelative("Trans").boolValue = !follower.FindPropertyRelative("Trans").boolValue;
            follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }


        GUI.enabled = follower.FindPropertyRelative("Rot").boolValue ? true : false;
        rect.y = rect.y + SplinePlusEditor.ElementHeight + SplinePlusEditor.ElementHeightSpace;
        var rotation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width - 40, SplinePlusEditor.ElementHeight), "Rotation", follower.FindPropertyRelative("Rotation").vector3Value);
        if (follower.FindPropertyRelative("Rotation").vector3Value != rotation)
        {
            follower.FindPropertyRelative("Rotation").vector3Value = rotation;
            follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }
        GUI.enabled = true;

        GUIStyle RotButtonStyle = new GUIStyle(GUI.skin.button);
        RotButtonStyle.normal = follower.FindPropertyRelative("Rot").boolValue ? RotButtonStyle.onNormal : RotButtonStyle.normal;
        if (GUI.Button(new Rect(rect.width, rect.y, 40, 15), new GUIContent("Rot"), RotButtonStyle))
        {
            follower.FindPropertyRelative("Rot").boolValue = !follower.FindPropertyRelative("Rot").boolValue;
            follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.FollowerClass.Follow(SPData);
        }

    }
}
