using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public float spinAmount;
    public float spinRate;

    private void Start()
    {
        StartCoroutine(Spin());
    }

    public IEnumerator Spin()
    {
        while(true)
        {
            gameObject.transform.Rotate(0, 0, spinAmount);
            yield return new WaitForSeconds(spinRate);
        }
    }
}
