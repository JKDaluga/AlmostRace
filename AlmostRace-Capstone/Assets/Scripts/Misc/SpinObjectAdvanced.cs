using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObjectAdvanced : MonoBehaviour
{

    public float spinAmountX;
    public float spinAmountY;
    public float spinAmountZ;
    public float spinRate;

    private void Start()
    {
        StartCoroutine(Spin());
    }

    public IEnumerator Spin()
    {
        while (true)
        {
            gameObject.transform.Rotate(spinAmountX, spinAmountY, spinAmountZ);
            yield return new WaitForSeconds(spinRate);
        }
    }
}
