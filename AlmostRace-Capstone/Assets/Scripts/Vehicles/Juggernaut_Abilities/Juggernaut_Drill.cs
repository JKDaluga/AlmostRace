using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juggernaut_Drill : MonoBehaviour
{
    private CarHeatManager damagedCarScript;

    private bool _isSpinning = false;
    private float _drillDamage;
    private float _drillFrequency;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CarHeatManager>() != null)
        {
            damagedCarScript = collision.gameObject.GetComponent<CarHeatManager>();

        }
    }

    private void DamageCar()
    {
        if (damagedCarScript != null)
        {
            if (damagedCarScript.heatCurrent >= damagedCarScript.heatExplodeLimit)
            {
                damagedCarScript = null;
                CancelInvoke("DamageCar");
            }
            else
            {
                damagedCarScript.heatCurrent += _drillDamage;

            }
        }
    }


    public void SetIsSpinning(bool spun)
    {
        _isSpinning = spun;
    }

    public bool GettIsSpinning()
    {
        return _isSpinning;
    }

}
