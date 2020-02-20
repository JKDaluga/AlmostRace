using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSwapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("Hit");
        if(other.gameObject.GetComponent<AIBehaviour>() != null)
        {
            other.gameObject.GetComponent<AIBehaviour>().SwapSpline();
        }
    }
}
