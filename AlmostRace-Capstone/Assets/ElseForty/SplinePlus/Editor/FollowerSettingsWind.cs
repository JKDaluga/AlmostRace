using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class FollowerSettingsWind : EditorWindow
{
    FollowerSettingsWind windw;
    public static FollowerSettingsWind oldWindw;
    SPData SPData;
    public SerializedProperty Follower;
    ReorderableList EventsList;
    FollowerSettingsType followerSettingsType;
    public Vector2 ScrolPos = new Vector2(0, 0);

    public static GUIContent Banner;

    public void ShowWindow(SPData sPData, SerializedProperty follower, FollowerSettingsType _followerSettingsType)
    {
        if (Banner == null) Banner = new GUIContent((Texture2D)EditorGUIUtility.Load("Assets/ElseForty/Media/Banner.png"));
        windw = this;
        windw.Follower = follower;
        windw.SPData = sPData;
        followerSettingsType = _followerSettingsType;
        if (oldWindw != null) oldWindw.Close();
        oldWindw = windw;

        Events();
        SceneView.RepaintAll();
    }

    void OnGUI()
    {
        if (Banner == null) return;
        var rect = EditorGUILayout.BeginHorizontal();
        var x = rect.x + (rect.width - Banner.image.width) * 0.5f;
        GUI.Label(new Rect(x, rect.y, Banner.image.width, Banner.image.height), Banner);
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

        if (windw.Follower != null) FolloweSettings();
        windw.Follower.serializedObject.ApplyModifiedProperties();

        Repaint();
    }

    void FolloweSettings()
    {
        SharedSettings();

        GUILayout.Space(10);
        if (windw.Follower.FindPropertyRelative("_FollowerAnimation").enumValueIndex != (int)FollowerAnimation.SceneClick)
        {

            EditorGUILayout.LabelField("Events:", EditorStyles.boldLabel);
            var eventsRect = EditorGUILayout.BeginHorizontal();

            ScrolPos = GUI.BeginScrollView(new Rect(eventsRect.x, eventsRect.y, eventsRect.width, this.position.height - eventsRect.y + 18),
            ScrolPos, new Rect(eventsRect.x, eventsRect.y, eventsRect.width, windw.EventsList.GetHeight() + 30), false, false);
            windw.EventsList.DoList(new Rect(eventsRect.x + 5, eventsRect.y + 10, eventsRect.width - 10, eventsRect.height));

            GUI.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }
    }

    void AgentExtraSettings()
    {
        GUILayout.Space(2);
        EditorGUILayout.PropertyField(windw.Follower.FindPropertyRelative("UpdateType"));

        EditorGUILayout.BeginHorizontal();
        var oneDirectionContent = (windw.Follower.FindPropertyRelative("FlipDirection").boolValue ? SplinePlusEditor.On : SplinePlusEditor.Off);
        if (GUILayout.Button(new GUIContent(oneDirectionContent.image, "Flip direction"), GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            windw.Follower.FindPropertyRelative("FlipDirection").boolValue = !windw.Follower.FindPropertyRelative("FlipDirection").boolValue;
            windw.Follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.PFFindAllShortestPaths();
        }
     
        EditorGUILayout.LabelField(new GUIContent("Flip direction"));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        var isConsidTangentsContent = (windw.Follower.FindPropertyRelative("ConsiderTangents").boolValue) ? SplinePlusEditor.On : SplinePlusEditor.Off;
        if (GUILayout.Button(new GUIContent(isConsidTangentsContent.image, "Consider tangents"), GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            windw.Follower.FindPropertyRelative("ConsiderTangents").boolValue = !windw.Follower.FindPropertyRelative("ConsiderTangents").boolValue;
            windw.Follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.PFFindAllShortestPaths();
        }
        EditorGUILayout.LabelField(new GUIContent("Consider tangents"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        var isMinDistContent = (windw.Follower.FindPropertyRelative("IsMinDist").boolValue) ? SplinePlusEditor.On : SplinePlusEditor.Off;
        if (GUILayout.Button(new GUIContent(isMinDistContent.image, "Enable/disable Min distance"), GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
        {
            windw.Follower.FindPropertyRelative("IsMinDist").boolValue = !windw.Follower.FindPropertyRelative("IsMinDist").boolValue;
            windw.Follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.PFFindAllShortestPaths();
        }

        if (windw.Follower.FindPropertyRelative("IsMinDist").boolValue)
        {
            EditorGUI.BeginChangeCheck();
            var minDist = EditorGUILayout.FloatField(new GUIContent("Min Distance"), windw.Follower.FindPropertyRelative("MinDistance").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                windw.Follower.FindPropertyRelative("MinDistance").floatValue = minDist;
                windw.Follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.PFFindAllShortestPaths();
            }
        }
        else
        {
            EditorGUILayout.LabelField(new GUIContent("Min Distance"));
        }
        EditorGUILayout.EndHorizontal();
    }

    void SharedSettings()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(windw.Follower.FindPropertyRelative("_FollowerAnimation"), new GUIContent("Animation type"));
        if (EditorGUI.EndChangeCheck())
        {
            if (windw.followerSettingsType != FollowerSettingsType.Follower && windw.Follower.FindPropertyRelative("_FollowerAnimation").intValue == 2
                ) windw.Follower.FindPropertyRelative("_FollowerAnimation").intValue = 0;
        }
        GUILayout.Space(2);


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Extra settings", EditorStyles.boldLabel);
        if (windw.Follower.FindPropertyRelative("_FollowerAnimation").enumValueIndex != (int)FollowerAnimation.SceneClick)
        {
            EditorGUI.BeginChangeCheck();
            var acceleration = EditorGUILayout.FloatField("Acceleration", windw.Follower.FindPropertyRelative("Acceleration").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                if (acceleration < 0) acceleration = 0;
                windw.Follower.FindPropertyRelative("Acceleration").floatValue = acceleration;
            }
            if (windw.followerSettingsType == FollowerSettingsType.Agent )
            {
                EditorGUI.BeginChangeCheck();
                var brakesForce = EditorGUILayout.FloatField("Brakes force", windw.Follower.FindPropertyRelative("BrakesForce").floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (brakesForce < 1) brakesForce = 1;
                    windw.Follower.FindPropertyRelative("BrakesForce").floatValue = brakesForce;
                }
            }
        
        }

        if (windw.Follower.FindPropertyRelative("_FollowerAnimation").enumValueIndex == (int)FollowerAnimation.KeyboardInput)
        {
            EditorGUI.BeginChangeCheck();
            var brakesForce = EditorGUILayout.FloatField("Brakes force", windw.Follower.FindPropertyRelative("BrakesForce").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                if (brakesForce < 1) brakesForce = 1;
                windw.Follower.FindPropertyRelative("BrakesForce").floatValue = brakesForce;
            }

            if (windw.followerSettingsType == FollowerSettingsType.Follower)
            {
                EditorGUILayout.BeginHorizontal();
                var flipDirectionContent = (windw.Follower.FindPropertyRelative("FlipDirection").boolValue ? SplinePlusEditor.On : SplinePlusEditor.Off);
                if (GUILayout.Button(new GUIContent(flipDirectionContent.image, "Flip direction"), GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
                {
                    windw.Follower.FindPropertyRelative("FlipDirection").boolValue = !windw.Follower.FindPropertyRelative("FlipDirection").boolValue;
                    windw.Follower.serializedObject.ApplyModifiedProperties();

                    SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);

                }
                EditorGUILayout.LabelField(new GUIContent("Flip direction"));
                EditorGUILayout.EndHorizontal();
            }
        }
      


        if (windw.followerSettingsType == FollowerSettingsType.Follower) PathFollowingType();
        if (windw.followerSettingsType == FollowerSettingsType.Agent) AgentExtraSettings();

        EditorGUILayout.EndVertical();
        AnimationEvents();
    }

    void PathFollowingType()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(windw.Follower.FindPropertyRelative("PathFollowingType"));
        if (EditorGUI.EndChangeCheck())
        {
            windw.Follower.serializedObject.ApplyModifiedProperties();

            SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
        }

        if (windw.Follower.FindPropertyRelative("PathFollowingType").intValue == 1)
        {
            EditorGUI.BeginChangeCheck();
            var groundRayRadius = EditorGUILayout.FloatField("Ground Ray Length", windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("GroundRayLength").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("GroundRayLength").floatValue = groundRayRadius;
                windw.Follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }

            EditorGUI.BeginChangeCheck();
            var obstacleRayRadius = EditorGUILayout.FloatField("Obstacle Ray Length", windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("ObstacleRayLength").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("ObstacleRayLength").floatValue = obstacleRayRadius;
                windw.Follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }


            EditorGUI.BeginChangeCheck();
            var offset = EditorGUILayout.FloatField("Follower ground offset", windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("FollowerGroundOffset").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("FollowerGroundOffset").floatValue = offset;
                windw.Follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);

            }

            EditorGUI.BeginChangeCheck();
            var height = EditorGUILayout.FloatField("Obstacle ray height", windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("ObstacleRayHeight").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("ObstacleRayHeight").floatValue = height;
                windw.Follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);

            var followGroundNormalContent = (windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("FollowGroundNormal").boolValue) ? SplinePlusEditor.On : SplinePlusEditor.Off;

            if (GUILayout.Button(followGroundNormalContent, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(20)))
            {
                windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("FollowGroundNormal").boolValue =
                    !windw.Follower.FindPropertyRelative("FollowerProjection").FindPropertyRelative("FollowGroundNormal").boolValue;
                windw.Follower.serializedObject.ApplyModifiedProperties();

                SPData.SplinePlus.SplineCreationClass.UpdateAllBranches(SPData);
            }
            EditorGUILayout.LabelField("Follow ground normal");

            EditorGUILayout.EndHorizontal();
        }
    }

    void AnimationEvents()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();

        var eventContent = windw.Follower.FindPropertyRelative("AnimationEvents").boolValue ? SplinePlusEditor.Plus : SplinePlusEditor.Minus;
        if (GUILayout.Button(eventContent, GUIStyle.none, GUILayout.MaxWidth(15), GUILayout.MaxHeight(15)))
        {
            windw.Follower.FindPropertyRelative("AnimationEvents").boolValue = !windw.Follower.FindPropertyRelative("AnimationEvents").boolValue;
        }


        EditorGUILayout.LabelField("Animation events", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        if (windw.Follower.FindPropertyRelative("AnimationEvents").boolValue)
        {
            SerializedProperty onMoveEvent;
            SerializedProperty iDLEEvent;
            SerializedProperty spaceEvent;
            SerializedProperty onAwakeEvent;

            EditorGUI.BeginChangeCheck();
            var onAwakeDelayTime = EditorGUILayout.FloatField("OnAwake Delay Time", windw.Follower.FindPropertyRelative("OnAwakeDelayTime").floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                windw.Follower.FindPropertyRelative("OnAwakeDelayTime").floatValue = onAwakeDelayTime;
                windw.Follower.serializedObject.ApplyModifiedProperties();
                windw.SPData.SplinePlus.FollowerClass.Follow(windw.SPData);
                windw.SPData.SplinePlus.TrainClass.Follow(windw.SPData);
            }

            onAwakeEvent = windw.Follower.FindPropertyRelative("OnAwakeEvent");
            onMoveEvent = windw.Follower.FindPropertyRelative("OnMoveEvent");
            iDLEEvent = windw.Follower.FindPropertyRelative("IDLEEvent");
            spaceEvent = windw.Follower.FindPropertyRelative("SpaceEvent");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(onAwakeEvent, new GUIContent("On Awake"));
            EditorGUILayout.PropertyField(iDLEEvent, new GUIContent("IDLE"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(onMoveEvent, new GUIContent("On Move"));
            EditorGUILayout.PropertyField(spaceEvent, new GUIContent("Space"));
            EditorGUILayout.EndHorizontal();




        }
        EditorGUILayout.EndVertical();
    }


    private void Events()
    {
        SerializedProperty EventsSP = windw.Follower.FindPropertyRelative("Events");
        windw.EventsList = new ReorderableList(EventsSP.serializedObject, EventsSP, true, false, true, true);

        windw.EventsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty myEvent = EventsSP.GetArrayElementAtIndex(index);

            GUIContent eventContentExpand = (myEvent.FindPropertyRelative("AnimationEvents").boolValue) ? SplinePlusEditor.Plus : SplinePlusEditor.Minus;
            if (GUI.Button(new Rect(rect.x, rect.y, 20, SplinePlusEditor.ElementHeight), eventContentExpand, GUIStyle.none))
            {
                myEvent.FindPropertyRelative("AnimationEvents").boolValue = !myEvent.FindPropertyRelative("AnimationEvents").boolValue;
            }

            EditorGUI.BeginChangeCheck();
            var eventName = EditorGUI.TextField(new Rect(rect.x + 20, rect.y, rect.width - 20, SplinePlusEditor.ElementHeight), myEvent.FindPropertyRelative("EventName").stringValue, EditorStyles.boldLabel);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(windw.SPData.SplinePlus, "eventName changed");
                myEvent.FindPropertyRelative("EventName").stringValue = eventName;
                EditorUtility.SetDirty(windw.SPData.SplinePlus);
            }

            if (!myEvent.FindPropertyRelative("AnimationEvents").boolValue) return;

            rect.y = rect.y + SplinePlusEditor.ElementHeight * 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 30, SplinePlusEditor.ElementHeight), myEvent.FindPropertyRelative("BranchIndexStartStr"), new GUIContent("Previous branch"));

            if (GUI.Button(new Rect(rect.width, rect.y, 20, SplinePlusEditor.ElementHeight), SplinePlusEditor.Return, GUIStyle.none))
            {
                myEvent.FindPropertyRelative("BranchIndexStartStr").stringValue = windw.SPData.Selections._BranchKey.ToString();
            }

            rect.y = rect.y + SplinePlusEditor.ElementHeight + 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 30, SplinePlusEditor.ElementHeight), myEvent.FindPropertyRelative("BranchIndexEndStr"), new GUIContent("next branch"));
            if (GUI.Button(new Rect(rect.width, rect.y, 20, SplinePlusEditor.ElementHeight), SplinePlusEditor.Return, GUIStyle.none))
            {
                myEvent.FindPropertyRelative("BranchIndexEndStr").stringValue = windw.SPData.Selections._BranchKey.ToString();
            }


            rect.y = rect.y + SplinePlusEditor.ElementHeight + 2;
            if (GUI.Button(new Rect(rect.width - 20, rect.y, 20, SplinePlusEditor.ElementHeight), SplinePlusEditor.Delete, GUIStyle.none))
            {
                if (myEvent.FindPropertyRelative("Conditions").arraySize > 0)
                {
                    myEvent.FindPropertyRelative("Conditions").DeleteArrayElementAtIndex(myEvent.FindPropertyRelative("_Condition").intValue);
                    myEvent.FindPropertyRelative("_Condition").intValue = 0;
                    Repaint();
                }
            }
            if (GUI.Button(new Rect(rect.width, rect.y, 20, SplinePlusEditor.ElementHeight), SplinePlusEditor.Return, GUIStyle.none))
            {
                var startStr = myEvent.FindPropertyRelative("BranchIndexStartStr").stringValue;
                var endStr = myEvent.FindPropertyRelative("BranchIndexEndStr").stringValue;
                if (startStr == null || endStr == null
                || (startStr == endStr)) return;
                //fill conditions list
                var cond = startStr + ";" + endStr;
                for (int i = 0; i < myEvent.FindPropertyRelative("Conditions").arraySize; i++)
                {
                    if (myEvent.FindPropertyRelative("Conditions").GetArrayElementAtIndex(i).stringValue == cond) return;
                }

                myEvent.FindPropertyRelative("Conditions").InsertArrayElementAtIndex(0);
                myEvent.FindPropertyRelative("Conditions").GetArrayElementAtIndex(0).stringValue = cond;
                myEvent.FindPropertyRelative("_Condition").intValue = 0;

            }

            string[] conditions = new string[myEvent.FindPropertyRelative("Conditions").arraySize];
            for (int i = 0; i < myEvent.FindPropertyRelative("Conditions").arraySize; i++)
            {
                conditions[i] = myEvent.FindPropertyRelative("Conditions").GetArrayElementAtIndex(i).stringValue;
            }
            EditorGUI.BeginChangeCheck();

            var _Condition = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width - 50, SplinePlusEditor.ElementHeight), "Conditions list",
         myEvent.FindPropertyRelative("_Condition").intValue, conditions);

            if (EditorGUI.EndChangeCheck())
            {

                myEvent.FindPropertyRelative("_Condition").intValue = _Condition;
                var cond = myEvent.FindPropertyRelative("Conditions").GetArrayElementAtIndex(_Condition).stringValue;
                string[] indices = cond.Split(';');

                myEvent.FindPropertyRelative("BranchIndexStartStr").stringValue = indices[0];
                myEvent.FindPropertyRelative("BranchIndexEndStr").stringValue = indices[1];
            }


            rect.y = rect.y + SplinePlusEditor.ElementHeight + 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 20, SplinePlusEditor.ElementHeight), myEvent.FindPropertyRelative("MyEvents"));

        };

        windw.EventsList.elementHeightCallback = (int index) =>
        {
            SerializedProperty myEvent = EventsSP.GetArrayElementAtIndex(index);
            if (!myEvent.FindPropertyRelative("AnimationEvents").boolValue) return SplinePlusEditor.ElementHeight + 2;

            SerializedProperty MyEventsCount = myEvent.FindPropertyRelative("MyEvents").FindPropertyRelative("m_PersistentCalls.m_Calls");

            var height = (MyEventsCount.arraySize > 1) ? 170 + (45 * (MyEventsCount.arraySize - 1)) : 170;
            return height;
        };


        windw.EventsList.onAddCallback = (ReorderableList eventsList) =>
        {
            var t = EventsSP.arraySize;
            EventsSP.InsertArrayElementAtIndex(t);
            var newEvent = EventsSP.GetArrayElementAtIndex(t);
            newEvent.FindPropertyRelative("EventName").stringValue = "Event name";
            newEvent.FindPropertyRelative("AnimationEvents").boolValue = true;
            newEvent.FindPropertyRelative("BranchIndexEndStr").stringValue = "";
            newEvent.FindPropertyRelative("BranchIndexStartStr").stringValue = "";
            newEvent.FindPropertyRelative("_Condition").intValue = -1;
            newEvent.FindPropertyRelative("Conditions").arraySize = 0;

        };
    }
}
