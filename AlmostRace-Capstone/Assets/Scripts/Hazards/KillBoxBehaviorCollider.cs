using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBoxBehaviorCollider : MonoBehaviour
{
    private void OnColliderEnter(Collision other)
    {
        Debug.Log("Collided");
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {  
            //if other is a car
            other.gameObject.GetComponent<CarHealthBehavior>().DamageCarTrue(other.gameObject.GetComponent<CarHealthBehavior>().healthCurrent);
        }
    }
}
