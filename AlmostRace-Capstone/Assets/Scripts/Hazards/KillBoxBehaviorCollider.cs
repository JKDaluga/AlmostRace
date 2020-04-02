using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBoxBehaviorCollider : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            collision.gameObject.GetComponent<CarHealthBehavior>().DamageCarTrue(collision.gameObject.GetComponent<CarHealthBehavior>().healthCurrent);
        }
    }
}
