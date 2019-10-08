 
public class EventClass
{
    public void EventsTriggering(Follower follower,int PreviousBranchKey,int CurrentBranchKey)
    {
        var condition = PreviousBranchKey.ToString() + ";" + CurrentBranchKey.ToString();
        for (int e = 0; e < follower.Events.Count; e++)
        {
           if (follower.Events[e].Conditions.Contains(condition))
           {
               follower.Events[e].MyEvents.Invoke();
           }
        }
    }

    public void EventsTriggering(Train train, int PreviousBranchKey, int CurrentBranchKey)
    {
        var condition = PreviousBranchKey.ToString() + ";" + CurrentBranchKey.ToString();
        for (int e = 0; e < train.Events.Count; e++)
        {
           if (train.Events[e].Conditions.Contains(condition))
           {
               train.Events[e].MyEvents.Invoke();
           }
        }
    }
}
