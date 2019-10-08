using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class SplinePlus : MonoBehaviour
{
    public SPData SPData;
    public FollowerClass FollowerClass;
    public TrainClass TrainClass;
    public SplineCreationClass SplineCreationClass;
    public EventClass EventClass;
    public ProjectionClass ProjectionClass;
    public GizmosClass GizmosClass;

   public SplinePlus()
    {
        SPData = CreateInstance(SPData);
        SPData.SplinePlus = this;

        SplineCreationClass = CreateInstance(SplineCreationClass);
        FollowerClass = CreateInstance(FollowerClass);
        TrainClass = CreateInstance(TrainClass);
        EventClass = CreateInstance(EventClass);
        GizmosClass = CreateInstance(GizmosClass);
        ProjectionClass = CreateInstance(ProjectionClass);

        SPData.Selections = CreateInstance(SPData.Selections);
        SPData.SmoothData = CreateInstance(SPData.SmoothData);
        SPData.SharedSettings = CreateInstance(SPData.SharedSettings);

    }

    private void Start()
    {
        SplineCreationClass.UpdateAllBranches(SPData);

        SPData.Selections._Follower = 0;

        for (int i = 0; i < SPData.Followers.Count; i++)
        {
            if (SPData.Followers[i].IsActive) StartCoroutine(OnAwakeFollowerEvent(SPData.Followers[i]));
        }

        for (int i = 0; i < SPData.PFFollowers.Count; i++)
        {
            SPData.PFFollowers[i].Goal.OnAwakeEvent.Invoke();

            for (int n = 0; n < SPData.PFFollowers[i].Agents.Count; n++)
            {
                if (SPData.PFFollowers[i].Agents[n].IsActive) StartCoroutine(OnAwakeFollowerEvent(SPData.PFFollowers[i].Agents[n]));
            }
        }
    }

    IEnumerator OnAwakeFollowerEvent(Follower follower)
    {
        follower.IsActive = false;
        yield return new WaitForSeconds(follower.OnAwakeDelayTime);
        follower.OnAwakeEvent.Invoke();
        follower.IsActive = true;
        if (SPData.FollowerType == FollowerType.PathFinding)
        {
            SPData.SplinePlus.PFFindAllShortestPaths();
        }
    }

    IEnumerator PFFollowerUpdatePath(Follower agent, PathFindingGoal pFGoal)
    {
        yield return new WaitForSeconds(agent.UpdateTime);
        PFFindPath(agent, pFGoal);
    }

    void Update()
    {
        FollowerClass.Follow(SPData);
        TrainClass.Follow(SPData);
        PFFollow();
    }

    public void PFFollow()
    {
        Type myType = null;
        try
        {
            myType = Type.GetType("PFFollowerClass");
        }
        catch
        {
            myType = null;
            SPData.FollowerType = FollowerType.Simple;
        }

        if (myType != null)
        {
            object instance = Activator.CreateInstance(myType);
            MethodInfo myMethod = myType.GetMethod("Follow");
            if (myMethod != null) myMethod.Invoke(instance, new object[] { SPData });
        }
    }

    public void PFFindPath(Follower agent, PathFindingGoal pFGoal)
    {
        //Debug.Log("here");
        Type myType = null;
        try
        {
            myType = Type.GetType("PathFinding");
        }
        catch
        {
            myType = null;
            SPData.FollowerType = FollowerType.Simple;
        }
        if (myType != null)
        {
            object instance = Activator.CreateInstance(myType);
            MethodInfo myMethod = myType.GetMethod("FindAgentShortestPathToGoal");
            if (myMethod != null) myMethod.Invoke(instance, new object[] { SPData, agent, pFGoal });
            else Debug.Log("Method FindShortestPathDijkstra not found");
        }
        else
        {
            Debug.Log(" Path finding package is not found");
        }
    }

    public void PFFindAllShortestPaths()
    {
        Type myType = null;
        try
        {
            myType = Type.GetType("PathFinding");
        }
        catch
        {
            myType = null;
            SPData.FollowerType = FollowerType.Simple;
        }
        if (myType != null)
        {
            object instance = Activator.CreateInstance(myType);
            MethodInfo myMethod = myType.GetMethod("FindAllShortestPaths");
            if (myMethod != null) myMethod.Invoke(instance, new object[] { SPData });
            else Debug.Log("Method FindAllShortestPaths not found");
        }
        else
        {
            Debug.Log(" Path finding package is not found");
        }
    }



    public void SelectFollower(int followerIndex)
    {
        SPData.Selections._Follower = followerIndex;
    }

    public void SetSpeed(float Speed)
    {
        SPData.Followers[SPData.Selections._Follower].Speed = Speed;
    }

    public void SetProgress(float Progress)
    {
        SPData.Followers[SPData.Selections._Follower].Progress = Progress;
    }

    public void GoToNewBranch(int Index)
    {
        SPData.Followers[SPData.Selections._Follower]._BranchKey = Index;
        SPData.Followers[SPData.Selections._Follower].Progress = 0;
    }

    public static T CreateInstance<T>(T type)
    {
        if (type == null)
        {
            type = Activator.CreateInstance<T>();
        }
        return type;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var branch in SPData.DictBranches)
        {
            if (branch.Value.Vertices.Count > 0) GizmosClass.DrawBranch(SPData, branch.Value, branch.Key);
        }

	
		for (int i = 0; i < SPData.Followers.Count; i++)
		{
			if(SPData.Followers[i].PathFollowingType==PathFollowingType.Projected) GizmosClass.FollowerProjectionLines(SPData.Followers[i]);
		}
        GizmosClass.NodesGizmos(SPData);
    }
}







