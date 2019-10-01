using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Eddie B. && Leo C.
 * This script handles behavior specific to the drill of the Juggernaut
 */

public class Juggernaut_Drill : MonoBehaviour
{
    private CarHeatManager damagedCarScript;

    private bool _isSpinning = false;
    private float _drillDamage;
    private float _drillFrequency;
    private GameObject _immunePlayer;

    private bool _isDrilling = false; //This is to prevent the InvokeRepeating in OnCollisionStay from happening multipleTimes

    public Material drillOffMaterial;
    public Material drillOnMaterial;



    private void DamageCar()
    {
        if (damagedCarScript != null)
        {
            if (damagedCarScript.heatCurrent >= damagedCarScript.heatExplodeLimit)
            {
                damagedCarScript = null;
                CancelInvoke("DamageCar");
                //mulch car into new train car.
            }
            else
            {
                damagedCarScript.heatCurrent += _drillDamage;
            }
        }
        else
        {
            CancelInvoke("DamageCar");
        }
    }

    #region Getters and Setters
    public void SetDrillInfo(float drillDamage, float drillFrequency, GameObject immunePlayer)
    {
        _drillDamage = drillDamage;
        _drillFrequency = drillFrequency;
        _immunePlayer = immunePlayer;
    }

    public void SetIsSpinning(bool spun)
    {
        _isSpinning = spun;
    }

    public bool GettIsSpinning()
    {
        return _isSpinning;
    }

    #endregion


    #region Visuals
    private void SpinDrill()
    {
        //gameObject.transform.Rotate(.1f,0,0);
        
    }

    public void StartSpinVisuals()
    {
        gameObject.GetComponent<MeshRenderer>().material = drillOnMaterial;
        //InvokeRepeating("SpinDrill", 0, .1f);
        //Will probably want to do this with an animation in the future.
    }

    public void StopSpinVisuals()
    {
        gameObject.GetComponent<MeshRenderer>().material = drillOffMaterial;
        //CancelInvoke("SpinDrill");
    }
    #endregion

    #region Collision Methods

    private void OnCollisionStay(Collision collision)
    {
        if (_isSpinning && collision.gameObject != _immunePlayer && !_isDrilling) //Makes sure the drill is spinning
        {
            _isDrilling = true;
            if (collision.gameObject.GetComponent<CarHeatManager>() != null && damagedCarScript == null) //Makes sure we don't hit multiple cars at once
            {
                damagedCarScript = collision.gameObject.GetComponent<CarHeatManager>();
                InvokeRepeating("DamageCar", 0, _drillFrequency);
            }
            else if (collision.gameObject.GetComponent<PickupBehavior>() != null)
            {
                //mulch the pickup an create a train car.
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.GetComponent<CarHeatManager>() != null == damagedCarScript)
        {
            _isDrilling = false;
            Debug.Log("Collision with :" + collision.gameObject.name + " STOPPED!");
            damagedCarScript = null;
        }
    }
    #endregion

}
