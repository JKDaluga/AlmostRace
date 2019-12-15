using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Mike Romeo
 * Created on: 15-12-2019
 * 
 * When triggered looks for al the objects collides with,
 * Saves all the objects and does damage to them when
 * DoDamage is called.
 * */

public class VoidWasp_Shield_Explosion : MonoBehaviour
{
    public List<GameObject> _hitObjects;

    private void Start()
    {
        _hitObjects = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider hit)
    {
        _hitObjects.Add(hit.gameObject);


        /*if (hit.gameObject.GetComponent<CarHeatManager>() != null)
        {
            Debug.Log("Car added: " + hit.gameObject.name);
        }
        else if (hit.gameObject.GetComponent<Interactable>() != null)
        {
            _hitObjects.Add(hit.gameObject);
            Debug.Log("interactable added: " + hit.gameObject.name);
        }*/
    }


    public void DoDamage(float damage)
    {

        foreach (GameObject hit in _hitObjects)
        {
            Debug.Log("Object hit: " + hit.gameObject.name);

            if (hit.gameObject.GetComponent<CarHeatManager>() != null)
            {
                hit.gameObject.GetComponent<CarHeatManager>().DamageCar(damage / 4);
                Debug.Log("1 damage done: " + damage);
            }
            else if (hit.gameObject.GetComponent<Interactable>() != null)
            {
                hit.gameObject.GetComponent<Interactable>().DamageInteractable(damage / 4);
                Debug.Log("2 damage done: " + damage);
            }
        }
    }

}
