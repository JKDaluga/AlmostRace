using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B. Lux defensive ability. Adds a shield to the car.
*/
public class Lux_ShieldPanelBehavior : MonoBehaviour
{
    public MeshRenderer _meshRender;
    private CarHeatManager _carHealthScript;

    private void Awake()
    {
        DestroyInteractable();
    }

    public void GiveInfo(CarHeatManager carHealthScript)
    {
        _carHealthScript = carHealthScript;
    }

    public IEnumerator TrackHealth()
    {
        while(true)
        {
            if(_carHealthScript.GetExtraHealth() <= 0)
            {
                Debug.Log("Track Health should have happened");
                DestroyInteractable();
                yield return null;
            }
            yield return null;
        }
    }

    public void TriggerInteractable()
    {
        _meshRender.enabled = true;  
        StartCoroutine(TrackHealth());
    }

    public void DestroyInteractable()
    {
        StopAllCoroutines();
        _meshRender.enabled = false;
    }
}
