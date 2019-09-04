using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackManager : MonoBehaviour
{
    public int lap = 0;
    private bool canIncrimate;
    public Text LapText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LapText.text = "Lap: " + lap;
    }

    public bool CheckIncrimate()
    {
        return canIncrimate;
    }

    public void AssignIncrimate(bool IncrimateValue)
    {
        canIncrimate = IncrimateValue;
    }
}
