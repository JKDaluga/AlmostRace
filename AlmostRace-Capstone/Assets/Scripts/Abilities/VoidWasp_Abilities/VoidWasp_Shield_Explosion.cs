using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Mike R.
 *  Eddie B.
 *
 *  Created on: 15-12-2019
 *
 *  When triggered looks for al the objects collides with,
 *  Saves all the objects and does damage to them when DoDamage is called.
 **/

public class VoidWasp_Shield_Explosion : MonoBehaviour
{
    public List<GameObject> _hitObjects;

    private void Start()
    {
        _hitObjects = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            _hitObjects.Add(hit.gameObject);
            //Debug.Log("Car added: " + hit.gameObject.name);
        }
        else if (hit.gameObject.GetComponent<Interactable>() != null)
        {
            _hitObjects.Add(hit.gameObject);
            //Debug.Log("interactable added: " + hit.gameObject.name);
        }
    }


    public void DoDamage(float damage, GameObject immunePlayer)
    {
        foreach (GameObject hit in _hitObjects)
        {

            if (hit.gameObject.GetComponent<CarHealthBehavior>() != null)
            {
                if (hit.gameObject != immunePlayer)
                {
                    hit.gameObject.GetComponent<CarHealthBehavior>().DamageCar(damage / _hitObjects.Count);
                    Debug.Log("1 damage done: " + damage + " To " + hit.gameObject.transform.parent.name);
                }
            }
            else if (hit.gameObject.GetComponent<Interactable>() != null)
            {
                hit.gameObject.GetComponent<Interactable>().DamageInteractable(damage / _hitObjects.Count);
                Debug.Log("2 damage done: " + damage + " To " + hit.gameObject.transform.parent.name);
            }
        }

        _hitObjects.Clear();
    }

}
