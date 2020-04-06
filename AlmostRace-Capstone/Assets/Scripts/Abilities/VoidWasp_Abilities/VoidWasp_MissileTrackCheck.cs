using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_MissileTrackCheck : MonoBehaviour
{
    public VoidWasp_HomingMissile homingMissileScript;

    private void OnTriggerEnter(Collider collision)
    {
        if (homingMissileScript.GetTarget() == null && !homingMissileScript.GetTracking())
        {
            if (collision.gameObject.GetComponent<CarHealthBehavior>() != null)
            {
                if (collision.gameObject != homingMissileScript.GetImmunePlayer())
                {
                    homingMissileScript.TrackDuringRuntime(collision.gameObject);
                }
            }
            else if (collision.gameObject.GetComponent<Interactable>() != null)
            {
                homingMissileScript.TrackDuringRuntime(collision.gameObject);
            }
        }
    }
}
