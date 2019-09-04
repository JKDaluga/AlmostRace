using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCheck : MonoBehaviour
{
    private TrackManager trackManager;
    public GameObject theEvent;
    public int lapToHappen;

    // Start is called before the first frame update
    void Start()
    {
        trackManager = GameObject.Find("Manager").GetComponent<TrackManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            if (trackManager.lap == lapToHappen && trackManager.CheckIncrimate() == false)
            {
                trackManager.AssignIncrimate(true);
                theEvent.GetComponent<BarrierEvent>().BarrierEventActiavtion();
            }
        }
    }
}
